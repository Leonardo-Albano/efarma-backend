using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Repositories.Interfaces;
using Newtonsoft.Json;

namespace EFarma.Business
{
    public class StockRoomBusiness : IStockRoomBusiness
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;

        public StockRoomBusiness(IUnitOfWork repository, IMapper mapper, HttpClient httpClient)
        {
            _repository = repository;
            _mapper = mapper;
            _httpClient = httpClient;
        }

        public async Task<ResultObject> CreateStockRoom(StockRoomDTO stockRoomDTO)
        {
            var stockRoom = _mapper.Map<StockRoom>(stockRoomDTO);

            var existingStockRoom = await _repository.StockRooms.FirstOrDefault(sr => sr.Name == stockRoomDTO.Name);
            if (existingStockRoom != null)
            {
                return new ResultObject
                {
                    Message = "StockRoom with the same name already exists.",
                    StatusCode = 409,
                    Success = false
                };
            }

            _repository.StockRooms.Add(stockRoom);
            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "StockRoom created successfully." : "Error creating the StockRoom.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<IEnumerable<StockRoom>>> GetAllStockRooms()
        {
            var stockRooms = await _repository.StockRooms.GetAll();
            var result = _mapper.Map<IEnumerable<StockRoom>>(stockRooms);

            bool success = result.Any();
            return new ResultDataObject<IEnumerable<StockRoom>>
            {
                Message = success ? "StockRooms retrieved successfully." : "No StockRooms found.",
                Data = result,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }

        public async Task<ResultObject> DeleteStockRoom(int id)
        {
            var stockRoom = await _repository.StockRooms.FirstOrDefault(sr => sr.Id == id);
            if (stockRoom == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "StockRoom not found.",
                    Success = false
                };
            }

            _repository.StockRooms.Remove(stockRoom);
            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "StockRoom deleted successfully." : "Error deleting the StockRoom.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<StockRoom?>> GetStockRoom(int id)
        {
            var stockRoom = await _repository.StockRooms.GetStockRoomDetailed(id);
            bool success = stockRoom != null;

            return new()
            {
                Message = success ? "Stock Room found." : "No Stock Room found.",
                Data = stockRoom,
                Success = success,
                StatusCode = success ? 200 : 404
            };
        }

        public async Task<ResultObject> InsertItemToStock(InStockItem inStockItem, int quantity)
        {
            var stockRoom = await _repository.StockRooms.FirstOrDefault(sr => sr.Id == inStockItem.StockRoomId);
            if (stockRoom == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "StockRoom not found.",
                    Success = false
                };
            }
            inStockItem.StockRoom = stockRoom;

            var medicament = await _repository.Medicaments.FirstOrDefault(m=>m.Id == inStockItem.MedicamentId);
            if (medicament == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Medicament not found.",
                    Success = false
                };
            }
            inStockItem.Medicament = medicament;

            var tagCodes = await GetNewTagCodes();

            if (quantity != tagCodes.Count())
            {
                return new ResultObject
                {
                    StatusCode = 400,
                    Message = "The amount of medicines indicated does not match those read on the shelves",
                    Success = false
                };
            }

            var stockItemsToAdd = new List<InStockItem>();
            foreach(var tagCode in tagCodes)
            {
                var stockItem = inStockItem.Clone();
                stockItem.TagCode = tagCode;
                stockItemsToAdd.Add(stockItem);
            }

            _repository.InStockItems.AddRange(stockItemsToAdd);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Item added on stock successfully." : "Error adding item.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultObject> RemoveItemsFromStock(IEnumerable<KeyValuePair<int, int>> medicamentIdList)
        {
            //List<KeyValuePair<int, int>> lackMedicines = [];
            //foreach (var medicament_kv in medicamentIdList)
            //{
            //    var medicaments = await _repository.InStockItems.Find(item=>item.MedicamentId == medicament_kv.Key);
            //    int lackMedicinesQtt = medicament_kv.Value - medicaments.Count();

            //    if (lackMedicinesQtt > 0)
            //    {
            //        lackMedicines.Add(new KeyValuePair<int, int>(medicament_kv.Key, lackMedicinesQtt));
            //    }
            //    else
            //    {
            //        var usedMedicaments = medicaments.Take(medicament_kv.Value).ToArray();
            //        _repository.InStockItems.RemoveRange(usedMedicaments);
            //    }
            //}

            return new();
        }
        public async Task<ResultObject> EntryStockRoom(EntryLogDTO entryLogDTO)
        {
            var accessLog = _mapper.Map<AccessLog>(entryLogDTO);
            var employee = await _repository.Employees.GetEmployeeByTagCode(entryLogDTO.TagCode);
            if (employee != null)
            {
                return new ResultObject
                {
                    Message = "Employee not found.",
                    StatusCode = 404,
                    Success = false
                };
            }
            accessLog.Employee = employee;

            var stockRoom = employee.Role.Permissions
                .SelectMany(p => p.StockRooms) // Isso une todas as StockRooms de todas as permissões
                .FirstOrDefault(sr => sr.UniqueId == entryLogDTO.StockRoomUniqueId);
            if (stockRoom == null)
            {
                return new ResultObject
                {
                    Message = "Access not allowed.",
                    StatusCode = 403,
                    Success = false
                };
            }

            accessLog.StockRoom = stockRoom;

            _repository.AccessLogs.Add(accessLog);

            await _repository.SaveChangesAsync();

            return new ResultObject
            {
                Message = "Access allowed",
                StatusCode = 200,
                Success = true
            };
        }

        private async Task<List<string>> GetNewTagCodes()
        {
            var readTagCodes = await GetReadTagCodes();
            var unassignedTags = new List<string>();

            foreach (var readTagCode in readTagCodes)
            {
                if(await _repository.InStockItems.FirstOrDefault(i=>i.TagCode == readTagCode) == null)
                {
                    unassignedTags.Add(readTagCode);
                }
            }

            return unassignedTags;
        }

        private async Task<List<string>> GetReadTagCodes()
        {
            try
            {
                var requestUrl = "http://127.0.0.1:5000/TagCodes";

                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var tagCodeResponse = JsonConvert.DeserializeObject<List<string>>(responseBody);

                return tagCodeResponse ?? [];
            }
            catch (Exception ex)
            {
                return [];
            }
        }

    }
}
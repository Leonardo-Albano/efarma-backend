using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Repositories.Interfaces;
using Newtonsoft.Json;
using System.Net.Mail;

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

        public async Task<ResultDataObject<List<StockRoom>>> GetAllStockRooms()
        {
            var stockRooms = await _repository.StockRooms.GetAll();
            var result = _mapper.Map<List<StockRoom>>(stockRooms);

            bool success = result.Any();
            return new ResultDataObject<List<StockRoom>>
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

            var newTagCodes = await GetNewTagCodes(inStockItem.StockRoom.UniqueId);

            if (quantity != newTagCodes.Count())
            {
                return new ResultObject
                {
                    StatusCode = 400,
                    Message = "The amount of medicines indicated does not match those read on the shelves",
                    Success = false
                };
            }

            var stockItemsToAdd = new List<InStockItem>();
            foreach(var tagCode in newTagCodes)
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

        public async Task<ResultObject> RemoveItemsFromStock(RemovePrescriptionItemsDTO prescriptionItemsDTO)
        {
            var prescription = await _repository.Prescriptions.GetDetailedPrescriptionById(prescriptionItemsDTO.PrescriptionId);
            if (prescription == null)
            {
                return new ResultObject
                {
                    Message = "Prescription not found.",
                    StatusCode = 404,
                    Success = false
                };
            }

            var stockRoom = await _repository.StockRooms.FirstOrDefault(p => p.Id == prescriptionItemsDTO.StockRoomId);
            if (stockRoom == null)
            {
                return new ResultObject
                {
                    Message = "Stock room not found.",
                    StatusCode = 404,
                    Success = false
                };
            }

            var responsible = await _repository.Employees.FirstOrDefault(p => p.Id == prescriptionItemsDTO.TakeOutResponsibleId);
            if (responsible == null)
            {
                return new ResultObject
                {
                    Message = "Funcionário não foi encontrado.",
                    StatusCode = 404,
                    Success = false
                };
            }

            prescription.TakeOutResponsible = responsible;
            var log = new AccessLog()
            {
                Employee = responsible,
                IsEntry = false,
                StockRoom = stockRoom,
                Time = DateTime.Now
            };


            var kvResult = await CompareWithActualMedicamentsAtStock(
            prescription.Items
                .SelectMany(i => Enumerable.Repeat(i.Medicament, i.PrescribedQuantity))
                .ToList(),
            stockRoom.UniqueId, stockRoom.Id);

            bool success = kvResult.Key;
            string message = kvResult.Value;

            if (!success)
            {
                return new()
                {
                    Message = kvResult.Value,
                    StatusCode = 403,
                    Success = kvResult.Key
                };
            }

            log.Message = message;
            prescription.Status = success ? Prescription.ConcludedMessage: Prescription.PendentMessage;

            _repository.AccessLogs.Add(log);
            _repository.Prescriptions.Update(prescription);
            success = await _repository.SaveChangesAsync() > 0;
            return new()
            {
                Message = message,
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }
        
        public async Task<ResultObject> EntryStockRoom(EntryLogDTO entryLogDTO)
        {
            var accessLog = _mapper.Map<AccessLog>(entryLogDTO);
            var employee = await _repository.Employees.GetEmployeeByTagCode(entryLogDTO.TagCode);
            if (employee == null)
            {
                return new ResultObject
                {
                    Message = "Tag não cadastrada.",
                    StatusCode = 404,
                    Success = false
                };
            }
            accessLog.Employee = employee;


            var stockRoom = await _repository.StockRooms.FirstOrDefault(s => s.UniqueId == entryLogDTO.StockRoomUniqueId);

            if (stockRoom == null)
            {
                return new ResultObject
                {
                    Message = "Sala não encontrada.",
                    StatusCode = 404,
                    Success = false
                };
            }
            accessLog.StockRoom = stockRoom;

            var employeeStockRoom = employee.Role.Permissions
                .SelectMany(p => p.StockRooms) // Une todas as StockRooms de todas as permissões
                .FirstOrDefault(sr=>sr == stockRoom);

            bool hasAccess = employeeStockRoom != null;

            accessLog.Message = hasAccess ? "Funcionário entrou na sala.": "Funcionário tentou acessar a sala, porém não possui acesso.";
            _repository.AccessLogs.Add(accessLog);

            await _repository.SaveChangesAsync();

            string[] splittedName = employee.Name.Split(' ');
            string formattedName = $"{splittedName.First()} {splittedName.Last()}";

            return new ResultObject
            {
                Message = hasAccess ? $"{employee.Name}. Acesso permitido." : $"{employee.Name}. Acesso não permitido, contate o RH.",
                StatusCode = hasAccess ? 200 : 403,
                Success = hasAccess
            };
        }

        public async Task<ResultObject> ExitStockRoom(string stockRoomUniqueId)
        {
            var entryAccessLog = await _repository.AccessLogs.GetDetailedLastEntryByStockRoomUniqueId(stockRoomUniqueId);
            if (entryAccessLog == null)
            {
                return new ResultObject
                {
                    Message = "Nenhuma entrada foi identificada na sala.",
                    StatusCode = 404,
                    Success = false
                };
            }

            var newAccessLog = entryAccessLog.Clone();
            newAccessLog.Message = "Funcionário saiu da sala.";
            
            var lastExit = await _repository.AccessLogs.GetDetailedLastExitByStockRoomUniqueId(stockRoomUniqueId);
            if (lastExit.Time > entryAccessLog.Time)
            {
                newAccessLog.Message = "Funcionário saiu da sala. (Havia mais de um funcionário na sala.)";
            }

            _repository.AccessLogs.Add(newAccessLog);
            var success = await _repository.SaveChangesAsync() > 0;
            return new()
            {
                Message = success ? "Log de saída registrado com sucesso" : "Log de saída falhou ao ser armazenado.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        private async Task<List<string>> GetNewTagCodes(string uniqueId)
        {
            var readTagCodes = await GetReadTagCodes(uniqueId);
            var unassignedTags = new List<string>();

            foreach (var readTagCode in readTagCodes)
            {
                var existentItemOnStock = await _repository.InStockItems.FirstOrDefault(i => i.TagCode == readTagCode);
                if (existentItemOnStock == null)
                {
                    unassignedTags.Add(readTagCode);
                }
            }

            return unassignedTags;
        }

        private async Task<List<string>> GetReadTagCodes(string uniqueId)
        {
            try
            {
                var requestUrl = $"http://127.0.0.1:5000/TagCodes?code={uniqueId}";

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

        private async Task<KeyValuePair<bool, string>> CompareWithActualMedicamentsAtStock(List<Medicament> prescriptionMedicaments, string stockRoomUniqueId, int stockRoomId)
        {
            var actualTagCodes = await GetReadTagCodes(stockRoomUniqueId);
            var actualItemsOnStock = await _repository.InStockItems.GetStockItemsByTagCodes(stockRoomId, actualTagCodes);
            var allItemsOnStock = await _repository.InStockItems.GetAll();

            var itemsTaken = allItemsOnStock.Except(actualItemsOnStock).ToList();
            int extraItemsCount = 0;
            int missingItemsCount = 0;

            foreach (var itemOnStock in itemsTaken)
            {
                // Procura um item correspondente na lista de medicamentos da prescrição
                var equivalentItem = prescriptionMedicaments.FirstOrDefault(m => m.Id == itemOnStock.MedicamentId);

                // Se não houver equivalente, conta como item extra
                if (equivalentItem == null)
                {
                    extraItemsCount++;
                }
                else
                {
                    prescriptionMedicaments.Remove(equivalentItem);
                    _repository.InStockItems.Remove(itemOnStock);
                }
            }

            // Contabiliza medicamentos restantes na prescrição como itens faltando
            missingItemsCount = prescriptionMedicaments.Count;

            // Monta a mensagem de retorno com as quantidades de medicamentos a mais e a menos
            if (extraItemsCount > 0 || missingItemsCount > 0)
            {
                var message = $"Os medicamentos retirados não estão de acordo com a receita. ";
                if (extraItemsCount > 0)
                    message += $"{extraItemsCount} medicamento(s) a mais.";
                if (missingItemsCount > 0)
                    message += $"{missingItemsCount} medicamento(s) a menos.";

                return new(false, message);
            }

            return new(true, "Medicamentos retirados de acordo com a receita.");
        }

        private async Task<bool> ValidateExitWithPendentPrescriptions(Employee employee, StockRoom stockRoom)
        {
            var prescriptions = await _repository.Prescriptions.GetPendentPrescriptionsByTakeOutResponsibleId(employee.Id);
            if(prescriptions.Count == 0)
            {
                return true;
            }

            MailMessage mail = new();

            mail.From = new MailAddress("efarma@avisos.com");
            mail.To.Add(employee.Mail);
            mail.To.Add(employee.ResponsibleMail);
            mail.Subject = $"Retirada indevida da sala de estoque: {stockRoom.Name}";

            string message = $"Uma retirada indevida foi feita pelo seguinte funcionário: " +
                           $"Nome: {employee.Name}" +
                           $"Email: {employee.Mail}" +
                           $"Telefone: {employee.Phone}\n";

            if (!string.IsNullOrEmpty(employee.EmployeeId))
                message += $"Id de funcionário: {employee.EmployeeId}";

            message += "Sala de estoque: " +
                      $"Nome: {stockRoom.Name}" +
                      $"Endereço: {stockRoom.Address}\n";

            foreach (var prescription in prescriptions)
            {
                message += "Receita(s): " +
                          $"Id: {prescription.Id}" +
                          $"Data de Criação: {prescription.Date}" +
                           "Itens:\n";
                foreach (var item in prescription.Items)
                {
                    string medicamentName = $"{item.Medicament.Description} {item.Medicament.Dosage}{item.Medicament.Measure}";
                    message += $"   {medicamentName}\n";
                }
            }

            mail.Body = message;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Send(mail);
        }
    }
}
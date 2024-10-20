using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class StockRoomBusiness : IStockRoomBusiness
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        public StockRoomBusiness(IUnitOfWork repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
    }
}

using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models;

namespace EFarma.Business.Interfaces
{
    public interface IStockRoomBusiness
    {
        Task<ResultObject> CreateStockRoom(StockRoomDTO stockRoomDTO);
        Task<ResultObject> DeleteStockRoom(int id);
        Task<ResultDataObject<IEnumerable<StockRoom>>> GetAllStockRooms();
        Task<ResultDataObject<StockRoom>> GetStockRoom(int id);
    }
}

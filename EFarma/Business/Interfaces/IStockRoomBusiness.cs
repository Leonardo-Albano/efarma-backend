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
        Task<ResultObject> InsertItemToStock(InStockItem inStockItem, int quantity);
        Task<ResultObject> RemoveItemsFromStock(IEnumerable<KeyValuePair<int, int>> medicamentIdList);
    }
}

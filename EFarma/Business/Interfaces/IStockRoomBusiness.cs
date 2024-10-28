using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models;

namespace EFarma.Business.Interfaces
{
    public interface IStockRoomBusiness
    {
        Task<ResultObject> CreateStockRoom(StockRoomDTO stockRoomDTO);
        Task<ResultObject> DeleteStockRoom(int id);
        Task<ResultObject> EntryStockRoom(EntryLogDTO entryLogDTO);
        Task<ResultObject> ExitStockRoom(string stockRoomUniqueId);
        Task<ResultDataObject<List<StockRoom>>> GetAllStockRooms();
        Task<ResultDataObject<StockRoom>> GetStockRoom(int id);
        Task<ResultObject> InsertItemToStock(InStockItem inStockItem, int quantity);
        Task<ResultObject> RemoveItemsFromStock(RemovePrescriptionItemsDTO prescriptionItemsDTO);
    }
}

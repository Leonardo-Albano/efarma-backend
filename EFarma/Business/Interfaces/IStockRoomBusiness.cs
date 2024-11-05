using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models;
using EFarma.Models.Views;

namespace EFarma.Business.Interfaces
{
    public interface IStockRoomBusiness
    {
        Task<ResultObject> CorrectAccess(EntryLogDTO entryLogDTO);
        Task<ResultObject> CreateStockRoom(StockRoomDTO stockRoomDTO);
        Task<ResultObject> DeleteStockRoom(int id);
        Task<ResultObject> EntryStockRoom(EntryLogDTO entryLogDTO);
        Task<ResultObject> ExitStockRoom(string stockRoomUniqueId);
        Task<ResultDataObject<List<StockRoom>>> GetAllStockRooms();
        Task<ResultDataObject<List<InStockItemView>>> GetAvailableMedicaments(string? medicamentName);
        Task<ResultDataObject<StockRoom>> GetStockRoom(int id);
        Task<ResultObject> InsertItemToStock(InStockItem inStockItem, int quantity);
    }
}

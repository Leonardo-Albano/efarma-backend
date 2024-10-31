using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IInStockItemRepository : IRepository<InStockItem>
    {
        Task<List<InStockItem>> GetDetailedStockItems();
        Task<List<InStockItem>> GetStockItemsByTagCodes(int stockRoomId, List<string> tagCodes);
    }
}

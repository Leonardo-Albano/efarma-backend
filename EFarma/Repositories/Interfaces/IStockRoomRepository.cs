using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IStockRoomRepository : IRepository<StockRoom>
    {
        Task<StockRoom?> GetStockRoomDetailed(int id);
    }
}

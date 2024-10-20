using EFarma.Data;
using EFarma.Models;
using EFarma.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EFarma.Repositories
{
    public class StockRoomRepository : Repository<StockRoom>, IStockRoomRepository
    {
        public StockRoomRepository(DbContext context) : base(context)
        {
        }

        public async Task<StockRoom?> GetStockRoomDetailed(int id)
            => await DataContext.StockRooms
            .Include(sr=>sr.InStockItems)
            .FirstOrDefaultAsync(sr => sr.Id == id);

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

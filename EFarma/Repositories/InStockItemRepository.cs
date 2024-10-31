using EFarma.Data;
using EFarma.Models;
using EFarma.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EFarma.Repositories
{
    public class InStockItemRepository : Repository<InStockItem>, IInStockItemRepository
    {
        public InStockItemRepository(DbContext context) : base(context)
        {
        }

        public async Task<List<InStockItem>> GetDetailedStockItems()
        {
            return await DataContext.InStockItems
                .Include(i=>i.StockRoom)
                .Include(i=>i.Medicament)
                .ToListAsync();
        }

        public async Task<List<InStockItem>> GetStockItemsByTagCodes(int stockRoomId, List<string> tagCodes)
        {
            return await DataContext.InStockItems
                .Where(i=>i.StockRoomId == stockRoomId && tagCodes.Contains(i.TagCode))
                .Include(i=>i.Medicament)
                .ToListAsync();
        }

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

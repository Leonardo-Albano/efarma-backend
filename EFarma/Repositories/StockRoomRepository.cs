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

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

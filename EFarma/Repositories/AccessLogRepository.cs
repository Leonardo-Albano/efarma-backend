using EFarma.Data;
using EFarma.Models;
using EFarma.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EFarma.Repositories
{
    public class AccessLogRepository : Repository<AccessLog>, IAccessLogRepository
    {
        public AccessLogRepository(DbContext context) : base(context)
        {
        }

        public async Task<AccessLog?> GetDetailedLastEntryByStockRoomUniqueId(string stockRoomUniqueId)
        {
            return await DataContext.AccessLogs
                    .Include(a => a.Employee)
                    .Include(a => a.StockRoom)
                    .Where(a => a.StockRoom.UniqueId == stockRoomUniqueId && a.IsEntry.HasValue && a.IsEntry.Value)
                    .OrderByDescending(a => a.Time)
                    .FirstOrDefaultAsync();
        }

        public async Task<AccessLog?> GetDetailedLastExitByStockRoomUniqueId(string stockRoomUniqueId)
        {
            return await DataContext.AccessLogs
                    .Include(a => a.Employee)
                    .Include(a => a.StockRoom)
                    .Where(a => a.StockRoom.UniqueId == stockRoomUniqueId && a.IsEntry.HasValue && !a.IsEntry.Value)
                    .OrderByDescending(a => a.Time)
                    .FirstOrDefaultAsync();
        }

        public async Task<List<AccessLog>> GetLogsByEmployeeAndStockRoom(int employeeId, int stockRoomId)
        {
            return await DataContext.AccessLogs
                    .Where(a => a.StockRoomId == stockRoomId &&
                            a.EmployeeId == employeeId)
                    .ToListAsync();
        }

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

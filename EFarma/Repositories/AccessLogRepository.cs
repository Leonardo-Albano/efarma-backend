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

        public async Task<AccessLog?> GetFirstUnmatchedEntry(int employeeId)
        {
            var accessLogs = await DataContext.AccessLogs
                .Include(a => a.Employee)
                .Include(a => a.StockRoom)
                .Where(a => a.EmployeeId == employeeId && a.IsEntry.HasValue)
                .OrderBy(a => a.Date)
                .ToListAsync();

            AccessLog? unmatchedEntry = null;
            var employeeEntryStacks = new Stack<AccessLog>();

            foreach (var log in accessLogs)
            {

                if (log.IsEntry.HasValue && log.IsEntry.Value)
                {
                    employeeEntryStacks.Push(log);
                }
                else if (log.IsEntry.HasValue && !log.IsEntry.Value && employeeEntryStacks.Count > 0)
                {
                    employeeEntryStacks.Pop();
                }
            }

            if (employeeEntryStacks.Count > 0)
            {
                unmatchedEntry = employeeEntryStacks.Peek();
            }

            return unmatchedEntry;
        }



        public async Task<AccessLog?> GetDetailedLastExitByStockRoomUniqueId(string stockRoomUniqueId)
        {
            return await DataContext.AccessLogs
                    .Include(a => a.Employee)
                    .Include(a => a.StockRoom)
                    .Where(a => a.StockRoom.UniqueId == stockRoomUniqueId && a.IsEntry.HasValue && !a.IsEntry.Value)
                    .OrderByDescending(a => a.Date)
                    .FirstOrDefaultAsync();
        }

        public async Task<List<AccessLog>> GetLogsByEmployeeAndStockRoom(int employeeId, int stockRoomId)
        {
            return await DataContext.AccessLogs
                    .Where(a => a.StockRoomId == stockRoomId &&
                            a.EmployeeId == employeeId)
                    .ToListAsync();
        }

        public async Task<AccessLog?> GetActualLogWithStockRoomByEmployee(int takeOutResponsibleId)
        {
            return await DataContext.AccessLogs
                    .Include(a => a.StockRoom)
                    .OrderByDescending(a => a.Date)
                    .LastOrDefaultAsync(a => a.EmployeeId == takeOutResponsibleId);
        }

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

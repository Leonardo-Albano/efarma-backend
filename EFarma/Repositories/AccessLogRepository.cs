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

        public async Task<AccessLog?> GetFirstUnmatchedEntry(string stockRoomUniqueId)
        {
            var accessLogs = await DataContext.AccessLogs
                .Include(a => a.Employee)
                .Include(a => a.StockRoom)
                .Where(a => a.StockRoom.UniqueId == stockRoomUniqueId && a.IsEntry.HasValue)
                .OrderBy(a => a.EmployeeId)
                .ThenBy(a => a.Date)
                .ToListAsync();

            AccessLog? unmatchedEntry = null;
            var employeeEntryStacks = new Dictionary<int, Stack<AccessLog>>();

            foreach (var log in accessLogs)
            {
                if (!employeeEntryStacks.ContainsKey(log.EmployeeId))
                {
                    employeeEntryStacks[log.EmployeeId] = new Stack<AccessLog>();
                }

                var entryStack = employeeEntryStacks[log.EmployeeId];

                if (log.IsEntry.HasValue && log.IsEntry.Value)
                {
                    // Entry log: push to stack
                    entryStack.Push(log);
                }
                else if (log.IsEntry.HasValue && !log.IsEntry.Value && entryStack.Count > 0)
                {
                    // Exit log: pop an entry from the stack (match found)
                    entryStack.Pop();
                }
            }

            // Find the first unmatched entry, if any
            foreach (var stack in employeeEntryStacks.Values)
            {
                if (stack.Count > 0)
                {
                    unmatchedEntry = stack.Peek();
                    break;
                }
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

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

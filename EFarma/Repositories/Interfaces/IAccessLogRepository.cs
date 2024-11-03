using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IAccessLogRepository : IRepository<AccessLog>
    {
        Task<AccessLog?> GetFirstUnmatchedEntry(string stockRoomUniqueId);
        Task<AccessLog?> GetDetailedLastExitByStockRoomUniqueId(string stockRoomUniqueId);
        Task<List<AccessLog>> GetLogsByEmployeeAndStockRoom(int employeeId, int stockRoomId);
    }
}

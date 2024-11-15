using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IAccessLogRepository : IRepository<AccessLog>
    {
        Task<AccessLog?> GetFirstUnmatchedEntry(int employeeId);
        Task<AccessLog?> GetDetailedLastExitByStockRoomUniqueId(string stockRoomUniqueId);
        Task<List<AccessLog>> GetLogsByEmployeeAndStockRoom(int employeeId, int stockRoomId);
        Task<AccessLog?> GetLastUnmatchedEntry(int takeOutResponsibleId);
        Task<List<AccessLog>> GetAllWithFilter(bool filterEntries);
    }
}

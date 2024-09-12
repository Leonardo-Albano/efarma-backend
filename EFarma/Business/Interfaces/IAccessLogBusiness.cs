using EFarma.Models;

namespace EFarma.Business.Interfaces
{
    public interface IAccessLogBusiness
    {
        Task<IEnumerable<AccessLog>> GetAllAccessLogs();
    }
}

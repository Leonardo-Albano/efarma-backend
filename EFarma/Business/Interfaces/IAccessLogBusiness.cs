using EFarma.Models;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IAccessLogBusiness
    {
        Task<DataResult<IEnumerable<AccessLog>>> GetAllAccessLogs();
    }
}

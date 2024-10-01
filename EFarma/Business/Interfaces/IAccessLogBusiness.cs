using EFarma.Models;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IAccessLogBusiness
    {
        Task<ResultDataObject<IEnumerable<AccessLog>>> GetAllAccessLogs();
    }
}

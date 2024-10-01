using EFarma.Models;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IRoleBusiness
    {
        Task<VoidResult> CreateRole(Role role);
        Task<DataResult<IEnumerable<KeyValuePair<int, string>>>> GetRoles();
    }
}

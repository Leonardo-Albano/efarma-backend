using EFarma.Models;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IRoleBusiness
    {
        Task<ResultObject> CreateRole(Role role);
        Task<ResultDataObject<IEnumerable<KeyValuePair<int, string>>>> GetRoles();
    }
}

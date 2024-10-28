using EFarma.Models;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IRoleBusiness
    {
        Task<ResultObject> CreateRole(Role role, List<int> PermissionIds);
        Task<ResultObject> DeleteRole(int id);
        Task<ResultDataObject<List<KeyValuePair<int, string>>>> GetRoles();
    }
}

using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Models.Views;

namespace EFarma.Business.Interfaces
{
    public interface IPermissionBusiness
    {
        Task<ResultObject> CreatePermission(Permission permission, IEnumerable<int>? stockRoomIds, IEnumerable<int>? pageIds);
        Task<ResultDataObject<Permission?>> GetPermissionDetailed(int id);
        Task<ResultDataObject<IEnumerable<PermissionView>>> GetPermissions();
        Task<ResultObject> UpdatePermission(int id, Permission updatedPermission, IEnumerable<int>? stockRoomIds, IEnumerable<int>? pageIds);
        Task<ResultObject> DeletePermission(int id);
    }
}

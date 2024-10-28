using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Models.Views;

namespace EFarma.Business.Interfaces
{
    public interface IPermissionBusiness
    {
        Task<ResultObject> CreatePermission(Permission permission, List<int>? stockRoomIds, List<int>? pageIds);
        Task<ResultDataObject<Permission?>> GetPermissionDetailed(int id);
        Task<ResultDataObject<List<PermissionView>>> GetPermissions();
        Task<ResultObject> UpdatePermission(int id, Permission updatedPermission, List<int>? stockRoomIds, List<int>? pageIds);
        Task<ResultObject> DeletePermission(int id);
    }
}

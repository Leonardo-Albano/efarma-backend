using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Models.Views;

namespace EFarma.Business.Interfaces
{
    public interface IPermissionBusiness
    {
        Task<ResultObject> CreatePermission(Permission permission);
        Task<ResultDataObject<IEnumerable<PermissionView>>> GetPermissions();
    }
}

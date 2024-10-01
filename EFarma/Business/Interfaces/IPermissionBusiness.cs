using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Models.Views;

namespace EFarma.Business.Interfaces
{
    public interface IPermissionBusiness
    {
        Task<VoidResult> CreatePermission(Permission permission);
        Task<DataResult<IEnumerable<PermissionView>>> GetPermissions();
    }
}

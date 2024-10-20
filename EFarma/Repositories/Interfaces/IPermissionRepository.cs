using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        Task<Permission?> GetPermissionDetailed(int id);
    }
}

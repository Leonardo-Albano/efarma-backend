using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role?> GetRoleByName(string roleName);
    }
}

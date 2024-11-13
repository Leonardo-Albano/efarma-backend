using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<List<Role>> GetAllDetailed();
        Task<Role?> GetRoleByName(string roleName);
    }
}

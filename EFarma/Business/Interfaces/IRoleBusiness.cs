using EFarma.Models;

namespace EFarma.Business.Interfaces
{
    public interface IRoleBusiness
    {
        Task<int> CreateRole(Role role);
        Task<IEnumerable<KeyValuePair<int, string>>> GetRoles();
    }
}

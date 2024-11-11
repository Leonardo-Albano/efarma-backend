using EFarma.Data;
using EFarma.Models;
using EFarma.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EFarma.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(DbContext context) : base(context)
        {
        }

        public async Task<Role?> GetRoleByName(string roleName)
        {
            return await DataContext.Roles
                .FirstOrDefaultAsync(r=>r.Name == roleName);
        }

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

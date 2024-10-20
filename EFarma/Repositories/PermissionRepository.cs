using EFarma.Data;
using EFarma.Models;
using EFarma.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EFarma.Repositories
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(DbContext context) : base(context)
        {
        }

        public async Task<Permission?> GetPermissionDetailed(int id)
            => await DataContext.Permissions
            .Include(p=>p.Pages)
            .Include(p=>p.StockRooms)
            .FirstOrDefaultAsync(p => p.Id == id);

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

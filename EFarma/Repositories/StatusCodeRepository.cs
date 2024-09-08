using EFarma.Data;
using EFarma.Models;
using EFarma.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EFarma.Repositories
{
    public class StatusCodeRepository : Repository<StatusCode>, IStatusCodeRepository
    {
        public StatusCodeRepository(DbContext context) : base(context)
        {
        }

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

using EFarma.Data;
using EFarma.Models;
using EFarma.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EFarma.Business
{
    public class StatusCodeRepository : Repository<StatusCode>
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

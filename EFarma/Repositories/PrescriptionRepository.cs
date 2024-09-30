using EFarma.Data;
using EFarma.Models;
using EFarma.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EFarma.Repositories
{
    public class PrescriptionRepository : Repository<Prescription>, IPrescriptionRepository
    {
        public PrescriptionRepository(DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Prescription>> GetDetailedPrescriptions()
        {
            return await DataContext.Prescriptions
                .Include(p => p.Items)
                .Take(500)
                .ToArrayAsync();
        }

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

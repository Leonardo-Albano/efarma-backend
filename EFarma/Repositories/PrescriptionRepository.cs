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

        public async Task<IEnumerable<Prescription>> GetDetailedPrescriptions(string? cpf = null, DateTime? date = null)
        {
            var query = DataContext.Prescriptions
                .Include(p => p.Items)
                .Include(p => p.Patient)
                .Include(p => p.Employee)
                .AsQueryable();

            if (!string.IsNullOrEmpty(cpf))
            {
                query = query.Where(p => p.CPF.Replace(".", "").Replace("-", "").Contains(cpf.Replace(".", "").Replace("-", "")));
            }

            if (date.HasValue)
            {
                query = query.Where(p => p.Date == date.Value);
            }

            return await query.Take(500).ToArrayAsync();
        }

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

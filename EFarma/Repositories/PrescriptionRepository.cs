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

        public async Task<Prescription?> GetDetailedPrescriptionById(int id)
        {
            return await DataContext.Prescriptions
                    .Include(p => p.Employee)
                    .Include(p => p.Patient)
                    .Include(p => p.Items)
                        .ThenInclude(i=>i.Medicament)
                    .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Prescription>> GetDetailedPrescriptions(string? cpf = null, DateTime? date = null)
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

            return await query.Take(500).ToListAsync();
        }

        public async Task<List<Prescription>> GetUnresolvedPrescriptionsByTakeOutResponsibleId(int takeOutResponsibleId)
        {
            return await DataContext.Prescriptions
                    .Include(p=>p.Items)
                        .ThenInclude(i=>i.Medicament)
                    .Where(p=>p.TakeOutResponsibleId == takeOutResponsibleId && p.Status == Prescription.UnresolvedMessage)
                    .ToListAsync();
        }

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

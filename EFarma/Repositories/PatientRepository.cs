using EFarma.Data;
using EFarma.Models;
using EFarma.Repositories;
using EFarma.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EFarma.Repository
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(DbContext context) : base(context)
        {
        }

        public async Task<Patient?> GetByCPF(string cpf)
        {
            return await DataContext.Patients
                .FirstOrDefaultAsync(p=>p.CPF == cpf);
        }

        public async Task<List<Patient>> GetFiltered(string? cpf, string? name)
        {
            return await DataContext.Patients
                .Where(p =>
                    (!string.IsNullOrEmpty(name) && p.Name.ToLower().Trim().Contains(name.ToLower().Trim())) ||
                    (!string.IsNullOrEmpty(cpf) && p.CPF == cpf) ||
                    (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(cpf))
                )
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

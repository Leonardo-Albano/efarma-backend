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

        public async Task<Patient?> GetPatientByCPF(string cpf)
        {
            return await DataContext.Patients
                .FirstOrDefaultAsync(p=>p.CPF == cpf);
        }

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

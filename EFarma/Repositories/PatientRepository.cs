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

        public IEnumerable<Patient> GetAllPatients()
        {
            throw new NotImplementedException();
        }

        public Patient GetPatientById(int id)
        {
            throw new NotImplementedException();
        }

        public DataContext DataContext
        {
            get { return _context as DataContext; }
    }
}

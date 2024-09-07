using EFarma.Data;
using EFarma.Repositories.Interfaces;
using EFarma.Repository;

namespace EFarma.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly DataContext _context;

        public UnitOfWork(DataContext context)
        {
            _context = context;
            Patients = new PatientRepository(_context);
        }

        public IPatientRepository Patients { get; private set };

        public void Dispose()
        {
            _context.Dispose();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}

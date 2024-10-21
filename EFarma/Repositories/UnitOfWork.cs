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
            AccessLogs = new AccessLogRepository(_context);
            Employees = new EmployeeRepository(_context);
            InStockItems = new InStockItemRepository(_context);
            Medicaments = new MedicamentRepository(_context);
            Pages = new PageRepository(_context);
            Patients = new PatientRepository(_context);
            Permissions = new PermissionRepository(_context);
            Prescriptions = new PrescriptionRepository(_context);
            PrescriptionItems = new PrescriptionItemRepository(_context);
            Roles = new RoleRepository(_context);
            StockRooms = new StockRoomRepository(_context);
        }

        public IAccessLogRepository AccessLogs { get; private set; }
        public IEmployeeRepository Employees { get; private set; }
        public IInStockItemRepository InStockItems { get; private set; }
        public IMedicamentRepository Medicaments { get; private set; }
        public IPageRepository Pages { get; private set; }
        public IPatientRepository Patients { get; private set; }
        public IPermissionRepository Permissions { get; private set; }
        public IPrescriptionRepository Prescriptions { get; private set; }
        public IPrescriptionItemRepository PrescriptionItems { get; private set; }
        public IRoleRepository Roles { get; private set; }
        public IStockRoomRepository StockRooms { get; private set; }

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

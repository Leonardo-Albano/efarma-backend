namespace EFarma.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAccessLogRepository AccessLogs { get; }
        IEmployeeRepository Employees { get; }
        IInStockItemRepository InStockItems { get; }
        IMedicamentRepository Medicaments { get; }
        IPatientRepository Patients { get; }
        IPermissionRepository Permissions { get; }
        IPrescriptionRepository Prescriptions { get; }
        IPrescriptionItemRepository PrescriptionItems { get; }
        IRoleRepository Roles { get; }
        IStatusCodeRepository StatusCodes { get; }
        Task<int> SaveChangesAsync();
    }
}

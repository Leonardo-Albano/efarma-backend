namespace EFarma.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAccessLogRepository AccessLogs { get; }
        IEmployeeRepository Employees { get; }
        IInStockItemsRepository InStockItems { get; }
        IMedicamentRepository Medicaments { get; }
        IPatientRepository Patients { get; }
        IPermissionRepository Permissions { get; }
        IRoleRepository Roles { get; }
        IStatusCodeRepository StatusCodes { get; }
        Task<int> SaveChangesAsync();
    }
}

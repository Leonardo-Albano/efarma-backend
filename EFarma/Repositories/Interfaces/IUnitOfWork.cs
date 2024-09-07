namespace EFarma.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IPatientRepository Patients { get; }
        Task<int> SaveChangesAsync();
    }
}

using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IPrescriptionRepository : IRepository<Prescription>
    {
        public Task<IEnumerable<Prescription>> GetDetailedPrescriptions();
    }
}

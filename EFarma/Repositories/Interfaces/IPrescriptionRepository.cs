using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IPrescriptionRepository : IRepository<Prescription>
    {
        Task<Prescription?> GetDetailedPrescriptionById(int id);
        Task<List<Prescription>> GetDetailedPrescriptions(string? cpf = null, DateTime? date = null);
        Task<List<Prescription>> GetPendentPrescriptionsByTakeOutResponsibleId(int takeOutResponsibleId);
    }
}

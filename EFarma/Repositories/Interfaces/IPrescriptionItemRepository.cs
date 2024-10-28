using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IPrescriptionItemRepository : IRepository<PrescriptionItem>
    {
        Task<List<PrescriptionItem>> GetPrescriptionItems(int prescriptionId);
    }
}

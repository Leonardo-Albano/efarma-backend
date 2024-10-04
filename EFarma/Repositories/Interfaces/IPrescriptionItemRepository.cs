using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IPrescriptionItemRepository : IRepository<PrescriptionItem>
    {
        Task<IEnumerable<PrescriptionItem>> GetPrescriptionItems(int prescriptionId);
    }
}

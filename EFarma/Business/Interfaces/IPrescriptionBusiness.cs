using EFarma.Models;
using EFarma.Models.Views;

namespace EFarma.Business.Interfaces
{
    public interface IPrescriptionBusiness
    {
        Task<Prescription> CreatePrescription(Prescription prescription);
        Task<IEnumerable<PrescriptionView>> GetPrescriptions();
    }
}

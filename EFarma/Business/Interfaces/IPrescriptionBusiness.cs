using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Models.Views;

namespace EFarma.Business.Interfaces
{
    public interface IPrescriptionBusiness
    {
        Task<VoidResult> CreatePrescription(Prescription prescription);
        Task<DataResult<IEnumerable<PrescriptionView>>> GetPrescriptions();
    }
}

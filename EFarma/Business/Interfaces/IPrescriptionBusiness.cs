using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Models.Views;

namespace EFarma.Business.Interfaces
{
    public interface IPrescriptionBusiness
    {
        Task<ResultObject> CreatePrescription(Prescription prescription);
        Task<ResultDataObject<IEnumerable<PrescriptionItemView>>> GetPrescriptionItems(int prescriptionId);
        Task<ResultDataObject<IEnumerable<PrescriptionView>>> GetPrescriptions(string? cpf, DateTime? date);
    }
}

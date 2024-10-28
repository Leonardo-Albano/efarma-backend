using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Models.Views;

namespace EFarma.Business.Interfaces
{
    public interface IPrescriptionBusiness
    {
        Task<ResultObject> CreatePrescription(Prescription prescription);
        Task<ResultObject> DeletePrescription(int id);
        Task<ResultDataObject<List<PrescriptionItemView>>> GetPrescriptionItems(int prescriptionId);
        Task<ResultDataObject<List<PrescriptionView>>> GetPrescriptions(string? cpf, DateTime? date);
    }
}

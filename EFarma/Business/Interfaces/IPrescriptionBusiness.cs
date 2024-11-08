using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models.Views;

namespace EFarma.Business.Interfaces
{
    public interface IPrescriptionBusiness
    {
        Task<ResultObject> CreatePrescription(Prescription prescription);
        Task<ResultObject> DeletePrescription(int id);
        Task<ResultDataObject<PrescriptionViewDetailed?>> GetPrescriptionDetailed(int id);
        Task<ResultDataObject<List<PrescriptionItemView>>> GetPrescriptionItems(int prescriptionId);
        Task<ResultDataObject<List<PrescriptionView>>> GetPrescriptions(string? cpf, DateTime? date, bool filterPendent);
        Task<ResultDataObject<List<WithdrawItem>>> WithdrawPrescription(RemovePrescriptionItemsDTO prescriptionItemsDTO);
    }
}

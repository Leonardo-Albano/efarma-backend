using EFarma.Models;

namespace EFarma.Business.Interfaces
{
    public interface IPrescriptionBusiness
    {
        Task<Prescription> CreatePrescription(Prescription prescription);
    }
}

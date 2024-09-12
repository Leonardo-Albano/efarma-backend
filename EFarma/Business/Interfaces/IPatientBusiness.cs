using EFarma.Models;

namespace EFarma.Business.Interfaces
{
    public interface IPatientBusiness
    {
        Task<IEnumerable<Patient>> GetAllPatients();
    }
}

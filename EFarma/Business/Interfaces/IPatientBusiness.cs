using EFarma.Models;
using EFarma.Models.Resource;

namespace EFarma.Business.Interfaces
{
    public interface IPatientBusiness
    {
        Task<bool> CreatePatient(PatientDTO patient);
        Task<IEnumerable<Patient>> GetAllPatients();
    }
}

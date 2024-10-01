using EFarma.Models;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IPatientBusiness
    {
        Task<VoidResult> CreatePatient(Patient patient);
        Task<DataResult<IEnumerable<Patient>>> GetAllPatients();
        Task<DataResult<Patient>> GetPatient(string cpf);
    }
}

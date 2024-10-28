using EFarma.Models;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IPatientBusiness
    {
        Task<ResultObject> CreatePatient(Patient patient);
        Task<ResultObject> DeletePatient(int id);
        Task<ResultDataObject<List<Patient>>> GetAllPatients();
        Task<ResultDataObject<Patient>> GetPatient(string cpf);
        Task<ResultDataObject<Patient?>> UpdatePatient(Patient patient);
    }
}

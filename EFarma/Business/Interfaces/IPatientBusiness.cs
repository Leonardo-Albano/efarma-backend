using EFarma.Models;

namespace EFarma.Business.Interfaces
{
    public interface IPatientBusiness
    {
        Task<int> CreatePatient(Patient patient);
        Task<IEnumerable<Patient>> GetAllPatients();
        Task<Patient?> GetPatient(string cpf);
    }
}

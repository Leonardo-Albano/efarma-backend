using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        IEnumerable<Patient> GetAllPatients();
        Patient GetPatientById(int id);
    }
}

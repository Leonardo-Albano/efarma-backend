using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IPatientRepository : IRepository<Patient>
    {
        IEnumerable<Patient> GetAllPatients();
        Patient GetPatientById(int id);
    }
}

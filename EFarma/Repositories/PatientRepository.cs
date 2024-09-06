using EFarma.Models;
using EFarma.Repositories.Interfaces;

namespace EFarma.Repository
{
    public class PatientRepository : IPatientRepository
    {
        public IEnumerable<Patient> GetAllPatients()
        {
            throw new NotImplementedException();
        }

        public Patient GetPatientById(int id)
        {
            throw new NotImplementedException();
        }
    }
}

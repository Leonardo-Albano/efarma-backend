using EFarma.Models;

namespace EFarma.Business.Interfaces
{
    public interface IPatientBusiness
    {
        IEnumerable<Patient> GetAllPatients();
    }
}

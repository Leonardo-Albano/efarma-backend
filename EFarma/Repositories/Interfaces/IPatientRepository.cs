using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient?> GetPatientByCPF(string cpf);
    }
}

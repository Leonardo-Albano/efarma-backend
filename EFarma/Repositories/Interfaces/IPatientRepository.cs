using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient?> GetByCPF(string cpf);
        Task<List<Patient>> GetFiltered(string? cpf, string? name);
    }
}

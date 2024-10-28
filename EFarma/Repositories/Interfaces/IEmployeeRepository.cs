using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<List<Employee?>> GetEmployeeByCpfOrName(string? cpf, string? name);
        Task<Employee?> GetEmployeeByTagCode(string code);
    }
}

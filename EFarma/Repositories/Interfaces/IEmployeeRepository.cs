using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<List<Employee>> GetAllDetailed();
        Task<List<Employee>> GetFiltered(string? cpf, string? name);
        Task<Employee?> GetEmployeeByTagCode(string code);
        Task<Employee?> GetEmployeeDetailedByMail(string mail);
    }
}

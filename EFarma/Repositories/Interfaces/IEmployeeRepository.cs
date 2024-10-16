using EFarma.Models;

namespace EFarma.Repositories.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<Employee?> GetEmployeeByTagCode(string code);
    }
}

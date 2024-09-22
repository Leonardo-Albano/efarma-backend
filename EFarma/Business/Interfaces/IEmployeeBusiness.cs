using EFarma.Models;

namespace EFarma.Business.Interfaces
{
    public interface IEmployeeBusiness
    {
        Task<int> CreateEmployee(Employee employee);
    }
}

using EFarma.Models;

namespace EFarma.Business.Interfaces
{
    public interface IEmployeeBusiness : IPersonBusiness
    {
        Task<int> CreateEmployee(Employee employee);
    }
}

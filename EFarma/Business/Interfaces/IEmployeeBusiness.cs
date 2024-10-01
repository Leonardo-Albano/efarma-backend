using EFarma.Models;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IEmployeeBusiness : IPersonBusiness
    {
        Task<VoidResult> CreateEmployee(Employee employee);
        Task<DataResult<Employee>> GetEmployee(string cpf);
    }
}

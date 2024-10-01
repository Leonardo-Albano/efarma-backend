using EFarma.Models;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IEmployeeBusiness : IPersonBusiness
    {
        Task<ResultObject> CreateEmployee(Employee employee);
        Task<ResultDataObject<Employee>> GetEmployee(string cpf);
    }
}

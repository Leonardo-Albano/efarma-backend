using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IEmployeeBusiness : IPersonBusiness
    {
        Task<ResultObject> CreateEmployee(Employee employee);
        Task<ResultObject> DeleteEmployee(int id);
        Task<ResultDataObject<Employee>> GetEmployee(string cpf);
        Task<ResultDataObject<List<Employee>>> GetEmployees();
        Task<ResultObject> Login(LoginDTO loginDTO);
        Task<ResultDataObject<Employee?>> UpdateEmployee(Employee employee);
        Task<ResultObject> UpdatePassword(EmployeeUpdatePasswordDTO updatePasswordDTO);
    }
}

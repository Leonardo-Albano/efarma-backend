using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;

namespace EFarma.Business.Interfaces
{
    public interface IEmployeeBusiness : IPersonBusiness
    {
        Task<ResultObject> CreateEmployee(Employee employee);
        Task<ResultObject> DeleteEmployee(int id);
        Task<ResultDataObject<byte[]?>> ExportEmployees();
        Task<ResultDataObject<Employee?>> GetDoctorByCrm(string crm);
        Task<ResultDataObject<Employee>> GetEmployee(string cpf);
        Task<ResultDataObject<List<Employee>>> GetEmployees();
        Task<ResultDataObject<List<string>>> ImportEmployees(byte[] csvData);
        Task<ResultDataObject<Employee?>> Login(LoginDTO loginDTO);
        Task<ResultObject> ResetPassword(int employeeId);
        Task<ResultObject> UpdateEmployee(int id, EmployeeDTO employee);
        Task<ResultObject> UpdatePassword(EmployeeUpdatePasswordDTO updatePasswordDTO);
    }
}

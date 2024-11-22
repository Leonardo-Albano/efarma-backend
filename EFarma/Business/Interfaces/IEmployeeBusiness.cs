using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models.Views;

namespace EFarma.Business.Interfaces
{
    public interface IEmployeeBusiness : IPersonBusiness
    {
        Task<ResultObject> Create(Employee employee);
        Task<ResultObject> Delete(int id);
        Task<ResultDataObject<byte[]?>> Export();
        Task<ResultDataObject<Employee?>> GetDoctorByCrm(string crm);
        Task<ResultDataObject<Employee?>> GetByCPF(string cpf);
        Task<ResultDataObject<List<Employee>>> GetAll();
        Task<ResultDataObject<List<ImportError>>> ImportEmployees(byte[] csvData);
        Task<ResultDataObject<Employee?>> Login(LoginDTO loginDTO);
        Task<ResultObject> ResetPassword(int employeeId);
        Task<ResultObject> Update(int id, EmployeeDTO employeeDto);
        Task<ResultObject> UpdatePassword(EmployeeUpdatePasswordDTO updatePasswordDTO);
        Task<ResultDataObject<DoctorInfoView?>> ValidateCrm(string crm);
    }
}

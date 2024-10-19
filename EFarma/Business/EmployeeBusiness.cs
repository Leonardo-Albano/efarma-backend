using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Config;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class EmployeeBusiness : PersonBusiness, IEmployeeBusiness
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public EmployeeBusiness(ILogger<EmployeeController> logger, IUnitOfWork repository, IMapper mapper, IPasswordHasher passwordHasher) : base(logger, repository, mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResultObject> CreateEmployee(Employee employee)
        {
            var existentEmployee = await _repository.Employees.Find(e => e.CPF == employee.CPF ||
                                     (!string.IsNullOrEmpty(employee.EmployeeId) && e.EmployeeId == employee.EmployeeId));
            if (existentEmployee.Any())
            {
                return new ResultObject
                {
                    Message = "CPF already exists in the system.",
                    StatusCode = 409,
                    Success = false
                };
            }

            var existentTagCode = await _repository.Employees.GetEmployeeByTagCode(employee.TagCode);
            if (existentTagCode != null)
            {
                return new ResultObject
                {
                    Message = "This tag code belong to another employee.",
                    StatusCode = 409,
                    Success = false
                };
            }

            employee.PasswordHash = _passwordHasher.Hash(employee.CPF.Replace(".", "").Replace("-", ""));
            _repository.Employees.Add(employee);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Employee created successfully." : "An error occurred while creating the employee.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultObject> DeleteEmployee(int id)
        {
            var employee = await _repository.Employees.FirstOrDefault(e=>e.Id == id);
            if (employee == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Employee not found.",
                    Success = false
                };
            }

            _repository.Employees.Remove(employee);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Employee deleted successfully." : "An error occurred while deleting the employee.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<Employee>> GetEmployee(string cpf)
        {
            var employee = await _repository.Employees.FirstOrDefault(e => e.CPF == cpf);
            bool success = employee != null;

            return new()
            {
                Message = success ? "Employee found." : "No employee found.",
                Data = employee,
                Success = success,
                StatusCode = success ? 200 : 404
            };
        }

        public async Task<ResultObject> Login(LoginDTO loginDTO)
        {
            var employee = await _repository.Employees.FirstOrDefault(e => e.CPF == loginDTO.Login);
            if (employee == null)
            {
                return new()
                {
                    Message = "Employee not found.",
                    StatusCode = 404,
                    Success = false
                };
            }

            var result = _passwordHasher.Verify(employee.PasswordHash, loginDTO.Password);

            return new()
            {
                Message = result ? "Access allowed." : "Access denied.",
                StatusCode = result ? 200 : 403,
                Success = result
            };
        }

        public async Task<ResultDataObject<Employee?>> UpdateEmployee(Employee existingEmployee)
        {
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultDataObject<Employee?>
            {
                Message = success ? "Employee updated successfully." : "An error occurred while updating the employee.",
                Data = success ? existingEmployee : null,
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }
    }
}

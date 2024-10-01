using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Config;
using EFarma.Controllers;
using EFarma.Models;
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
            var existent_employee = await _repository.Employees.Find(e => e.CPF == employee.CPF ||
                                     (!string.IsNullOrEmpty(employee.EmployeeId) && e.EmployeeId == employee.EmployeeId));
            if (existent_employee.Any())
            {
                return new ResultObject
                {
                    Message = "CPF already exists in the system.",
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
    }
}

using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Config;
using EFarma.Controllers;
using EFarma.Models;
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

        public async Task<int> CreateEmployee(Employee employee)
        {
            var existent_employee = await _repository.Employees.Find(e => e.CPF == employee.CPF ||
                                     (!string.IsNullOrEmpty(employee.EmployeeId) && e.EmployeeId == employee.EmployeeId));
            if (existent_employee.Any())
            {
                return 409;
            }
            
            employee.PasswordHash = _passwordHasher.Hash(employee.CPF.Replace(".", "").Replace("-", ""));
            _repository.Employees.Add(employee);

            return await _repository.SaveChangesAsync() > 0 ? 200 : 500;
        }

        public async Task<Employee?> GetEmployee(string cpf) 
            => await _repository.Employees.FirstOrDefault(e => e.CPF == cpf);
    }
}

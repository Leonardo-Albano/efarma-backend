using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class EmployeeBusiness : PersonBusiness, IEmployeeBusiness
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IUnitOfWork _repository;

        public EmployeeBusiness(ILogger<EmployeeController> logger, IUnitOfWork repository) : base(logger, repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<int> CreateEmployee(Employee employee)
        {
            var existent_employee = await _repository.Employees.Find(e => e.CPF == employee.CPF ||
                                                                    (!string.IsNullOrEmpty(employee.EmployeeId) && e.EmployeeId == employee.EmployeeId));
            if (existent_employee.Any())
            {
                return 409;
            }
            
            employee.PasswordHash = employee.CPF;

            _repository.Employees.Add(employee);

            return await _repository.SaveChangesAsync() > 0 ? 200 : 500;
        }


    }
}

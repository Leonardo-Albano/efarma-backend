using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class EmployeeBusiness : IEmployeeBusiness
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeRepository _repository;

        public EmployeeBusiness(ILogger<EmployeeController> logger, IEmployeeRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }


    }
}

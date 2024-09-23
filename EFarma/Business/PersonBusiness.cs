using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models.Views;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class PersonBusiness : IPersonBusiness 
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IUnitOfWork _repository;

        public PersonBusiness(ILogger<EmployeeController> logger, IUnitOfWork repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public Task<IEnumerable<PersonView>> GetPersonList()
        {
            var persons = _repository;
            return null;
        }
    }
}

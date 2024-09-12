using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class PatientBusiness : IPatientBusiness
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IPatientRepository _repository;
        public PatientBusiness(ILogger<PatientController> logger, IPatientRepository repository) 
        { 
            _logger = logger;
            _repository = repository;
        }

        public async Task<IEnumerable<Patient>> GetAllPatients()
        {
            return await _repository.GetAll();
        }
    }
}

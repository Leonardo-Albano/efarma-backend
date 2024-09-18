using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.Resource;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class PatientBusiness : IPatientBusiness
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IUnitOfWork _repository;
        public PatientBusiness(ILogger<PatientController> logger, IUnitOfWork repository) 
        { 
            _logger = logger;
            _repository = repository;
        }

        public async Task<int> CreatePatient(Patient patient)
        {
            var existent_patient = await _repository.Patients.Find(p=>p.CPF == patient.CPF);
            if(existent_patient.Any())
            {
                return 409;
            } 

            _repository.Patients.Add(patient);
            
            return await _repository.SaveChangesAsync() > 0 ? 200 : 500;
        }

        public async Task<IEnumerable<Patient>> GetAllPatients()
        {
            return await _repository.Patients.GetAll();
        }
    }
}

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

        public async Task<int> CreatePatient(Patient patientDTO)
        {
            Patient patient = new()
            {
                BirthDay = patientDTO.BirthDay,
                CPF = patientDTO.CPF,
                Id = 1,
                Mail = patientDTO.Mail,
                Name = patientDTO.Name,
                Observations = patientDTO.Observations,
                PhoneNumber = patientDTO.PhoneNumber
            };

            var existent_patient = _repository.Patients.Find(patient=>patient.CPF.Equals(patient.CPF));
            if(existent_patient != null)
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

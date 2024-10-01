using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class PatientBusiness : IPatientBusiness
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        public PatientBusiness(ILogger<PatientController> logger, IUnitOfWork repository, IMapper mapper) 
        { 
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<VoidResult> CreatePatient(Patient patient)
        {
            var existent_patients = await _repository.Patients.FirstOrDefault(p=>p.CPF == patient.CPF);
            if(existent_patients != null)
            {
                return new VoidResult
                {
                    Message = "CPF already exists in the system.",
                    StatusCode = 409,
                    Success = false
                };
            } 

            _repository.Patients.Add(patient);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new VoidResult
            {
                Message = success ? "Employee created successfully." : "An error occurred while creating the employee.",
                Success = success,
                StatusCode = success ? 200 : 500
            };
        }

        public async Task<DataResult<IEnumerable<Patient>>> GetAllPatients()
        {
            var patients = await _repository.Patients.GetAll();

            bool success = patients.Any();

            return new()
            {
                Message = success ? "Patients found." : "No patients found.",
                Result = patients,
                Success = success,
                StatusCode = success ? 200 : 404
            };
        }

        public async Task<DataResult<Patient>> GetPatient(string cpf)
        {
            var patient = await _repository.Patients.FirstOrDefault(p => p.CPF == cpf);
            bool success = patient != null;

            return new()
            {
                Message = success ? "Patient found." : "No patient found.",
                Result = patient,
                Success = success,
                StatusCode = success ? 200 : 404
            };
        }
    }
}

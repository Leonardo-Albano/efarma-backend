using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
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

        public async Task<int> CreatePatient(Patient patient)
        {
            var existent_patients = await _repository.Patients.FirstOrDefault(p=>p.CPF == patient.CPF);
            if(existent_patients != null)
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

        public async Task<Patient?> GetPatient(string cpf)
            => await _repository.Patients.FirstOrDefault(p => p.CPF == cpf);
    }
}

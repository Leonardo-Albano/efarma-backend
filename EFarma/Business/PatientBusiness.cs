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

        public async Task<ResultObject> CreatePatient(Patient patient)
        {
            var existent_patients = await _repository.Patients.FirstOrDefault(p => p.CPF == patient.CPF);
            if (existent_patients != null)
            {
                return new ResultObject
                {
                    Message = "CPF já existe no sistema.",
                    StatusCode = 409,
                    Success = false
                };
            }

            _repository.Patients.Add(patient);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Paciente criado com sucesso." : "Ocorreu um erro ao criar o paciente.",
                Success = success,
                StatusCode = success ? 200 : 500
            };
        }

        public async Task<ResultObject> DeletePatient(int id)
        {
            var patient = await _repository.Patients.FirstOrDefault(e => e.Id == id);
            if (patient == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Paciente não encontrado.",
                    Success = false
                };
            }

            _repository.Patients.Remove(patient);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Paciente excluído com sucesso." : "Ocorreu um erro ao excluir o paciente.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<List<Patient>>> GetAllPatients()
        {
            var patients = await _repository.Patients.GetAll();

            bool success = patients.Any();

            return new()
            {
                Message = success ? "Pacientes encontrados." : "Nenhum paciente encontrado.",
                Data = patients,
                Success = success,
                StatusCode = success ? 200 : 404
            };
        }

        public async Task<ResultDataObject<Patient>> GetPatient(string cpf)
        {
            var patient = await _repository.Patients.FirstOrDefault(p => p.CPF == cpf);
            bool success = patient != null;

            return new()
            {
                Message = success ? "Paciente encontrado." : "Nenhum paciente encontrado.",
                Data = patient,
                Success = success,
                StatusCode = success ? 200 : 404
            };
        }

        public async Task<ResultDataObject<Patient?>> UpdatePatient(Patient patient)
        {
            _repository.Patients.Update(patient);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultDataObject<Patient?>
            {
                Message = success ? "Paciente atualizado com sucesso." : "Ocorreu um erro ao atualizar o paciente.",
                Data = success ? patient : null,
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }
    }
}

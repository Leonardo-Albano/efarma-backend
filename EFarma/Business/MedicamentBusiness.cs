using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Repositories.Interfaces;
using System.Data;

namespace EFarma.Business
{
    public class MedicamentBusiness : IMedicamentBusiness
    {
        private readonly ILogger<Medicament> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        public MedicamentBusiness(ILogger<Medicament> logger, IUnitOfWork repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResultObject> CreateMedicament(MedicamentDTO medicamentDTO)
        {
            var medicament = _mapper.Map<Medicament>(medicamentDTO);

            var existant_medicament = await _repository.Medicaments.FirstOrDefault(m =>
                m.Description == medicamentDTO.Description &&
                m.Dosage == medicamentDTO.Dosage &&
                m.Measure == medicamentDTO.Measure
            );

            if (existant_medicament != null)
            {
                return new ResultObject
                {
                    Message = "Este medicamento já existe no sistema.",
                    StatusCode = 409,
                    Success = false
                };
            }

            _repository.Medicaments.Add(medicament);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Medicamento criado com sucesso." : "Ocorreu um erro ao criar o medicamento.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultObject> DeleteMedicament(int id)
        {
            var medicament = await _repository.Medicaments.FirstOrDefault(e => e.Id == id);
            if (medicament == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Medicamento não encontrado.",
                    Success = false
                };
            }

            _repository.Medicaments.Remove(medicament);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Medicamento excluído com sucesso." : "Ocorreu um erro ao excluir o medicamento.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<List<Medicament>>> GetAllMedicaments()
        {
            var medicaments = await _repository.Medicaments.GetAll();

            bool success = medicaments.Any();

            return new()
            {
                Message = success ? "Medicamentos encontrados." : "Nenhum medicamento encontrado.",
                Data = medicaments,
                StatusCode = success ? 200 : 400,
                Success = success
            };
        }

        public async Task<ResultObject> UpdateMedicament(int id, MedicamentDTO medicamentDto)
        {
            var existingMedicament = await _repository.Medicaments.FirstOrDefault(p => p.Id == id);

            if (existingMedicament == null)
            {
                return new ResultObject
                {
                    Message = "Medicamento não foi encontrado.",
                    StatusCode = 404,
                    Success = false
                };
            }

            existingMedicament.Description = medicamentDto.Description;
            existingMedicament.Dosage = medicamentDto.Dosage;
            existingMedicament.Measure = medicamentDto.Measure;

            _repository.Medicaments.Update(existingMedicament);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Medicamento atualizado com sucesso." : "Um erro ocorreu durante a atualização do medicamento.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }
    }
}

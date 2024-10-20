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

            if(existant_medicament != null)
            {
                return new ResultObject
                {
                    Message = "This medicament already exists in the system.",
                    StatusCode = 409,
                    Success = false
                };
            }

            _repository.Medicaments.Add(medicament);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Medicament created successfully." : "An error occurred while creating the medicament.",
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
                    Message = "Medicament not found.",
                    Success = false
                };
            }

            _repository.Medicaments.Remove(medicament);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Medicament deleted successfully." : "An error occurred while deleting the medicament.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<IEnumerable<Dictionary<int, string>>>> GetAllMedicaments()
        {
            var medicaments = await _repository.Medicaments.GetAll();
            var result = medicaments.Select(m => new Dictionary<int, string>
            {
                { m.Id, $"{m.Description} {(int)m.Dosage}{m.Measure}" }
            });

            bool success = result.Any();
            
            return new()
            {
                Message = success ? "Medicaments found" : "No medicaments found.",
                Data = result,
                StatusCode = success ? 200 : 400,
                Success = success
            };
        }
    }
}

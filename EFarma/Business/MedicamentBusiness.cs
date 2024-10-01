using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Repositories.Interfaces;

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

        public async Task<DataResult<IEnumerable<Dictionary<int, string>>>> GetAllMedicaments()
        {
            var medicaments = await _repository.Medicaments.GetAll();
            var result = medicaments.Select(m => new Dictionary<int, string>
            {
                { m.Id, $"{m.Description} {m.Dosage}{m.Measure}" }
            });

            bool success = result.Any();
            
            return new()
            {
                Message = success ? "Medicaments found" : "No medicaments found.",
                Result = result,
                StatusCode = success ? 200 : 400,
                Success = success
            };
        }
    }
}

using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class MedicamentBusiness : IMedicamentBusiness
    {
        private readonly ILogger<Medicament> _logger;
        private readonly IUnitOfWork _repository;

        public MedicamentBusiness(ILogger<Medicament> logger, IUnitOfWork repository)
        {
            _logger = logger;
            _repository = repository;
        }


    }
}

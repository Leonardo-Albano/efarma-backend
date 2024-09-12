using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class MedicamentBusiness : IMedicamentBusiness
    {
        private readonly ILogger<Medicament> _logger;
        private readonly IMedicamentRepository _medicamentRepository;

        public MedicamentBusiness(ILogger<Medicament> logger, IMedicamentRepository medicamentRepository)
        {
            _logger = logger;
            _medicamentRepository = medicamentRepository;
        }


    }
}

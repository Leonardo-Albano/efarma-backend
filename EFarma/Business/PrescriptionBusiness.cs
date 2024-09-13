using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class PrescriptionBusiness : IPrescriptionBusiness
    {
        private readonly ILogger<PrescriptionController> _logger;
        private readonly IPrescriptionRepository _repository;

        public PrescriptionBusiness(ILogger<PrescriptionController> logger, IPrescriptionRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

    }
}

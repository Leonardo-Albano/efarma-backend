using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class PrescriptionBusiness : IPrescriptionBusiness
    {
        private readonly ILogger<PrescriptionController> _logger;
        private readonly IUnitOfWork _repository;

        public PrescriptionBusiness(ILogger<PrescriptionController> logger, IUnitOfWork repository)
        {
            _logger = logger;
            _repository = repository;
        }

    }
}

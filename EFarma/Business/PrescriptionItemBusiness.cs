using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class PrescriptionItemBusiness : IPrescriptionBusiness
    {
        private readonly ILogger<PrescriptionController> _logger;
        private readonly IPrescriptionItemRepository _repository;

        public PrescriptionItemBusiness(ILogger<PrescriptionController> logger, IPrescriptionItemRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
    }
}

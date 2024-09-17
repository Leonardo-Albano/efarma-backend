using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class PermissionBusiness : IPermissionBusiness
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly IUnitOfWork _repository;

        public PermissionBusiness(ILogger<PermissionController> logger, IUnitOfWork repository)
        {
            _logger = logger;
            _repository = repository;
        }
    }
}

using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class PermissionBusiness : IPermissionBusiness
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly IPermissionRepository _repository;

        public PermissionBusiness(ILogger<PermissionController> logger, IPermissionRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
    }
}

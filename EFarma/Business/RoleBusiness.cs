using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class RoleBusiness : IRoleBusiness
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IUnitOfWork _business;

        public RoleBusiness(ILogger<RoleController> logger, IUnitOfWork business)
        {
            _logger = logger;
            _business = business;
        }
    }
}

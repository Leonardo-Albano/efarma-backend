using EFarma.Business.Interfaces;
using EFarma.Controllers;

namespace EFarma.Business
{
    public class RoleBusiness : IRoleBusiness
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IRoleBusiness _business;

        public RoleBusiness(ILogger<RoleController> logger, IRoleBusiness business)
        {
            _logger = logger;
            _business = business;
        }
    }
}

using EFarma.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly IPermissionBusiness _business;

        public PermissionController(ILogger<PermissionController> logger, IPermissionBusiness business)
        {
            _logger = logger;
            _business = business;
        }
    }
}

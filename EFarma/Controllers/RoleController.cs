using EFarma.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IRoleBusiness _business;

        public RoleController(ILogger<RoleController> logger, IRoleBusiness business)
        {
            _logger = logger;
            _business = business;
        }
    }
}

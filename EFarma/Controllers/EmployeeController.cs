using EFarma.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeBusiness _business;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeBusiness business)
        {
            _logger = logger;
            _business = business;
        }
    }
}

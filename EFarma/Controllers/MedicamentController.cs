using EFarma.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicamentController : ControllerBase
    {
        private readonly ILogger<MedicamentController> _logger;
        private readonly IMedicamentBusiness _business;

        public MedicamentController(ILogger<MedicamentController> logger, IMedicamentBusiness business)
        {
            _logger = logger;
            _business = business;
        }
    }
}

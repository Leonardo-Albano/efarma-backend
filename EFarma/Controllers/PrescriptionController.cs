using EFarma.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly ILogger<PrescriptionController> _logger;
        private readonly IPrescriptionBusiness _business;

        public PrescriptionController(ILogger<PrescriptionController> logger, IPrescriptionBusiness business)
        {
            _logger = logger;
            _business = business;
        }
    }
}

using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.Response;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;

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

        [HttpGet]
        public async Task<ActionResult<ResultDataObject<IEnumerable<Dictionary<int, string>>>>> GetAllMedicaments()
        {
            var result = await _business.GetAllMedicaments();
  
            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }
    }
}

using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessLogController : ControllerBase
    {
        private readonly ILogger<AccessLogController> _logger;
        private readonly IAccessLogBusiness _business;

        public AccessLogController(ILogger<AccessLogController> logger, IAccessLogBusiness business)
        {
            _logger = logger;
            _business = business;
        }

        [HttpGet]
        public async Task<ActionResult<ResultDataObject<IEnumerable<AccessLog>>>> GetAllAccessLogs()
        {
            var accessLogs = await _business.GetAllAccessLogs();

            return StatusCode(
                statusCode: accessLogs.StatusCode,
                value: accessLogs
            );
        }
    }
}

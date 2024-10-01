using EFarma.Business.Interfaces;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly IPermissionBusiness _business;
        private readonly IMapper _mapper;

        public PermissionController(ILogger<PermissionController> logger, IPermissionBusiness business, IMapper mapper)
        {
            _logger = logger;
            _business = business;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ResultObject>> CreatePermission([FromBody] PermissionDTO permissionDto)
        {
            var permission = _mapper.Map<Permission>(permissionDto);
            var result = await _business.CreatePermission(permission);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpGet]
        public async Task<ActionResult<ResultDataObject<IEnumerable<KeyValuePair<int, string>>>>> GetPermissions()
        {
            var result = await _business.GetPermissions();

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }
    }
}

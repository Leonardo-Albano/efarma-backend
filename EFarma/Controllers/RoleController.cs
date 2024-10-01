using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IRoleBusiness _business;
        private readonly IMapper _mapper;

        public RoleController(ILogger<RoleController> logger, IRoleBusiness business, IMapper mapper)
        {
            _logger = logger;
            _business = business;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ResultObject>> CreateRole([FromBody] RoleDTO roleDto)
        {
            var role = _mapper.Map<Role>(roleDto);
            var result = await _business.CreateRole(role);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpGet]
        public async Task<ActionResult<ResultDataObject<IEnumerable<KeyValuePair<int, string>>>>> GetRoles()
        {
            var result = await _business.GetRoles();

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }
    }
}

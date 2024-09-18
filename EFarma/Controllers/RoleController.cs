using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTO;
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
        public async Task<ActionResult> CreateRole([FromBody] RoleDTO roleDto)
        {
            var role = _mapper.Map<Role>(roleDto);
            var result = await _business.CreateRole(role);
            return StatusCode(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KeyValuePair<int, string>>>> GetRoles()
        {
            var result = await _business.GetRoles();

            if (!result.Any())
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}

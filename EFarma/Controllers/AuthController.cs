using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeBusiness _business;
        private readonly IMapper _mapper;

        public AuthController(ILogger<EmployeeController> logger, IEmployeeBusiness business, IMapper mapper)
        {
            _logger = logger;
            _business = business;
            _mapper = mapper;
        }

        /// <summary>
        /// Método para realizar o login do usuário.
        /// </summary>
        /// <param name="loginDTO">Objeto contendo o email do funcionário e a senha.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o resultado do login.</returns>
        [HttpPost("Login")]
        public async Task<ActionResult<ResultObject>> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _business.Login(loginDTO);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpPost("UpdatePassword")]
        public async Task<ActionResult<ResultObject>> UpdatePassword([FromBody] EmployeeUpdatePasswordDTO updatePasswordDTO)
        {
            var result = await _business.UpdatePassword(updatePasswordDTO);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }
    }
}

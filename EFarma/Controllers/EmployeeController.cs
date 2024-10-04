using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models.Views;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeBusiness _business;
        private readonly IMapper _mapper;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeBusiness business, IMapper mapper)
        {
            _logger = logger;
            _business = business;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ResultObject>> CreateEmployee([FromBody] EmployeeDTO employeeDto)
        {
            var employee = _mapper.Map<Employee>(employeeDto);
            var result = await _business.CreateEmployee(employee);
            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpGet("{cpf}")]
        public async Task<ActionResult<ResultDataObject<Employee>>> GetEmployee(string cpf)
        {
            var result = await _business.GetEmployee(cpf);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpGet("GetPersons")]
        public async Task<ActionResult<ResultDataObject<IEnumerable<PersonView>>>> GetPersonList(string? name, string? cpf)
        {
            var result = await _business.GetPersonList(name, cpf);
            
            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ResultObject>> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _business.Login(loginDTO);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

    }
}

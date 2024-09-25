using EFarma.Business.Interfaces;
using EFarma.Models.Resource;
using EFarma.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using EFarma.Models.DTO;
using EFarma.Models.Views;

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
        public async Task<ActionResult> CreateEmployee([FromBody] EmployeeDTO employeeDto)
        {
            var employee = _mapper.Map<Employee>(employeeDto);
            var result = await _business.CreateEmployee(employee);
            return StatusCode(result);
        }

        [HttpGet("GetPersons")]
        public async Task<ActionResult<IEnumerable<PersonView>>> GetPersonList(string? name, string? cpf)
        {
            var persons = await _business.GetPersonList(name, cpf);
            return Ok(persons);
        }
    }
}

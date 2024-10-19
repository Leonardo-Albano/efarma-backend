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

        [HttpPut]
        public async Task<ActionResult<ResultDataObject<Employee?>>> UpdateEmployee([FromBody] EmployeeDTO employeeDto)
        {
            var existingEmployeeResult = await _business.GetEmployee(employeeDto.CPF);

            if (existingEmployeeResult == null || !existingEmployeeResult.Success && existingEmployeeResult.Data == null)
            {
                return NotFound(new ResultDataObject<Employee?>
                {
                    StatusCode = 404,
                    Message = "Employee not found.",
                    Data = null
                });
            }

            var existingEmployee = existingEmployeeResult.Data;

            _mapper.Map(employeeDto, existingEmployee);
            var result = await _business.UpdateEmployee(existingEmployee);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultObject>> DeleteEmployee(int id)
        {

            var deleteResult = await _business.DeleteEmployee(id);

            return StatusCode(
                statusCode: deleteResult.StatusCode,
                value: deleteResult
            );
        }

    }
}

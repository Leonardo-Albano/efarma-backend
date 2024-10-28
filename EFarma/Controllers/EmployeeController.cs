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

        /// <summary>
        /// Cria um novo funcionário no sistema após validar o CPF e o código de identificação (TagCode) para evitar duplicidades.
        /// </summary>
        /// <param name="employee">Objeto contendo as informações do funcionário a ser criado.</param>
        /// <returns>Objeto <see cref="ResultObject"/> contendo o status da operação, uma mensagem de sucesso ou erro, e o código de status HTTP.</returns>
        /// <response code="200">Funcionário criado com sucesso.</response>
        /// <response code="409">CPF ou código de identificação (TagCode) já estão cadastrados para outro funcionário.</response>
        /// <response code="500">Erro interno ao tentar criar o funcionário.</response>
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

        /// <summary>
        /// Obtém as informações de um funcionário com base no CPF fornecido.
        /// </summary>
        /// <param name="cpf">O CPF do funcionário que deseja consultar.</param>
        /// <returns>Objeto <see cref="ResultDataObject{Employee}"/> contendo as informações do funcionário, uma mensagem de sucesso ou erro, e o código de status HTTP.</returns>
        /// <response code="200">Funcionário encontrado com sucesso.</response>
        /// <response code="404">Nenhum funcionário encontrado com o CPF fornecido.</response>
        [HttpGet("{cpf}")]
        public async Task<ActionResult<ResultDataObject<Employee>>> GetEmployee(string cpf)
        {
            var result = await _business.GetEmployee(cpf);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para obter uma lista de pessoas (pacientes e funcionários) com base no nome ou CPF.
        /// </summary>
        /// <param name="name">Nome da pessoa a ser consultada (opcional).</param>
        /// <param name="cpf">CPF da pessoa a ser consultada (opcional).</param>
        /// <returns>Objeto <see cref="ResultDataObject{List{PersonView}}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Pessoas encontradas com base nos critérios fornecidos.</response>
        /// <response code="404">Nenhuma pessoa encontrada com o nome ou CPF fornecido.</response>
        [HttpGet("GetPersons")]
        public async Task<ActionResult<ResultDataObject<List<PersonView>>>> GetPersonList(string? name, string? cpf)
        {
            var result = await _business.GetPersonList(name, cpf);
            
            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para atualizar as informações de um funcionário existente no sistema.
        /// Valida se o funcionário existe antes de realizar a atualização.
        /// </summary>
        /// <param name="employeeDto">Objeto <see cref="EmployeeDTO"/> contendo as novas informações do funcionário a ser atualizado.</param>
        /// <returns>Objeto <see cref="ResultDataObject{Employee?}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Funcionário atualizado com sucesso.</response>
        /// <response code="404">Funcionário não encontrado.</response>
        /// <response code="500">Erro interno ao tentar atualizar o funcionário.</response>
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

        /// <summary>
        /// Endpoint para deletar um funcionário existente no sistema com base no ID fornecido.
        /// </summary>
        /// <param name="id">ID do funcionário a ser deletado.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Funcionário deletado com sucesso.</response>
        /// <response code="404">Funcionário não encontrado.</response>
        /// <response code="500">Erro interno ao tentar deletar o funcionário.</response>
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

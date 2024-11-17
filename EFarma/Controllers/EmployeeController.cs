using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models.Views;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    /// <summary>
    /// Controlador responsável por fornecer endpoints relacionados aos funcionários.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeBusiness _business;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa uma nova instância do controlador <see cref="AccessLogController"/>.
        /// </summary>
        /// <param name="logger">Instância de logger para registrar informações de execução.</param>
        /// <param name="business">Serviço de negócio para manipulação de operações de funcionários.</param>
        /// <param name="mapper">Serviço utilizado para auto-mapear objetos.</param>
        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeBusiness business, IMapper mapper)
        {
            _logger = logger;
            _business = business;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria um novo funcionário no sistema após validar o CPF e o código de identificação (TagCode) para evitar duplicidades.
        /// </summary>
        /// <param name="employeeDto">Objeto contendo as informações do funcionário a ser criado.</param>
        /// <returns>Objeto <see cref="ResultObject"/> contendo o status da operação, uma mensagem de sucesso ou erro, e o código de status HTTP.</returns>
        /// <response code="200">Funcionário criado com sucesso.</response>
        /// <response code="409">CPF ou código de identificação (TagCode) já estão cadastrados para outro funcionário.</response>
        /// <response code="500">Erro interno ao tentar criar o funcionário.</response>
        [HttpPost]
        public async Task<ActionResult<ResultObject>> Create([FromBody] EmployeeDTO employeeDto)
        {
            var employee = _mapper.Map<Employee>(employeeDto);
            var result = await _business.Create(employee);
            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para obter uma lista de todos os funcionários cadastrados no sistema.
        /// </summary>
        /// <returns>Objeto <see cref="List{PersonView}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Funcionários encontrados com sucesso.</response>
        /// <response code="404">Nenhum funcionário encontrado.</response>
        [HttpGet]
        public async Task<ActionResult<ResultDataObject<List<PersonView>>>> GetAll()
        {
            var result = await _business.GetAll();

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Obtém as informações de um funcionário com base no CPF fornecido.
        /// </summary>
        /// <param name="cpf">O CPF do funcionário que deseja consultar.</param>
        /// <returns>Objeto <see cref="Employee"/> contendo as informações do funcionário, uma mensagem de sucesso ou erro, e o código de status HTTP.</returns>
        /// <response code="200">Funcionário encontrado com sucesso.</response>
        /// <response code="404">Nenhum funcionário encontrado com o CPF fornecido.</response>
        [HttpGet("{cpf}")]
        public async Task<ActionResult<ResultDataObject<Employee?>>> GetByCPF(string cpf)
        {
            var result = await _business.GetByCPF(cpf);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para obter as informações de um médico específico com base no CRM fornecido.
        /// </summary>
        /// <param name="crm">CRM do médico a ser consultado.</param>
        /// <returns>Objeto <see cref="Employee"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Médico encontrado com sucesso.</response>
        /// <response code="404">Nenhum médico encontrado com o CRM fornecido.</response>
        [HttpGet("GetDoctor/{crm}")]
        public async Task<ActionResult<ResultDataObject<Employee>>> GetDoctorByCrm(string crm)
        {
            var result = await _business.GetDoctorByCrm(crm);

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
        /// <returns>Objeto <see cref="List{PersonView}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Pessoas encontradas com base nos critérios fornecidos.</response>
        /// <response code="404">Nenhuma pessoa encontrada com o nome ou CPF fornecido.</response>
        [HttpGet("GetPersons")]
        public async Task<ActionResult<ResultDataObject<List<PersonView>>>> GetPersons(string? name, string? cpf)
        {
            var result = await _business.GetPersons(name, cpf);
            
            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para validar o CRM de um médico e obter suas informações detalhadas, caso encontrado.
        /// </summary>
        /// <param name="crm">CRM do médico a ser validado.</param>
        /// <returns>Objeto <see cref="ResultDataObject{DoctorInfoView}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Doutor(a) encontrado com sucesso.</response>
        /// <response code="404">Doutor(a) não encontrado.</response>
        /// <response code="500">Erro interno ao tentar validar o CRM.</response>
        [HttpGet("ValidateCrm")]
        public async Task<IActionResult> ValidateCrm(string crm)
        {
            var result = await _business.ValidateCrm(crm);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para atualizar as informações de um funcionário específico com base no ID fornecido.
        /// </summary>
        /// <param name="id">ID do funcionário a ser atualizado.</param>
        /// <param name="employeeDto">Objeto <see cref="EmployeeDTO"/> contendo as novas informações do funcionário.</param>
        /// <returns>Objeto <see cref="Employee"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Funcionário atualizado com sucesso.</response>
        /// <response code="404">Funcionário não encontrado.</response>
        /// <response code="500">Erro interno ao tentar atualizar o funcionário.</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<ResultDataObject<Employee?>>> Update(int id, [FromBody] EmployeeDTO employeeDto)
        {
            var result = await _business.Update(id, employeeDto);

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
        public async Task<ActionResult<ResultObject>> Delete(int id)
        {
            var deleteResult = await _business.Delete(id);

            return StatusCode(
                statusCode: deleteResult.StatusCode,
                value: deleteResult
            );
        }

        /// <summary>
        /// Exporta uma lista de todos os funcionários cadastrados no sistema em formato CSV.
        /// </summary>
        /// <returns>Um arquivo CSV contendo a lista de funcionários.</returns>
        /// <response code="200">Arquivo CSV exportado com sucesso.</response>
        /// <response code="404">Nenhum funcionário encontrado para exportação.</response>
        /// <response code="500">Erro interno ao tentar exportar o arquivo CSV.</response>
        [HttpGet("Export")]
        public async Task<IActionResult> ExportEmployees()
        {
            var result = await _business.Export();

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            var fileContent = result.Data;
            var fileName = "employees.csv";

            return File(fileContent, "text/csv", fileName);
        }

        /// <summary>
        /// Importa uma lista de funcionários a partir de um arquivo CSV. O arquivo deve conter os campos na mesma estrutura do CSV exportado.
        /// Observação: As funções devem ser criadas antes de importar funcionários, pois são referenciadas pelo nome no arquivo.
        /// </summary>
        /// <param name="file">Arquivo CSV contendo a lista de funcionários a serem importados.</param>
        /// <returns>Resultado da operação, incluindo os funcionários criados e as entradas incorretas (caso existam).</returns>
        /// <response code="200">Funcionários importados com sucesso.</response>
        /// <response code="400">Arquivo CSV inválido ou estrutura incorreta.</response>
        /// <response code="500">Erro interno ao tentar importar o arquivo CSV.</response>
        [HttpPost("Import")]
        public async Task<IActionResult> ImportEmployees(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Arquivo CSV inválido ou vazio.");
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var csvData = memoryStream.ToArray();

            var result = await _business.ImportEmployees(csvData);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            return Ok(new
            {
                result.Message,
                InvalidEntries = result.Data
            });
        }
    }
}

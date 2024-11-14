using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Resource;
using EFarma.Models.Response;
using EFarma.Models.Views;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IPatientBusiness _business;
        private readonly IMapper _mapper;

        public PatientController(ILogger<PatientController> logger, IPatientBusiness business, IMapper mapper)
        {
            _logger = logger;
            _business = business;
            _mapper = mapper;
        }

        /// <summary>
        /// Endpoint para criar um novo paciente no sistema.
        /// Verifica se um paciente com o mesmo CPF já existe antes de realizar a criação.
        /// </summary>
        /// <param name="patientDto">Objeto <see cref="PatientDTO"/> contendo as informações do paciente a ser criado.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Paciente criado com sucesso.</response>
        /// <response code="409">O CPF já existe no sistema.</response>
        /// <response code="500">Erro interno ao tentar criar o paciente.</response>
        [HttpPost]
        public async Task<ActionResult<ResultObject>> CreatePatient([FromBody]PatientDTO patientDto)
        {
            var patient = _mapper.Map<Patient>(patientDto);
            var result = await _business.CreatePatient(patient);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para obter as informações de um paciente com base no CPF fornecido.
        /// </summary>
        /// <param name="cpf">CPF do paciente a ser consultado.</param>
        /// <returns>Objeto <see cref="ResultDataObject{Patient}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Paciente encontrado com sucesso.</response>
        /// <response code="404">Nenhum paciente encontrado com o CPF fornecido.</response>
        [HttpGet("{cpf}")]
        public async Task<ActionResult<ResultDataObject<Patient>>> GetPatient(string cpf)
        {
            var result = await _business.GetPatient(cpf);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para obter todos os pacientes cadastrados no sistema.
        /// </summary>
        /// <returns>Objeto <see cref="ResultDataObject{List{Patient}}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Pacientes encontrados com sucesso.</response>
        /// <response code="404">Nenhum paciente encontrado.</response>
        [HttpGet]
        public async Task<ActionResult<ResultDataObject<List<Patient>>>> GetAllPatients()
        {
            var result = await _business.GetAllPatients();

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para atualizar as informações de um paciente existente no sistema.
        /// Valida se o paciente existe antes de realizar a atualização.
        /// </summary>
        /// <param name="patientDto">Objeto <see cref="PatientDTO"/> contendo as novas informações do paciente a ser atualizado.</param>
        /// <returns>Objeto <see cref="ResultDataObject{Patient?}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Paciente atualizado com sucesso.</response>
        /// <response code="404">Paciente não encontrado.</response>
        /// <response code="500">Erro interno ao tentar atualizar o paciente.</response>
        [HttpPut]
        public async Task<ActionResult<ResultDataObject<Patient?>>> UpdatePatient([FromBody] PatientDTO patientDto)
        {
            var existingPatientResult = await _business.GetPatient(patientDto.CPF);

            if (existingPatientResult == null || !existingPatientResult.Success && existingPatientResult.Data == null)
            {
                return NotFound(new ResultDataObject<Patient?>
                {
                    StatusCode = 404,
                    Message = "Patient not found.",
                    Data = null
                });
            }

            var existingPatient = existingPatientResult.Data;

            _mapper.Map(patientDto, existingPatient);
            var result = await _business.UpdatePatient(existingPatient);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para deletar um paciente existente no sistema com base no ID fornecido.
        /// </summary>
        /// <param name="id">ID do paciente a ser deletado.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Paciente deletado com sucesso.</response>
        /// <response code="404">Paciente não encontrado.</response>
        /// <response code="500">Erro interno ao tentar deletar o paciente.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultObject>> DeletePatient(int id)
        {
            var result = await _business.DeletePatient(id);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Exporta uma lista de todos os pacientes cadastrados no sistema em formato CSV.
        /// </summary>
        /// <returns>Um arquivo CSV contendo a lista de pacientes.</returns>
        /// <response code="200">Arquivo CSV exportado com sucesso.</response>
        /// <response code="404">Nenhum paciente encontrado para exportação.</response>
        /// <response code="500">Erro interno ao tentar exportar o arquivo CSV.</response>
        [HttpGet("Export")]
        public async Task<IActionResult> ExportPatients()
        {
            var result = await _business.ExportPatients();

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Message);
            }

            var fileContent = result.Data;
            var fileName = "patients.csv";

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

            var result = await _business.ImportPatients(csvData);

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

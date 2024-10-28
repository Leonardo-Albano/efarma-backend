using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicamentController : ControllerBase
    {
        private readonly ILogger<MedicamentController> _logger;
        private readonly IMedicamentBusiness _business;

        public MedicamentController(ILogger<MedicamentController> logger, IMedicamentBusiness business)
        {
            _logger = logger;
            _business = business;
        }

        /// <summary>
        /// Endpoint para criar um novo medicamento no sistema.
        /// Verifica se um medicamento com a mesma descrição, dosagem e medida já existe antes de realizar a criação.
        /// </summary>
        /// <param name="medicamentDTO">Objeto <see cref="MedicamentDTO"/> contendo as informações do medicamento a ser criado.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Medicamento criado com sucesso.</response>
        /// <response code="409">O medicamento já existe no sistema.</response>
        /// <response code="500">Erro interno ao tentar criar o medicamento.</response>
        [HttpPost]
        public async Task<ActionResult<ResultObject>> CreateMedicament(MedicamentDTO medicamentDTO)
        {
            var result = await _business.CreateMedicament(medicamentDTO);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para obter uma lista de todos os medicamentos no sistema.
        /// Retorna um dicionário contendo o ID do medicamento e sua descrição formatada (descrição, dosagem e medida).
        /// </summary>
        /// <returns>Objeto <see cref="ResultDataObject{List{Dictionary{int, string}}}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Medicamentos encontrados com sucesso.</response>
        /// <response code="400">Nenhum medicamento encontrado.</response>
        [HttpGet]
        public async Task<ActionResult<ResultDataObject<List<Dictionary<int, string>>>>> GetAllMedicaments()
        {
            var result = await _business.GetAllMedicaments();
  
            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para deletar um medicamento existente no sistema com base no ID fornecido.
        /// </summary>
        /// <param name="id">ID do medicamento a ser deletado.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Medicamento deletado com sucesso.</response>
        /// <response code="404">Medicamento não encontrado.</response>
        /// <response code="500">Erro interno ao tentar deletar o medicamento.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultObject>> DeleteMedicament(int id)
        {
            var result = await _business.DeleteMedicament(id);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }
    }
}

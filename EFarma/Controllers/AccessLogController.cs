using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    /// <summary>
    /// Controlador responsável por fornecer endpoints relacionados aos logs de acesso.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccessLogController : ControllerBase
    {
        private readonly ILogger<AccessLogController> _logger;
        private readonly IAccessLogBusiness _business;

        /// <summary>
        /// Inicializa uma nova instância do controlador <see cref="AccessLogController"/>.
        /// </summary>
        /// <param name="logger">Instância de logger para registrar informações de execução.</param>
        /// <param name="business">Serviço de negócio para manipulação de operações com logs de acesso.</param>
        public AccessLogController(ILogger<AccessLogController> logger, IAccessLogBusiness business)
        {
            _logger = logger;
            _business = business;
        }

        /// <summary>
        /// Endpoint para obter todos os logs de acesso, com a opção de filtrar apenas entradas.
        /// </summary>
        /// <param name="filterEntries">Booleano que indica se apenas as entradas devem ser retornadas nos logs.</param>
        /// <returns>Objeto <see cref="List{AccessLog}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Logs de acesso obtidos com sucesso.</response>
        /// <response code="404">Nenhum log de acesso encontrado.</response>
        /// <response code="500">Erro interno ao tentar obter os logs de acesso.</response>
        [HttpGet]
        public async Task<ActionResult<ResultDataObject<List<AccessLog>>>> GetAllAccessLogs(bool filterEntries)
        {
            var accessLogs = await _business.GetAllAccessLogs(filterEntries);

            return StatusCode(
                statusCode: accessLogs.StatusCode,
                value: accessLogs
            );
        }
    }
}

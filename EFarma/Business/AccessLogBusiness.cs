using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    /// <summary>
    /// Classe responsável por manipular operações de logs de acesso no sistema de controle de estoque hospitalar.
    /// </summary>
    public class AccessLogBusiness : IAccessLogBusiness
    {
        private readonly ILogger<AccessLogController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="AccessLogBusiness"/>.
        /// </summary>
        /// <param name="logger">Instância de logger para registrar informações de execução.</param>
        /// <param name="repository">Instância de repositório para manipulação de dados de logs de acesso.</param>
        /// <param name="mapper">Instância de mapeamento de objetos.</param>
        public AccessLogBusiness(ILogger<AccessLogController> logger, IUnitOfWork repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém todos os logs de acesso com a opção de aplicar um filtro para obter somente as entradas/saídas na sala de estoque.
        /// </summary>
        /// <param name="filterEntries">Indica se o filtro para entradas deve ser aplicado.</param>
        /// <returns>Retorna um objeto <see cref="ResultDataObject{T}"/> contendo uma lista de logs de acesso.</returns>
        public async Task<ResultDataObject<List<AccessLog>>> GetAllAccessLogs(bool filterEntries)
        {
            _logger.LogInformation("Iniciando método GetAllAccessLogs {filterStatus} filtro de entradas.", filterEntries ? "com" : "sem");

            try
            {
                var access_logs = await _repository.AccessLogs.GetAllWithFilter(filterEntries);
                var hasLogs = access_logs.Count != 0;

                if (hasLogs)
                {
                    _logger.LogInformation("Logs de acesso encontrados: {logCount}", access_logs.Count);
                }
                else
                {
                    _logger.LogWarning("Nenhum log de acesso encontrado.");
                }

                return new ResultDataObject<List<AccessLog>>()
                {
                    Data = access_logs.OrderByDescending(a => a.Date).ToList(),
                    Message = hasLogs ? "Logs de acesso obtidos com sucesso." : "Nenhum log de acesso encontrado.",
                    Success = hasLogs,
                    StatusCode = hasLogs ? 200 : 404
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter os logs de acesso.");
                return new ResultDataObject<List<AccessLog>>()
                {
                    Data = [],
                    Message = $"Erro: {ex.Message}",
                    Success = false,
                    StatusCode = 500
                };
            }
        }
    }
}

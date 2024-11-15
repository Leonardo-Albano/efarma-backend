using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models.Response;
using EFarma.Models.Views;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    /// <summary>
    /// Classe base para operações relacionadas a pessoas, como pacientes e funcionários.
    /// </summary>
    public class PersonBusiness : IPersonBusiness
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PersonBusiness"/>.
        /// </summary>
        /// <param name="logger">Instância de logger para registrar informações de execução.</param>
        /// <param name="repository">Instância do repositório para manipulação de dados de pacientes e funcionários.</param>
        /// <param name="mapper">Instância de mapeamento de objetos.</param>
        public PersonBusiness(ILogger<EmployeeController> logger, IUnitOfWork repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém uma lista de pessoas, incluindo pacientes e funcionários, filtrada por nome e/ou CPF.
        /// </summary>
        /// <param name="name">Nome parcial ou completo da pessoa a ser filtrada (opcional).</param>
        /// <param name="cpf">CPF da pessoa a ser filtrada (opcional).</param>
        /// <returns>Objeto de resultado com a lista de pessoas encontradas e o status da operação.</returns>
        public async Task<ResultDataObject<List<PersonView>>> GetPersons(string? name, string? cpf)
        {
            var patients = await _repository.Patients.GetFiltered(cpf, name);
            var employees = await _repository.Employees.GetFiltered(cpf, name);

            var people = _mapper.Map<List<PersonView>>(patients)
                         .Concat(_mapper.Map<List<PersonView>>(employees)).ToList();

            bool success = people.Count != 0;
            return new()
            {
                Message = success ? "Pessoas encontradas." : "Nenhuma pessoa encontrada.",
                Data = people,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }
    }
}

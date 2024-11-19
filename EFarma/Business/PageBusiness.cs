using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    /// <summary>
    /// Classe responsável pelas operações relacionadas a páginas do sistema.
    /// </summary>
    public class PageBusiness : IPageBusiness
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PageBusiness"/>.
        /// </summary>
        /// <param name="repository">Instância do repositório para manipulação de dados das páginas.</param>
        /// <param name="mapper">Instância de mapeamento de objetos.</param>
        public PageBusiness(IUnitOfWork repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria uma nova página no sistema, verificando se uma página com o mesmo nome já existe.
        /// </summary>
        /// <param name="page">Objeto da página a ser criada.</param>
        /// <returns>Resultado da operação de criação com o status e mensagem apropriada.</returns>
        public async Task<ResultObject> CreatePage(Page page)
        {
            var existingPage = await _repository.Pages.FirstOrDefault(p => p.Name == page.Name);
            if (existingPage != null)
            {
                return new ResultObject
                {
                    Message = "Uma página com o mesmo nome já existe.",
                    StatusCode = 409,
                    Success = false
                };
            }

            _repository.Pages.Add(page);
            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Página criada com sucesso." : "Erro ao criar a página.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        /// <summary>
        /// Obtém todas as páginas cadastradas no sistema.
        /// </summary>
        /// <returns>Objeto de resultado contendo a lista de páginas, o status da operação e uma mensagem apropriada.</returns>
        public async Task<ResultDataObject<List<Page>>> GetAllPages()
        {
            var pages = await _repository.Pages.GetAll();
            var result = _mapper.Map<List<Page>>(pages);

            bool success = result.Count != 0;
            return new ResultDataObject<List<Page>>
            {
                Message = success ? "Páginas recuperadas com sucesso." : "Nenhuma página encontrada.",
                Data = result,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }

        /// <summary>
        /// Exclui uma página existente no sistema com base no ID fornecido.
        /// </summary>
        /// <param name="id">ID da página a ser excluída.</param>
        /// <returns>Resultado da operação de exclusão com o status e mensagem apropriada.</returns>
        public async Task<ResultObject> DeletePage(int id)
        {
            var page = await _repository.Pages.FirstOrDefault(p => p.Id == id);
            if (page == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Página não encontrada.",
                    Success = false
                };
            }

            _repository.Pages.Remove(page);
            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Página excluída com sucesso." : "Erro ao excluir a página.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }
    }
}

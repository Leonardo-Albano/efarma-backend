using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly ILogger<PageController> _logger;
        private readonly IPageBusiness _business;
        private readonly IMapper _mapper;

        public PageController(ILogger<PageController> logger, IPageBusiness business, IMapper mapper)
        {
            _logger = logger;
            _business = business;
            _mapper = mapper;
        }

        /// <summary>
        /// Endpoint para criar uma nova página no sistema.
        /// Verifica se uma página com o mesmo nome já existe antes de realizar a criação.
        /// </summary>
        /// <param name="pageDTO">Objeto <see cref="PageDTO"/> contendo as informações da página a ser criada.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Página criada com sucesso.</response>
        /// <response code="409">Uma página com o mesmo nome já existe.</response>
        /// <response code="500">Erro interno ao tentar criar a página.</response>
        [HttpPost]
        public async Task<ActionResult<ResultObject>> CreatePage([FromBody] PageDTO pageDTO)
        {
            var result = await _business.CreatePage(pageDTO);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Endpoint para obter todas as páginas cadastradas no sistema.
        /// </summary>
        /// <returns>Objeto <see cref="ResultDataObject{IEnumerable{Page}}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Páginas recuperadas com sucesso.</response>
        /// <response code="404">Nenhuma página encontrada.</response>
        [HttpGet]
        public async Task<ActionResult<ResultDataObject<IEnumerable<Page>>>> GetAllPages()
        {
            var result = await _business.GetAllPages();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Endpoint para deletar uma página existente no sistema com base no ID fornecido.
        /// </summary>
        /// <param name="id">ID da página a ser deletada.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Página deletada com sucesso.</response>
        /// <response code="404">Página não encontrada.</response>
        /// <response code="500">Erro interno ao tentar deletar a página.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultObject>> DeletePage(int id)
        {
            var result = await _business.DeletePage(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}

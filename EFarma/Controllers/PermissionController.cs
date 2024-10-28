using EFarma.Business.Interfaces;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly IPermissionBusiness _business;
        private readonly IMapper _mapper;

        public PermissionController(ILogger<PermissionController> logger, IPermissionBusiness business, IMapper mapper)
        {
            _logger = logger;
            _business = business;
            _mapper = mapper;
        }

        /// <summary>
        /// Endpoint para criar uma nova permissão no sistema.
        /// Verifica se as salas de estoque (Stock Rooms) e páginas (Pages) fornecidas existem antes de associá-las à permissão.
        /// </summary>
        /// <param name="permissionDto">Objeto <see cref="PermissionDTO"/> contendo as informações da permissão a ser criada, juntamente com as IDs das salas de estoque e páginas associadas.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Permissão criada com sucesso.</response>
        /// <response code="404">Uma ou mais salas de estoque ou páginas fornecidas não existem.</response>
        /// <response code="500">Erro interno ao tentar criar a permissão.</response>
        [HttpPost]
        public async Task<ActionResult<ResultObject>> CreatePermission([FromBody] PermissionDTO permissionDto)
        {
            var permission = _mapper.Map<Permission>(permissionDto);
            var result = await _business.CreatePermission(permission, permissionDto.StockRoomIds, permissionDto.PageIds);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para obter todas as permissões cadastradas no sistema.
        /// Retorna uma lista de pares chave-valor contendo o ID da permissão e seu nome.
        /// </summary>
        /// <returns>Objeto <see cref="ResultDataObject{List{KeyValuePair{int, string}}}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Permissões encontradas com sucesso.</response>
        /// <response code="404">Nenhuma permissão encontrada.</response>
        [HttpGet]
        public async Task<ActionResult<ResultDataObject<List<KeyValuePair<int, string>>>>> GetPermissions()
        {
            var result = await _business.GetPermissions();

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para obter os detalhes de uma permissão específica com base no ID fornecido.
        /// </summary>
        /// <param name="id">ID da permissão a ser consultada.</param>
        /// <returns>Objeto <see cref="ResultDataObject{Permission}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Permissão encontrada com sucesso.</response>
        /// <response code="404">Permissão não encontrada.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultDataObject<Permission>>> GetPermissionDetailed(int id)
        {
            var result = await _business.GetPermissionDetailed(id);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para atualizar uma permissão existente com base no ID fornecido.
        /// Atualiza o nome, descrição, e as salas de estoque (Stock Rooms) e páginas (Pages) associadas à permissão.
        /// </summary>
        /// <param name="id">ID da permissão a ser atualizada.</param>
        /// <param name="permissionDto">Objeto <see cref="PermissionDTO"/> contendo as novas informações da permissão.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Permissão atualizada com sucesso.</response>
        /// <response code="404">Permissão, salas de estoque ou páginas não encontradas.</response>
        /// <response code="500">Erro interno ao tentar atualizar a permissão.</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<ResultObject>> UpdatePermission(int id, [FromBody] PermissionDTO permissionDto)
        {
            var permission = _mapper.Map<Permission>(permissionDto);
            var result = await _business.UpdatePermission(id, permission, permissionDto.StockRoomIds, permissionDto.PageIds);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para deletar uma permissão existente no sistema com base no ID fornecido.
        /// </summary>
        /// <param name="id">ID da permissão a ser deletada.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Permissão deletada com sucesso.</response>
        /// <response code="404">Permissão não encontrada.</response>
        /// <response code="500">Erro interno ao tentar deletar a permissão.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultObject>> DeletePermission(int id)
        {
            var result = await _business.DeletePermission(id);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }
    }
}

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
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IRoleBusiness _business;
        private readonly IMapper _mapper;

        public RoleController(ILogger<RoleController> logger, IRoleBusiness business, IMapper mapper)
        {
            _logger = logger;
            _business = business;
            _mapper = mapper;
        }

        /// <summary>
        /// Endpoint para criar uma nova função (role) no sistema.
        /// Verifica se já existe uma função com o mesmo nome antes de realizar a criação.
        /// </summary>
        /// <param name="roleDto">Objeto <see cref="RoleDTO"/> contendo as informações da função a ser criada.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Função criada com sucesso.</response>
        /// <response code="409">Uma função com o mesmo nome já existe no sistema.</response>
        /// <response code="500">Erro interno ao tentar criar a função.</response>
        [HttpPost]
        public async Task<ActionResult<ResultObject>> CreateRole([FromBody] RoleDTO roleDto)
        {
            var role = _mapper.Map<Role>(roleDto);
            var result = await _business.CreateRole(role, roleDto.PermissionIds);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para obter todas as funções (roles) cadastradas no sistema.
        /// Retorna uma lista de pares chave-valor contendo o ID da função e seu nome.
        /// </summary>
        /// <returns>Objeto <see cref="ResultDataObject{List{KeyValuePair{int, string}}}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Funções encontradas com sucesso.</response>
        /// <response code="400">Nenhuma função encontrada.</response>
        [HttpGet]
        public async Task<ActionResult<ResultDataObject<List<KeyValuePair<int, string>>>>> GetRoles()
        {
            var result = await _business.GetRoles();

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para deletar uma função (role) existente no sistema com base no ID fornecido.
        /// </summary>
        /// <param name="id">ID da função a ser deletada.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Função deletada com sucesso.</response>
        /// <response code="404">Função não encontrada.</response>
        /// <response code="500">Erro interno ao tentar deletar a função.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultObject>> DeleteRole(int id)
        {
            var result = await _business.DeleteRole(id);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }
    }
}

using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace EFarma.Controllers
{
    /// <summary>
    /// Controlador responsável por fornecer endpoints para autenticação e gerenciamento de senhas no sistema.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeBusiness _business;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa uma nova instância do controlador <see cref="AuthController"/>.
        /// </summary>
        /// <param name="logger">Instância de logger para registrar informações de execução.</param>
        /// <param name="business">Serviço de negócios para manipulação das operações de autenticação e atualização de senha.</param>
        /// <param name="mapper">Instância de mapeamento de objetos.</param>
        public AuthController(ILogger<EmployeeController> logger, IEmployeeBusiness business, IMapper mapper)
        {
            _logger = logger;
            _business = business;
            _mapper = mapper;
        }

        /// <summary>
        /// Método para realizar o login do usuário. Retorna <c>Success = true</c> se o login for bem-sucedido, e <c>false</c> caso contrário.
        /// A propriedade <c>message</c> conterá detalhes do possível erro ocorrido no processo de login.
        /// </summary>
        /// <param name="loginDTO">Objeto contendo as informações necessárias para realizar o login, como email e senha.</param>
        /// <returns>Objeto <see cref="ResultObject"/> contendo o status do login, a mensagem de erro (se houver), e o código de status HTTP.</returns>
        /// <response code="200">Login bem-sucedido. Retorna o objeto <see cref="ResultObject"/> com <c>Success = true</c>.</response>
        /// <response code="400">Requisição inválida. Retorna o objeto <see cref="ResultObject"/> com <c>Success = false</c> e uma mensagem de erro.</response>
        /// <response code="403">Acesso negado. O login falhou devido a credenciais inválidas ou falta de permissão.</response>
        /// <response code="404">Acesso negado. O login falhou devido ao usuário que não foi encontrado no sistema.</response>
        [HttpPost("Login")]
        public async Task<ActionResult<ResultObject>> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _business.Login(loginDTO);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Atualiza a senha do funcionário. Valida se o funcionário existe, se a senha antiga está correta, e se a nova senha é válida. 
        /// Em caso de sucesso, atualiza a senha no banco de dados.
        /// </summary>
        /// <param name="updatePasswordDTO">Objeto contendo as informações para realizar a atualização da senha: email, senha antiga e nova senha.</param>
        /// <returns>Retorna um objeto <see cref="ResultObject"/> com o status da operação, uma mensagem indicando sucesso ou falha, e o código de status HTTP.</returns>
        /// <response code="200">Senha atualizada com sucesso.</response>
        /// <response code="403">Senha antiga incorreta ou nova senha inválida.</response>
        /// <response code="404">Funcionário não encontrado.</response>
        /// <response code="500">Erro interno ao tentar atualizar a senha.</response>
        [HttpPost("UpdatePassword")]
        public async Task<ActionResult<ResultObject>> UpdatePassword([FromBody] EmployeeUpdatePasswordDTO updatePasswordDTO)
        {
            var result = await _business.UpdatePassword(updatePasswordDTO);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }

        /// <summary>
        /// Endpoint para redefinir a senha de um funcionário com base no ID fornecido.
        /// A nova senha será gerada a partir do CPF do funcionário.
        /// </summary>
        /// <param name="employeeId">ID do funcionário cuja senha será redefinida.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Senha redefinida com sucesso.</response>
        /// <response code="404">Funcionário não encontrado.</response>
        /// <response code="500">Erro interno ao tentar redefinir a senha.</response>
        [HttpGet("ResetPassword/{employeeId}")]
        public async Task<ActionResult<ResultObject>> ResetPassword(int employeeId)
        {
            var result = await _business.ResetPassword(employeeId);

            return StatusCode(
                statusCode: result.StatusCode,
                value: result
            );
        }
    }
}

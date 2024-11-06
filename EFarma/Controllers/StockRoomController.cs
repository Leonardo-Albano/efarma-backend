using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models;
using Microsoft.AspNetCore.Mvc;
using EFarma.Models.Views;
using EFarma.Business;

namespace EFarma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockRoomController : ControllerBase
    {
        private readonly ILogger<StockRoomController> _logger;
        private readonly IStockRoomBusiness _business;
        private readonly IMapper _mapper;

        public StockRoomController(ILogger<StockRoomController> logger, IStockRoomBusiness business, IMapper mapper)
        {
            _logger = logger;
            _business = business;
            _mapper = mapper;
        }

        /// <summary>
        /// Endpoint para criar uma nova sala de estoque (StockRoom) no sistema.
        /// Verifica se já existe uma sala de estoque com o mesmo nome antes de realizar a criação.
        /// </summary>
        /// <param name="stockRoomDTO">Objeto <see cref="StockRoomDTO"/> contendo as informações da sala de estoque a ser criada.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Sala de estoque criada com sucesso.</response>
        /// <response code="409">Uma sala de estoque com o mesmo nome já existe no sistema.</response>
        /// <response code="500">Erro interno ao tentar criar a sala de estoque.</response>
        [HttpPost]
        public async Task<ActionResult<ResultObject>> CreateStockRoom([FromBody] StockRoomDTO stockRoomDTO)
        {
            var result = await _business.CreateStockRoom(stockRoomDTO);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Endpoint para obter todas as salas de estoque (StockRooms) cadastradas no sistema.
        /// </summary>
        /// <returns>Objeto <see cref="ResultDataObject{List{StockRoom}}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Salas de estoque recuperadas com sucesso.</response>
        /// <response code="404">Nenhuma sala de estoque encontrada.</response>
        [HttpGet]
        public async Task<ActionResult<ResultDataObject<List<StockRoom>>>> GetAllStockRooms()
        {
            var result = await _business.GetAllStockRooms();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Endpoint para obter os detalhes de uma sala de estoque (StockRoom) específica com base no ID fornecido.
        /// </summary>
        /// <param name="id">ID da sala de estoque a ser consultada.</param>
        /// <returns>Objeto <see cref="ResultDataObject{StockRoom}"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Sala de estoque encontrada com sucesso.</response>
        /// <response code="404">Nenhuma sala de estoque encontrada com o ID fornecido.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultDataObject<StockRoom>>> GetStockRoom(int id)
        {
            var result = await _business.GetStockRoom(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Endpoint para deletar uma sala de estoque (StockRoom) existente no sistema com base no ID fornecido.
        /// </summary>
        /// <param name="id">ID da sala de estoque a ser deletada.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Sala de estoque deletada com sucesso.</response>
        /// <response code="404">Sala de estoque não encontrada.</response>
        /// <response code="500">Erro interno ao tentar deletar a sala de estoque.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultObject>> DeleteStockRoom(int id)
        {
            var result = await _business.DeleteStockRoom(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Endpoint para obter a lista de medicamentos disponíveis em estoque, com a opção de filtrar por nome de medicamento. 
        /// Os resultados são agrupados por sala de estoque e medicamento.
        /// </summary>
        /// <param name="medicamentName">Nome do medicamento para filtragem opcional. Caso não seja informado, todos os medicamentos em estoque serão retornados.</param>
        /// <returns>Objeto <see cref="ResultDataObject{List{InStockItemView}}"/> contendo o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Medicamentos encontrados com sucesso.</response>
        /// <response code="404">Nenhum medicamento encontrado em estoque.</response>
        [HttpGet("AvailableMedicaments")]
        public async Task<ActionResult<ResultDataObject<List<InStockItemView>>>> GetAvailableMedicaments(string? medicamentName){
            var result = await _business.GetAvailableMedicaments(medicamentName);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Endpoint para adicionar um item no estoque em uma sala de estoque específica.
        /// Valida se a sala de estoque e o medicamento existem antes de realizar a inserção.
        /// Verifica também se a quantidade de itens indicada corresponde aos itens lidos nas prateleiras pelo embarcado.
        /// </summary>
        /// <param name="inStockItemDTO">Objeto <see cref="InStockItemDTO"/> contendo as informações do item a ser inserido no estoque.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Item adicionado ao estoque com sucesso.</response>
        /// <response code="400">A quantidade de medicamentos indicada não corresponde aos lidos nas prateleiras.</response>
        /// <response code="404">Sala de estoque ou medicamento não encontrado.</response>
        /// <response code="500">Erro interno ao tentar adicionar o item ao estoque.</response>
        [HttpPost("InsertItems")]
        public async Task<ActionResult<ResultObject>> InsertItemToStock([FromBody] InStockItemDTO inStockItemDTO)
        {
            var inStockItem = _mapper.Map<InStockItem>(inStockItemDTO);
            var result = await _business.InsertItemToStock(inStockItem, inStockItemDTO.Quantity);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Endpoint para registrar a entrada de um funcionário em uma sala de estoque (StockRoom).
        /// Verifica se o funcionário e a sala de estoque existem, e se o funcionário tem permissão de acesso à sala.
        /// </summary>
        /// <param name="entryLogDTO">Objeto <see cref="EntryLogDTO"/> contendo as informações da tentativa de acesso, incluindo o código de identificação do funcionário e o ID único da sala de estoque.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente. Sempre trará as respostas sem acentos, para permitir sua visualização no display do embarcado. </returns>
        /// <response code="200">Acesso permitido e registrado com sucesso.</response>
        /// <response code="403">Acesso não permitido para a sala de estoque especificada.</response>
        /// <response code="404">Funcionário não encontrado.</response>
        /// <response code="500">Erro interno ao tentar registrar a entrada na sala de estoque.</response>
        [HttpPost("Entry")]
        public async Task<ActionResult<ResultObject>> EntryStockRoom([FromBody] EntryLogDTO entryLogDTO)
        {
            var result = await _business.EntryStockRoom(entryLogDTO);
            result.Message = StockRoomBusiness.RemoveAcentuation(result.Message);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Endpoint para registrar a saída de um funcionário de uma sala de estoque (StockRoom) com base no ID único da sala.
        /// Valida se houve uma entrada registrada e verifica pendências de prescrições antes de permitir a saída.
        /// </summary>
        /// <param name="stockRoomUniqueId">ID único da sala de estoque.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente. Sempre trará as respostas sem acentos, para permitir sua visualização no display do embarcado.</returns>
        /// <response code="200">Saída registrada com sucesso.</response>
        /// <response code="403">Saída bloqueada devido a prescrições pendentes.</response>
        /// <response code="404">Nenhuma entrada foi identificada na sala.</response>
        /// <response code="500">Erro interno ao tentar registrar a saída.</response>
        [HttpPost("Exit")]
        public async Task<ActionResult<ResultObject>> ExitStockRoom(string stockRoomUniqueId)
        {
            var result = await _business.ExitStockRoom(stockRoomUniqueId);
            result.Message = StockRoomBusiness.RemoveAcentuation(result.Message);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Endpoint para corrigir o registro de acesso de um funcionário a uma sala de estoque (StockRoom).
        /// Valida se o funcionário possui permissão de acesso e se ele possui uma entrada pendente na sala.
        /// </summary>
        /// <param name="entryLogDTO">Objeto <see cref="EntryLogDTO"/> contendo as informações para corrigir o acesso, incluindo o código de identificação do funcionário e o ID único da sala de estoque.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Correção de acesso registrada com sucesso.</response>
        /// <response code="403">Usuário não tem entradas pendentes ou não possui permissão de acesso.</response>
        /// <response code="404">Tag do funcionário ou sala de estoque não encontrada.</response>
        /// <response code="500">Erro interno ao tentar salvar a correção de acesso.</response>
        [HttpPost("CorrectAccess")]
        public async Task<ActionResult<ResultObject>> CorrectAccess([FromBody] EntryLogDTO entryLogDTO)
        {
            var result = await _business.CorrectAccess(entryLogDTO);
            return StatusCode(result.StatusCode, result);
        }
    }
}

using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models;
using Microsoft.AspNetCore.Mvc;
using EFarma.Models.Views;

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

        [HttpGet("AvailableMedicaments")]
        public async Task<ActionResult<ResultDataObject<List<InStockItemView>>>> GetAvailableMedicaments(){
            var result = await _business.GetAvailableMedicaments();
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
        [HttpPost("AddItem")]
        public async Task<ActionResult<ResultObject>> InsertItemToStock([FromBody] InStockItemDTO inStockItemDTO)
        {
            var inStockItem = _mapper.Map<InStockItem>(inStockItemDTO);
            var result = await _business.InsertItemToStock(inStockItem, inStockItemDTO.Quantity);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Endpoint para remover itens do estoque com base em uma prescrição.
        /// Valida se a prescrição e a sala de estoque existem e verifica se os medicamentos no estoque correspondem aos itens prescritos.
        /// Caso sejam retirados mais medicamentos do que os prescritos na receita, o sistema não permitirá.
        /// Caso sejam retirados menos medicamentos do que os prescritos na receita, o sistema permitirá.
        /// </summary>
        /// <param name="prescriptionItemsDTO">Objeto <see cref="RemovePrescriptionItemsDTO"/> contendo as informações da prescrição e da sala de estoque.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Itens removidos do estoque com sucesso e prescrição finalizada.</response>
        /// <response code="403">Os medicamentos no estoque não correspondem aos itens prescritos.</response>
        /// <response code="404">Prescrição ou sala de estoque não encontrada.</response>
        /// <response code="500">Erro interno ao tentar remover os itens do estoque.</response>
        [HttpPost("RemoveItems")]
        public async Task<ActionResult<ResultObject>> RemoveItemsFromStock([FromBody] RemovePrescriptionItemsDTO prescriptionItemsDTO)
        {
            var result = await _business.RemoveItemsFromStock(prescriptionItemsDTO);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Endpoint para registrar a entrada de um funcionário em uma sala de estoque (StockRoom).
        /// Verifica se o funcionário e a sala de estoque existem, e se o funcionário tem permissão de acesso à sala.
        /// </summary>
        /// <param name="entryLogDTO">Objeto <see cref="EntryLogDTO"/> contendo as informações da tentativa de acesso, incluindo o código de identificação do funcionário e o ID único da sala de estoque.</param>
        /// <returns>Objeto <see cref="ResultObject"/> com o status da operação e o código HTTP correspondente.</returns>
        /// <response code="200">Acesso permitido e registrado com sucesso.</response>
        /// <response code="403">Acesso não permitido para a sala de estoque especificada.</response>
        /// <response code="404">Funcionário não encontrado.</response>
        /// <response code="500">Erro interno ao tentar registrar a entrada na sala de estoque.</response>
        [HttpPost("Entry")]
        public async Task<ActionResult<ResultObject>> EntryStockRoom([FromBody] EntryLogDTO entryLogDTO)
        {
            var result = await _business.EntryStockRoom(entryLogDTO);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Exit")]
        public async Task<ActionResult<ResultObject>> ExitStockRoom(string stockRoomUniqueId)
        {
            var result = await _business.ExitStockRoom(stockRoomUniqueId);
            return StatusCode(result.StatusCode, result);
        }
    }
}

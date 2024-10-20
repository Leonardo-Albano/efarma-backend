using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Models.DTOs;
using EFarma.Models.Response;
using EFarma.Models;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<ActionResult<ResultObject>> CreateStockRoom([FromBody] StockRoomDTO stockRoomDTO)
        {
            var result = await _business.CreateStockRoom(stockRoomDTO);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<ActionResult<ResultDataObject<IEnumerable<StockRoom>>>> GetAllStockRooms()
        {
            var result = await _business.GetAllStockRooms();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResultDataObject<StockRoom>>> GetStockRoom(int id)
        {
            var result = await _business.GetStockRoom(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultObject>> DeleteStockRoom(int id)
        {
            var result = await _business.DeleteStockRoom(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}

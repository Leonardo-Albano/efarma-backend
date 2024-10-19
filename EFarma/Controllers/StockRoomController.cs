using AutoMapper;
using EFarma.Business.Interfaces;
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
    }
}

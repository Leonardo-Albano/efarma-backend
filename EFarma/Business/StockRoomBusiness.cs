using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class StockRoomBusiness : IStockRoomBusiness
    {
        private readonly ILogger<StockRoomController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        public StockRoomBusiness(ILogger<StockRoomController> logger, IUnitOfWork repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }
    }
}

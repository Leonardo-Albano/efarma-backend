using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class AccessLogBusiness : IAccessLogBusiness
    {
        private readonly ILogger<AccessLogController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        public AccessLogBusiness(ILogger<AccessLogController> logger, IUnitOfWork repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccessLog>> GetAllAccessLogs()
        {
            return await _repository.AccessLogs.GetAll();
        }
    }
}

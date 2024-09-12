using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class AccessLogBusiness : IAccessLogBusiness
    {
        private readonly ILogger<AccessLogController> _logger;
        private readonly IAccessLogRepository _repository;

        public AccessLogBusiness(ILogger<AccessLogController> logger, IAccessLogRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<IEnumerable<AccessLog>> GetAllAccessLogs()
        {
            return await _repository.GetAll();
        }
    }
}

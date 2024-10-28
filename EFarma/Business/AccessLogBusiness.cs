using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.Response;
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

        public async Task<ResultDataObject<List<AccessLog>>> GetAllAccessLogs()
        {
            try
            {
                var access_logs = await _repository.AccessLogs.GetAll();
                var hasLogs = access_logs.Any();

                return new ResultDataObject<List<AccessLog>>()
                {
                    Data = access_logs,
                    Message = hasLogs ? "Access logs retrieved successfully." : "No access logs found.",
                    Success = hasLogs,
                    StatusCode = hasLogs ? 200 : 404
                };
            }
            catch (Exception ex)
            {
                return new ResultDataObject<List<AccessLog>>()
                {
                    Data = [],
                    Message = $"Error: {ex.Message}",
                    Success = false,
                    StatusCode = 500
                };
            }
        }
    }
}

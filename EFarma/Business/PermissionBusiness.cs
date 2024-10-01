using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Models.Views;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class PermissionBusiness : IPermissionBusiness
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        public PermissionBusiness(ILogger<PermissionController> logger, IUnitOfWork repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<VoidResult> CreatePermission(Permission permission)
        {
            if(permission.StockRoomId.HasValue)
            {
                var stockRoom = await _repository.StockRooms.FirstOrDefault(s=>s.Id == permission.StockRoomId.Value);
                if(stockRoom == null)
                {
                    return new VoidResult
                    {
                        Message = $"Stock Room with id {permission.StockRoomId.Value} doesn't exists.",
                        StatusCode = 404,
                        Success = false
                    };
                }
                permission.StockRoom = stockRoom;
            }

            _repository.Permissions.Add(permission);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new VoidResult
            {
                Message = success ? "Permission created successfully." : "An error occurred while creating the permission.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<DataResult<IEnumerable<PermissionView>>> GetPermissions()
        {
            var permissions = await _repository.Permissions.GetAll();
            var permissionsView = _mapper.Map<IEnumerable<PermissionView>>(permissions);

            bool success = permissionsView.Any();

            return new()
            {
                Message = success ? "Found permissions." : "No permissions found.",
                Result = permissionsView,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }
    }
}

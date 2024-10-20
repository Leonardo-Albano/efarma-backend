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

        public async Task<ResultObject> CreatePermission(Permission permission, IEnumerable<int>? stockRoomIds, IEnumerable<int>? pageIds)
        {
            if (stockRoomIds != null && stockRoomIds.Any())
            {
                var stockRooms = await _repository.StockRooms.Find(s => stockRoomIds.Contains(s.Id));

                if (stockRooms.Count() != stockRoomIds.Count())
                {
                    return new ResultObject
                    {
                        Message = "One or more Stock Rooms don't exist.",
                        StatusCode = 404,
                        Success = false
                    };
                }
                permission.StockRooms = stockRooms.ToList();
            }

            if (pageIds != null && pageIds.Any())
            {
                var pages = await _repository.Pages.Find(s => pageIds.Contains(s.Id));

                if (pages.Count() != pageIds.Count())
                {
                    return new ResultObject
                    {
                        Message = "One or more Pages don't exist.",
                        StatusCode = 404,
                        Success = false
                    };
                }
                permission.Pages = pages.ToList();
            }

            _repository.Permissions.Add(permission);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Permission created successfully." : "An error occurred while creating the permission.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<IEnumerable<PermissionView>>> GetPermissions()
        {
            var permissions = await _repository.Permissions.GetAll();
            var permissionsView = _mapper.Map<IEnumerable<PermissionView>>(permissions);

            bool success = permissionsView.Any();

            return new()
            {
                Message = success ? "Found permissions." : "No permissions found.",
                Data = permissionsView,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }

        public async Task<ResultDataObject<Permission?>> GetPermissionDetailed(int id)
        {
            var permission = await _repository.Permissions.GetPermissionDetailed(id);

            bool success = permission != null;

            return new()
            {
                Message = success ? "Found permission." : "Permission not found.",
                Data = permission,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }

        public async Task<ResultObject> UpdatePermission(int id, Permission updatedPermission, IEnumerable<int>? stockRoomIds, IEnumerable<int>? pageIds)
        {
            var existingPermission = await _repository.Permissions.FirstOrDefault(p => p.Id == id);

            if (existingPermission == null)
            {
                return new ResultObject
                {
                    Message = "Permission not found.",
                    StatusCode = 404,
                    Success = false
                };
            }

            existingPermission.Name = updatedPermission.Name;
            existingPermission.Description = updatedPermission.Description;

            if (stockRoomIds != null && stockRoomIds.Any())
            {
                var stockRooms = await _repository.StockRooms.Find(s => stockRoomIds.Contains(s.Id));

                if (stockRooms.Count() != stockRoomIds.Count())
                {
                    return new ResultObject
                    {
                        Message = "One or more Stock Rooms don't exist.",
                        StatusCode = 404,
                        Success = false
                    };
                }

                existingPermission.StockRooms = stockRooms.ToList();
            }

            if (pageIds != null && pageIds.Any())
            {
                var pages = await _repository.Pages.Find(s => pageIds.Contains(s.Id));

                if (pages.Count() != pageIds.Count())
                {
                    return new ResultObject
                    {
                        Message = "One or more Pages don't exist.",
                        StatusCode = 404,
                        Success = false
                    };
                }

                existingPermission.Pages = pages.ToList();
            }

            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Permission updated successfully." : "An error occurred while updating the permission.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultObject> DeletePermission(int id)
        {
            var permission = await _repository.Permissions.FirstOrDefault(p => p.Id == id);

            if (permission == null)
            {
                return new ResultObject
                {
                    Message = "Permission not found.",
                    StatusCode = 404,
                    Success = false
                };
            }

            _repository.Permissions.Remove(permission);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Permission deleted successfully." : "An error occurred while deleting the permission.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }
    }
}

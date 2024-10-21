using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class RoleBusiness : IRoleBusiness
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        public RoleBusiness(ILogger<RoleController> logger, IUnitOfWork repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResultObject> CreateRole(Role role)
        {
            var existent_roles = await _repository.Roles.Find(r => r.Name == role.Name);
            if (existent_roles.Any())
            {
                return new ResultObject
                {
                    Message = $"Role with the name '{role.Name}' already exists in the system.",
                    StatusCode = 409,
                    Success = false
                };
            }

            _repository.Roles.Add(role);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Role created successfully." : "An error occurred while creating the role.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultObject> DeleteRole(int id)
        {
            var role = await _repository.Roles.FirstOrDefault(e => e.Id == id);
            if (role == null)
            {
                return new ResultObject
                {
                    StatusCode = 404,
                    Message = "Role not found.",
                    Success = false
                };
            }

            _repository.Roles.Remove(role);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Role deleted successfully." : "An error occurred while deleting the role.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<IEnumerable<KeyValuePair<int, string>>>> GetRoles()
        {
            var roles = await _repository.Roles.GetAll();
            var result = roles.Select(r => new KeyValuePair<int, string>(r.Id, r.Name)).ToList();
            
            bool success = result.Any();

            return new()
            {
                Message = success ? "Roles found" : "No roles found.",
                Data = result,
                StatusCode = success ? 200 : 400,
                Success = success
            };
        }
    }
}

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

        public async Task<VoidResult> CreateRole(Role role)
        {
            var existent_roles = await _repository.Roles.Find(r => r.Name == role.Name);
            if (existent_roles.Any())
            {
                return new VoidResult
                {
                    Message = $"Role with the name '{role.Name}' already exists in the system.",
                    StatusCode = 409,
                    Success = false
                };
            }

            _repository.Roles.Add(role);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new VoidResult
            {
                Message = success ? "Role created successfully." : "An error occurred while creating the role.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<DataResult<IEnumerable<KeyValuePair<int, string>>>> GetRoles()
        {
            var roles = await _repository.Roles.GetAll();
            var result = roles.Select(r => new KeyValuePair<int, string>(r.Id, r.Name)).ToList();
            
            bool success = result.Any();

            return new()
            {
                Message = success ? "Roles found" : "No roles found.",
                Result = result,
                StatusCode = success ? 200 : 400,
                Success = success
            };
        }
    }
}

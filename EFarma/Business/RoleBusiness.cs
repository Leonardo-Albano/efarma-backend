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

        public async Task<ResultObject> CreateRole(Role role, List<int> permissionIds)
        {
            var existent_roles = await _repository.Roles.Find(r => r.Name == role.Name);
            if (existent_roles.Any())
            {
                return new ResultObject
                {
                    Message = $"A função com o nome '{role.Name}' já existe no sistema.",
                    StatusCode = 409,
                    Success = false
                };
            }

            var permissions = await _repository.Permissions.Find(p => permissionIds.Contains(p.Id));
            if (permissions.Count() != permissionIds.Count)
            {
                var missingPermissions = permissionIds.Except(permissions.Select(p => p.Id)).ToList();
                return new ResultObject
                {
                    Message = $"Algumas permissões não foram encontradas: {string.Join(", ", missingPermissions)}",
                    StatusCode = 404,
                    Success = false
                };
            }

            role.Permissions = permissions;
            _repository.Roles.Add(role);

            bool success = await _repository.SaveChangesAsync() > 0;
            return new ResultObject
            {
                Message = success ? "Função criada com sucesso." : "Ocorreu um erro ao criar a função.",
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
                    Message = "Função não encontrada.",
                    Success = false
                };
            }

            _repository.Roles.Remove(role);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Função excluída com sucesso." : "Ocorreu um erro ao excluir a função.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        public async Task<ResultDataObject<List<KeyValuePair<int, string>>>> GetRoles()
        {
            var roles = await _repository.Roles.GetAll();
            var result = roles.Select(r => new KeyValuePair<int, string>(r.Id, r.Name)).ToList();

            bool success = result.Any();

            return new()
            {
                Message = success ? "Funções encontradas." : "Nenhuma função encontrada.",
                Data = result,
                StatusCode = success ? 200 : 400,
                Success = success
            };
        }
    }
}

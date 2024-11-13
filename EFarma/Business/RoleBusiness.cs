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

        public async Task<ResultDataObject<List<Role>>> GetRoles()
        {
            var roles = await _repository.Roles.GetAllDetailed();

            bool success = roles.Any();

            return new()
            {
                Message = success ? "Funções encontradas." : "Nenhuma função encontrada.",
                Data = roles,
                StatusCode = success ? 200 : 400,
                Success = success
            };
        }

        public async Task<ResultObject> UpdateRole(int id, Role updatedRole, List<int>? permissionIds)
        {
            var existingRole = await _repository.Roles.FirstOrDefault(r => r.Id == id);

            if (existingRole == null)
            {
                return new ResultObject
                {
                    Message = "Função não encontrada.",
                    StatusCode = 404,
                    Success = false
                };
            }

            existingRole.Name = updatedRole.Name;

            if (permissionIds != null && permissionIds.Any())
            {
                var permissions = await _repository.Permissions.Find(p => permissionIds.Contains(p.Id));

                if (permissions.Count != permissionIds.Count)
                {
                    return new ResultObject
                    {
                        Message = "Uma ou mais permissões não existem.",
                        StatusCode = 404,
                        Success = false
                    };
                }

                existingRole.Permissions = permissions;
            }

            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Função atualizada com sucesso." : "Ocorreu um erro ao atualizar a função.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }
    }
}

using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    /// <summary>
    /// Classe responsável pela lógica de negócios relacionada às funções (roles) no sistema.
    /// Inclui operações para criar, atualizar, deletar e listar funções.
    /// </summary>
    public class RoleBusiness : IRoleBusiness
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="RoleBusiness"/>.
        /// </summary>
        /// <param name="logger">Instância do logger para registro de informações e erros.</param>
        /// <param name="repository">Instância do repositório para acesso ao banco de dados.</param>
        /// <param name="mapper">Instância do mapper para conversão entre modelos.</param>
        public RoleBusiness(ILogger<RoleController> logger, IUnitOfWork repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria uma nova função (role) no sistema.
        /// Valida se já existe uma função com o mesmo nome e se todas as permissões fornecidas existem.
        /// </summary>
        /// <param name="role">Objeto <see cref="Role"/> contendo as informações da função a ser criada.</param>
        /// <param name="permissionIds">Lista de IDs das permissões associadas à função.</param>
        /// <returns>
        /// Um <see cref="ResultObject"/> indicando o sucesso ou falha da operação, com mensagens detalhadas.
        /// </returns>
        public async Task<ResultObject> CreateRole(Role role, List<int> permissionIds)
        {
            var existent_roles = await _repository.Roles.Find(r => r.Name == role.Name);
            if (existent_roles.Count != 0)
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

        /// <summary>
        /// Deleta uma função (role) existente no sistema com base no ID fornecido.
        /// Valida se a função existe antes de tentar excluí-la.
        /// </summary>
        /// <param name="id">ID da função a ser deletada.</param>
        /// <returns>
        /// Um <see cref="ResultObject"/> indicando o sucesso ou falha da operação.
        /// </returns>
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

        /// <summary>
        /// Obtém todas as funções (roles) cadastradas no sistema, incluindo suas permissões associadas.
        /// </summary>
        /// <returns>
        /// Um <see cref="List{Role}"/> contendo:
        /// - Data: Lista de funções com detalhes completos.
        /// - Message: Mensagem indicando o status da operação.
        /// </returns>
        public async Task<ResultDataObject<List<Role>>> GetRoles()
        {
            var roles = await _repository.Roles.GetAllDetailed();

            bool success = roles.Count != 0;

            return new()
            {
                Message = success ? "Funções encontradas." : "Nenhuma função encontrada.",
                Data = roles,
                StatusCode = success ? 200 : 400,
                Success = success
            };
        }

        /// <summary>
        /// Atualiza uma função (role) existente no sistema.
        /// Permite atualizar o nome da função e as permissões associadas.
        /// </summary>
        /// <param name="id">ID da função a ser atualizada.</param>
        /// <param name="updatedRole">Objeto <see cref="Role"/> contendo as novas informações da função.</param>
        /// <param name="permissionIds">Lista de IDs das permissões a serem associadas à função.</param>
        /// <returns>
        /// Um <see cref="ResultObject"/> indicando o sucesso ou falha da operação.
        /// </returns>
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

            if (permissionIds != null && permissionIds.Count != 0)
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

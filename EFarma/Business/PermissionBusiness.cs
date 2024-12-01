using AutoMapper;
using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Models.Response;
using EFarma.Models.Views;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    /// <summary>
    /// Classe responsável pelas operações relacionadas às permissões no sistema.
    /// </summary>
    public class PermissionBusiness : IPermissionBusiness
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PermissionBusiness"/>.
        /// </summary>
        /// <param name="logger">Instância de logger para registrar informações de execução.</param>
        /// <param name="repository">Instância do repositório para manipulação de dados das permissões.</param>
        /// <param name="mapper">Instância de mapeamento de objetos.</param>
        public PermissionBusiness(ILogger<PermissionController> logger, IUnitOfWork repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria uma nova permissão no sistema, associando salas de estoque e páginas existentes.
        /// </summary>
        /// <param name="permission">Objeto da permissão a ser criada.</param>
        /// <param name="stockRoomIds">Lista de IDs das salas de estoque a serem associadas.</param>
        /// <param name="pageIds">Lista de IDs das páginas a serem associadas.</param>
        /// <returns>Resultado da operação de criação com o status e mensagem apropriada.</returns>
        public async Task<ResultObject> CreatePermission(Permission permission, List<int>? stockRoomIds, List<int>? pageIds)
        {
            if (stockRoomIds != null && stockRoomIds.Count != 0)
            {
                var stockRooms = await _repository.StockRooms.Find(s => stockRoomIds.Contains(s.Id));

                if (stockRooms.Count() != stockRoomIds.Count())
                {
                    return new ResultObject
                    {
                        Message = "Uma ou mais Salas de Estoque não existem.",
                        StatusCode = 404,
                        Success = false
                    };
                }
                permission.StockRooms = stockRooms.ToList();
            }

            if (pageIds != null && pageIds.Count != 0)
            {
                var pages = await _repository.Pages.Find(s => pageIds.Contains(s.Id));

                if (pages.Count() != pageIds.Count())
                {
                    return new ResultObject
                    {
                        Message = "Uma ou mais Páginas não existem.",
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
                Message = success ? "Permissão criada com sucesso." : "Ocorreu um erro ao criar a permissão.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        /// <summary>
        /// Obtém todas as permissões cadastradas no sistema.
        /// </summary>
        /// <returns>Objeto de resultado contendo a lista de permissões, o status da operação e uma mensagem apropriada.</returns>
        public async Task<ResultDataObject<List<PermissionView>>> GetPermissions()
        {
            var permissions = await _repository.Permissions.GetAll();
            var permissionsView = _mapper.Map<List<PermissionView>>(permissions);

            bool success = permissionsView.Count != 0;

            return new()
            {
                Message = success ? "Permissões encontradas." : "Nenhuma permissão encontrada.",
                Data = permissionsView,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }

        /// <summary>
        /// Obtém os detalhes de uma permissão específica com base no ID fornecido.
        /// </summary>
        /// <param name="id">ID da permissão a ser consultada.</param>
        /// <returns>Objeto de resultado contendo os detalhes da permissão, o status da operação e uma mensagem apropriada.</returns>
        public async Task<ResultDataObject<Permission?>> GetPermissionDetailed(int id)
        {
            var permission = await _repository.Permissions.GetPermissionDetailed(id);

            bool success = permission != null;

            return new()
            {
                Message = success ? "Permissão encontrada." : "Permissão não encontrada.",
                Data = permission,
                StatusCode = success ? 200 : 404,
                Success = success
            };
        }

        /// <summary>
        /// Atualiza uma permissão existente no sistema, associando salas de estoque e páginas fornecidas.
        /// </summary>
        /// <param name="id">ID da permissão a ser atualizada.</param>
        /// <param name="updatedPermission">Objeto contendo os novos dados da permissão.</param>
        /// <param name="stockRoomIds">Lista de IDs das salas de estoque a serem associadas.</param>
        /// <param name="pageIds">Lista de IDs das páginas a serem associadas.</param>
        /// <returns>Resultado da operação de atualização com o status e mensagem apropriada.</returns>
        public async Task<ResultObject> UpdatePermission(int id, Permission updatedPermission, List<int>? stockRoomIds, List<int>? pageIds)
        {
            var existingPermission = await _repository.Permissions.FirstOrDefault(p => p.Id == id);

            if (existingPermission == null)
            {
                return new ResultObject
                {
                    Message = "Permissão não encontrada.",
                    StatusCode = 404,
                    Success = false
                };
            }

            existingPermission.Name = updatedPermission.Name;
            existingPermission.Description = updatedPermission.Description;

            if (stockRoomIds != null && stockRoomIds.Count != 0)
            {
                var stockRooms = await _repository.StockRooms.Find(s => stockRoomIds.Contains(s.Id));

                if (stockRooms.Count() != stockRoomIds.Count())
                {
                    return new ResultObject
                    {
                        Message = "Uma ou mais Salas de Estoque não existem.",
                        StatusCode = 404,
                        Success = false
                    };
                }

                existingPermission.StockRooms = stockRooms.ToList();
            }

            if (pageIds != null && pageIds.Count != 0)
            {
                var pages = await _repository.Pages.Find(s => pageIds.Contains(s.Id));

                if (pages.Count() != pageIds.Count())
                {
                    return new ResultObject
                    {
                        Message = "Uma ou mais Páginas não existem.",
                        StatusCode = 404,
                        Success = false
                    };
                }

                existingPermission.Pages = pages.ToList();
            }

            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Permissão atualizada com sucesso." : "Ocorreu um erro ao atualizar a permissão.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }

        /// <summary>
        /// Exclui uma permissão existente no sistema com base no ID fornecido.
        /// </summary>
        /// <param name="id">ID da permissão a ser excluída.</param>
        /// <returns>Resultado da operação de exclusão com o status e mensagem apropriada.</returns>
        public async Task<ResultObject> DeletePermission(int id)
        {
            var permission = await _repository.Permissions.FirstOrDefault(p => p.Id == id);

            if (permission == null)
            {
                return new ResultObject
                {
                    Message = "Permissão não encontrada.",
                    StatusCode = 404,
                    Success = false
                };
            }

            _repository.Permissions.Remove(permission);
            bool success = await _repository.SaveChangesAsync() > 0;

            return new ResultObject
            {
                Message = success ? "Permissão excluída com sucesso." : "Ocorreu um erro ao excluir a permissão.",
                StatusCode = success ? 200 : 500,
                Success = success
            };
        }
    }
}

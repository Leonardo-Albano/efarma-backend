using EFarma.Business.Interfaces;
using EFarma.Controllers;
using EFarma.Models;
using EFarma.Repositories;
using EFarma.Repositories.Interfaces;

namespace EFarma.Business
{
    public class RoleBusiness : IRoleBusiness
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IUnitOfWork _repository;

        public RoleBusiness(ILogger<RoleController> logger, IUnitOfWork repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<int> CreateRole(Role role)
        {
            var existent_roles = await _repository.Roles.Find(r => r.Name == role.Name);
            if (existent_roles.Any())
            {
                return 409;
            }

            _repository.Roles.Add(role);

            return await _repository.SaveChangesAsync() > 0 ? 200 : 500;
        }

        public async Task<IEnumerable<KeyValuePair<int, string>>> GetRoles()
        {
            var roles = await _repository.Roles.GetAll();
            var result = roles.Select(r => new KeyValuePair<int, string>(r.Id, r.Name)).ToList();
            return result;
        }
    }
}

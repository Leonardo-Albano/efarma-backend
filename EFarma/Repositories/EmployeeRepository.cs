using EFarma.Data;
using EFarma.Models;
using EFarma.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EFarma.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(DbContext context) : base(context)
        {
        }

        public async Task<List<Employee?>> GetEmployeeByCpfOrName(string? cpf, string? name)
            => await DataContext.Employees
                .Include(e=>e.Role)
                .Where(p =>
               (!string.IsNullOrEmpty(name) && p.Name.ToLower().Trim().Contains(name.ToLower().Trim())) ||
               (!string.IsNullOrEmpty(cpf) && p.CPF == cpf) ||
               (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(cpf))).ToListAsync();

        public async Task<Employee?> GetEmployeeByTagCode(string code) 
            => await DataContext.Employees
                .Include(e=>e.Role)
                    .ThenInclude(r=>r.Permissions)
                        .ThenInclude(p => p.StockRooms)
                .FirstOrDefaultAsync(e => e.TagCode == code);

        public async Task<List<Employee>> GetAllDetailed()
        {
            return await DataContext.Employees
                .Include(e=>e.Role)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeDetailedByMail(string mail)
        {
            return await DataContext.Employees
            .Include(e => e.Role)
                .ThenInclude(r => r.Permissions)
                    .ThenInclude(p => p.Pages)
            .Include(e => e.Role)
                .ThenInclude(r => r.Permissions)
                    .ThenInclude(p => p.StockRooms)
            .FirstOrDefaultAsync(e => e.Mail == mail);
        }

        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

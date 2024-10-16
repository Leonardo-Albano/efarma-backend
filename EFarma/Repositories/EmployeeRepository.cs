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

        public async Task<Employee?> GetEmployeeByTagCode(string code) 
            => await DataContext.Employees.FirstOrDefaultAsync(e => e.TagCode == code);
        public DataContext DataContext
        {
            get { return _context as DataContext; }
        }
    }
}

using EFarma.Models;
using Microsoft.EntityFrameworkCore;

namespace EFarma.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<AccessLog> AccessLogs { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<InStockItem> InStockItems { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<StatusCode> StatusCodes { get; set; }
        public DbSet<StockRoom> StockRooms { get; set; }

    }
}

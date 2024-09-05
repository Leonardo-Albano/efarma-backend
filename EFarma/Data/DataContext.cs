using EFarma.Models;
using Microsoft.EntityFrameworkCore;

namespace EFarma.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Patient> Patients { get; set; }
    }
}

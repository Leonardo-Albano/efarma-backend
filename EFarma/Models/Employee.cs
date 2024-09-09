using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFarma.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Permission")]
        public int PermissionId { get; set; }
        
        [ForeignKey("Role")]
        public int RoleId { get; set; }

        public int EmployeeId { get; set; }
        public required string Name { get; set; }
        public required string CPF { get; set; }
        public DateOnly BirthDate { get; set; }
        public string? Phone { get; set; } 
        public required string Mail { get; set; }
        public required string PasswordHash { get; set; }
        public int? CRM { get; set; }

        public required Permission Permission { get; set; }
        public required Role Role { get; set; }
    }
}

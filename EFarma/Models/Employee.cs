using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFarma.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Role")]
        public int RoleId { get; set; }

        public string? EmployeeId { get; set; }
        public required string Name { get; set; }
        public required string CPF { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Phone { get; set; } 
        public required string Mail { get; set; }
        public required string ResponsibleMail { get; set; }
        public required string PasswordHash { get; set; }
        public string? CRM { get; set; }
        public string? TagCode { get; set; } 

        public required Role Role { get; set; }
    }
}

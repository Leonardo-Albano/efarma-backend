namespace EFarma.Models.DTOs
{
    public class EmployeeDTO
    {
        public DateTime BirthDate { get; set; }
        public required string CPF { get; set; }
        public int? CRM { get; set; }
        public string? EmployeeId { get; set; }
        public required string Mail { get; set; }
        public required string ResponsibleMail { get; set; }
        public required string Name { get; set; }
        public required string Phone { get; set; }

        public int PermissionId { get; set; }
        public int RoleId { get; set; }


    }
}

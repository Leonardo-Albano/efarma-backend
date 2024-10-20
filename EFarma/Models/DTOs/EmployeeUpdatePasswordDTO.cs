namespace EFarma.Models.DTOs
{
    public class EmployeeUpdatePasswordDTO
    {
        public required string Mail { get; set; }
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}

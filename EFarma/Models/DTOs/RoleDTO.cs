namespace EFarma.Models.DTOs
{
    public class RoleDTO
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required List<int> PermitionsId { get; set; }
    }
}

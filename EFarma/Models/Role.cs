using System.ComponentModel.DataAnnotations;

namespace EFarma.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }
        public required string Description { get; set; }

        public List<Permission> Permissions { get; set; } = [];
    }
}

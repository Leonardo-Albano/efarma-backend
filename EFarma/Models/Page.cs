using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EFarma.Models
{
    public class Page
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }
        public required string Description { get; set; }

        [JsonIgnore]
        public List<Permission>? Permissions { get; set; } = [];
    }
}

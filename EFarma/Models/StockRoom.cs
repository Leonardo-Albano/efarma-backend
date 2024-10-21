using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EFarma.Models
{
    public class StockRoom
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string UniqueId { get; set; }

        public required List<InStockItem> InStockItems { get; set; } = [];
        [JsonIgnore]
        public List<Permission>? Permissions { get; set; } = [];
    }
}

using System.ComponentModel.DataAnnotations;

namespace EFarma.Models
{
    public class StockRoom
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }
        public required string Address { get; set; }

        public required List<InStockItem> InStockItems { get; set; } = [];
    }
}

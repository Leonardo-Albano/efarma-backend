namespace EFarma.Models
{
    public class StockRoom
    {
        public int Id { get; set; }

        public required string Name { get; set; }
        public required string Address { get; set; }

        public required List<InStockItem> InStockItems { get; set; } = [];
    }
}

namespace EFarma.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public int StockRoomId { get; set; }

        public required string Name { get; set; }
        public required string Description { get; set; }

        public required StockRoom StockRoom { get; set; }
    }
}

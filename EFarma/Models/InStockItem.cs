namespace EFarma.Models
{
    public class InStockItem
    {
        public int Id { get; set; }
        public int StockRoomId { get; set; }
        public int MedicamentId { get; set; }

        public DateOnly ExpirationDate { get; set; }
        public int Quantity { get; set; }
        public int Shelf { get; set; }


        public required StockRoom StockRoom { get; set; }
        public required Medicament Medicament { get; set; }

    }
}

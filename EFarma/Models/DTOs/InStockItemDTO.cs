namespace EFarma.Models.DTOs
{
    public class InStockItemDTO
    {
        public int EmployeeId { get; set; }
        public int StockRoomId { get; set; }
        public int MedicamentId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int Quantity { get; set; }
    }
}
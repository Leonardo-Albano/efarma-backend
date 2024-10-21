namespace EFarma.Models.DTOs
{
    public class EntryLogDTO
    {
        public required string StockRoomUniqueId { get; set; }
        public required string TagCode { get; set; }
        public DateTime DateTimeAccess { get; set; }
    }
}

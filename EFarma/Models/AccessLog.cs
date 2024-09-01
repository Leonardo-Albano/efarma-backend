namespace EFarma.Models
{
    public class AccessLog
    {
        public int EmployeeId { get; set; }
        public int StockRoomId { get; set; }
        public int StatusCodeId { get; set; }

        public DateTime DateTimeAccess { get; set; }
        public bool IsEntry { get; set; }

        public required Employee Employee { get; set; }
        public required StockRoom StockRoom { get; set; }
        public required StatusCode StatusCode { get; set; }

    }
}

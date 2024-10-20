namespace EFarma.Models.DTOs
{
    public class PermissionDTO
    {
        public required string Description { get; set; }
        public required string Name { get; set; }
        public IEnumerable<int>? StockRoomIds { get; set; }
        public IEnumerable<int>? PageIds { get; set; }
    }
}

namespace EFarma.Models.DTOs
{
    public class PermissionDTO
    {
        public required string Description { get; set; }
        public required string Name { get; set; }
        public List<int>? StockRoomIds { get; set; }
        public List<int>? PageIds { get; set; }
    }
}

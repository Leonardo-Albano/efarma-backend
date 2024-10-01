using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EFarma.Models.DTOs
{
    public class PermissionDTO
    {
        public required string Description { get; set; }
        public required string Name { get; set; }
        public int? StockRoomId { get; set; }
    }
}

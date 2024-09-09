using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFarma.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("StockRoom")]
        public int StockRoomId { get; set; }

        public required string Name { get; set; }
        public required string Description { get; set; }

        public required StockRoom StockRoom { get; set; }
    }
}

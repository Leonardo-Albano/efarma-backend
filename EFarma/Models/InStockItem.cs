using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFarma.Models
{
    public class InStockItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("StockRoom")]
        public int StockRoomId { get; set; }
        
        [ForeignKey("Medicament")]
        public int MedicamentId { get; set; }

        public DateTime ExpirationDate { get; set; }
        public int Quantity { get; set; }
        public int Shelf { get; set; }

        public required StockRoom StockRoom { get; set; }
        public required Medicament Medicament { get; set; }

    }
}

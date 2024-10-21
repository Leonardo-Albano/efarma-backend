using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        public required string TagCode { get; set; }
        public DateTime ExpirationDate { get; set; }

        [JsonIgnore]
        public StockRoom StockRoom { get; set; }
        public Medicament Medicament { get; set; }

        public InStockItem Clone()
        {
            return (InStockItem)this.MemberwiseClone();
        }
    }
}

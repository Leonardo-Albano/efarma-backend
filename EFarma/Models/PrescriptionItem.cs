using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFarma.Models
{
    public class PrescriptionItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Prescription")]
        public int PrescriptionId { get; set; }

        [ForeignKey("InStockItem")]
        public int InStockItemId { get; set; }

        public int PrescribedQuantity { get; set; }

        public required Prescription Prescription { get; set; }
        public required InStockItem InStockItem { get; set; }

    }
}

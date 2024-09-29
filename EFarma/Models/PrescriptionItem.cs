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

        [ForeignKey("Medicament")]
        public int MedicamentId { get; set; }

        public int PrescribedQuantity { get; set; }

        public required Prescription Prescription { get; set; }
        public required Medicament Medicament { get; set; }

    }
}

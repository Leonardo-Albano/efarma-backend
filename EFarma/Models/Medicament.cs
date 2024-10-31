using System.ComponentModel.DataAnnotations;

namespace EFarma.Models
{
    public class Medicament
    {
        [Key]
        public int Id { get; set; }

        public required string Description { get; set; }
        public decimal Dosage { get; set; }
        public required string Measure { get; set; }

        public string GetMedicamentName()
        {
            return $"{this.Description} {this.Dosage}{this.Measure}";
        }
    }
}

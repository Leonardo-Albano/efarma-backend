namespace EFarma.Models
{
    public class Medicament
    {
        public int Id { get; set; }

        public required string Description { get; set; }
        public decimal Dosage { get; set; }
        public required string Measure { get; set; }
    }
}

namespace EFarma.Models.DTOs
{
    public class MedicamentDTO
    {
        public required string Description { get; set; }
        public decimal Dosage { get; set; }
        public required string Measure { get; set; }
    }
}

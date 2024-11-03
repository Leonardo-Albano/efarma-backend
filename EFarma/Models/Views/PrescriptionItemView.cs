namespace EFarma.Models.Views
{
    public class PrescriptionItemView
    {
        public string Id { get; } = Guid.NewGuid().ToString()[..8];
        public required string Name { get; set; }
        public required decimal Dosage { get; set; }
        public required string Measure { get; set; }
        public int Quantity { get; set; }
    }
}

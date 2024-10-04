namespace EFarma.Models.Views
{
    public class PrescriptionItemView
    {
        public required string Name { get; set; }
        public required string Dosage { get; set; }
        public int Quantity { get; set; }
    }
}

namespace EFarma.Models.Views
{
    public class PrescriptionItemViewDetailed
    {
        public int Id { get; set; }
        public string MedicamentName { get; set; }
        public string MedicamentDosage { get; set; }
        public string MedicamentMeasure { get; set; }
        public int PrescribedQuantity { get; set; }
        public string Observation {  get; set; }
    }
}

namespace EFarma.Models.Views
{
    public class PrescriptionViewDetailed
    {
        public int Id { get; set; }
        public required string PatientName { get; set; }
        public required string DoctorName { get; set; }
        public required string CRM { get; set; }
        public required string CPF { get; set; }
        public DateTime Data { get; set; }
        public string Local { get; set; }

        public List<PrescriptionItemViewDetailed> Items { get; set; }
    }
}

namespace EFarma.Models.Views
{
    public class PrescriptionView
    {
        public int Id { get; set; }
        public required string PatientName { get; set; }
        public required string DoctorName { get; set; }
        public required string CPF { get; set; }
    }
}

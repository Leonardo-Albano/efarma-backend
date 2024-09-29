namespace EFarma.Models.DTOs
{
    public class PrescriptionDTO
    {
        public int EmployeeId { get; set; }

        public required string CPF { get; set; }
        public required string Local { get; set; }

        public required List<PrescriptionItemDTO> Items { get; set; }
    }
}

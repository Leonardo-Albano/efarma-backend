using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFarma.Models
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }

        public required string CPF { get; set; }
        public required string Status { get; set; }
        public required string Local { get; set; }

        public List<PrescriptionItem> Items { get; set; } = new();
    }
}

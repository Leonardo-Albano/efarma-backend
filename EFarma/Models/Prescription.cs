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

        [ForeignKey("TakeOutResponsible")]
        public int TakeOutResponsibleId { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }

        public required string CPF { get; set; }
        public required string Status { get; set; }
        public required string Local { get; set; }
        public required DateTime Date { get; set; } = DateTime.Now;

        public required Patient Patient { get; set; }
        public required Employee Employee { get; set; }
        public required Employee TakeOutResponsible { get; set; }
        public List<PrescriptionItem> Items { get; set; } = new();

        public static readonly string CreatedMessage = "Nova Receita";
        public static readonly string PendentMessage = "Pendente";
        public static readonly string ConcludedMessage = "Concluído";
    }
}

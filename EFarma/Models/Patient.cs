using System.ComponentModel.DataAnnotations;

namespace EFarma.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }
        public required string CPF { get; set; }
        public DateTime BirthDay { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Mail { get; set; }
        public string? Observations { get; set; }
    }
}

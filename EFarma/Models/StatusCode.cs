using System.ComponentModel.DataAnnotations;

namespace EFarma.Models
{
    public class StatusCode
    {
        [Key]
        public int Id { get; set; }

        public required string Description { get; set; }
        public required string ShortDescription { get; set; }
    }
}

namespace EFarma.Models
{
    public class StatusCode
    {
        public int Id { get; set; }

        public required string Description { get; set; }
        public required string ShortDescription { get; set; }
    }
}

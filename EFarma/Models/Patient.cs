namespace EFarma.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Cpf { get; set; }
        public DateOnly BirthDay { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Observations { get; set; } = string.Empty;
    }
}

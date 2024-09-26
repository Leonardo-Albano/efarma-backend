namespace EFarma.Models.Resource
{
    public class PatientResource
    {
        public required string Name { get; set; }
        public required string CPF { get; set; }
        public DateOnly BirthDay { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Mail { get; set; }
        public string? Observations { get; set; }
    }
}

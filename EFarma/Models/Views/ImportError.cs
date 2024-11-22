namespace EFarma.Models.Views
{
    public class ImportError
    {
        public Guid Id { get; } = Guid.NewGuid();
        public required string Line { get; set; }
        public string Error { get; set; } = "";
    }
}

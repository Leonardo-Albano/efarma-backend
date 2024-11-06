using System.Text.Json.Serialization;

namespace EFarma.Models.Views
{
    public class WithdrawItem
    {
        public string Id { get; } = Guid.NewGuid().ToString()[..8];
        public required string Name { get; set; }
        public required decimal Dosage { get; set; }
        public required string Measure { get; set; }
        public string Message { get; set; }

        [JsonIgnore]
        public int Quantity { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace EFarma.Models.Views
{
    public class InStockItemView
    {
        public string StockRoomName { get; set; }
        public int Quantity { get; set; }
        public string MedicamentName { get; set; }  
        public string MedicamentDosage { get; set; }

        [JsonIgnore]
        public int StockRoomId { get; set; }
        [JsonIgnore]
        public int MedicamentId { get; set; }
    }
}

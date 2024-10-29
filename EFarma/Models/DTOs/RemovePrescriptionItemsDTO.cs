namespace EFarma.Models.DTOs
{
    public class RemovePrescriptionItemsDTO
    {
        public int StockRoomId { get; set; }
        public int PrescriptionId { get; set;}
        public int TakeOutResponsibleId { get; set; }
    }
}

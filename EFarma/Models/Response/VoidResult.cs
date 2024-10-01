namespace EFarma.Models.Response
{
    public class VoidResult
    {
        public string Message { get; set; } = "";
        public bool Success { get; set; }
        public int StatusCode { get; set; }
    }
}

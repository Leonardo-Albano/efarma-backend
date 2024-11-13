namespace EFarma.Models.Views
{
    public class PersonView
    {
        public string UniqueId { get; } = Guid.NewGuid().ToString()[..8];
        public int Id { get; set; }
        public string Name { get; set; }
        public string CPF { get; set; }
        public string Role { get; set; }
        public int RoleId { get; set; }
    }
}

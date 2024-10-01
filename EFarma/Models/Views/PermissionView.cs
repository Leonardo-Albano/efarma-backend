using System.ComponentModel.DataAnnotations.Schema;

namespace EFarma.Models.Views
{
    public class PermissionView
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFarma.Models
{
    [Keyless]
    public class AccessLog
    {
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }

        [ForeignKey("StockRoom")]
        public int StockRoomId { get; set; }


        public DateTime DateTimeAccess { get; set; }
        public bool IsEntry { get; set; }

        public required Employee Employee { get; set; }
        public required StockRoom StockRoom { get; set; }

    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFarma.Models
{
    public class AccessLog
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }

        [ForeignKey("StockRoom")]
        public int StockRoomId { get; set; }


        public DateTime Date { get; set; }
        public string Message { get; set; }
        public bool? IsEntry { get; set; }

        public required Employee Employee { get; set; }
        public required StockRoom StockRoom { get; set; }

        public AccessLog Clone()
        {
            return (AccessLog)this.MemberwiseClone();
        }
    }
}

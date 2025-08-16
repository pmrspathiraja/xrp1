using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRP.Domain.Entity
{
    public class BookingAllocations
    {
        public int UId { get; set; }
        public int BookingUId { get; set; }
        public int UserId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? AllocatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool Active { get; set; }
        public decimal Commision { get; set; }
        public decimal Price { get; set; }
        public decimal GiftPrecentage { get; set; }
        public int Stage { get; set; }
        public int XRate { get; set; }


        public virtual Bookings Bookings { get; set; } = null!;
        public virtual Users Users { get; set; } = null!;
    }
}

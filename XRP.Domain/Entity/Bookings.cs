using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRP.Domain.Entity
{
    public class Bookings
    {
        public int UId { get; set; }
        public string? Title { get; set; }       
        public string? Description { get; set; }
        public Decimal Price { get; set; }
        public Decimal Commision { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ImagePath { get; set; }
        public string? Comments { get; set; }
        public bool Active { get; set; }

    }
}

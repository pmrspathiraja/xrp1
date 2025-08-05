using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRP.Domain.DTO
{
    public class UpdateAllocation
    {
        public int UId { get; set; }
        public decimal Price { get; set; }
        public decimal Commision { get; set; }
        public decimal GiftPrecentage { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRP.Domain.Entity
{
    public class Deposits
    {
        [Key]
        public int UId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Active { get; set; }

        public virtual Users Users { get; set; }
    }
}

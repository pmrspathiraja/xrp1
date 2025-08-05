using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRP.Domain.Entity
{
    public class Withdrawal
    {
        [Key]
        public int UId { get; set; }
        public int UserId { get; set; }
        public int BankId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Approved { get; set; }
        public bool Active { get; set; }

        public virtual BankAccounts BankAccount { get; set; }
    }
}

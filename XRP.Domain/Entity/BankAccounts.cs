using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRP.Domain.Entity
{
    public class BankAccounts
    {
        public int UId { get; set; }
        public int UserId { get; set; }
        public string BankName { get; set; }
        public string AccNo { get; set; }
        public string ContactNo { get; set; }
        public string AccHolder { get; set; }
        public string UPI { get; set; }
        public string IFSC { get; set; }
        public bool Active { get; set; }
    }
}

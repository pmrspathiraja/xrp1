using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRP.Domain.DTO
{
    public class WithdrawalPostDto
    {
        public string BankName { get; set; }
        public string AccNo { get; set; }
        public string ContactNo { get; set; }
        public string AccHolder { get; set; }
        public string UPI { get; set; }
        public string IFSC { get; set; }
        public decimal Amount { get; set; }
    } 
}

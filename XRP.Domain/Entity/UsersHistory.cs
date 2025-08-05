using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRP.Domain.Entity
{
    public class UsersHistory
    {
        [Key]
        public int HistoryId { get; set; }
        public int UId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string InvitaionCode { get; set; }
        public string Contact { get; set; }
        public string Level { get; set; }
        public decimal LevelBonus { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal TodayCommission { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal PendingAmount { get; set; }
        public int BookingCount { get; set; }
        public decimal TotalWithdraw { get; set; }
        public decimal TotalDeposit { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Gender { get; set; }
        public bool Active { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

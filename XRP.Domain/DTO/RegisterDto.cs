using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRP.Domain.DTO
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string CountryCode { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public string ConfPassword { get; set; }
        public string InvitaionCode { get; set; }
    }
}

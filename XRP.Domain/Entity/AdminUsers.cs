using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRP.Domain.Entity
{
    public class AdminUsers
    {
        public int UId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
    }
}

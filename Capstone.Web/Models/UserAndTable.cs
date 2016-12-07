using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    public class UserAndTable
    {
        public UserModel User { get; set; }
        public Table Table { get; set; }
        public int MoneyToTheTable { get; set; }
        public bool WasFailure { get; set; }
    }
}
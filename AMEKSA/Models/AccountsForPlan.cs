using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class AccountsForPlan
    {
        public int Id { get; set; }

        public string AccountName { get; set; }

        public bool Choosed { get; set; } = false;
    }
}

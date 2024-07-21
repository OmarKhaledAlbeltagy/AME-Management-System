using AMEKSA.CustomEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class AccountsPerUserModel
    {
        public string FullName { get; set; }

        public List<CustomAccount> Accounts { get; set; }
    }
}

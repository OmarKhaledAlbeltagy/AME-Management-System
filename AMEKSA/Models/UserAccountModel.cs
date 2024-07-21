using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class UserAccountModel
    {
        public string userId { get; set; }

        public List<int> AccountsIds { get; set; }
    }
}

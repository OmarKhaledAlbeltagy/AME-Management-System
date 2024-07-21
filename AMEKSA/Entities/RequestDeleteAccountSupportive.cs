using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class RequestDeleteAccountSupportive
    {
        public int Id { get; set; }

        public int AccountSupportiveVisitId { get; set; }

        public AccountSupportiveVisit accountsupportivevisit { get; set; }
    }
}

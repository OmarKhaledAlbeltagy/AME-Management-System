using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class AccountSupportiveVisitProduct
    {
        public int Id { get; set; }

        public int AccountSupportVisitId { get; set; }

        public AccountSupportiveVisit accountsupportivevisit { get; set; }

        public int ProductId { get; set; }

        public Product product { get; set; }
    }
}

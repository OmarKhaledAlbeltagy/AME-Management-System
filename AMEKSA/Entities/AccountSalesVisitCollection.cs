using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class AccountSalesVisitCollection
    {
        public int Id { get; set; }

        public int AccountSalesVisitId { get; set; }

        public AccountSalesVisit accountsalesvisit { get; set; }

        public float Collection { get; set; }

        public int BrandId { get; set; }

        public Brand brand { get; set; }
    }
}

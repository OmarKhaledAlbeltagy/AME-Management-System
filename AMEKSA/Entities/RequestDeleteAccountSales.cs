using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class RequestDeleteAccountSales
    {
        public int Id { get; set; }

        public int AccountSalesVisitId { get; set; }

        public AccountSalesVisit AccountSalesVisit { get; set; }
    }
}

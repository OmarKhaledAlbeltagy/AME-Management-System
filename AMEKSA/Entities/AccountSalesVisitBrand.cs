using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMEKSA.Entities
{
    public class AccountSalesVisitBrand
    {
        public int Id { get; set; }

        public int AccountSalesVisitId { get; set; }

        public AccountSalesVisit accountsalesvisit { get; set; }

        public int BrandId { get; set; }

        public Brand brand { get; set; }
    }
}

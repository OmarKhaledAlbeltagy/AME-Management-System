using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMEKSA.Entities
{
    public class Brand
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string BrandName { get; set; }

        public ICollection<AccountBalance> accountbalance { get; set; }

        public ICollection<AccountBrandPayment> accountbrandpayment { get; set; }

        public ICollection<AccountSalesVisitBrand> accountsalesvisitbrand { get; set; }

        public ICollection<Product> product { get; set; }

        public ICollection<UserBrand> userbrand { get; set; }

        public ICollection<AccountMonthlyPlanCollection> accountMonthlyPlanCollection { get; set; }

        public ICollection<AccountSalesVisitCollection> accountSalesVisitCollection { get; set; }
    }
}

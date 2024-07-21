using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class AccountBalanceModel
    {
        public int Id { get; set; }

        public string AccountName { get; set; }

        public float TotalBalance { get; set; }

        public float TotalCollection { get; set; }

        public float CreditLimit { get; set; }

        public float Residual { get; set; }

        public List<CollectionByBrandModel> CollectionByBrand { get; set; }

        public List<BalanceByBrandModel> BalanceByBrand { get; set; }
    }
}

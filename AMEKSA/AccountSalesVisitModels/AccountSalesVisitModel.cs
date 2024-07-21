using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.AccountSalesVisitModels
{
    public class AccountSalesVisitModel
    {
 
        public string extendidentityuserid { get; set; }

        public int AccountId { get; set; }

        public DateTime VisitDate { get; set; }

        public DateTime VisitTime { get; set; }

        public string VisitNotes { get; set; }

        public string PaymentNotes { get; set; }

        public string ManagerId { get; set; }

        public ICollection<int> BrandsIds { get; set; }

        public ICollection<AccountSalesVisitPersonModel> PersonModel { get; set; }

        public ICollection<AccountSalesVisitBalanceModel> BalanceModel { get; set; }

        public ICollection<AccountSalesVisitCollectionModel> CollectionModel { get; set; }


    }
}

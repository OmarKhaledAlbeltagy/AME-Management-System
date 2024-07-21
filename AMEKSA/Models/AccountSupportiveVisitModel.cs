using AMEKSA.AccountSalesVisitModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class AccountSupportiveVisitModel
    {
        public string extendidentityuserid { get; set; }

        public int AccountId { get; set; }

        public DateTime VisitDate { get; set; }

        public DateTime VisitTime { get; set; }

        public string VisitNotes { get; set; }

        public string AdditionalNotes { get; set; }

        public string ManagerId { get; set; }

        public ICollection<int> ProductsIds { get; set; }

        public ICollection<AccountSalesVisitPersonModel> PersonModel { get; set; }
    }
}

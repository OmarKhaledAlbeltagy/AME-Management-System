using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.AccountMedicalVisitModels
{
    public class AccountMedicalVisitModel
    {
        public string extendidentityuserid { get; set; }

        public int AccountId { get; set; }

        public DateTime VisitDate { get; set; }

        public DateTime VisitTime { get; set; }

        public string VisitNotes { get; set; }

        public string AdditionalNotes { get; set; }

        public string ManagerId { get; set; }

        public ICollection<AccountMedicalVisitProductModel> ProductModel { get; set; }

        public ICollection<AccountMedicalVisitPersonModel> PersonModel { get; set; }
    }
}

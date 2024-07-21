using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class AccountMonthlyPlan
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public Account account { get; set; }

        public string ExtendIdentityUserId { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        [DataType(DataType.Date)]
        public DateTime PlannedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public bool Status { get; set; } = false;

        public int? AccountMedicalVisitId { get; set; }

        public AccountMedicalVisit AccountMedicalVisit { get; set; }

        public int? AccountSalesVisitId { get; set; }

        public AccountSalesVisit accountsalesvisit { get; set; }

        public int? AccountSupportiveVisitId { get; set; }

        public AccountSupportiveVisit AccountSupportiveVisit { get; set; }

        public ICollection<AccountMonthlyPlanCollection> accountMonthlyPlanCollection { get; set; }
    }
}

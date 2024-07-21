using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class ContactMonthlyPlan
    {
        public int Id { get; set; }

        public int ContactId { get; set; }

        public Contact contact { get; set; }

        public string ExtendIdentityUserId { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }


        [DataType(DataType.Date)]
        public DateTime PlannedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public bool Status { get; set; } = false;

        public int? ContactMedicalVisitId { get; set; }

        public ContactMedicalVisit contactmedicalvisit { get; set; }
    }
}

using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class AccountSupportiveVisit
    {
        public int Id { get; set; }

        public string extendidentityuserid { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        public int AccountId { get; set; }

        public Account account { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime VisitTime { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime VisitDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime SubmittingTime { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime SubmittingDate { get; set; }

        [MaxLength(1000)]
        public string VisitNotes { get; set; }

        [MaxLength(1000)]
        public string AdditionalNotes { get; set; }

        public string ManagerId { get; set; }

        public ExtendIdentityUser Manager { get; set; }

        public ICollection<AccountSupportiveVisitProduct> accountsupportivevisitproduct { get; set; }

        public ICollection<AccountSupportiveVisitPerson> accountsupportivevisitperson { get; set; }
    }
}

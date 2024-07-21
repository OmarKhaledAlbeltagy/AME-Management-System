using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AMEKSA.Privilage;

namespace AMEKSA.Entities
{
    public class AccountMedicalVisit
    {
        public int Id { get; set; }

        public string extendidentityuserid { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        public int AccountId { get; set; }

        public Account account { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime VisitDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime VisitTime { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime SubmittingDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime SubmittingTime { get; set; }

        [MaxLength(1000)]
        public string VisitNotes { get; set; }

        [MaxLength(1000)]
        public string AdditionalNotes { get; set; }

        [ForeignKey("Manager")]
        public string ManagerId { get; set; }

        public ExtendIdentityUser Manager { get; set; }

        public ICollection<AccountMedicalVisitProducts> accountmedicalvisitproducts { get; set; }

        public ICollection<AccountMedicalVisitPerson> accountmedicalvisitperson { get; set; }
    }
}

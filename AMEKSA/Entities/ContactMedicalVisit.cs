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
    public class ContactMedicalVisit
    {
        public int Id { get; set; }

        public string extendidentityuserid { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        public int ContactId { get; set; }

        public Contact contact { get; set; }

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
        public string Requests { get; set; }

        public string ManagerId { get; set; }

        public ExtendIdentityUser Manager { get; set; }

        public ICollection<ContactMedicalVisitProduct> contactmedicalvisitproduct { get; set; }

        public ICollection<ContactSalesAid> contactsalesaid { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMEKSA.Entities
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ContactName { get; set; }

        public bool Gender { get; set; }

        public int? DistrictId { get; set; }
        
        public District district { get; set; }

        [MaxLength(300)]
        public string Address { get; set; }

        [MaxLength(15)]
        public string LandLineNumber { get; set; }

        [MaxLength(15)]
        public string MobileNumber { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        public int? ContactTypeId { get; set; }

        public ContactType contacttype { get; set; }

        [MaxLength(1000)]
        public string PaymentNotes { get; set; }

        [MaxLength(1000)]
        public string RelationshipNote { get; set; }

        [DataType(DataType.Time)]
        public DateTime? BestTimeFrom { get; set; }

        [DataType(DataType.Time)]
        public DateTime? BestTimeTo { get; set; }

        public int? PurchaseTypeId { get; set; }

        public PurchaseType purchasetype { get; set; }

        public int? AccountId { get; set; }

        public Account account { get; set; }

        public int? CategoryId { get; set; }

        //public int? MonthlyTarget { get; set; }

       // public Category category { get; set; }

        public ICollection<ContactMedicalVisit> contactmedicalvisit { get; set; }

        public ICollection<UserContact> usercontact { get; set; }

        public Guid Guidd { get; set; } = Guid.NewGuid();

        public string Hsan { get; set; }
    }
}

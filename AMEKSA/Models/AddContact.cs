using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class AddContact
    {
        public string ContactName { get; set; }

        public bool Gender { get; set; }

        public int? DistrictId { get; set; }

        public string Address { get; set; }

        public string LandLineNumber { get; set; }

        public string MobileNumber { get; set; }

        public string Email { get; set; }

        public int? ContactTypeId { get; set; }

        public string PaymentNotes { get; set; }

        public string RelationshipNote { get; set; }

        public DateTime? BestTimeFrom { get; set; }

        public DateTime? BestTimeTo { get; set; }

        public int? PurchaseTypeId { get; set; }

        public string AccountName { get; set; }

        //public int? MonthlyTarget { get; set; }

        public int? CategoryId { get; set; }
    }
}

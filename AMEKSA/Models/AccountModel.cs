using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class AccountModel
    {
        public string AccountName { get; set; }

        public int? AccountTypeId { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public int? DistrictId { get; set; }

        public string Email { get; set; }

        public int? NumberOfDoctors { get; set; }

        public int? PurchaseTypeId { get; set; }

        public string PaymentNote { get; set; }

        public DateTime? BestTimeFrom { get; set; }

        public DateTime? BestTimeTo { get; set; }

        public string RelationshipNote { get; set; }

        public int? CategoryId { get; set; }
    }
}

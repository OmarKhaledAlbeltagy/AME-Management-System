using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMEKSA.Entities
{
    public class Account
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string AccountName { get; set; }

        public int? AccountTypeId { get; set; }

        public AccountType accounttype { get; set; }


        [MaxLength(300)]
        public string Address { get; set; }

        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        public int? DistrictId { get; set; }

        public District district { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        [Column(TypeName = "smallint")]
        
        public int? NumberOfDoctors { get; set; }

        public int? PurchaseTypeId { get; set; }

        public PurchaseType purchasetype { get; set; }

        [MaxLength(1000)]
        public string PaymentNote { get; set; }

        [DataType(DataType.Time)]
        public DateTime? BestTimeFrom { get; set; }

        [DataType(DataType.Time)]
        public DateTime? BestTimeTo { get; set; }

        [MaxLength(1000)]
        public string RelationshipNote { get; set; }

        public int? CategoryId { get; set; }

        public Category category { get; set; }

        public float CreditLimit { get; set; }

        public ICollection<AccountBalance> accountbalance { get; set; }

        public ICollection<AccountBrandPayment> accountbrandpayment { get; set; }

        public ICollection<AccountMedicalVisit> accountmedicalvisit { get; set; }

        public ICollection<AccountSalesVisit> accountsalesvisit { get; set; }

        public ICollection<Contact> contact { get; set; }

        public ICollection<UserAccount> useraccount { get; set; }
    }
}

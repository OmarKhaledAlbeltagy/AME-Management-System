using AMEKSA.Privilage;
using System;

namespace AMEKSA.Entities
{
    public class ExpensRequestDocument
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreationDateTime { get; set; }

        public double Amount { get; set; }

        public string AmountWord { get; set; }

        public int EventtId { get; set; }

        public Event eventt { get; set; }

        public int ContactId { get; set; }

        public Contact contact { get; set; }

        public string Note { get; set; }

        public string? ExtendIdentityUserId { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        public string BankAccounOwner { get; set; }

        public string BankName { get; set; }

        public string Iban { get; set; }


    }
}

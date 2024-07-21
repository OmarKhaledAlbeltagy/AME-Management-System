using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class ContactGeneralInfo
    {
        public int Id { get; set; }

        public string ContactName { get; set; }

        public bool Gender { get; set; }

        public int ContactTypeId { get; set; }

        public int PurchaseTypeId { get; set; }

        public int? AccountId { get; set; }

        public string AccountName { get; set; }

    }
}

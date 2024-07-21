using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class AccountGeneralInfo
    {
        public int Id { get; set; }

        public string AccountName { get; set; }

        public int AccountTypeId { get; set; }

        public int PurchaseTypeId { get; set; }

        public int CategoryId { get; set; }

        public int? NumberOfDoctors { get; set; }
    }
}

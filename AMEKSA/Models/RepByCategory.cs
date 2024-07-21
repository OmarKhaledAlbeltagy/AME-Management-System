using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class RepByCategory
    {
        public string RepName { get; set; }

        public int TotalContacts { get; set; }

        public int APlusContacts { get; set; }

        public int AContacts { get; set; }

        public int BContacts { get; set; }

        public int CContacts { get; set; }

        public float APlusContactsPercentage { get; set; }

        public float AContactsPercentage { get; set; }

        public float BContactsPercentage { get; set; }

        public float CContactsPercentage { get; set; }

        public int? APlusContactsTarget { get; set; }

        public int? AContactsTarget { get; set; }

        public int? BContactsTarget { get; set; }

        public int? CContactsTarget { get; set; }

        public float? APlusContactsTargetPercentage { get; set; }

        public float? AContactsTargetPercentage { get; set; }

        public float? BContactsTargetPercentage { get; set; }

        public float? CContactsTargetPercentage { get; set; }

        public int? TotalTarget { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.FirstManagerModels
{
    public class MyTeamModel
    {
        public string FullName { get; set; }

        public string userId { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public int AccountsVisits { get; set; }

        public int AccountsVisitsPastMonth { get; set; }

        public int? ContactsVisits { get; set; }

        public int? ContactsVisitsPastMonth { get; set; }

        public int? ContactTarget { get; set; }

        public int? ContactTargetPastMonth { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class RepContactNotVisitsReport
    {
        public int ContactId { get; set; }

        public string ContactName { get; set; }

        public string AccountName { get; set; }

        public string CategoryName { get; set; }

        public int? MonthlyTarget { get; set; }
    }
}

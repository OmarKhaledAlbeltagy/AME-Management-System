using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class RepContactVisitsReport
    {
        public int ContactId { get; set; }

        public string ContactName { get; set; }

        public string AccountName { get; set; }

        public string CategoryName { get; set; }

        public int? MonthlyTarget { get; set; }

        public int VisitsCount { get; set; }

        public DateTime LastVisit { get; set; }

        public DateTime BeforeLastVisit { get; set; }

        public DateTime BeforeBeforeLastVisit { get; set; }

        public DateTime BeforeBeforeBeforeLastVisit { get; set; }
    }
}

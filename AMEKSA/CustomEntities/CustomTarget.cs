using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomTarget
    {
        public string FullName { get; set; }

        public string ContactName { get; set; }

        public int CurrentVisits { get; set; }

        public int? MonthlyTarget { get; set; }

        public string CategoryName { get; set; }
    }
}

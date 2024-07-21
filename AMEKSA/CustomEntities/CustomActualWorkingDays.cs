using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomActualWorkingDays
    {
        public string FullName { get; set; }

        public int? WorkingDays { get; set; }

        public int ActualWorkingDays { get; set; }

        public int TimeOffTerritory { get; set; }

        public int Role { get; set; }
    }
}

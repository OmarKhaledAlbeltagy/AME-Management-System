using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class WorkingDays
    {
        public int Id { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public int? NumberOfWorkingDays { get; set; }
    }
}

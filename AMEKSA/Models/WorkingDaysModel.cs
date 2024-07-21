using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class WorkingDaysModel
    {
        public int Id { get; set; }

        public string Month { get; set; }

        public int? WorkingDays { get; set; }
    }
}

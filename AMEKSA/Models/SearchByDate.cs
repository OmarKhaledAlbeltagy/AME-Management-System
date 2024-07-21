using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class SearchByDate
    {
        public string UserId { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }
    }
}

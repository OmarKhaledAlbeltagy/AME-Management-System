using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomRequestContactChangeTarget
    {
        public int Id { get; set; }

        public string RepName { get; set; }

        public string ContactName { get; set; }

        public int TargetFrom { get; set; }

        public int TargetTo { get; set; }

        public string RequestDateTime { get; set; }

        public DateTime DT { get; set; }

        public bool? Status { get; set; }
    }
}

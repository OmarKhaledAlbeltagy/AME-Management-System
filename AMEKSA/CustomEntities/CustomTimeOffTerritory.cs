using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomTimeOffTerritory
    {
        public int Id { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public string Reason { get; set; }

        public string Description { get; set; }

        public string UserId { get; set; }

        public string FullName { get; set; }

        public bool? Accepted { get; set; }

        public bool? status { get; set; }
    }
}

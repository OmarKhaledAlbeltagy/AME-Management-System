using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class CollectionByRepModel
    {
        public string FullName { get; set; }

        public string UserId { get; set; }

        public float? PlannedTotal { get; set; }

        public float? ActualTotal { get; set; }

        public List<CollectionByBrandModel> list { get; set; }
    }
}

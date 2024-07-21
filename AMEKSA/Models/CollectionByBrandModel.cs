using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class CollectionByBrandModel
    {
        public string BrandName { get; set; }

        public float? PlannedCollection { get; set; }

        public float? ActualCollection { get; set; }
    }
}

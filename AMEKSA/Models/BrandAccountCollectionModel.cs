using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class BrandAccountCollectionModel
    {
        public string AccountName { get; set; }

        public string BrandName { get; set; }

        public float? PlannedCollection { get; set; }

        public float ActualCollection { get; set; }
    }
}

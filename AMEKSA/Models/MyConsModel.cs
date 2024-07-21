using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class MyConsModel
    {
        public int Id { get; set; }

        public string ContactName { get; set; }

        public int? DistrictId { get; set; }

        public int CityId { get; set; }

        public int? CategoryId { get; set; }
    }
}

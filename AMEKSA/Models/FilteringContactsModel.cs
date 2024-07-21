using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class FilteringContactsModel
    {
        public int ContactTypeId { get; set; }

        public int CityId { get; set; }

        public int DistrictId { get; set; }
    }
}

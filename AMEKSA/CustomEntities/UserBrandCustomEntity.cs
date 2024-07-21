using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class UserBrandCustomEntity
    {
        public string UserId { get; set; }

        public IEnumerable<int> BrandsIds { get; set; }
    }
}

using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class UserDistrict
    {
        public int Id { get; set; }

        public string extendidentityuserid { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        public int Dis { get; set; }

    }
}

using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class RequestChangeContactCategory
    {
        public int Id { get; set; }

        public string ExtendIdentityUserId { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        public int ContactId { get; set; }

        public int CategoryFromId { get; set; }

        public int CategoryToId { get; set; }

        public bool Confirmed { get; set; }

        public bool Rejected { get; set; }
    }
}

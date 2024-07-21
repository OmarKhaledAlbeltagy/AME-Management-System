using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class RequestChangeContactTarget
    {
        public int Id { get; set; }

        public int ContactId { get; set; }

        public Contact contact { get; set; }

        public int OldTarget { get; set; }

        public int NewTarget { get; set; }

        public string ManagerId { get; set; }

        public ExtendIdentityUser manager { get; set; }

        public string RepId { get; set; }

        public ExtendIdentityUser rep { get; set; }

        public DateTime RequestDateTime { get; set; }

        public bool? Status { get; set; }
    }
}

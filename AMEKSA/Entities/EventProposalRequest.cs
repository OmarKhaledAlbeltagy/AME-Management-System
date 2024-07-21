using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class EventProposalRequest
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public Event eventt { get; set; }

        public string ExtendIdentityUserId { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        public int ContactId { get; set; }

        public Contact contact { get; set; }

        public bool Confirmed { get; set; } = false;

        public bool Rejected { get; set; } = false;

        public bool TopConfirmed { get; set; } = false;

        public bool TopRejected { get; set; } = false;

        public bool TopAction { get; set; } = false;

        public string TopActionUserId { get; set; } = null;

        public ExtendIdentityUser TopActionUser { get; set; }

        public DateTime RequestDate { get; set; }
    }
}

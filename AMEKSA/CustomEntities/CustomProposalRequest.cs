using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomProposalRequest
    {
        public int Id { get; set; }

        public string RepName { get; set; }

        public string EventName { get; set; }

        public string EventTypeName { get; set; }

        public string ContactName { get; set; }

        public string AccountAffiliation { get; set; }

        public string ContactPhone { get; set; }

        public string ContactMail { get; set; }

        public bool? Gender { get; set; }

        public bool Confirmed { get; set; }

        public bool Rejected { get; set; }

        public bool TopConfirmed { get; set; }

        public bool TopRejected { get; set; }

        public bool TopAction { get; set; }

        public string TopActionUserId { get; set; }

        public string TopActionUserName { get; set; }

        public DateTime RequestDate { get; set; }

        public string RepCity { get; set; }

    }
}

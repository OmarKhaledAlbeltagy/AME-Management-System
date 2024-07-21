using AMEKSA.Privilage;
using System;

namespace AMEKSA.Entities
{
    public class EventFees
    {
        public int Id { get; set; }

        public int EventtId { get; set; }

        public Event eventt { get; set; }

        public string ExtendIdentityUserId { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        public string Title { get; set; }

        public double Value { get; set; }

        public string? Note { get; set; }

        public byte[] file { get; set; }

        public string ContentType { get; set; }

        public string Extension { get; set; }

        public int? Sort { get; set; }

        public DateTime dateTime { get; set; }
    }
}

using System;

namespace AMEKSA.Models
{
    public class EventFeesSimpleModel
    {
        public int Id { get; set; }

        public string? EventName { get; set; }

        public string? EventType { get; set; }

        public string? UserName { get; set; }

        public string Title { get; set; }

        public double Value { get; set; }

        public string Note { get; set; }

        public string DateTime { get; set; }

        public DateTime SortDateTime { get; set; }

        public bool? Confirmed { get; set; }

        public string? RejectionReason { get; set; }

        public bool Confirmable { get; set; } = true;
    }
}

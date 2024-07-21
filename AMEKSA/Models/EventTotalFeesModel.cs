using System;

namespace AMEKSA.Models
{
    public class EventTotalFeesModel
    {
        public int Id { get; set; }

        public string EventName { get; set; }

        public double TotalFees { get; set; }

        public double TotalFeesRequested { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public DateTime date { get; set; }

        public int? Attendees { get; set; }

        public int? Pending { get; set; }
    }
}

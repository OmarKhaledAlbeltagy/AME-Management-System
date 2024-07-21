using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomEvent
    {
        public int Id { get; set; }

        public string EventName { get; set; }

        public DateTime OrderDate { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string CityName { get; set; }

        public string EventTypeName { get; set; }

        public int AcceptedRequests { get; set; }

        public int RejectedRequests { get; set; }

        public int PendingRequests { get; set; }

        public int TotalRequests { get; set; }

        public bool? IsUpcoming { get; set; }
    }
}

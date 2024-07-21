using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class Event
    {
        public int Id { get; set; }

        public string EventName { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int TravelCitiesId { get; set; }

        public TravelCities TravelCities { get; set; }

        public int EventTypeId { get; set; }

        public EventType EventType { get; set; }

        public string LocationName { get; set; }

        public ICollection<EventSpeaker> speakers { get; set; }
    }
}

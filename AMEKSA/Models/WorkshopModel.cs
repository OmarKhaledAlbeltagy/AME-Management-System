using AMEKSA.Entities;
using System;
using System.Collections.Generic;

namespace AMEKSA.Models
{
    public class WorkshopModel
    {
        public int Id { get; set; }

        public string EventName { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int TravelCitiesId { get; set; }

        public string LocationName { get; set; }

        public List<string> speakers { get; set; }
    }
}

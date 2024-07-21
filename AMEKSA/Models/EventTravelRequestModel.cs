using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class EventTravelRequestModel
    {
        public int? Id { get; set; }

        public int EventId { get; set; }

        public string ExtendIdentityUserId { get; set; }

        public int ContactId { get; set; }

        public DateTime WayInDeparture { get; set; }

        public DateTime WayOutDeparture { get; set; }

        public DateTime WayInArrival { get; set; }

        public DateTime WayOutArrival { get; set; }

        public int WayInCityId { get; set; }

        public int WayOutCityId { get; set; }

        public string WayInFlightNumber { get; set; }

        public string WayOutFlightNumber { get; set; }

        public int WayInDestinationId { get; set; }

        public int WayOutDestinationId { get; set; }

        public string HotelName { get; set; }

        public string RoomType { get; set; }

        public byte Accumpained { get; set; }

        public bool Confirmed { get; set; } = false;

        public bool Rejected { get; set; } = false;

        public bool TopConfirmed { get; set; } = false;

        public bool TopRejected { get; set; } = false;

        public bool TopAction { get; set; } = false;

        public string TopActionUserId { get; set; } = null;

        public bool HotelEdited { get; set; } = false;

        public string PassportNumber { get; set; }

        public DateTime PassportExpiryDate { get; set; }

        public IFormFile file { get; set; }
    }
}

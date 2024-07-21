using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomEventRequest
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

        public DateTime? WayInDeparture { get; set; }

        public DateTime? WayOutDeparture { get; set; }

        public DateTime? WayInArrival { get; set; }

        public DateTime? WayOutArrival { get; set; }

        public string WayInCityName { get; set; }

        public string WayOutCityName { get; set; }

        public string WayInFlightNumber { get; set; }

        public string WayOutFlightNumber { get; set; }

        public string WayInDestinationName { get; set; }

        public string WayOutDestinationName { get; set; }

        public string HotelName { get; set; }

        public string RoomType { get; set; }

        public byte Accumpained { get; set; }

        public bool Confirmed { get; set; }

        public bool Rejected { get; set; }

        public bool TopConfirmed { get; set; }

        public bool TopRejected { get; set; }

        public bool TopAction { get; set; }

        public string TopActionUserId { get; set; }

        public string TopActionUserName { get; set; }

        public string PassportNumber { get; set; }

        public DateTime? PassportExpiryDate { get; set; }

        public bool IsPassport { get; set; }

        public bool IsTicket { get; set; }

        public string RepCity { get; set; }

        public string Hsan { get; set; }

        public DateTime? FirstActionDateTime { get; set; }

        public DateTime? TopActionDateTime { get; set; }

        public string? FirstActionDateTimestr { get; set; }

        public string? TopActionDateTimestr { get; set; }
    }
}

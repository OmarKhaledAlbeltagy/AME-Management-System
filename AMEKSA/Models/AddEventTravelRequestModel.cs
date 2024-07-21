using Microsoft.AspNetCore.Http;
using System;

namespace AMEKSA.Models
{
    public class AddEventTravelRequestModel
    {
        public int EventId { get; set; }

        public string ExtendIdentityUserId { get; set; }

        public int ContactId { get; set; }

        public DateTime? WayInDeparture { get; set; }

        public DateTime? WayOutDeparture { get; set; }

        public DateTime? WayInArrival { get; set; }

        public DateTime? WayOutArrival { get; set; }

        public int? WayInCityId { get; set; }

        public int? WayOutCityId { get; set; }

        public string WayInFlightNumber { get; set; }

        public string WayOutFlightNumber { get; set; }

        public int? WayInDestinationId { get; set; }

        public int? WayOutDestinationId { get; set; }

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

        public DateTime? PassportExpiryDate { get; set; }

        public bool IsPassport { get; set; } = false;

        public string PassportFileName { get; set; } = null;

        public string PassportFileContentType { get; set; } = null;

        public bool IsTicket { get; set; } = false;

        public string TicketFileName { get; set; } = null;

        public string TicketFileContentType { get; set; } = null;

        public DateTime? FirstActionDateTime { get; set; }

        public DateTime? TopActionDateTime { get; set; }

        public IFormFile? file { get; set; }

    }
}

using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IEventRep
    {
        IEnumerable<CustomEventRequest> TedSheet();

        Event GetEventSpeakersAndLocation(int id);

        bool AddNewEvent(Event ev);

        bool EditEvent(Event ev);

        WorkshopModel GetWorkshopById(int id);

        bool EditWorkshop(EditWorkshopModel w);

        IEnumerable<EventType> GetEventTypes();

        IEnumerable<Event> GetUpComingEvents();

        IEnumerable<Event> GetSixMonthsBackEvents();

        IEnumerable<Event> GetEventsForSystemAdmin();

        IEnumerable<TravelCountriesModel> GetCountries();

        IEnumerable<TravelCities> GetCitiesByCountryName(string CountryName);

        IEnumerable<TravelCities> GetAllCities();

        Task<bool> MakeEventRequestAsync(AddEventTravelRequestModel ev);

        Event GetEventTravellingById(int id);

        IEnumerable<CustomEvent> GetAllEvents(string userid);

        IEnumerable<CustomEventRequest> GetMyTeamRequests(string ManagerId);

        IEnumerable<CustomEventRequest> GetMyRequests(string Id);

        IEnumerable<CustomEventRequest> GetMyTeamRequestsByEventId(int Id, string ManagerId);

        IEnumerable<CustomEventRequest> GetAllRequestsByEventId(int Id);

        EventTravelRequest GetEventTravelRequest(int id);

        bool DeleteRequest(int id);

        bool DeletePassport(int id);

        bool DeleteTicket(int id);

        bool DeleteProposal(int id);

        bool EditRequest(EventTravelRequest ev, string UserId);

        bool ConfirmRequest(int id);

        bool RejectRequest(int id);

        bool TopConfirmRequest(int id, string TopId);

        bool TopRejectRequest(int id, string TopId);

        bool TopConfirmProposal(int id, string TopId);

        bool TopRejectProposal(int id, string TopId);

        bool TopHoldProposal(int id);

        bool TopHoldRequest(int id);

        bool HoldRequest(int id);

        bool ConfirmProposal(int id);

        bool RejectProposal(int id);

        bool HoldProposal(int id);

        bool DeleteEventAndRequests(int id);

        IEnumerable<CustomEventRequest> GetAllRequests();

        IEnumerable<CustomEventRequest> GetAllPastRequests();

        List<EventProposalCheckModel> MakeEventProposal(List<EventProposalRequest> evp);

        IEnumerable<CustomProposalRequest> GetMyProposalRequests(string Id);

        IEnumerable<CustomProposalRequest> GetMyTeamProposalRequests(string ManagerId);

        IEnumerable<CustomProposalRequest> GetMyTeamPendingProposalRequests(string ManagerId);

        IEnumerable<CustomProposalRequest> GetAllProposalRequests();

        IEnumerable<CustomProposalRequest> GetAllPendingProposalRequests();

        IEnumerable<CustomProposalRequest> GetEventProposalRequests(int id);

        IEnumerable<Contact> GetApprovedEventContacts(string UserId, int EventId);

        Event GetEventById(int id);

        WorkshopInfoModel GetWorkshopInfo(int id);

        List<Event> GetSixMonthEvents();

        List<EventAttendanceModel> GetEventAttenance(int EventId);
    }
}

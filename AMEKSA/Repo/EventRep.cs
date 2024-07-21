using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using ClosedXML;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class EventRep : IEventRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public EventRep(DbContainer db, ITimeRep ti, UserManager<ExtendIdentityUser> userManager)
        {
            this.db = db;
            this.ti = ti;
            this.userManager = userManager;
        }

        public bool AddNewEvent(Event ev)
        {
            DateTime now = ti.GetCurrentTime();
            Event e = new Event();
            e.TravelCitiesId = ev.TravelCitiesId;
            e.EventName = ev.EventName;
            e.EventTypeId = ev.EventTypeId;
            e.From = ev.From;
            e.To = ev.To;
            e.LocationName = ev.LocationName;
            db.Event.Add(e);
            db.SaveChanges();
            foreach (var item in ev.speakers)
            {
                EventSpeaker evs = new EventSpeaker();
                evs.EventId = e.Id;
                evs.SpeakerName = item.SpeakerName;
                db.EventSpeaker.Add(evs);
            }
            List<string> managers = userManager.GetUsersInRoleAsync("First Line Manager").Result.Select(a => a.Id).ToList();
            List<string> medical = userManager.GetUsersInRoleAsync("Medical Representative").Result.Select(a => a.Id).ToList();
            foreach (var item in managers)
            {
                Notifications n = new Notifications();
                n.NitificationDateTime = now;
                n.ExtendIdetityUserId = item;
                n.NotificationDetails = "A new Event: " + ev.EventName + " on " + ev.From.ToString("dd MMMM yyyy") + " has been added";
                n.Url = "EventList.html";
                db.notifications.Add(n);
            }
            foreach (var item in medical)
            {
                Notifications n = new Notifications();
                n.NitificationDateTime = now;
                n.ExtendIdetityUserId = item;
                n.NotificationDetails = "A new Event: " + ev.EventName + " on " + ev.From.ToString("dd MMMM yyyy") + " has been added";
                n.Url = "EventProposal.html";
                db.notifications.Add(n);
            }
            db.SaveChanges();
            return true;
        }

        public bool ConfirmRequest(int id)
        {
            DateTime now = ti.GetCurrentTime();
            EventTravelRequest res = db.EventTravelRequest.Find(id);
            res.Confirmed = true;
            res.Rejected = false;
            res.FirstActionDateTime = now;

            EventProposalRequest p = db.EventProposalRequest.Where(a => a.ExtendIdentityUserId == res.ExtendIdentityUserId && a.ContactId == res.ContactId && res.EventId == res.EventId).FirstOrDefault();
            if (p != null)
            {
                p.Rejected = false;
                p.Confirmed = true;
            }

            string contactname = db.contact.Find(res.ContactId).ContactName;

            Notifications n = new Notifications();
            n.ExtendIdetityUserId = res.ExtendIdentityUserId;
            n.NitificationDateTime = now;
            n.NotificationDetails = "Your Event Travel Request for " + contactname + " has been Accepted";
            n.Url = "MyEventRequests.html";
            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }

        public bool DeleteEventAndRequests(int id)
        {
            DateTime now = new DateTime();
            List<string> managers = userManager.GetUsersInRoleAsync("First Line Manager").Result.Select(a => a.Id).ToList();
            List<string> medical = userManager.GetUsersInRoleAsync("Medical Representative").Result.Select(a => a.Id).ToList();
            List<string> top = userManager.GetUsersInRoleAsync("Top Line Manager").Result.Select(a => a.Id).ToList();
            List<string> system = userManager.GetUsersInRoleAsync("System Admin").Result.Select(a => a.Id).ToList();

            List<EventTravelRequest> list = db.EventTravelRequest.Where(a => a.EventId == id).ToList();
            List<EventProposalRequest> pro = db.EventProposalRequest.Where(a => a.EventId == id).ToList();
            List<EventSpeaker> evspeaker = db.EventSpeaker.Where(a => a.EventId == id).ToList();
            foreach (var item in list)
            {
                db.EventTravelRequest.Remove(item);
            }
            foreach (var item in pro)
            {
                db.EventProposalRequest.Remove(item);
            }
            foreach (var item in evspeaker)
            {
                db.EventSpeaker.Remove(item);
            }

            Event ev = db.Event.Find(id);
            db.Event.Remove(ev);
            foreach (var item in managers)
            {
                Notifications n = new Notifications();
                n.ExtendIdetityUserId = item;
                n.NitificationDateTime = now;
                n.NotificationDetails = "Event: " + ev.EventName + " has been deleted, if you have requested travel or proposal for this event, it was deleted also";
                n.Url = "EventList.html";
                db.notifications.Add(n);
            }
            foreach (var item in medical)
            {
                Notifications n = new Notifications();
                n.ExtendIdetityUserId = item;
                n.NitificationDateTime = now;
                n.NotificationDetails = "Event: " + ev.EventName + " has been deleted, if you have requested travel or proposal for this event, it was deleted also";
                n.Url = "MyEventRequests.html";
                db.notifications.Add(n);
            }
            foreach (var item in top)
            {
                Notifications n = new Notifications();
                n.ExtendIdetityUserId = item;
                n.NitificationDateTime = now;
                n.NotificationDetails = "Event: " + ev.EventName + " has been deleted, if you have requested travel or proposal for this event, it was deleted also";
                n.Url = "EventList.html";
                db.notifications.Add(n);
            }

         

            db.SaveChanges();
            return true;
        }

        public bool DeleteRequest(int id)
        {
            bool f = false;
            EventTravelRequest res = db.EventTravelRequest.Find(id);
            EventProposalRequest p = db.EventProposalRequest.Where(a=>a.ExtendIdentityUserId == res.ExtendIdentityUserId && a.EventId == res.EventId && a.ContactId == res.ContactId).FirstOrDefault();
            if (res.IsPassport == true)
            {
                f = true;
            }
            if (p != null)
            {
                db.EventProposalRequest.Remove(p);
            }
            db.EventTravelRequest.Remove(res);
            db.SaveChanges();

            if(f == true)
            {
                var path = Path.Combine(
                              Directory.GetCurrentDirectory(),
                              "wwwroot", "Passport",
                              res.PassportFileName);

                File.Delete(path);
                
            }
            



            return true;
        }

        public bool EditRequest(EventTravelRequest ev, string UserId)
        {
            DateTime now = ti.GetCurrentTime();
            EventTravelRequest obj = db.EventTravelRequest.Find(ev.Id);

            string role = userManager.GetRolesAsync(userManager.FindByIdAsync(UserId).Result).Result.FirstOrDefault();

            if (role == "Top Line Manager" || role == "First Line Manager" || role == "System Admin")
            {
                if (ev.HotelName != obj.HotelName)
                {
                    obj.HotelEdited = true;
                }
            }

            int oldcontact = obj.ContactId;
            int newcontact = ev.ContactId;
            string managerid = userManager.FindByIdAsync(ev.ExtendIdentityUserId).Result.extendidentityuserid;
            string repname = userManager.FindByIdAsync(ev.ExtendIdentityUserId).Result.FullName;

            obj.Accumpained = ev.Accumpained;
            obj.ContactId = ev.ContactId;
            obj.EventId = ev.EventId;
            obj.HotelName = ev.HotelName;
            obj.PassportExpiryDate = ev.PassportExpiryDate;
            obj.PassportNumber = ev.PassportNumber;
            obj.RoomType = ev.RoomType;
            obj.WayInArrival = ev.WayInArrival;
            obj.WayInDeparture = ev.WayInDeparture;
            obj.WayInFlightNumber = ev.WayInFlightNumber;
            obj.WayOutArrival = ev.WayOutArrival;
            obj.WayOutDeparture = ev.WayOutDeparture;
            obj.WayOutFlightNumber = ev.WayOutFlightNumber;

            if (ev.WayInDestinationId != 0)
            {
                obj.WayInDestinationId = ev.WayInDestinationId;
            }
            if (ev.WayOutDestinationId != 0)
            {
                obj.WayOutDestinationId = ev.WayOutDestinationId;
            }
            if (ev.WayInCityId != 0)
            {
                obj.WayInCityId = ev.WayInCityId;
            }
            if (ev.WayOutCityId != 0)
            {
                obj.WayOutCityId = ev.WayOutCityId;
            }

            if (managerid != null || managerid != "")
            {

                if (obj.Confirmed == true && obj.Rejected == false)
                {
                    Notifications n = new Notifications();
                    n.NitificationDateTime = now;
                    n.ExtendIdetityUserId = managerid;
                    n.NotificationDetails = "A travel request you have been accepted for " + repname + " has been edited";
                    n.Url = "MyTeamEventRequest.html";
                    db.notifications.Add(n);
                }
                else
                {
                    if (obj.Confirmed == false && obj.Rejected == true)
                    {
                        Notifications n = new Notifications();
                        n.NitificationDateTime = now;
                        n.ExtendIdetityUserId = managerid;
                        n.NotificationDetails = "A travel request you have been rejected for " + repname + " has been edited";
                        n.Url = "MyTeamEventRequest.html";
                        db.notifications.Add(n);
                    }
                }
            }

            db.SaveChanges();
            return true;

        }

        public IEnumerable<TravelCities> GetAllCities()
        {
            IEnumerable<TravelCities> res = db.travelCities.Select(a=>a);
            return res.OrderBy(a => a.City);
        }

        public IEnumerable<CustomEvent> GetAllEvents(string userid)
        {

            string role = userManager.GetRolesAsync(userManager.FindByIdAsync(userid).Result).Result.ToList().FirstOrDefault();



            DateTime now = ti.GetCurrentTime();

            List<CustomEvent> res = db.Event.Join(db.travelCities, a => a.TravelCitiesId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityName = b.City,
                EventName = a.EventName,
                EventTypeId = a.EventTypeId,
                From = a.From,
                To = a.To
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new CustomEvent
            {
                Id = a.Id,
                CityName = a.CityName,
                EventName = a.EventName,
                EventTypeName = b.TypeName,
                OrderDate = a.From,
                From = a.From.ToString("dd MMMM yyyy"),
                To = a.To.ToString("dd MMMM yyyy")
            }).ToList();


            List<CustomEvent> ress = new List<CustomEvent>();


            foreach (var item in res)
            {

                if (role == "First Line Manager" && item.EventTypeName == "Workshop")
                {
                    int ManagerCityId = (int)userManager.FindByIdAsync(userid).Result.CityId;
                    string AccountName = db.Event.Find(item.Id).LocationName;
                    Account acc = db.account.Where(a => a.AccountName == AccountName).FirstOrDefault();
                    
                    if (acc == null)
                    {
                        CustomEvent obj = new CustomEvent();
                        obj.Id = item.Id;
                        obj.CityName = item.CityName;
                        obj.EventName = item.EventName;
                        obj.EventTypeName = item.EventTypeName;
                        obj.OrderDate = item.OrderDate;
                        obj.From = item.From;
                        obj.To = item.To;
                        List<EventTravelRequest> ev = db.EventTravelRequest.Where(a => a.EventId == item.Id).ToList();
                        obj.TotalRequests = ev.Count();

                        int accepted = 0;
                        int rejected = 0;
                        int pending = 0;



                        foreach (var e in ev)
                        {
                            if (e.TopConfirmed == true && e.TopRejected == false)
                            {
                                accepted = accepted + 1;
                            }
                            else
                            {
                                if (e.TopConfirmed == false && e.TopRejected == true)
                                {
                                    rejected = rejected + 1;
                                }

                                else
                                {
                                    if (e.TopConfirmed == false && e.TopRejected == false && e.Confirmed == true && e.Rejected == false)
                                    {
                                        accepted = accepted + 1;
                                    }

                                    else
                                    {
                                        if (e.TopConfirmed == false && e.TopRejected == false && e.Confirmed == false && e.Rejected == true)
                                        {
                                            rejected = rejected + 1;
                                        }
                                        else
                                        {
                                            if (e.TopConfirmed == false && e.TopRejected == false && e.Confirmed == false && e.Rejected == false)
                                            {
                                                pending = pending + 1;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        obj.AcceptedRequests = accepted;
                        obj.RejectedRequests = rejected;
                        obj.PendingRequests = pending;

                        if (item.OrderDate.Date > now.Date)
                        {
                            obj.IsUpcoming = true;
                        }
                        else
                        {
                            obj.IsUpcoming = false;
                        }


                        ress.Add(obj);
                    }

                    else
                    {
                        int? AccountDistrictId = acc.DistrictId;
                        int AccountCityId = db.district.Find(AccountDistrictId).CityId;

                        if (ManagerCityId == AccountCityId)
                        {
                            CustomEvent obj = new CustomEvent();
                            obj.Id = item.Id;
                            obj.CityName = item.CityName;
                            obj.EventName = item.EventName;
                            obj.EventTypeName = item.EventTypeName;
                            obj.OrderDate = item.OrderDate;
                            obj.From = item.From;
                            obj.To = item.To;
                            List<EventTravelRequest> ev = db.EventTravelRequest.Where(a => a.EventId == item.Id).ToList();
                            obj.TotalRequests = ev.Count();

                            int accepted = 0;
                            int rejected = 0;
                            int pending = 0;



                            foreach (var e in ev)
                            {
                                if (e.TopConfirmed == true && e.TopRejected == false)
                                {
                                    accepted = accepted + 1;
                                }
                                else
                                {
                                    if (e.TopConfirmed == false && e.TopRejected == true)
                                    {
                                        rejected = rejected + 1;
                                    }

                                    else
                                    {
                                        if (e.TopConfirmed == false && e.TopRejected == false && e.Confirmed == true && e.Rejected == false)
                                        {
                                            accepted = accepted + 1;
                                        }

                                        else
                                        {
                                            if (e.TopConfirmed == false && e.TopRejected == false && e.Confirmed == false && e.Rejected == true)
                                            {
                                                rejected = rejected + 1;
                                            }
                                            else
                                            {
                                                if (e.TopConfirmed == false && e.TopRejected == false && e.Confirmed == false && e.Rejected == false)
                                                {
                                                    pending = pending + 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            obj.AcceptedRequests = accepted;
                            obj.RejectedRequests = rejected;
                            obj.PendingRequests = pending;

                            if (item.OrderDate.Date > now.Date)
                            {
                                obj.IsUpcoming = true;
                            }
                            else
                            {
                                obj.IsUpcoming = false;
                            }


                            ress.Add(obj);
                        }
                    }
                

                }

                else
                {
                



                CustomEvent obj = new CustomEvent();
                obj.Id = item.Id;
                obj.CityName = item.CityName;
                obj.EventName = item.EventName;
                obj.EventTypeName = item.EventTypeName;
                obj.OrderDate = item.OrderDate;
                obj.From = item.From;
                obj.To = item.To;
                List <EventTravelRequest> ev = db.EventTravelRequest.Where(a => a.EventId == item.Id).ToList();
                obj.TotalRequests = ev.Count();

                int accepted = 0;
                int rejected = 0;
                int pending = 0;

        

                foreach (var e in ev)
                {
                    if (e.TopConfirmed == true && e.TopRejected == false)
                    {
                        accepted = accepted + 1;
                    }
                    else
                    {
                        if (e.TopConfirmed == false && e.TopRejected == true)
                        {
                            rejected = rejected + 1;
                        }

                        else
                        {
                            if (e.TopConfirmed == false && e.TopRejected == false && e.Confirmed == true && e.Rejected == false)
                            {
                                accepted = accepted + 1;
                            }

                            else
                            {
                                if (e.TopConfirmed == false && e.TopRejected == false && e.Confirmed == false && e.Rejected == true)
                                {
                                    rejected = rejected + 1;
                                }
                                else
                                {
                                    if (e.TopConfirmed == false && e.TopRejected == false && e.Confirmed == false && e.Rejected == false)
                                    {
                                        pending = pending + 1;
                                    }
                                }
                            }
                        }
                    }
                }

                obj.AcceptedRequests = accepted;
                obj.RejectedRequests = rejected;
                obj.PendingRequests = pending;

                if (item.OrderDate.Date > now.Date)
                {
                    obj.IsUpcoming = true;
                }
                else
                {
                    obj.IsUpcoming = false;
                }


                ress.Add(obj);
                }
            }
            return ress.OrderByDescending(a => a.OrderDate);
        }

        public IEnumerable<CustomEventRequest> GetAllRequestsByEventId(int Id)
        {
            DateTime now = ti.GetCurrentTime();

            List<EventTravelRequest> list = db.EventTravelRequest.Where(a => a.EventId == Id).ToList();

            IEnumerable<CustomEventRequest> res = list.Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                RepName = b.FullName,
                ContactId = a.ContactId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = b.CityId
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = b.Gender,
                Hsan = b.Hsan,
                RepName = a.RepName,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.travelCities, a => a.WayInCityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = b.City,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.travelCities, a => a.WayOutCityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = b.City,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.travelCities, a => a.WayInDestinationId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = b.City,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.travelCities, a => a.WayOutDestinationId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = b.City,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeId = b.EventTypeId,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeName = b.TypeName,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.city, a => a.RepCityId, b => b.Id, (a, b) => new CustomEventRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeName = a.EventTypeName,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCity = b.CityName
            });



            IEnumerable<CustomEventRequest> restwo = list.Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                RepName = b.FullName,
                ContactId = a.ContactId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = b.CityId
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = b.Gender,
                Hsan = b.Hsan,
                RepName = a.RepName,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeId = b.EventTypeId,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeName = b.TypeName,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.city, a => a.RepCityId, b => b.Id, (a, b) => new CustomEventRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeName = a.EventTypeName,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCity = b.CityName
            });


            List<CustomEventRequest> ress = new List<CustomEventRequest>();

            foreach (var item in res)
            {
                CustomEventRequest x = new CustomEventRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.WayInDeparture = item.WayInDeparture;
                x.WayOutDeparture = item.WayOutDeparture;
                x.WayInArrival = item.WayInArrival;
                x.WayOutArrival = item.WayOutArrival;
                x.WayInCityName = item.WayInCityName;
                x.WayOutCityName = item.WayOutCityName;
                x.WayInFlightNumber = item.WayInFlightNumber;
                x.WayOutFlightNumber = item.WayOutFlightNumber;
                x.WayInDestinationName = item.WayInDestinationName;
                x.WayOutDestinationName = item.WayOutDestinationName;
                x.HotelName = item.HotelName;
                x.RoomType = item.RoomType;
                x.Accumpained = item.Accumpained;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.PassportNumber = item.PassportNumber;
                x.PassportExpiryDate = item.PassportExpiryDate;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.IsPassport = item.IsPassport;
                x.Hsan = item.Hsan;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                x.RepCity = item.RepCity;
                ress.Add(x);
            }

            foreach (var item in restwo)
            {
                CustomEventRequest x = new CustomEventRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.WayInDeparture = item.WayInDeparture;
                x.WayOutDeparture = item.WayOutDeparture;
                x.WayInArrival = item.WayInArrival;
                x.WayOutArrival = item.WayOutArrival;
                x.WayInFlightNumber = item.WayInFlightNumber;
                x.WayOutFlightNumber = item.WayOutFlightNumber;
                x.HotelName = item.HotelName;
                x.RoomType = item.RoomType;
                x.Accumpained = item.Accumpained;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.PassportNumber = item.PassportNumber;
                x.PassportExpiryDate = item.PassportExpiryDate;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.IsPassport = item.IsPassport;
                x.Hsan = item.Hsan;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                x.RepCity = item.RepCity;
                ress.Add(x);
            }

            return ress.DistinctBy(a=>a.Id).OrderBy(a => a.Id);
        }

        public IEnumerable<TravelCities> GetCitiesByCountryName(string CountryName)
        {
            IEnumerable<TravelCities> res = db.travelCities.Where(a => a.Country == CountryName).OrderBy(a => a.City);
            return res;
        }

        public IEnumerable<TravelCountriesModel> GetCountries()
        {
            IEnumerable<string> list = db.travelCities.Select(a => a.Country).Distinct();
            List<TravelCountriesModel> res = new List<TravelCountriesModel>();
            foreach (var item in list)
            {
                TravelCountriesModel x = new TravelCountriesModel();
                x.CountryName = item;
                res.Add(x);
            }
            return res.OrderBy(a => a.CountryName);
        }

        public Event GetEventTravellingById(int id)
        {
            Event res = db.Event.Find(id);
            res.From = res.From.AddDays(-1);
            res.To = res.To.AddDays(1);
            return res;
        }

        public EventTravelRequest GetEventTravelRequest(int id)
        {
            EventTravelRequest res = db.EventTravelRequest.Find(id);
            return res;
        }

        public IEnumerable<EventType> GetEventTypes()
        {
            IEnumerable<EventType> res = db.EventType.Select(a => a);
            return res.OrderBy(a => a.TypeName);
        }

        public IEnumerable<CustomEventRequest> GetMyRequests(string Id)
        {
            DateTime now = ti.GetCurrentTime();

            string myname = db.Users.Find(Id).FullName;

            List<EventTravelRequest> list = db.Event.Where(a => a.From.Date >= now.Date).Join(db.EventTravelRequest.Where(a => a.ExtendIdentityUserId == Id), a => a.Id, b => b.EventId, (a, b) => new EventTravelRequest
            {
                Id = b.Id,
                EventId = b.EventId,
                ExtendIdentityUserId = b.ExtendIdentityUserId,
                ContactId = b.ContactId,
                WayInDeparture = b.WayInDeparture,
                WayOutDeparture = b.WayOutDeparture,
                WayInArrival = b.WayInArrival,
                WayOutArrival = b.WayOutArrival,
                WayInFlightNumber = b.WayInFlightNumber,
                WayOutFlightNumber = b.WayOutFlightNumber,
                HotelName = b.HotelName,
                RoomType = b.RoomType,
                Accumpained = b.Accumpained,
                Confirmed = b.Confirmed,
                Rejected = b.Rejected,
                TopConfirmed = b.TopConfirmed,
                TopRejected = b.TopRejected,
                TopAction = b.TopAction,
                TopActionUserId = b.TopActionUserId,
                HotelEdited = b.HotelEdited,
                PassportNumber = b.PassportNumber,
                PassportExpiryDate = b.PassportExpiryDate,
                IsPassport = b.IsPassport,
                IsTicket = b.IsTicket,
                PassportFileName = b.PassportFileName,
                PassportFileContentType = b.PassportFileContentType,
                WayInCityId = b.WayInCityId,
                WayOutCityId = b.WayOutCityId,
                WayInDestinationId = b.WayInDestinationId,
                WayOutDestinationId = b.WayOutDestinationId
            }).ToList();


            IEnumerable<CustomEventRequest> sres = list.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Gender = b.Gender,
                Hsan = b.Hsan,
                EventId = a.EventId,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.travelCities, a => a.WayInCityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = b.City,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.travelCities, a => a.WayOutCityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = b.City,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.travelCities, a => a.WayInDestinationId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = b.City,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.travelCities, a => a.WayOutDestinationId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = b.City,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeId = b.EventTypeId,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new CustomEventRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeName = b.TypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                RepName = myname,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            });

            IEnumerable<CustomEventRequest> fres = list.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Gender = b.Gender,
                Hsan = b.Hsan,
                EventId = a.EventId,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeId = b.EventTypeId,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new CustomEventRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeName = b.TypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                RepName = myname,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            });



         

            List<CustomEventRequest> ress = new List<CustomEventRequest>();


            foreach (var item in sres)
            {
                CustomEventRequest x = new CustomEventRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.WayInDeparture = item.WayInDeparture;
                x.WayOutDeparture = item.WayOutDeparture;
                x.WayInArrival = item.WayInArrival;
                x.WayOutArrival = item.WayOutArrival;
                x.WayInCityName = item.WayInCityName;
                x.WayOutCityName = item.WayOutCityName;
                x.WayInFlightNumber = item.WayInFlightNumber;
                x.WayOutFlightNumber = item.WayOutFlightNumber;
                x.WayInDestinationName = item.WayInDestinationName;
                x.WayOutDestinationName = item.WayOutDestinationName;
                x.HotelName = item.HotelName;
                x.RoomType = item.RoomType;
                x.Accumpained = item.Accumpained;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.PassportNumber = item.PassportNumber;
                x.PassportExpiryDate = item.PassportExpiryDate;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.IsPassport = item.IsPassport;
                x.IsTicket = item.IsTicket;
                x.Hsan = item.Hsan;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                ress.Add(x);
            }


            foreach (var item in fres)
            {
                CustomEventRequest x = new CustomEventRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.WayInDeparture = item.WayInDeparture;
                x.WayOutDeparture = item.WayOutDeparture;
                x.WayInArrival = item.WayInArrival;
                x.WayOutArrival = item.WayOutArrival;
                x.WayInFlightNumber = item.WayInFlightNumber;
                x.WayOutFlightNumber = item.WayOutFlightNumber;
                x.HotelName = item.HotelName;
                x.RoomType = item.RoomType;
                x.Accumpained = item.Accumpained;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.PassportNumber = item.PassportNumber;
                x.PassportExpiryDate = item.PassportExpiryDate;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.IsPassport = item.IsPassport;
                x.IsTicket = item.IsTicket;
                x.Hsan = item.Hsan;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                ress.Add(x);
            }

           
            List<CustomEventRequest> r = ress.DistinctBy(a => a.Id).OrderBy(a => a.WayInDeparture).ToList();

            return r;
        }

        public IEnumerable<CustomEventRequest> GetMyTeamRequests(string ManagerId)
        {
            DateTime now = ti.GetCurrentTime();
            List<string> MyTeamIds = db.Users.Where(a => a.extendidentityuserid == ManagerId).Select(a => a.Id).ToList();

            List<EventTravelRequest> list = new List<EventTravelRequest>();

            foreach (var item in MyTeamIds)
            {
                
                List<EventTravelRequest> l = db.Event.Where(a => a.From.Date >= now.Date).Join(db.EventTravelRequest, a => a.Id, b => b.EventId, (a, b) => new EventTravelRequest
                {
                    Id = b.Id,
                    EventId = b.EventId,
                    ExtendIdentityUserId = b.ExtendIdentityUserId,
                    ContactId = b.ContactId,
                    WayInDeparture = b.WayInDeparture,
                    WayOutDeparture = b.WayOutDeparture,
                    WayInArrival = b.WayInArrival,
                    WayOutArrival = b.WayOutArrival,
                    WayInCityId = b.WayInCityId,
                    WayOutCityId = b.WayOutCityId,
                    WayInFlightNumber = b.WayInFlightNumber,
                    WayOutFlightNumber = b.WayOutFlightNumber,
                    WayInDestinationId = b.WayInDestinationId,
                    WayOutDestinationId = b.WayOutDestinationId,
                    HotelName = b.HotelName,
                    RoomType = b.RoomType,
                    Accumpained = b.Accumpained,
                    Confirmed = b.Confirmed,
                    Rejected = b.Rejected,
                    TopConfirmed = b.TopConfirmed,
                    TopRejected = b.TopRejected,
                    TopAction = b.TopAction,
                    TopActionUserId = b.TopActionUserId,
                    HotelEdited = b.HotelEdited,
                    PassportNumber = b.PassportNumber,
                    PassportExpiryDate = b.PassportExpiryDate,
                    IsPassport = b.IsPassport,
                    IsTicket = b.IsTicket,
                    PassportFileName = b.PassportFileName,
                    PassportFileContentType = b.PassportFileContentType
                }).Where(a => a.ExtendIdentityUserId == item).ToList();
                foreach (var i in l)
                {
                    list.Add(i);
                }

            }

            List<EventTravelRequest> ll =  db.Event.Where(a => a.From.Date >= now.Date).Join(db.EventTravelRequest, a => a.Id, b => b.EventId, (a, b) => new EventTravelRequest
            {
                Id = b.Id,
                EventId = b.EventId,
                ExtendIdentityUserId = b.ExtendIdentityUserId,
                ContactId = b.ContactId,
                WayInDeparture = b.WayInDeparture,
                WayOutDeparture = b.WayOutDeparture,
                WayInArrival = b.WayInArrival,
                WayOutArrival = b.WayOutArrival,
                WayInCityId = b.WayInCityId,
                WayOutCityId = b.WayOutCityId,
                WayInFlightNumber = b.WayInFlightNumber,
                WayOutFlightNumber = b.WayOutFlightNumber,
                WayInDestinationId = b.WayInDestinationId,
                WayOutDestinationId = b.WayOutDestinationId,
                HotelName = b.HotelName,
                RoomType = b.RoomType,
                Accumpained = b.Accumpained,
                Confirmed = b.Confirmed,
                Rejected = b.Rejected,
                TopConfirmed = b.TopConfirmed,
                TopRejected = b.TopRejected,
                TopAction = b.TopAction,
                TopActionUserId = b.TopActionUserId,
                HotelEdited = b.HotelEdited,
                PassportNumber = b.PassportNumber,
                PassportExpiryDate = b.PassportExpiryDate,
                IsPassport = b.IsPassport,
                IsTicket = b.IsTicket,
                PassportFileName = b.PassportFileName,
                PassportFileContentType = b.PassportFileContentType
            }).Where(a => a.ExtendIdentityUserId == ManagerId).ToList();




            foreach (var i in ll)
            {
                list.Add(i);
            }

            List<CustomEventRequest> fres = list.Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                RepName = b.FullName,
                ContactId = a.ContactId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = b.Gender,
                Hsan = b.Hsan,
                RepName = a.RepName,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeId = b.EventTypeId,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new CustomEventRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeName = b.TypeName,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).ToList();

            List<CustomEventRequest> sres = list.Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                RepName = b.FullName,
                ContactId = a.ContactId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = b.Gender,
                Hsan = b.Hsan,
                RepName = a.RepName,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.travelCities, a => a.WayInCityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = b.City,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.travelCities, a => a.WayOutCityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = b.City,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.travelCities, a => a.WayInDestinationId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = b.City,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.travelCities, a => a.WayOutDestinationId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = b.City,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeId = b.EventTypeId,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new CustomEventRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeName = b.TypeName,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket
            }).ToList();



            List<CustomEventRequest> ress = new List<CustomEventRequest>();

            

            foreach (var item in sres)
            {
                CustomEventRequest x = new CustomEventRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.WayInDeparture = item.WayInDeparture;
                x.WayOutDeparture = item.WayOutDeparture;
                x.WayInArrival = item.WayInArrival;
                x.WayOutArrival = item.WayOutArrival;
                x.WayInCityName = item.WayInCityName;
                x.WayOutCityName = item.WayOutCityName;
                x.WayInFlightNumber = item.WayInFlightNumber;
                x.WayOutFlightNumber = item.WayOutFlightNumber;
                x.WayInDestinationName = item.WayInDestinationName;
                x.WayOutDestinationName = item.WayOutDestinationName;
                x.HotelName = item.HotelName;
                x.RoomType = item.RoomType;
                x.Accumpained = item.Accumpained;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.PassportNumber = item.PassportNumber;
                x.PassportExpiryDate = item.PassportExpiryDate;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.IsPassport = item.IsPassport;
                x.IsTicket = item.IsTicket;
                x.Hsan = item.Hsan;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                ress.Add(x);
            }


            foreach (var item in fres)
            {
                CustomEventRequest x = new CustomEventRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.WayInDeparture = item.WayInDeparture;
                x.WayOutDeparture = item.WayOutDeparture;
                x.WayInArrival = item.WayInArrival;
                x.WayOutArrival = item.WayOutArrival;
                x.WayInFlightNumber = item.WayInFlightNumber;
                x.WayOutFlightNumber = item.WayOutFlightNumber;
                x.HotelName = item.HotelName;
                x.RoomType = item.RoomType;
                x.Accumpained = item.Accumpained;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.PassportNumber = item.PassportNumber;
                x.PassportExpiryDate = item.PassportExpiryDate;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.IsPassport = item.IsPassport;
                x.IsTicket = item.IsTicket;
                x.Hsan = item.Hsan;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                ress.Add(x);
            }



            return ress.DistinctBy(a=>a.Id).OrderBy(a => a.WayInDeparture);
        }

        public IEnumerable<CustomEventRequest> GetAllRequests()
        {
            DateTime now = ti.GetCurrentTime();

            List<CustomEventRequest> fres = db.EventTravelRequest.Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                RepName = b.FullName,
                ContactId = a.ContactId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = b.Gender,
                Hsan = b.Hsan,
                RepName = a.RepName,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                From = b.From,
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeId = b.EventTypeId,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Where(a=>a.From.Date >= now.Date).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new CustomEventRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeName = b.TypeName,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).ToList();




            List<CustomEventRequest> sres = db.EventTravelRequest.Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                RepName = b.FullName,
                ContactId = a.ContactId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = b.Gender,
                Hsan = b.Hsan,
                RepName = a.RepName,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.travelCities, a => a.WayInCityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = b.City,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.travelCities, a => a.WayOutCityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = b.City,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.travelCities, a => a.WayInDestinationId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = b.City,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.travelCities, a => a.WayOutDestinationId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = b.City,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                From = b.From,
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeId = b.EventTypeId,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Where(a => a.From.Date >= now.Date).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new CustomEventRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeName = b.TypeName,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                IsTicket = a.IsTicket,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).ToList();

            List<CustomEventRequest> ress = new List<CustomEventRequest>();


            foreach (var item in sres)
            {
                CustomEventRequest x = new CustomEventRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.WayInDeparture = item.WayInDeparture;
                x.WayOutDeparture = item.WayOutDeparture;
                x.WayInArrival = item.WayInArrival;
                x.WayOutArrival = item.WayOutArrival;
                x.WayInCityName = item.WayInCityName;
                x.WayOutCityName = item.WayOutCityName;
                x.WayInFlightNumber = item.WayInFlightNumber;
                x.WayOutFlightNumber = item.WayOutFlightNumber;
                x.WayInDestinationName = item.WayInDestinationName;
                x.WayOutDestinationName = item.WayOutDestinationName;
                x.HotelName = item.HotelName;
                x.RoomType = item.RoomType;
                x.Accumpained = item.Accumpained;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.PassportNumber = item.PassportNumber;
                x.PassportExpiryDate = item.PassportExpiryDate;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.IsPassport = item.IsPassport;
                x.IsTicket = item.IsTicket;
                x.Hsan = item.Hsan;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }

                if (item.FirstActionDateTime != null)
                {
                    x.FirstActionDateTimestr = ((DateTime)item.FirstActionDateTime).ToString("dd/MM/yyyy - hh:mm tt");
                }
                if (item.TopActionDateTime != null)
                {
                    x.TopActionDateTimestr = ((DateTime)item.TopActionDateTime).ToString("dd/MM/yyyy - hh:mm tt");
                }


                ress.Add(x);
            }

            foreach (var item in fres)
            {
                CustomEventRequest x = new CustomEventRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.WayInDeparture = item.WayInDeparture;
                x.WayOutDeparture = item.WayOutDeparture;
                x.WayInArrival = item.WayInArrival;
                x.WayOutArrival = item.WayOutArrival;
                x.WayInFlightNumber = item.WayInFlightNumber;
                x.WayOutFlightNumber = item.WayOutFlightNumber;
                x.HotelName = item.HotelName;
                x.RoomType = item.RoomType;
                x.Accumpained = item.Accumpained;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.PassportNumber = item.PassportNumber;
                x.PassportExpiryDate = item.PassportExpiryDate;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.IsPassport = item.IsPassport;
                x.IsTicket = item.IsTicket;
                x.Hsan = item.Hsan;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                
                if (item.FirstActionDateTime != null)
                {
                    x.FirstActionDateTimestr = ((DateTime)item.FirstActionDateTime).ToString("dd/MM/yyyy - hh:mm tt");
                }
                if (item.TopActionDateTime != null)
                {
                    x.TopActionDateTimestr = ((DateTime)item.TopActionDateTime).ToString("dd/MM/yyyy - hh:mm tt");
                }

                ress.Add(x);
            }






            return ress.DistinctBy(a=>a.Id);
        }

        public IEnumerable<CustomEventRequest> GetMyTeamRequestsByEventId(int Id, string ManagerId)
        {
            DateTime now = ti.GetCurrentTime();
            IEnumerable<string> MyTeamIds = db.Users.Where(a => a.extendidentityuserid == ManagerId).Select(a => a.Id);

            List<EventTravelRequest> list = new List<EventTravelRequest>();

            foreach (var item in MyTeamIds)
            {
                List<EventTravelRequest> l = db.EventTravelRequest.Where(a => a.ExtendIdentityUserId == item && a.EventId == Id).ToList();

                foreach (var i in l)
                {
                    list.Add(i);
                }
            }

            List<EventTravelRequest> ll = db.EventTravelRequest.Where(a => a.ExtendIdentityUserId == ManagerId && a.EventId == Id).ToList();

            foreach (var i in ll)
            {
                list.Add(i);
            }

            IEnumerable<CustomEventRequest> res = list.Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                RepName = b.FullName,
                ContactId = a.ContactId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = b.Gender,
                Hsan = b.Hsan,
                RepName = a.RepName,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport
            }).Join(db.travelCities, a => a.WayInCityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = b.City,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport
            }).Join(db.travelCities, a => a.WayOutCityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = b.City,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport
            }).Join(db.travelCities, a => a.WayInDestinationId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = b.City,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport
            }).Join(db.travelCities, a => a.WayOutDestinationId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                Hsan = a.Hsan,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = b.City,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeId = b.EventTypeId,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new CustomEventRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                Hsan = a.Hsan,
                EventTypeName = b.TypeName,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport
            });



            List<CustomEventRequest> ress = new List<CustomEventRequest>();

            foreach (var item in res)
            {
                CustomEventRequest x = new CustomEventRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.WayInDeparture = item.WayInDeparture;
                x.WayOutDeparture = item.WayOutDeparture;
                x.WayInArrival = item.WayInArrival;
                x.WayOutArrival = item.WayOutArrival;
                x.WayInCityName = item.WayInCityName;
                x.WayOutCityName = item.WayOutCityName;
                x.WayInFlightNumber = item.WayInFlightNumber;
                x.WayOutFlightNumber = item.WayOutFlightNumber;
                x.WayInDestinationName = item.WayInDestinationName;
                x.WayOutDestinationName = item.WayOutDestinationName;
                x.HotelName = item.HotelName;
                x.RoomType = item.RoomType;
                x.Accumpained = item.Accumpained;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.PassportNumber = item.PassportNumber;
                x.PassportExpiryDate = item.PassportExpiryDate;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.IsPassport = item.IsPassport;
                x.Hsan = item.Hsan;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                ress.Add(x);
            }


            return ress.OrderBy(a => a.WayInDeparture);
        }

        public IEnumerable<Event> GetUpComingEvents()
        {
            DateTime now = ti.GetCurrentTime();
            IEnumerable<Event> res = db.Event.Where(a => a.From >= now);
            return res.OrderByDescending(a => a.From);
        }

        public IEnumerable<Event> GetSixMonthsBackEvents()
        {
            DateTime now = ti.GetCurrentTime();
            IEnumerable<Event> res = db.Event.Where(a => a.From >= now.AddMonths(-6));
            return res.OrderByDescending(a => a.From);
        }

        public bool HoldRequest(int id)
        {
            DateTime now = ti.GetCurrentTime();

            EventTravelRequest res = db.EventTravelRequest.Find(id);
            res.Confirmed = false;
            res.Rejected = false;
            EventProposalRequest p = db.EventProposalRequest.Where(a => a.ExtendIdentityUserId == res.ExtendIdentityUserId && a.ContactId == res.ContactId && res.EventId == res.EventId).FirstOrDefault();
            if (p != null)
            {
                p.Rejected = false;
                p.Confirmed = false;
            }
            string contactname = db.contact.Find(res.ContactId).ContactName;

            Notifications n = new Notifications();
            n.NitificationDateTime = now;
            n.ExtendIdetityUserId = res.ExtendIdentityUserId;
            n.NotificationDetails = n.NotificationDetails = "Your Event Travel Request for " + contactname + " has been Holded";
            n.Url = "MyEventRequests.html";
            db.notifications.Add(n);

            db.SaveChanges();
            return true;
        }

        public async Task<bool> MakeEventRequestAsync(AddEventTravelRequestModel ev)
        {
            DateTime now = ti.GetCurrentTime();
            DateTime eventstart = db.Event.Find(ev.EventId).From.Date;
            DateTime eventend = db.Event.Find(ev.EventId).To.Date;
            ExtendIdentityUser user = userManager.FindByIdAsync(ev.ExtendIdentityUserId).Result;

            string repname = user.FullName;
            string managerid = user.extendidentityuserid;
            string role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
            string contactname = db.contact.Find(ev.ContactId).ContactName;

            EventTravelRequest r = new EventTravelRequest();
            r.Accumpained = ev.Accumpained;
            r.Confirmed = ev.Confirmed;
            r.ContactId = ev.ContactId;
            r.EventId = ev.EventId;
            r.ExtendIdentityUserId = ev.ExtendIdentityUserId;
            r.HotelName = ev.HotelName;
            r.PassportExpiryDate = ev.PassportExpiryDate;
            r.PassportNumber = ev.PassportNumber;
            r.Rejected = ev.Rejected;
            r.RoomType = ev.RoomType;
            r.TopAction = ev.TopAction;
            r.TopActionUserId = ev.TopActionUserId;
            r.TopConfirmed = ev.TopConfirmed;
            r.TopRejected = ev.TopRejected;
            r.WayInArrival = ev.WayInArrival;
            
            r.WayInDeparture = ev.WayInDeparture;
            
            r.WayInFlightNumber = ev.WayInFlightNumber;
            r.WayOutArrival = ev.WayOutArrival;
           
            r.WayOutDeparture = ev.WayOutDeparture;
            
            r.WayOutFlightNumber = ev.WayOutFlightNumber;
            if (ev.WayInDestinationId != 0)
            {
                r.WayInDestinationId = ev.WayInDestinationId;
            }
            if (ev.WayOutDestinationId != 0)
            {
                r.WayOutDestinationId = ev.WayOutDestinationId;
            }
            if (ev.WayInCityId != 0)
            {
                r.WayInCityId = ev.WayInCityId;
            }
            if (ev.WayOutCityId != 0)
            {
                r.WayOutCityId = ev.WayOutCityId;
            }


            if (r.WayInDeparture == null || r.WayOutDeparture == null)
            {

            }

           

            ////Temp Stop
            //else
            //{
            //    DateTime indep = (DateTime)r.WayInDeparture;
            //    DateTime outdep = (DateTime)r.WayOutDeparture;
            //    if (indep.Date == eventstart || indep.Date == eventstart.AddDays(-1).Date)
            //    {
            //        if (outdep.Date == eventend || outdep.Date == eventend.AddDays(1).Date)
            //        {
            //            r.TopConfirmed = true;
            //            r.TopAction = true;
            //        }
            //    }
            //}

            db.EventTravelRequest.Add(r);
            db.SaveChanges();

            if (ev.file == null || ev.file.Length == 0)
            {

            }
            else
            {
                int indx = ev.file.FileName.Split('.').Length - 1;
                string ext = ev.file.FileName.Split('.')[indx];
                string filename = r.Id + "p." + ext;
                string contenttype = ev.file.ContentType;
                var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "Passport",
                filename);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ev.file.CopyToAsync(stream);
                }

                r.IsPassport = true;
                r.PassportFileName = filename;
                r.PassportFileContentType = contenttype;
                db.SaveChanges();
            }
         

            //if (role == "Medical Representative")
            //{
            //    Notifications n = new Notifications();
            //    n.NitificationDateTime = now;
            //    n.ExtendIdetityUserId = managerid;
            //    n.NotificationDetails = repname + " has make a travel request for contact: " + contactname;
            //    n.Url = "MyTeamEventRequest.html";
            //    db.notifications.Add(n);
            //}

            //else
            //{
            //    if (role == "First Line Manager")
            //    {
            //        List<string> TopIds = userManager.GetUsersInRoleAsync("Top Line Manager").Result.Select(a => a.Id).ToList();
            //        foreach (var item in TopIds)
            //        {
            //            Notifications n = new Notifications();
            //            n.NitificationDateTime = now;
            //            n.ExtendIdetityUserId = item;
            //            n.NotificationDetails = repname + " has make a travel request for contact: " + contactname;
            //            n.Url = "TravelRequests.html";
            //            db.notifications.Add(n);
            //        }

            //    }
            //    else
            //    {
                 
            //            if (role == "System Admin")
            //            {
            //                List<string> TopIds = userManager.GetUsersInRoleAsync("Top Line Manager").Result.Select(a => a.Id).ToList();
            //                foreach (var item in TopIds)
            //                {
            //                    Notifications n = new Notifications();
            //                    n.NitificationDateTime = now;
            //                    n.ExtendIdetityUserId = item;
            //                    n.NotificationDetails = repname + " has make a travel request for contact: " + contactname;
            //                    n.Url = "TravelRequests.html";
            //                    db.notifications.Add(n);
            //                }
                       
            //        }
            //    }
            //}
            //db.SaveChanges();

            return true;
        }

        public bool RejectRequest(int id)
        {
            DateTime now = ti.GetCurrentTime();

            EventTravelRequest res = db.EventTravelRequest.Find(id);
            res.Confirmed = false;
            res.Rejected = true;
            res.FirstActionDateTime = now;

            EventProposalRequest p = db.EventProposalRequest.Where(a => a.ExtendIdentityUserId == res.ExtendIdentityUserId && a.ContactId == res.ContactId && res.EventId == res.EventId).FirstOrDefault();
            if (p != null)
            {
                p.Rejected = true;
                p.Confirmed = false;
            }

            string contactname = db.contact.Find(res.ContactId).ContactName;

            Notifications n = new Notifications();
            n.NitificationDateTime = now;
            n.ExtendIdetityUserId = res.ExtendIdentityUserId;
            n.NotificationDetails = n.NotificationDetails = "Your Event Travel Request for " + contactname + " has been Rejected";
            n.Url = "MyEventRequests.html";
            db.notifications.Add(n);

            db.SaveChanges();
            return true;
        }

        public bool TopConfirmRequest(int id, string TopId)
        {
            DateTime now = ti.GetCurrentTime();
            
            EventTravelRequest res = db.EventTravelRequest.Find(id);
            res.TopConfirmed = true;
            res.TopRejected = false;
            res.TopAction = true;
            res.TopActionUserId = TopId;
            res.TopActionDateTime = now;

            EventProposalRequest p = db.EventProposalRequest.Where(a => a.ExtendIdentityUserId == res.ExtendIdentityUserId && a.EventId == res.EventId && a.ContactId == res.ContactId).FirstOrDefault();

            if (p != null)
            {
                p.TopActionUserId = TopId;
                p.TopRejected = false;
                p.TopConfirmed = true;
                p.TopAction = true;
            }

            ExtendIdentityUser user = userManager.FindByIdAsync(res.ExtendIdentityUserId).Result;
            string contactname = db.contact.Find(res.ContactId).ContactName;
            Notifications n = new Notifications();
            n.ExtendIdetityUserId = res.ExtendIdentityUserId;
            n.NitificationDateTime = now;
            n.NotificationDetails = "Your Event Travel Request for " + contactname + " has been Accepted by top manager utterly";

            string role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if(role == "System Admin")
            {
                n.Url = "TravelRequests.html";
            }
            else
            {
                if (role == "System Adminn")
                {
                    n.Url = "TravelRequests.html";
                }
                else
                {
                    if (role == "First Line Manager")
                    {
                        n.Url = "MyTeamEventRequest.html";
                    }

                    else
                    {
                        if (role == "Medical Representative")
                        {
                            n.Url = "TravelRequests.html";
                        }
                    }
                }
               
            }
            

            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }

        public bool TopHoldRequest(int id)
        {
            DateTime now = ti.GetCurrentTime();
            EventTravelRequest res = db.EventTravelRequest.Find(id);
            res.TopConfirmed = false;
            res.TopRejected = false;
            res.TopAction = false;
            res.Confirmed = false;
            res.Rejected = false;
            res.TopActionUserId = null;
            res.TopActionDateTime = now;

            EventProposalRequest p = db.EventProposalRequest.Where(a => a.ExtendIdentityUserId == res.ExtendIdentityUserId && a.EventId == res.EventId && a.ContactId == res.ContactId).FirstOrDefault();

            if (p != null)
            {
                
                p.TopRejected = false;
                p.TopConfirmed = false;
                p.TopAction = false;
            }

            string contactname = db.contact.Find(res.ContactId).ContactName;
            ExtendIdentityUser user = userManager.FindByIdAsync(res.ExtendIdentityUserId).Result;
            Notifications n = new Notifications();
            n.ExtendIdetityUserId = res.ExtendIdentityUserId;
            n.NitificationDateTime = now;
            n.NotificationDetails = "Your Event Travel Request for " + contactname + " has been Holded by top manager utterly";
            string role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if (role == "System Admin")
            {
                n.Url = "TravelRequests.html";
            }
            else
            {
                if (role == "System Adminn")
                {
                    n.Url = "TravelRequests.html";
                }
                else
                {
                    if (role == "First Line Manager")
                    {
                        n.Url = "MyTeamEventRequest.html";
                    }

                    else
                    {
                        if (role == "Medical Representative")
                        {
                            n.Url = "TravelRequests.html";
                        }
                    }
                }
                
            }
            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }

        public bool TopRejectRequest(int id, string TopId)
        {
            DateTime now = ti.GetCurrentTime();
            EventTravelRequest res = db.EventTravelRequest.Find(id);
            res.TopConfirmed = false;
            res.TopRejected = true;
            res.TopAction = true;
            res.TopActionUserId = TopId;
            res.TopActionDateTime = now;

            EventProposalRequest p = db.EventProposalRequest.Where(a => a.ExtendIdentityUserId == res.ExtendIdentityUserId && a.EventId == res.EventId && a.ContactId == res.ContactId).FirstOrDefault();

            if (p != null)
            {
                p.TopActionUserId = TopId;
                p.TopRejected = true;
                p.TopConfirmed = false;
                p.TopAction = true;
            }

            string contactname = db.contact.Find(res.ContactId).ContactName;
            ExtendIdentityUser user = userManager.FindByIdAsync(res.ExtendIdentityUserId).Result;
            Notifications n = new Notifications();
            n.ExtendIdetityUserId = res.ExtendIdentityUserId;
            n.NitificationDateTime = now;
            n.NotificationDetails = "Your Event Travel Request for " + contactname + " has been Rejected by top manager utterly";
            string role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if (role == "System Admin")
            {
                n.Url = "TravelRequests.html";
            }
            else
            {
                if (role == "System Adminn")
                {
                    n.Url = "TravelRequests.html";
                }
                else
                {
                    if (role == "First Line Manager")
                    {
                        n.Url = "MyTeamEventRequest.html";
                    }
                    else
                    {
                        if (role == "Medical Representative")
                        {
                            n.Url = "TravelRequests.html";
                        }
                    }
                }
               
            }

            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }

        public List<EventProposalCheckModel> MakeEventProposal(List<EventProposalRequest> evp)
        {
            DateTime now = ti.GetCurrentTime();
            DateTime EventDate = db.Event.Find(evp.FirstOrDefault().EventId).From;
            ExtendIdentityUser user = userManager.FindByIdAsync(evp.FirstOrDefault().ExtendIdentityUserId).Result;
            string repname = user.FullName;
            string managerid = user.extendidentityuserid;
            string role = userManager.GetRolesAsync(user).Result.FirstOrDefault();

            List<EventProposalCheckModel> check = new List<EventProposalCheckModel>();
           


            foreach (var item in evp)
            {
                EventProposalRequest chh = db.EventProposalRequest.Where(a => a.EventId == item.EventId && a.ContactId == item.ContactId).FirstOrDefault();
                if (chh == null)
                {
                    item.RequestDate = EventDate;
                    db.EventProposalRequest.Add(item);

                    if (role == "Medical Representative")
                    {
                        Notifications n = new Notifications();
                        n.NitificationDateTime = now;
                        n.ExtendIdetityUserId = managerid;
                        n.NotificationDetails = repname + " has make an event proposal for contact: " + db.contact.Find(item.ContactId).ContactName;
                        n.Url = "MyProposalRequests.html";
                        db.notifications.Add(n);
                    }

                    else
                    {
                        if (role == "First Line Manager")
                        {
                            EventTravelRequest ch = db.EventTravelRequest.Where(a => a.EventId == item.EventId && a.ContactId == item.ContactId).FirstOrDefault();

                            if (ch == null)
                            {
                                Event e = db.Event.Find(item.EventId);
                                EventTravelRequest et = new EventTravelRequest();
                                et.Confirmed = true;
                                et.Accumpained = 0;
                                et.ContactId = item.ContactId;
                                et.EventId = e.Id;
                                et.ExtendIdentityUserId = item.ExtendIdentityUserId;
                                et.WayInDestinationId = e.TravelCitiesId;
                                et.WayOutCityId = e.TravelCitiesId;
                                et.WayInDeparture = e.From.AddDays(-1);
                                et.WayInArrival = e.From.AddDays(-1);
                                et.WayOutDeparture = e.To.AddDays(1);
                                et.WayOutArrival = e.To.AddDays(1);
                                db.EventTravelRequest.Add(et);
                            }

                            List<string> TopIds = userManager.GetUsersInRoleAsync("Top Line Manager").Result.Select(a => a.Id).ToList();
                            foreach (var it in TopIds)
                            {
                                Notifications n = new Notifications();
                                n.NitificationDateTime = now;
                                n.ExtendIdetityUserId = it;
                                n.NotificationDetails = repname + " has make an event proposal for contact: " + db.contact.Find(item.ContactId).ContactName;
                                n.Url = "ProposalRequests.html";
                                db.notifications.Add(n);
                            }

                        }

                        else
                        {
                            if (role == "Top Line Manager")
                            {
                                EventTravelRequest ch = db.EventTravelRequest.Where(a => a.EventId == item.EventId && a.ContactId == item.ContactId).FirstOrDefault();

                                if (ch == null)
                                {
                                    Event e = db.Event.Find(item.EventId);
                                    EventTravelRequest et = new EventTravelRequest();
                                    et.TopConfirmed = true;
                                    et.TopAction = true;
                                    et.Accumpained = 0;
                                    et.ContactId = item.ContactId;
                                    et.EventId = e.Id;
                                    et.ExtendIdentityUserId = item.ExtendIdentityUserId;
                                    et.WayInDestinationId = e.TravelCitiesId;
                                    et.WayOutCityId = e.TravelCitiesId;
                                    et.WayInDeparture = e.From.AddDays(-1);
                                    et.WayInArrival = e.From.AddDays(-1);
                                    et.WayOutDeparture = e.To.AddDays(1);
                                    et.WayOutArrival = e.To.AddDays(1);
                                    db.EventTravelRequest.Add(et);
                                }
                            }
                            else { 
                            if (role == "System Admin")
                            {
                                EventTravelRequest ch = db.EventTravelRequest.Where(a => a.EventId == item.EventId && a.ContactId == item.ContactId).FirstOrDefault();

                                if (ch == null)
                                {
                                    Event e = db.Event.Find(item.EventId);
                                    EventTravelRequest et = new EventTravelRequest();
                                    et.TopConfirmed = true;
                                    et.TopAction = true;
                                    et.Accumpained = 0;
                                    et.ContactId = item.ContactId;
                                    et.EventId = e.Id;
                                    et.ExtendIdentityUserId = item.ExtendIdentityUserId;
                                    et.WayInDestinationId = e.TravelCitiesId;
                                    et.WayOutCityId = e.TravelCitiesId;
                                    et.WayInDeparture = e.From.AddDays(-1);
                                    et.WayInArrival = e.From.AddDays(-1);
                                    et.WayOutDeparture = e.To.AddDays(1);
                                    et.WayOutArrival = e.To.AddDays(1);
                                    db.EventTravelRequest.Add(et);
                                }
                            }
                        }
                        }

                    }
                }

                else
                {

                    
                 
                        EventProposalCheckModel ec = new EventProposalCheckModel();
                        ec.Contact = db.contact.Find(chh.ContactId).ContactName;
                        ec.Rep = db.Users.Find(chh.ExtendIdentityUserId).FullName;
                        check.Add(ec);
                    

              
                }
             


              

            }

            db.SaveChanges();
            return check;
        }

        public IEnumerable<CustomProposalRequest> GetMyProposalRequests(string Id)
        {
            DateTime now = ti.GetCurrentTime();

            string myname = db.Users.Find(Id).FullName;

            List<EventProposalRequest> list = db.EventProposalRequest.Where(a => a.ExtendIdentityUserId == Id && a.RequestDate.Date >= now.Date).ToList();

            IEnumerable<CustomProposalRequest> res = list.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Gender = b.Gender,
                EventId = a.EventId,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                EventTypeId = b.EventTypeId,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new CustomProposalRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = b.TypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                RepName = myname,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate
            });

            List<CustomProposalRequest> ress = new List<CustomProposalRequest>();

            foreach (var item in res)
            {
                CustomProposalRequest x = new CustomProposalRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.RequestDate = item.RequestDate;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                ress.Add(x);
            }


            return ress.OrderBy(a => a.RequestDate);
        }

        public IEnumerable<Contact> GetApprovedEventContacts(string UserId, int EventId)
        {
            List<EventProposalRequest> list = db.EventProposalRequest.Where(a => a.ExtendIdentityUserId == UserId && a.EventId == EventId && a.TopRejected == false).ToList();

            List<Contact> res = new List<Contact>();
            foreach (var item in list)
            {
                Contact obj = new Contact();
                if (item.TopAction == true)
                {
                    if (item.TopConfirmed == true)
                    {
                        obj = db.contact.Find(item.ContactId);
                        res.Add(obj);
                    }
                }
                else
                {
                    if (item.Confirmed == true)
                    {
                        obj = db.contact.Find(item.ContactId);
                        res.Add(obj);
                    }
                }
            }
            List<Contact> ress = new List<Contact>();
            foreach (var item in res)
            {
                EventTravelRequest check = db.EventTravelRequest.Where(a => a.ContactId == item.Id && a.EventId == EventId && a.TopRejected == false).FirstOrDefault();

                if(check == null)
                {
                    ress.Add(item);
                }
                else
                {
                    if(check.Rejected == false)
                    {
                       
                    }
                    else
                    {
                        ress.Add(item);
                    }
                }
            }

            return ress.OrderBy(a=>a.ContactName);
        }

        public IEnumerable<CustomProposalRequest> GetMyTeamProposalRequests(string ManagerId)
        {
            DateTime now = ti.GetCurrentTime();

            List<string> MyTeamIds = userManager.Users.Where(a => a.extendidentityuserid == ManagerId).Select(a=>a.Id).ToList();
            List<EventProposalRequest> list = new List<EventProposalRequest>();
            foreach (var item in MyTeamIds)
            {
                List<EventProposalRequest> l = db.EventProposalRequest.Where(a => a.ExtendIdentityUserId == item && a.RequestDate >= now).ToList();

                foreach (var it in l)
                {
                    list.Add(it);
                }
            }

            List<EventProposalRequest> ll = db.EventProposalRequest.Where(a => a.ExtendIdentityUserId == ManagerId && a.RequestDate >= now).ToList();

            foreach (var it in ll)
            {
                list.Add(it);
            }

            IEnumerable<CustomProposalRequest> res = list.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Gender = b.Gender,
                EventId = a.EventId,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.ExtendIdentityUserId
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.RepId
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                EventTypeId = b.EventTypeId,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.RepId
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = b.TypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.RepId
            }).Join(db.Users, a => a.RepId, b => b.Id, (a, b) => new CustomProposalRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = a.EventTypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepName = b.FullName
            });

            List<CustomProposalRequest> ress = new List<CustomProposalRequest>();

            foreach (var item in res)
            {
                CustomProposalRequest x = new CustomProposalRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.RequestDate = item.RequestDate;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                ress.Add(x);
            }
            return ress;
        }

        public bool ConfirmProposal(int id)
        {
            DateTime now = ti.GetCurrentTime();
            EventProposalRequest x = db.EventProposalRequest.Find(id);
            x.Confirmed = true;
            x.Rejected = false;


            EventTravelRequest ch = db.EventTravelRequest.Where(a => a.EventId == x.EventId && a.ContactId == x.ContactId).FirstOrDefault();

            if (ch == null)
            {
                Event e = db.Event.Find(x.EventId);
                EventTravelRequest et = new EventTravelRequest();
                et.Confirmed = true;
                et.FirstActionDateTime = now;
                et.Accumpained = 0;
                et.ContactId = x.ContactId;
                et.EventId = e.Id;
                et.ExtendIdentityUserId = x.ExtendIdentityUserId;
                et.WayInDestinationId = e.TravelCitiesId;
                et.WayOutCityId = e.TravelCitiesId;
                et.WayInDeparture = e.From.AddDays(-1);
                et.WayInArrival = e.From.AddDays(-1);
                et.WayOutDeparture = e.To.AddDays(1);
                et.WayOutArrival = e.To.AddDays(1);
                db.EventTravelRequest.Add(et);
            }

            
            

            string contactname = db.contact.Find(x.ContactId).ContactName;

            Notifications n = new Notifications();
            n.ExtendIdetityUserId = x.ExtendIdentityUserId;
            n.NitificationDateTime = now;
            n.NotificationDetails = "Your Event Travel Request for " + contactname + " has been Accepted by your first manager";
            n.Url = "MyProposalRequests.html";

            db.notifications.Add(n);

            db.SaveChanges();
            return true;
        }

        public bool RejectProposal(int id)
        {
            DateTime now = ti.GetCurrentTime();
            EventProposalRequest x = db.EventProposalRequest.Find(id);
            x.Confirmed = false;
            x.Rejected = true;

            string contactname = db.contact.Find(x.ContactId).ContactName;

            Notifications n = new Notifications();
            n.ExtendIdetityUserId = x.ExtendIdentityUserId;
            n.NitificationDateTime = now;
            n.NotificationDetails = "Your Event Travel Request for " + contactname + " has been Rejected by your first manager";
            n.Url = "MyProposalRequests.html";

            db.notifications.Add(n);

            db.SaveChanges();
            return true;
        }

        public bool HoldProposal(int id)
        {
            DateTime now = ti.GetCurrentTime();
            EventProposalRequest x = db.EventProposalRequest.Find(id);
            x.Confirmed = false;
            x.Rejected = false;

            string contactname = db.contact.Find(x.ContactId).ContactName;

            Notifications n = new Notifications();
            n.ExtendIdetityUserId = x.ExtendIdentityUserId;
            n.NitificationDateTime = now;
            n.NotificationDetails = "Your Event Travel Request for " + contactname + " has been Holded by your first manager";
            n.Url = "MyProposalRequests.html";

            db.notifications.Add(n);


            db.SaveChanges();
            return true;
        }

        public IEnumerable<CustomProposalRequest> GetAllProposalRequests()
        {
            DateTime now = ti.GetCurrentTime();


            List<EventProposalRequest> list = db.EventProposalRequest.Where(a => a.RequestDate >= now.AddMonths(-6)).ToList();
           

            IEnumerable<CustomProposalRequest> res = list.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Gender = b.Gender,
                EventId = a.EventId,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.ExtendIdentityUserId
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.RepId
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                EventTypeId = b.EventTypeId,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.RepId
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = b.TypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.RepId
            }).Join(db.Users, a => a.RepId, b => b.Id, (a, b) => new 
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = a.EventTypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepName = b.FullName,
                RepCityId = b.CityId
            }).Join(db.city,a=>a.RepCityId,b=>b.Id,(a,b)=>new CustomProposalRequest 
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = a.EventTypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepName = a.RepName,
                RepCity = b.CityName
            });

            List<CustomProposalRequest> ress = new List<CustomProposalRequest>();

            foreach (var item in res)
            {
                CustomProposalRequest x = new CustomProposalRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.RequestDate = item.RequestDate;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                x.RepCity = item.RepCity;
                ress.Add(x);
            }
            return ress;
        }


        public IEnumerable<CustomProposalRequest> GetAllPendingProposalRequests()
        {
            DateTime now = ti.GetCurrentTime();


            List<EventProposalRequest> list = db.EventProposalRequest.Where(a => a.RequestDate >= now.AddMonths(-6) && a.Confirmed == false && a.Rejected == false && a.TopConfirmed == false && a.TopRejected == false).ToList();


            IEnumerable<CustomProposalRequest> res = list.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Gender = b.Gender,
                EventId = a.EventId,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.ExtendIdentityUserId
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.RepId
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                EventTypeId = b.EventTypeId,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.RepId
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = b.TypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.RepId
            }).Join(db.Users, a => a.RepId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = a.EventTypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepName = b.FullName,
                RepCityId = b.CityId
            }).Join(db.city, a => a.RepCityId, b => b.Id, (a, b) => new CustomProposalRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = a.EventTypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepName = a.RepName,
                RepCity = b.CityName
            });

            List<CustomProposalRequest> ress = new List<CustomProposalRequest>();

            foreach (var item in res)
            {
                CustomProposalRequest x = new CustomProposalRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.RequestDate = item.RequestDate;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                x.RepCity = item.RepCity;
                ress.Add(x);
            }
            return ress;
        }

        public bool TopConfirmProposal(int id, string TopId)
        {
            DateTime now = ti.GetCurrentTime();
            EventProposalRequest res = db.EventProposalRequest.Find(id);
            res.TopConfirmed = true;
            res.TopRejected = false;
            res.TopAction = true;
            res.TopActionUserId = TopId;

            EventTravelRequest ch = db.EventTravelRequest.Where(a => a.EventId == res.EventId && a.ContactId == res.ContactId).FirstOrDefault();

            if (ch == null)
            {
                Event e = db.Event.Find(res.EventId);
                EventTravelRequest et = new EventTravelRequest();
                et.TopConfirmed = true;
                et.TopActionDateTime = now;
                et.TopAction = true;
                et.TopActionUserId = TopId;
                et.Accumpained = 0;
                et.ContactId = res.ContactId;
                et.EventId = e.Id;
                et.ExtendIdentityUserId = res.ExtendIdentityUserId;
                et.WayInDestinationId = e.TravelCitiesId;
                et.WayOutCityId = e.TravelCitiesId;
                et.WayInDeparture = e.From.AddDays(-1);
                et.WayInArrival = e.From.AddDays(-1);
                et.WayOutDeparture = e.To.AddDays(1);
                et.WayOutArrival = e.To.AddDays(1);
                db.EventTravelRequest.Add(et);
            }
            else
            {
                ch.TopRejected = false;
                ch.TopConfirmed = true;
                ch.TopActionUserId = TopId;
                ch.TopAction = true;
            }



            string contactname = db.contact.Find(res.ContactId).ContactName;
            ExtendIdentityUser user = userManager.FindByIdAsync(res.ExtendIdentityUserId).Result;
            Notifications n = new Notifications();
            n.ExtendIdetityUserId = res.ExtendIdentityUserId;
            n.NitificationDateTime = now;
            n.NotificationDetails = "Your Event Proposal Request for " + contactname + " has been Accepted by top manager utterly";
            string role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if (role == "System Admin")
            {
                n.Url = "ProposalRequests.html";
            }
            else
            {
                if (role == "System Adminn")
                {
                    n.Url = "ProposalRequests.html";
                }
                else
                {

               
                if (role == "First Line Manager")
                {
                    n.Url = "MyProposalRequests.html";
                }
                else
                {
                    if (role == "Medical Representative")
                    {
                        n.Url = "MyProposalRequests.html";
                    }
                }
                }
            }

            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }

        public bool TopRejectProposal(int id, string TopId)
        {
            DateTime now = ti.GetCurrentTime();
            EventProposalRequest res = db.EventProposalRequest.Find(id);
            res.TopConfirmed = false;
            res.TopRejected = true;
            res.TopAction = true;
            res.TopActionUserId = TopId;
            string contactname = db.contact.Find(res.ContactId).ContactName;
            EventTravelRequest etr = db.EventTravelRequest.Where(a => a.ContactId == res.ContactId && a.EventId == res.EventId && a.ExtendIdentityUserId == res.ExtendIdentityUserId).FirstOrDefault();
            if (etr != null)
            {
                etr.TopAction = true;
                etr.TopRejected = true;
                etr.TopActionUserId = TopId;
                etr.TopActionDateTime = now;
            }
       
            ExtendIdentityUser user = userManager.FindByIdAsync(res.ExtendIdentityUserId).Result;
            Notifications n = new Notifications();
            n.ExtendIdetityUserId = res.ExtendIdentityUserId;
            n.NitificationDateTime = now;
            n.NotificationDetails = "Your Event Proposal Request for " + contactname + " has been Rejected by top manager utterly";
            string role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if (role == "System Admin")
            {
                n.Url = "ProposalRequests.html";
            }
            else
            {
                if (role == "System Adminn")
                {
                    n.Url = "ProposalRequests.html";
                }
                else
                {

                if (role == "First Line Manager")
                {
                    n.Url = "MyProposalRequests.html";
                }
                else
                {
                    if (role == "Medical Representative")
                    {
                        n.Url = "MyProposalRequests.html";
                    }
                }

                }
            }

            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }

        public bool TopHoldProposal(int id)
        {
            DateTime now = ti.GetCurrentTime();
            EventProposalRequest res = db.EventProposalRequest.Find(id);
            res.TopConfirmed = false;
            res.TopRejected = false;
            res.TopAction = false;
            res.Confirmed = false;
            res.Rejected = false;
            res.TopActionUserId = null;
            string contactname = db.contact.Find(res.ContactId).ContactName;
            ExtendIdentityUser user = userManager.FindByIdAsync(res.ExtendIdentityUserId).Result;
            Notifications n = new Notifications();
            n.ExtendIdetityUserId = res.ExtendIdentityUserId;
            n.NitificationDateTime = now;
            n.NotificationDetails = "Your Event Travel Request for " + contactname + " has been Holded by top manager utterly";
            string role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if (role == "System Admin")
            {
                n.Url = "ProposalRequests.html";
            }
            else
            {
                if (role == "System Adminn")
                {
                    n.Url = "ProposalRequests.html";
                }
                else
                {

               
                if (role == "First Line Manager")
                {
                    n.Url = "MyProposalRequests.html";
                }
                else
                {
                    if (role == "Medical Representative")
                    {
                        n.Url = "MyProposalRequests.html";
                    }
                }
                }
            }

            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }

        public bool DeleteProposal(int id)
        {
            EventProposalRequest obj = db.EventProposalRequest.Find(id);

            int? eventtravelid = db.EventTravelRequest.Where(a => a.EventId == obj.EventId && a.ContactId == obj.ContactId && a.ExtendIdentityUserId == obj.ExtendIdentityUserId).Select(a=>a.Id).FirstOrDefault();

            if(eventtravelid == null || eventtravelid == 0)
            {
                db.EventProposalRequest.Remove(obj);
                db.SaveChanges();
                return true;
            }
            else
            {
               bool x = DeleteRequest((int)eventtravelid);
                if(x == true)
                {
                    db.EventProposalRequest.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public bool EditEvent(Event ev)
        {
            Event old = db.Event.Find(ev.Id);
            old.EventName = ev.EventName;
            old.EventTypeId = ev.EventTypeId;
            old.From = ev.From;
            old.To = ev.To;
            old.TravelCitiesId = ev.TravelCitiesId;
            db.SaveChanges();

            List<EventProposalRequest> proposallist = db.EventProposalRequest.Where(a => a.EventId == old.Id).ToList();
            foreach (var item in proposallist)
            {
                item.RequestDate = ev.From;
            }
            db.SaveChanges();
            return true;
        }

        public Event GetEventById(int id)
        {
            Event e = db.Event.Find(id);
            return e;
        }

        public WorkshopModel GetWorkshopById(int id)
        {
            Event e = db.Event.Find(id);
            List<string> s = new List<string>();
            List<EventSpeaker> sp = db.EventSpeaker.Where(a => a.EventId == id).ToList();
            foreach (var item in sp)
            {
                s.Add(item.SpeakerName);
            }
            WorkshopModel res = new WorkshopModel();
            res.EventName = e.EventName;
            res.From = e.From;
            res.To = e.To;
            res.LocationName = e.LocationName;
            res.TravelCitiesId = e.TravelCitiesId;
            res.speakers = s;
            return res;
        }

        public bool DeletePassport(int id)
        {
            string filename = db.EventTravelRequest.Find(id).PassportFileName;
            var path = Path.Combine(
                             Directory.GetCurrentDirectory(),
                             "wwwroot", "Passport",
                             filename);

            File.Delete(path);
            
            EventTravelRequest x = db.EventTravelRequest.Find(id);
            x.IsPassport = false;
            db.SaveChanges();

            return true;
        }

        public WorkshopInfoModel GetWorkshopInfo(int id)
        {
            Event e = db.Event.Find(id);
            string location = e.LocationName;

            List<string> speakers = db.EventSpeaker.Where(a => a.EventId == id).Select(a => a.SpeakerName).ToList();

            WorkshopInfoModel res = new WorkshopInfoModel();

            res.location = location;
            res.speakers = speakers;
            res.From = e.From.ToString("hh:mm tt");
            res.To = e.To.ToString("hh:mm tt");
            return res;
        }

        public bool EditWorkshop(EditWorkshopModel ev)
        {
            Event old = db.Event.Find(ev.Id);
            List<EventSpeaker> splist = db.EventSpeaker.Where(a => a.EventId == old.Id).ToList();

            foreach (var item in splist)
            {
                db.EventSpeaker.Remove(item);
            }

      
                old.EventName = ev.EventName;
                old.From = ev.From;
                old.To = ev.To;
                old.TravelCitiesId = ev.TravelCitiesId;
                old.LocationName = ev.LocationName;
                foreach (var item in ev.Speakers)
                {
                    EventSpeaker s = new EventSpeaker();
                    s.SpeakerName = item.S;
                    s.EventId = old.Id;
                    db.EventSpeaker.Add(s);
                }
            

            

            List<EventProposalRequest> proposallist = db.EventProposalRequest.Where(a => a.EventId == old.Id).ToList();
            foreach (var item in proposallist)
            {
                item.RequestDate = ev.From;
            }
            db.SaveChanges();
            return true;
        }

        public Event GetEventSpeakersAndLocation(int id)
        {
            Event res = db.Event.Find(id);
            List<EventSpeaker> sp = db.EventSpeaker.Where(a => a.EventId == id).ToList();
            res.speakers = sp;

            return res;
        }

        public IEnumerable<CustomEventRequest> TedSheet()
        {
            DateTime now = ti.GetCurrentTime();

            List<EventTravelRequest> list = db.EventTravelRequest.Where(a => a.EventId == 9).ToList();

            IEnumerable<CustomEventRequest> res = list.Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                RepName = b.FullName,
                ContactId = a.ContactId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = b.CityId
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = b.Gender,
                RepName = a.RepName,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.travelCities, a => a.WayInCityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = b.City,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.travelCities, a => a.WayOutCityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = b.City,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.travelCities, a => a.WayInDestinationId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = b.City,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.travelCities, a => a.WayOutDestinationId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = b.City,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                EventTypeId = b.EventTypeId,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = b.TypeName,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.city, a => a.RepCityId, b => b.Id, (a, b) => new CustomEventRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = a.EventTypeName,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCity = b.CityName
            });



            IEnumerable<CustomEventRequest> restwo = list.Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                RepName = b.FullName,
                ContactId = a.ContactId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = b.CityId
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = b.Gender,
                RepName = a.RepName,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                EventTypeId = b.EventTypeId,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = b.TypeName,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCityId = a.RepCityId
            }).Join(db.city, a => a.RepCityId, b => b.Id, (a, b) => new CustomEventRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = a.EventTypeName,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                RepCity = b.CityName
            });


            List<CustomEventRequest> ress = new List<CustomEventRequest>();

            foreach (var item in res)
            {
                CustomEventRequest x = new CustomEventRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.WayInDeparture = item.WayInDeparture;
                x.WayOutDeparture = item.WayOutDeparture;
                x.WayInArrival = item.WayInArrival;
                x.WayOutArrival = item.WayOutArrival;
                x.WayInCityName = item.WayInCityName;
                x.WayOutCityName = item.WayOutCityName;
                x.WayInFlightNumber = item.WayInFlightNumber;
                x.WayOutFlightNumber = item.WayOutFlightNumber;
                x.WayInDestinationName = item.WayInDestinationName;
                x.WayOutDestinationName = item.WayOutDestinationName;
                x.HotelName = item.HotelName;
                x.RoomType = item.RoomType;
                x.Accumpained = item.Accumpained;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.PassportNumber = item.PassportNumber;
                x.PassportExpiryDate = item.PassportExpiryDate;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.IsPassport = item.IsPassport;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                x.RepCity = item.RepCity;
                ress.Add(x);
            }

            foreach (var item in restwo)
            {
                CustomEventRequest x = new CustomEventRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.WayInDeparture = item.WayInDeparture;
                x.WayOutDeparture = item.WayOutDeparture;
                x.WayInArrival = item.WayInArrival;
                x.WayOutArrival = item.WayOutArrival;
                x.WayInFlightNumber = item.WayInFlightNumber;
                x.WayOutFlightNumber = item.WayOutFlightNumber;
                x.HotelName = item.HotelName;
                x.RoomType = item.RoomType;
                x.Accumpained = item.Accumpained;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.PassportNumber = item.PassportNumber;
                x.PassportExpiryDate = item.PassportExpiryDate;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.IsPassport = item.IsPassport;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                x.RepCity = item.RepCity;
                ress.Add(x);
            }

            return ress.DistinctBy(a => a.Id).OrderBy(a => a.WayInDeparture);
        }

        public IEnumerable<Event> GetEventsForSystemAdmin()
        {
            DateTime now = ti.GetCurrentTime().AddMonths(-6);
            IEnumerable<Event> res = db.Event.Where(a => a.To >= now);
            return res.OrderBy(a => a.EventName);
        }

        public IEnumerable<CustomEventRequest> GetAllPastRequests()
        {
            DateTime now = ti.GetCurrentTime();

            IEnumerable<CustomEventRequest> fres = db.EventTravelRequest.Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                RepName = b.FullName,
                ContactId = a.ContactId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = b.Gender,
                RepName = a.RepName,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                From = b.From,
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                EventTypeId = b.EventTypeId,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Where(a => a.From.Date <= now).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new CustomEventRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = b.TypeName,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            });




            List<CustomEventRequest> sres = db.EventTravelRequest.Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                RepName = b.FullName,
                ContactId = a.ContactId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = b.Gender,
                RepName = a.RepName,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityId = a.WayInCityId,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.travelCities, a => a.WayInCityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = b.City,
                WayOutCityId = a.WayOutCityId,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.travelCities, a => a.WayOutCityId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = b.City,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationId = a.WayInDestinationId,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.travelCities, a => a.WayInDestinationId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = b.City,
                WayOutDestinationId = a.WayOutDestinationId,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.travelCities, a => a.WayOutDestinationId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = b.City,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                From = b.From,
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                EventTypeId = b.EventTypeId,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).Where(a => a.From.Date <= now).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new CustomEventRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = b.TypeName,
                RepName = a.RepName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                WayInDeparture = a.WayInDeparture,
                WayOutDeparture = a.WayOutDeparture,
                WayInArrival = a.WayInArrival,
                WayOutArrival = a.WayOutArrival,
                WayInCityName = a.WayInCityName,
                WayOutCityName = a.WayOutCityName,
                WayInFlightNumber = a.WayInFlightNumber,
                WayOutFlightNumber = a.WayOutFlightNumber,
                WayInDestinationName = a.WayInDestinationName,
                WayOutDestinationName = a.WayOutDestinationName,
                HotelName = a.HotelName,
                RoomType = a.RoomType,
                Accumpained = a.Accumpained,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                PassportNumber = a.PassportNumber,
                PassportExpiryDate = a.PassportExpiryDate,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                IsPassport = a.IsPassport,
                FirstActionDateTime = a.FirstActionDateTime,
                TopActionDateTime = a.TopActionDateTime
            }).ToList();

            List<CustomEventRequest> ress = new List<CustomEventRequest>();


            foreach (var item in sres)
            {
                CustomEventRequest x = new CustomEventRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.WayInDeparture = item.WayInDeparture;
                x.WayOutDeparture = item.WayOutDeparture;
                x.WayInArrival = item.WayInArrival;
                x.WayOutArrival = item.WayOutArrival;
                x.WayInCityName = item.WayInCityName;
                x.WayOutCityName = item.WayOutCityName;
                x.WayInFlightNumber = item.WayInFlightNumber;
                x.WayOutFlightNumber = item.WayOutFlightNumber;
                x.WayInDestinationName = item.WayInDestinationName;
                x.WayOutDestinationName = item.WayOutDestinationName;
                x.HotelName = item.HotelName;
                x.RoomType = item.RoomType;
                x.Accumpained = item.Accumpained;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.PassportNumber = item.PassportNumber;
                x.PassportExpiryDate = item.PassportExpiryDate;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.IsPassport = item.IsPassport;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }

                if (item.FirstActionDateTime != null)
                {
                    x.FirstActionDateTimestr = ((DateTime)item.FirstActionDateTime).ToString("dd/MM/yyyy - hh:mm tt");
                }
                if (item.TopActionDateTime != null)
                {
                    x.TopActionDateTimestr = ((DateTime)item.TopActionDateTime).ToString("dd/MM/yyyy - hh:mm tt");
                }


                ress.Add(x);
            }

            foreach (var item in fres)
            {
                CustomEventRequest x = new CustomEventRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.WayInDeparture = item.WayInDeparture;
                x.WayOutDeparture = item.WayOutDeparture;
                x.WayInArrival = item.WayInArrival;
                x.WayOutArrival = item.WayOutArrival;
                x.WayInFlightNumber = item.WayInFlightNumber;
                x.WayOutFlightNumber = item.WayOutFlightNumber;
                x.HotelName = item.HotelName;
                x.RoomType = item.RoomType;
                x.Accumpained = item.Accumpained;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.PassportNumber = item.PassportNumber;
                x.PassportExpiryDate = item.PassportExpiryDate;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.IsPassport = item.IsPassport;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                ress.Add(x);
            }






            return ress.DistinctBy(a => a.Id).OrderByDescending(a=>a.WayInDeparture);
        }

        public bool DeleteTicket(int id)
        {
            string filename = db.EventTravelRequest.Find(id).TicketFileName;
            var path = Path.Combine(
                             Directory.GetCurrentDirectory(),
                             "wwwroot", "Ticket",
                             filename);
            File.Delete(path);
            EventTravelRequest x = db.EventTravelRequest.Find(id);
            x.IsTicket = false;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<CustomProposalRequest> GetEventProposalRequests(int id)
        {
            DateTime now = ti.GetCurrentTime();


            List<EventProposalRequest> list = db.EventProposalRequest.Where(a => a.RequestDate >= now.AddMonths(-6) && a.EventId == id).ToList();


            IEnumerable<CustomProposalRequest> res = list.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Gender = b.Gender,
                EventId = a.EventId,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.ExtendIdentityUserId
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.RepId
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                EventTypeId = b.EventTypeId,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.RepId
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = b.TypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.RepId
            }).Join(db.Users, a => a.RepId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = a.EventTypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepName = b.FullName,
                RepCityId = b.CityId
            }).Join(db.city, a => a.RepCityId, b => b.Id, (a, b) => new CustomProposalRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = a.EventTypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepName = a.RepName,
                RepCity = b.CityName
            });

            List<CustomProposalRequest> ress = new List<CustomProposalRequest>();

            foreach (var item in res)
            {
                CustomProposalRequest x = new CustomProposalRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.RequestDate = item.RequestDate;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                x.RepCity = item.RepCity;
                ress.Add(x);
            }
            return ress;
        }

        public List<Event> GetSixMonthEvents()
        {
            DateTime now = ti.GetCurrentTime();
            List<Event> res = db.Event.Where(a => a.From >= now.AddMonths(-6)).ToList();
            return res.OrderBy(a => a.EventName).ToList();
        }

        public IEnumerable<CustomProposalRequest> GetMyTeamPendingProposalRequests(string ManagerId)
        {
            DateTime now = ti.GetCurrentTime();

            List<string> MyTeamIds = userManager.Users.Where(a => a.extendidentityuserid == ManagerId).Select(a => a.Id).ToList();
            List<EventProposalRequest> list = new List<EventProposalRequest>();
            foreach (var item in MyTeamIds)
            {
                List<EventProposalRequest> l = db.EventProposalRequest.Where(a => a.ExtendIdentityUserId == item && a.RequestDate >= now && a.Confirmed == false && a.Rejected == false && a.TopConfirmed == false && a.TopRejected == false).ToList();

                foreach (var it in l)
                {
                    list.Add(it);
                }
            }

            List<EventProposalRequest> ll = db.EventProposalRequest.Where(a => a.ExtendIdentityUserId == ManagerId && a.RequestDate >= now && a.Confirmed == false && a.Rejected == false && a.TopConfirmed == false && a.TopRejected == false).ToList();

            foreach (var it in ll)
            {
                list.Add(it);
            }

            IEnumerable<CustomProposalRequest> res = list.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Gender = b.Gender,
                EventId = a.EventId,
                ContactName = b.ContactName,
                ContactPhone = b.MobileNumber,
                ContactMail = b.Email,
                AccountId = b.AccountId,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.ExtendIdentityUserId
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventId = a.EventId,
                Gender = a.Gender,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = b.AccountName,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.RepId
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Gender = a.Gender,
                EventTypeId = b.EventTypeId,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.RepId
            }).Join(db.EventType, a => a.EventTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = b.TypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepId = a.RepId
            }).Join(db.Users, a => a.RepId, b => b.Id, (a, b) => new CustomProposalRequest
            {
                Id = a.Id,
                EventName = a.EventName,
                Gender = a.Gender,
                EventTypeName = a.EventTypeName,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                ContactMail = a.ContactMail,
                AccountAffiliation = a.AccountAffiliation,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                TopConfirmed = a.TopConfirmed,
                TopRejected = a.TopRejected,
                TopAction = a.TopAction,
                TopActionUserId = a.TopActionUserId,
                RequestDate = a.RequestDate,
                RepName = b.FullName
            });

            List<CustomProposalRequest> ress = new List<CustomProposalRequest>();

            foreach (var item in res)
            {
                CustomProposalRequest x = new CustomProposalRequest();
                x.Id = item.Id;
                x.EventName = item.EventName;
                x.Gender = item.Gender;
                x.EventTypeName = item.EventTypeName;
                x.RepName = item.RepName;
                x.ContactName = item.ContactName;
                x.ContactPhone = item.ContactPhone;
                x.ContactMail = item.ContactMail;
                x.AccountAffiliation = item.AccountAffiliation;
                x.Confirmed = item.Confirmed;
                x.Rejected = item.Rejected;
                x.TopConfirmed = item.TopConfirmed;
                x.TopRejected = item.TopRejected;
                x.TopAction = item.TopAction;
                x.TopActionUserId = item.TopActionUserId;
                x.RequestDate = item.RequestDate;
                if (item.TopActionUserId == null)
                {
                    x.TopActionUserName = null;
                }

                else
                {
                    x.TopActionUserName = userManager.FindByIdAsync(item.TopActionUserId).Result.FullName;
                }
                ress.Add(x);
            }
            return ress;
        }

        public List<EventAttendanceModel> GetEventAttenance(int EventId)
        {
            List<EventAttendanceModel> res = db.Event.Where(a => a.Id == EventId).Join(db.EventProposalRequest, a => a.Id, b => b.EventId, (a, b) => new
            {
                ContactId = b.ContactId,
                EventId = b.EventId,
                ExtendIdentityUserId = b.ExtendIdentityUserId
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                ContactName = b.ContactName,
                Email = b.Email,
                ContactId = a.ContactId,
                EventId = a.EventId,
                ExtendIdentityUserId = a.ExtendIdentityUserId,
                AccountId = b.AccountId
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                ContactName = a.ContactName,
                Email = a.Email,
                ExtendIdentityUserId = a.ExtendIdentityUserId,
                AccountName = b.AccountName
            }).Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new EventAttendanceModel
            {
                ContactName = a.ContactName,
                Email = a.Email,
                UserName = b.FullName,
                AccountName = a.AccountName
            }).ToList();

            return res;
        }
    }
}

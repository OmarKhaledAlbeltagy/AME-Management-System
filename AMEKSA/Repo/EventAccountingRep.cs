using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using MoreLinq;
using MoreLinq.Extensions;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AMEKSA.Repo
{
    public class EventAccountingRep:IEventAccountingRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;

        public EventAccountingRep(DbContainer db, ITimeRep ti)
        {
            this.db = db;
            this.ti = ti;
        }

        public EventFeesSimpleModel AddAccountingItem(AddAccountingItemModel obj)
        {
            DateTime now = ti.GetCurrentTime();

            int indx = obj.file.FileName.Split('.').Length - 1;
            string extension = obj.file.FileName.Split('.')[indx];
            string fileType = obj.file.ContentType;
            EventFees ef = new EventFees();
            using (var stream = new MemoryStream())
            {
                obj.file.CopyTo(stream);
                var bytes = stream.ToArray();
            
                ef.Title = obj.Title;
                ef.Sort = obj.Sort;
                ef.ExtendIdentityUserId = obj.ExtendIdentityUserId;
                ef.EventtId = obj.EventtId;
                ef.dateTime = now;
                ef.ContentType = fileType;
                ef.file = bytes;
                ef.Extension = extension;
                ef.Note = obj.Note;
                ef.Value = obj.Value;
                db.eventFees.Add(ef);
                db.SaveChanges();
            }
            EventFeesSimpleModel res = new EventFeesSimpleModel();
            res.Id = ef.Id;
            res.Value = ef.Value;
            res.Title= ef.Title;
            res.Note = ef.Note;
            res.DateTime = ef.dateTime.ToString("dd MMMM yyyy - hh:mm tt");
            return res;
        }

        public EventFeesSimpleModel AddAccountingItemRequest(AddAccountingItemModel obj)
        {
            DateTime now = ti.GetCurrentTime();

            int indx = obj.file.FileName.Split('.').Length - 1;
            string extension = obj.file.FileName.Split('.')[indx];
            string fileType = obj.file.ContentType;
            EventFeesRequest ef = new EventFeesRequest();
            using (var stream = new MemoryStream())
            {
                obj.file.CopyTo(stream);
                var bytes = stream.ToArray();
                ef.Title = obj.Title;
                ef.Sort = obj.Sort;
                ef.ExtendIdentityUserId = obj.ExtendIdentityUserId;
                ef.EventtId = obj.EventtId;
                ef.dateTime = now;
                ef.ContentType = fileType;
                ef.file = bytes;
                ef.Extension = extension;
                ef.Note = obj.Note;
                ef.Value = obj.Value;
                ef.Confirmed = null;
                db.eventFeesRequest.Add(ef);
                db.SaveChanges();
            }
            EventFeesSimpleModel res = new EventFeesSimpleModel();
            res.Id = ef.Id;
            res.Value = ef.Value;
            res.Title = ef.Title;
            res.Note = ef.Note;
            res.DateTime = ef.dateTime.ToString("dd MMMM yyyy - hh:mm tt");
            return res;
        }

        public bool ConfirmRequest(int id)
        {
            using (IDbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    EventFeesRequest efr = db.eventFeesRequest.Find(id);
                    efr.Confirmed = true;
                    db.SaveChanges();

                    EventFees res = new EventFees();
                    res.Title = efr.Title;
                    res.EventtId = efr.EventtId;
                    res.Value = efr.Value;
                    res.Note  = efr.Note;
                    res.ExtendIdentityUserId = efr.ExtendIdentityUserId;
                    res.dateTime = efr.dateTime;
                    res.ContentType = efr.ContentType;
                    res.file = efr.file;
                    res.Extension = efr.Extension;
                    res.Sort = efr.Sort;
                    db.eventFees.Add(res);
                    db.SaveChanges();
                    transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    var message = ex.InnerException;
                    transaction.Rollback();
                    return false;
                }
            }
                
        }

        public bool DeleteExpense(int id)
        {
            EventFees ef = db.eventFees.Find(id);
            db.eventFees.Remove(ef);
            db.SaveChanges();
            return true;
        }

        public bool DeleteExpenseRequest(int id)
        {
            EventFeesRequest efr = db.eventFeesRequest.Find(id);
            db.eventFeesRequest.Remove(efr);
            db.SaveChanges();
            return true;
        }

        public bool EditExpenseRequestWithFile(EditExpenseWithFileModel obj)
        {
            int indx = obj.file.FileName.Split('.').Length - 1;
            string extension = obj.file.FileName.Split('.')[indx];
            string fileType = obj.file.ContentType;
            EventFeesRequest efr = db.eventFeesRequest.Find(obj.Id);
            using (var stream = new MemoryStream())
            {
                obj.file.CopyTo(stream);
                var bytes = stream.ToArray();
                efr.Title = obj.Title;
                efr.Sort = obj.Sort;
                efr.ContentType = fileType;
                efr.file = bytes;
                efr.Extension = extension;
                efr.Note = obj.Note;
                efr.Value = obj.Value;
                efr.Confirmed = null;
                db.SaveChanges();
            }

            return true;
        }

        public bool EditExpenseRequestWithoutFile(EditExpenseWithoutFileModel obj)
        {
            EventFeesRequest efr = db.eventFeesRequest.Find(obj.Id);
            efr.Title = obj.Title;
            efr.Sort = obj.Sort;
            efr.Note = obj.Note;
            efr.Value = obj.Value;
            efr.Confirmed = null;
            db.SaveChanges();

            return true;
        }

        public bool EditExpenseWithFile(EditExpenseWithFileModel obj)
        {
            int indx = obj.file.FileName.Split('.').Length - 1;
            string extension = obj.file.FileName.Split('.')[indx];
            string fileType = obj.file.ContentType;
            EventFees ef = db.eventFees.Find(obj.Id);
            using (var stream = new MemoryStream())
            {
                obj.file.CopyTo(stream);
                var bytes = stream.ToArray();
                ef.Title = obj.Title;
                ef.Sort = obj.Sort;
                ef.ContentType = fileType;
                ef.file = bytes;
                ef.Extension = extension;
                ef.Note = obj.Note;
                ef.Value = obj.Value;
                db.SaveChanges();
            }

            return true;
        }

        public bool EditExpenseWithoutFile(EditExpenseWithoutFileModel obj)
        {
            EventFees ef = db.eventFees.Find(obj.Id);
            ef.Title = obj.Title;
            ef.Sort = obj.Sort;
            ef.Note = obj.Note;
            ef.Value = obj.Value;
            db.SaveChanges();

            return true;
        }

     

        public EventExcelModel GetAllExpensesForExcel(int EventId, string UserId)
        {
            DateTime now = ti.GetCurrentTime();
            Event ev = db.Event.Find(EventId);
            EventExcelModel res = new EventExcelModel();
            res.Attendees = db.EventTravelRequest.Where(a => (a.Confirmed == true && a.EventId == EventId) || (a.TopConfirmed == true && a.EventId == EventId)).Count();
            res.EventName = ev.EventName;
            res.EventType = db.EventType.Find(ev.EventTypeId).TypeName;
            res.StartDate = ev.From.ToString("dd MMMM yyyy");
            res.EndDate = ev.To.ToString("dd MMMM yyyy");
            res.UserNameExported = db.Users.Find(UserId).FullName;
            res.ExportingDateTime = now.ToString("dd MMMM yyyy - hh:mm tt");

            if (ev.EventTypeId == 2)
            {
                List<EventSpeaker> sp = db.EventSpeaker.Where(a => a.EventId == EventId).ToList();
                res.speakers = sp;
            }

            List<EventFees> ef = db.eventFees.Where(a => a.EventtId == EventId && a.ExtendIdentityUserId == UserId).OrderBy(a => a.Sort).ToList();
            List<EventFeesRequest> efr = db.eventFeesRequest.Where(a => a.EventtId == EventId).OrderBy(a => a.Sort).ToList();

            List<EventFeesSimpleModel> Expenses = new List<EventFeesSimpleModel>();

            foreach (var item in efr)
            {
                EventFeesSimpleModel obj = new EventFeesSimpleModel();
                obj.Id = item.Id;
                obj.Title = item.Title;
                obj.Value = item.Value;
                obj.Note = item.Note;
                obj.DateTime = item.dateTime.ToString("dd MMMM yyyy - hh:mm tt");
                obj.RejectionReason = item.RejectionReason;
                obj.Confirmed = item.Confirmed;
                obj.UserName = db.Users.Find(item.ExtendIdentityUserId).FullName;
                Expenses.Add(obj);
            }
            foreach (var item in ef)
            {
                EventFeesSimpleModel obj = new EventFeesSimpleModel();
                obj.Id = item.Id;
                obj.Title = item.Title;
                obj.Value = item.Value;
                obj.Note = item.Note;
                obj.DateTime = item.dateTime.ToString("dd MMMM yyyy - hh:mm tt");
                obj.RejectionReason = "";
                obj.Confirmed = true;
                obj.UserName = db.Users.Find(item.ExtendIdentityUserId).FullName;
                Expenses.Add(obj);
            }
            res.Expenses = Expenses.OrderBy(a=>a.UserName).ToList();
            return res;
        }

        public List<EventFeesSimpleModel> GetEventExpenses(int id, string UserId)
        {
            List<EventFeesSimpleModel> res = db.eventFeesRequest.Where(a => a.EventtId == id).Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new EventFeesSimpleModel
            {
                Id = a.Id,
                Title = a.Title,
                Value = a.Value,
                Note = a.Note,
                SortDateTime = a.dateTime,
                DateTime = a.dateTime.ToString("dd MMMM yyyy - hh:mm tt"),
                UserName = b.FullName,
                Confirmed = a.Confirmed,
                RejectionReason = a.RejectionReason
            }).ToList();

            List<EventFeesSimpleModel> ress = db.eventFees.Where(a => a.EventtId == id && a.ExtendIdentityUserId == UserId).Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new EventFeesSimpleModel
            {
                Id = a.Id,
                Title = a.Title,
                Value = a.Value,
                Note = a.Note,
                SortDateTime = a.dateTime,
                DateTime = a.dateTime.ToString("dd MMMM yyyy - hh:mm tt"),
                UserName = b.FullName,
                Confirmable = false
            }).ToList();

            res.AddRange(ress);

            return res.OrderByDescending(a=>a.SortDateTime).ToList();

        }

        public List<EventExcelModel> GetEventsTotalExpenses(DateTime from, DateTime to, string UserId)
        {
            to = new DateTime(to.Year,to.Month,to.Day,11,59,59);
            List<Event> ev = db.Event.Where(a => a.From >= from && a.To <= to).OrderBy(a=>a.EventName).ToList();
            DateTime now = ti.GetCurrentTime();
            List<EventExcelModel> ress = new List<EventExcelModel>();
            foreach (var item in ev)
            {
                double check = db.eventFees.Where(a => a.EventtId == item.Id).Select(a => a.Value).Sum();

                if (check == 0)
                {
                    continue;
                }

                EventExcelModel res = new EventExcelModel();
                res.Attendees = db.EventTravelRequest.Where(a => (a.Confirmed == true && a.EventId == item.Id) || (a.TopConfirmed == true && a.EventId == item.Id)).Count();
                res.EventName = item.EventName;
                res.EventType = db.EventType.Find(item.EventTypeId).TypeName;
                res.StartDate = item.From.ToString("dd MMMM yyyy");
                res.EndDate = item.To.ToString("dd MMMM yyyy");
                res.UserNameExported = db.Users.Find(UserId).FullName;
                res.ExportingDateTime = now.ToString("dd MMMM yyyy - hh:mm tt");

                if (item.EventTypeId == 2)
                {
                    List<EventSpeaker> sp = db.EventSpeaker.Where(a => a.EventId == item.Id).ToList();
                    res.speakers = sp;
                }

                List<EventTotalExpensesModel> totals = new List<EventTotalExpensesModel>();


                double rcheck = db.eventFees.Where(a => a.EventtId == item.Id && a.Title == "Registration").Select(a => a.Value).Sum();

                if (rcheck > 0)
                {
                    EventTotalExpensesModel robj = new EventTotalExpensesModel();
                    robj.Title = "Registration";
                    robj.Value = rcheck;
                    totals.Add(robj);
                }
                
                double tcheck = db.eventFees.Where(a => a.EventtId == item.Id && a.Title == "Tickets").Select(a => a.Value).Sum();
                if (tcheck > 0)
                {
                    EventTotalExpensesModel tobj = new EventTotalExpensesModel();
                    tobj.Title = "Tickets";
                    tobj.Value = tcheck;
                    totals.Add(tobj);
                }
                
                double hcheck = db.eventFees.Where(a => a.EventtId == item.Id && a.Title == "Hotels").Select(a => a.Value).Sum();
                if (hcheck > 0)
                {
                    EventTotalExpensesModel hobj = new EventTotalExpensesModel();
                    hobj.Title = "Hotels";
                    hobj.Value = hcheck;
                    totals.Add(hobj);
                }
             
                double trcheck = db.eventFees.Where(a => a.EventtId == item.Id && a.Title == "Transportation").Select(a => a.Value).Sum();
                if (trcheck > 0)
                {
                    EventTotalExpensesModel trobj = new EventTotalExpensesModel();
                    trobj.Title = "Transportation";
                    trobj.Value = trcheck;
                    totals.Add(trobj);
                }
                

                List<EventFees> other = db.eventFees.Where(a=>a.Title != "Registration" && a.Title != "Tickets" && a.Title != "Hotels" && a.Title != "Transportation" && a.EventtId == item.Id).ToList();

                foreach (var o in other)
                {
                    EventTotalExpensesModel oobj = new EventTotalExpensesModel();
                    oobj.Title = o.Title;
                    oobj.Value = other.Where(a => a.Title == o.Title).Select(a => a.Value).Sum();
                    //oobj.Value  = o.Value;
                    totals.Add(oobj);
                }
                res.Total = check;
                res.totals = DistinctByExtension.DistinctBy(totals,a=>a.Title).ToList();
                ress.Add(res);
            }

            return ress;
           
        }

        public List<EventTotalFeesModel> GetEventsTotalFees()
        {
            List<EventTotalFeesModel> res = new List<EventTotalFeesModel>();
            DateTime now = ti.GetCurrentTime();
            List<Event> events = db.Event.Where(a=>a.From.Date >= now.AddMonths(-3).Date).ToList();

            foreach (var item in events)
            {
                EventTotalFeesModel obj = new EventTotalFeesModel();
                obj.Id = item.Id;
                obj.EventName = item.EventName;
                obj.TotalFees = db.eventFees.Where(a => a.EventtId == item.Id).Select(a => a.Value).Sum();
                obj.TotalFeesRequested = db.eventFeesRequest.Where(a => a.EventtId == item.Id).Select(a => a.Value).Sum();
                obj.date = item.From;
                obj.StartDate = item.From.ToString("dd MMMM yyyy (dddd)");
                obj.EndDate = item.To.ToString("dd MMMM yyyy (dddd)");
                obj.Attendees = db.EventTravelRequest.Where(a => ( a.Confirmed == true && a.EventId == item.Id) || (a.TopConfirmed == true && a.EventId == item.Id)).Count();
                obj.Pending = db.eventFeesRequest.Where(a=>a.Confirmed == null && a.EventtId == item.Id).Count();
                res.Add(obj);
            }
            return res.OrderByDescending(a=>a.date).ToList();
        }

        public List<MorrisDonutModel> GetMorrisChart(DateTime from, DateTime to)
        {
            to = new DateTime(to.Year,to.Month,to.Day,23,59,59);
            List<MorrisDonutModel> res = new List<MorrisDonutModel>();
            List<Event> ev = db.Event.Where(a=>a.From >= from && a.To <= to).ToList();

            foreach (var e in ev)
            {
                MorrisDonutModel obj = new MorrisDonutModel();
                obj.label = e.EventName;
                obj.value = db.eventFees.Where(a => a.EventtId == e.Id).Select(a => a.Value).Sum();
                if (obj.value == 0)
                {

                }
                else
                {
                    res.Add(obj);
                }
                
            }
            return res.OrderByDescending(a=>a.value).ToList();
        }

        public dynamic GetMyEventAccounting(int EventId, string UserId)
        {
            List<EventFees> ef = db.eventFees.Where(a=>a.ExtendIdentityUserId == UserId && a.EventtId == EventId).OrderBy(a=>a.Sort).ToList();
            List<EventFeesSimpleModel> res = new List<EventFeesSimpleModel>();
            string EventType = db.EventType.Find(db.Event.Find(EventId).EventTypeId).TypeName;
            foreach (var item in ef) 
            {
                EventFeesSimpleModel obj = new EventFeesSimpleModel();
                obj.Title = item.Title;
                obj.Id = item.Id;
                obj.Value = item.Value;
                obj.Note = item.Note;
                obj.EventType = EventType;
                obj.DateTime = item.dateTime.ToString("dd MMMM yyyy - hh:mm tt");
                res.Add(obj);
            }

            if (res.Count == 0)
            {
                return EventType;
            }
            else
            {
                return res;
            }
           
        }

        public dynamic GetMyEventAccountingRequests(int EventId, string UserId)
        {
            List<EventFeesRequest> efr = db.eventFeesRequest.Where(a => a.ExtendIdentityUserId == UserId && a.EventtId == EventId).OrderBy(a => a.Sort).ToList();
            List<EventFeesSimpleModel> res = new List<EventFeesSimpleModel>();
            string EventType = db.EventType.Find(db.Event.Find(EventId).EventTypeId).TypeName;
            foreach (var item in efr)
            {
                EventFeesSimpleModel obj = new EventFeesSimpleModel();
                obj.Title = item.Title;
                obj.Id = item.Id;
                obj.Value = item.Value;
                obj.Note = item.Note;
                obj.DateTime = item.dateTime.ToString("dd MMMM yyyy - hh:mm tt");
                obj.Confirmed = item.Confirmed;
                obj.RejectionReason = item.RejectionReason;
                obj.EventType = EventType;
                res.Add(obj);
            }
            if (res.Count == 0)
            {
                return EventType;
            }
            else
            {
                return res;
            }
        }

        public List<EventTotalFeesModel> GetMyEventsTotalFees(string UserId)
        {
            List<EventTotalFeesModel> res = new List<EventTotalFeesModel>();
            DateTime now = ti.GetCurrentTime();
            List<Event> events = db.Event.Where(a => a.From.Date >= now.AddMonths(-3).Date).ToList();

            foreach (var item in events)
            {
                EventTotalFeesModel obj = new EventTotalFeesModel();
                obj.Id = item.Id;
                obj.EventName = item.EventName;
                obj.TotalFees = db.eventFees.Where(a => a.EventtId == item.Id && a.ExtendIdentityUserId == UserId).Select(a => a.Value).Sum();
                obj.TotalFeesRequested = db.eventFeesRequest.Where(a => a.EventtId == item.Id && a.ExtendIdentityUserId == UserId).Select(a => a.Value).Sum();
                obj.date = item.From;
                obj.StartDate = item.From.ToString("dd MMMM yyyy (dddd)");
                obj.EndDate = item.To.ToString("dd MMMM yyyy (dddd)");
                obj.Attendees = db.EventTravelRequest.Where(a => (a.Confirmed == true && a.EventId == item.Id) || (a.TopConfirmed == true && a.EventId == item.Id)).Count();
                res.Add(obj);
            }
            return res.OrderByDescending(a => a.date).ToList();
        }

        public EventExcelModel GetMyExpensesForExcel(int EventId, string UserId)
        {
            DateTime now = ti.GetCurrentTime();
            Event ev = db.Event.Find(EventId);
            EventExcelModel res = new EventExcelModel();
            res.Attendees = db.EventTravelRequest.Where(a => (a.Confirmed == true && a.EventId == EventId) || (a.TopConfirmed == true && a.EventId == EventId)).Count();
            res.EventName = ev.EventName;
            res.EventType = db.EventType.Find(ev.EventTypeId).TypeName;
            res.StartDate = ev.From.ToString("dd MMMM yyyy");
            res.EndDate = ev.To.ToString("dd MMMM yyyy");
            res.UserNameExported = db.Users.Find(UserId).FullName;
            res.ExportingDateTime = now.ToString("dd MMMM yyyy - hh:mm tt");

            if (ev.EventTypeId == 2)
            {
                List<EventSpeaker> sp = db.EventSpeaker.Where(a => a.EventId == EventId).ToList();
                res.speakers = sp;
            }

            List<EventFeesRequest> efr = db.eventFeesRequest.Where(a => a.EventtId == EventId && a.ExtendIdentityUserId == UserId).OrderBy(a => a.Sort).ToList();

            List<EventFeesSimpleModel> Expenses = new List<EventFeesSimpleModel>();

            foreach (var item in efr)
            {
                EventFeesSimpleModel obj = new EventFeesSimpleModel();
                obj.Id = item.Id;
                obj.Title = item.Title;
                obj.Value = item.Value;
                obj.Note = item.Note;
                obj.DateTime = item.dateTime.ToString("dd MMMM yyyy - hh:mm tt");
                obj.RejectionReason = item.RejectionReason;
                obj.Confirmed = item.Confirmed;
                Expenses.Add(obj);
            }
            res.Expenses = Expenses;
            return res;

        }

        public List<EventFeesSimpleModel> GetNotMyEventAccountingRequests(int EventId, string UserId)
        {
            
            List<EventFeesRequest> efr = db.eventFeesRequest.Where(a => a.ExtendIdentityUserId != UserId && a.EventtId == EventId).OrderBy(a => a.Sort).ToList();
            

            List<EventFeesSimpleModel> res = new List<EventFeesSimpleModel>();
            string EventType = db.EventType.Find(db.Event.Find(EventId).EventTypeId).TypeName;
            foreach (var item in efr)
            {
                EventFeesSimpleModel obj = new EventFeesSimpleModel();
                obj.Title = item.Title;
                obj.UserName = db.Users.Find(item.ExtendIdentityUserId).FullName;
                obj.Id = item.Id;
                obj.Value = item.Value;
                obj.Note = item.Note;
                obj.DateTime = item.dateTime.ToString("dd MMMM yyyy - hh:mm tt");
                obj.Confirmed = item.Confirmed;
                obj.RejectionReason = item.RejectionReason;
                obj.EventType = EventType;
                res.Add(obj);
            }
        
            return res;
        }

        public List<EventFeesSimpleModel> GetAccountingManagerAccountingRequests(int EventId, string UserId)
        {
       
            List<EventFees> efrmanager = db.eventFees.Where(a => a.EventtId == EventId && a.ExtendIdentityUserId == "f8027f97-30e7-45db-943a-aa023ce3bda8").ToList();

            List<EventFeesSimpleModel> res = new List<EventFeesSimpleModel>();
            foreach (var item in efrmanager)
            {
                EventFeesSimpleModel obj = new EventFeesSimpleModel();
                obj.Title = item.Title;
                obj.UserName = db.Users.Find(item.ExtendIdentityUserId).FullName;
                obj.Id = item.Id;
                obj.Value = item.Value;
                obj.Note = item.Note;
                obj.DateTime = item.dateTime.ToString("dd MMMM yyyy - hh:mm tt");
                res.Add(obj);
            }
            return res;

        }

        public List<EventFeesSimpleModel> GetPendingRequests()
        {
            List<EventFeesSimpleModel> res = db.eventFeesRequest.Where(a => a.Confirmed == null).OrderByDescending(a => a.dateTime).Join(db.Event, a => a.EventtId, b => b.Id, (a, b) => new 
            {
                Id = a.Id,
                EventName = b.EventName,
                Title = a.Title,
                Value = a.Value,
                Note = a.Note,
                DateTime = a.dateTime.ToString("dd MMMM yyyy - hh:mm tt"),
                ExtendIdentityUserId = a.ExtendIdentityUserId
            }).Join(db.Users,a=>a.ExtendIdentityUserId,b=>b.Id,(a,b)=>new EventFeesSimpleModel 
            {
                Id = a.Id,
                EventName = a.EventName,
                Title = a.Title,
                Value = a.Value,
                Note = a.Note,
                DateTime = a.DateTime,
                UserName = b.FullName
            }).ToList();
           
            return res;
        }

        public List<EventFeesSimpleModel> GetPreviousRequests()
        {
            DateTime now = ti.GetCurrentTime();
            List<EventFeesSimpleModel> res = db.eventFeesRequest.Where(a => a.Confirmed != null && a.dateTime >= now.AddYears(-1)).OrderByDescending(a => a.dateTime).Join(db.Event, a => a.EventtId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                EventName = b.EventName,
                Title = a.Title,
                Value = a.Value,
                Note = a.Note,
                DateTime = a.dateTime.ToString("dd MMMM yyyy - hh:mm tt"),
                ExtendIdentityUserId = a.ExtendIdentityUserId,
                Confirmed = a.Confirmed
            }).Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new EventFeesSimpleModel
            {
                Id = a.Id,
                EventName = a.EventName,
                Title = a.Title,
                Value = a.Value,
                Note = a.Note,
                DateTime = a.DateTime,
                UserName = b.FullName,
                Confirmed = a.Confirmed
            }).ToList();

            return res;
        }

        public bool HoldRequest(int id)
        {
            using (IDbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {

                    EventFeesRequest efr = db.eventFeesRequest.Find(id);
                    bool? current = efr.Confirmed;
                    efr.Confirmed = null;

                    if (current == true)
                    {
                        EventFees ef = db.eventFees.Where(a => a.Title == efr.Title && a.ExtendIdentityUserId == efr.ExtendIdentityUserId && a.Note == efr.Note && a.Value == efr.Value && a.Extension == efr.Extension && a.ContentType == efr.ContentType).First();
                        db.eventFees.Remove(ef);
                    }

                   
                    db.SaveChanges();
                    transaction.Commit();
                    return true;
                }

                catch (Exception ex)
                {
                    var message = ex.InnerException;
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public bool RejectRequest(RejectExpenseRequestModel obj)
        {
            EventFeesRequest efr = db.eventFeesRequest.Find(obj.Id);
            efr.Confirmed = false;
            efr.RejectionReason = obj.Reason;
            db.SaveChanges();
            return true;
        }

        public bool RejectRequestAfterConfirm(RejectExpenseRequestModel obj)
        {


            using (IDbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                   
                    EventFeesRequest efr = db.eventFeesRequest.Find(obj.Id);
                    efr.Confirmed = false;
                    efr.RejectionReason = obj.Reason;
                    EventFees ef = db.eventFees.Where(a => a.Title == efr.Title && a.ExtendIdentityUserId == efr.ExtendIdentityUserId && a.Note == efr.Note && a.Value == efr.Value && a.Extension == efr.Extension && a.ContentType == efr.ContentType).First();
                    db.eventFees.Remove(ef);
                    db.SaveChanges();
                    transaction.Commit();
                    return true;
                }

                catch(Exception ex)
                {
                    var message = ex.InnerException;
                    transaction.Rollback();
                    return false;
                }
            }
            
        }
    }
}

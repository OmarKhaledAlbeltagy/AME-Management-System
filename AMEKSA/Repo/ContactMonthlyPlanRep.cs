using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.MonthlyPlanModels;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class ContactMonthlyPlanRep:IContactMonthlyPlanRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;
        
        public ContactMonthlyPlanRep(DbContainer db,ITimeRep ti)
        {
            this.db = db;
            this.ti = ti;
        }

        public bool DeletePlannedVisit(int id)
        {
            ContactMonthlyPlan del = db.contactMonthlyPlan.Find(id);
            db.contactMonthlyPlan.Remove(del);
            db.SaveChanges();
            return true;
        }

        public IEnumerable<ContactsForPlan> GetMyContactsForPlan(string userid)
        {
            DateTime now = ti.GetCurrentTime();
            DateTime s = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            DateTime e = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            DayOfWeek day = now.DayOfWeek;
            DateTime start = new DateTime();
            DateTime end = new DateTime();

            if (day == System.DayOfWeek.Monday)
            {
                start = s.AddDays(-2);
                end = e.AddDays(4);
            }
            else
            {
                if (day == System.DayOfWeek.Tuesday)
                {
                    start = s.AddDays(-3);
                    end = e.AddDays(3);
                }
                else
                {
                    if (day == System.DayOfWeek.Wednesday)
                    {
                        start = s.AddDays(-4);
                        end = e.AddDays(2);
                    }
                    else
                    {
                        if (day == System.DayOfWeek.Thursday)
                        {
                            start = s.AddDays(-5);
                            end = e.AddDays(1);
                        }
                        else
                        {
                            if (day == System.DayOfWeek.Friday)
                            {
                                start = s.AddDays(-6);
                                end = e;
                            }
                            else
                            {
                                if (day == System.DayOfWeek.Saturday)
                                {
                                    start = s;
                                    end = e.AddDays(6);
                                }
                                else
                                {
                                    if (day == System.DayOfWeek.Sunday)
                                    {
                                        start = s.AddDays(-1);
                                        end = e.AddDays(5);
                                    }
                                }
                            }
                        }
                    }
                }
            }



            List<Contact> con = db.userContact.Where(a => a.extendidentityuserid == userid).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            Contact
            {
                Id = b.Id,
                ContactName = b.ContactName,
                Gender = b.Gender
            }).ToList();

            List<Contact> conn = DistinctByExtension.DistinctBy(con, a => a.Id).OrderBy(b=>b.ContactName).ToList();

            List<ContactsForPlan> res = new List<ContactsForPlan>();

            foreach (var item in conn)
            {
                ContactMonthlyPlan plan = db.contactMonthlyPlan.Where(a => a.PlannedDate >= start && a.PlannedDate <= end && a.ExtendIdentityUserId == userid && a.ContactId == item.Id).FirstOrDefault();
                ContactsForPlan obj = new ContactsForPlan();
                obj.Id = item.Id;
                obj.ContactName = item.ContactName;
                if (plan == null)
                {
                    obj.Choosed = false;
                }
                else
                {
                    obj.Choosed = true;
                }
                res.Add(obj);

            }

            return res;
        }

        public IEnumerable<CustomMonthlyPlan> GetMyPlanThisMonth(string userId)
        {
            DateTime now = ti.GetCurrentTime();
            DateTime monthstart = now.AddMonths(-1);
            DateTime monthend = now.AddMonths(1);
            int monthenddays = DateTime.DaysInMonth(monthend.Year, monthend.Month);
            DateTime start = new DateTime(monthstart.Year, monthstart.Month, 1, 0, 0, 0);
            DateTime end = new DateTime(monthend.Year, monthend.Month, monthenddays, 23, 59, 59);
            List<ContactMonthlyPlan> myplans = db.contactMonthlyPlan.Where(a => a.ExtendIdentityUserId == userId && a.PlannedDate >= start && a.PlannedDate <= end).ToList();

            List<CustomMonthlyPlan> res = new List<CustomMonthlyPlan>();


            List<CustomMonthlyPlan> withaff = myplans.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                OrgId = b.Id,
                OrgName = b.ContactName,
                PlannedDate = a.PlannedDate,
                Status = a.Status,
                now = now,
                AccountId = b.AccountId
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomMonthlyPlan
            {
                Id = a.Id,
                OrgId = a.OrgId,
                OrgName = a.OrgName,
                PlannedDate = a.PlannedDate,
                Status = a.Status,
                now = a.now,
                Aff = b.AccountName
            }).ToList();

            List<CustomMonthlyPlan> withoutaff = myplans.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new CustomMonthlyPlan
            {
                Id = a.Id,
                OrgId = b.Id,
                OrgName = b.ContactName,
                PlannedDate = a.PlannedDate,
                Status = a.Status,
                now = now
            }).ToList();

            foreach (var item in withaff)
            {
                res.Add(item);
            }

            foreach (var item in withoutaff)
            {
                res.Add(item);
            }

            
            return DistinctByExtension.DistinctBy(res, a => a.Id).ToList();
        }

        public ContactMonthlyPlan PlanVisit(ContactMonthlyPlan obj)
        {
            ContactMonthlyPlan x = new ContactMonthlyPlan();
            x.ContactId = obj.ContactId;
            x.ExtendIdentityUserId = obj.ExtendIdentityUserId;
            x.PlannedDate = obj.PlannedDate;
            x.Date = obj.Date;
            db.contactMonthlyPlan.Add(x);
            db.SaveChanges();
            return x;
        }
    }
}

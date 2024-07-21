using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class TimeOffRep:ITimeOffRep
    {
        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly ITimeRep ti;
        private readonly IKpisRep kpisRep;

        public TimeOffRep(DbContainer db,UserManager<ExtendIdentityUser> userManager,ITimeRep ti,IKpisRep kpisRep)
        {
            this.db = db;
            this.userManager = userManager;
            this.ti = ti;
            this.kpisRep = kpisRep;
        }

        public bool AcceptVacancyRequest(int id)
        {
            VacancyRequests obj = db.vacancyRequests.Find(id);
            obj.Accepted = true;
            obj.Rejected = false;

            DateTime start = obj.VacancyDateTimeFrom;
            DateTime end = obj.VacancyDateTimeTo;
            DateTime x = start;
            do
            {
                if (x.DayOfWeek == DayOfWeek.Friday || x.DayOfWeek == DayOfWeek.Saturday)
                {
                    continue;
                }
                else
                {
                    UserTimeOff objj = new UserTimeOff();
                    objj.DateTimeFrom = x;
                    objj.DateTimeTo = new DateTime(x.Year, x.Month, x.Day, 23, 59, 59);
                    objj.ExtendIdentityUserId = obj.extendidentityuserid;
                    objj.TimeOffTerritoryReasonsId = obj.TimeOffTerritoryReasonsId;
                    objj.Description = obj.Description;
                    objj.Accepted = true;
                    x = x.AddDays(1);
                    db.userTimeOff.Add(objj);
                }
            
            } while (x <= end);

            Notifications n = new Notifications();
            n.NitificationDateTime = ti.GetCurrentTime();
            n.ExtendIdetityUserId = obj.extendidentityuserid;
            n.NotificationDetails = "Your vacancy request from " + obj.VacancyDateTimeFrom.ToString("dddd dd/MM/yyyy") + " to "+obj.VacancyDateTimeTo.ToString("dddd dd/MM/yyyy") + " has been accepted";
            n.Url = "Vacancies.html";
            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }

        public IEnumerable<CustomActualWorkingDays> CaculateActualWorkingDaysByMonthMedical(string userId)
        {
            DateTime thismonth = ti.GetCurrentTime();
            DateTime pastmonth = thismonth.AddMonths(-1);
         
            int? thisworkingdays = db.workingDays.Where(a => a.Month == thismonth.Month && a.Year == thismonth.Year).Select(a => a.NumberOfWorkingDays).SingleOrDefault();
            int? pastworkingdays = db.workingDays.Where(a => a.Month == pastmonth.Month && a.Year == pastmonth.Year).Select(a => a.NumberOfWorkingDays).SingleOrDefault();
            ExtendIdentityUser me = userManager.FindByIdAsync(userId).Result;
            int daysthismonth = DateTime.DaysInMonth(thismonth.Year, thismonth.Month);
            DateTime startthismonth = new DateTime(thismonth.Year, thismonth.Month, 1);
            DateTime endthismonth = new DateTime(thismonth.Year, thismonth.Month, daysthismonth);

            int dayspastmonth = DateTime.DaysInMonth(pastmonth.Year, pastmonth.Month);
            DateTime startpastmonth = new DateTime(pastmonth.Year, pastmonth.Month, 1);
            DateTime endpastmonth = new DateTime(pastmonth.Year, pastmonth.Month, dayspastmonth);

            List<CustomActualWorkingDays> res = new List<CustomActualWorkingDays>();
            CustomActualWorkingDays acc = new CustomActualWorkingDays();
            List<ContactMedicalVisit> visits = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= startthismonth && a.VisitDate <= endthismonth), a => a.VisitDate.Day).ToList();

            int UT = kpisRep.GetTimeOffDiff(userId, thismonth.Year, thismonth.Month, 1, thismonth.Year, thismonth.Month, daysthismonth);
            acc.ActualWorkingDays = visits.Count;
            acc.TimeOffTerritory = UT;
            acc.WorkingDays = thisworkingdays;
            acc.Role = 1;
            res.Add(acc);
            CustomActualWorkingDays accpast = new CustomActualWorkingDays();
            List<ContactMedicalVisit> visitspast = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= startpastmonth && a.VisitDate <= endpastmonth).ToList(), a => a.VisitDate.Day).ToList();
            int UTpast = kpisRep.GetTimeOffDiff(userId, pastmonth.Year, pastmonth.Month, 1, pastmonth.Year, pastmonth.Month, dayspastmonth);
            accpast.ActualWorkingDays = visitspast.Count;
            accpast.TimeOffTerritory = UTpast;
            accpast.WorkingDays = pastworkingdays;
            accpast.Role = 2;
            res.Add(accpast);
            return res;

        }

        public IEnumerable<CustomActualWorkingDays> CaculateActualWorkingDaysByMonthSales(string userId)
        {
            DateTime thismonth = ti.GetCurrentTime();
            DateTime pastmonth = thismonth.AddMonths(-1);
            

            int? thisworkingdays = db.workingDays.Where(a => a.Month == thismonth.Month && a.Year == thismonth.Year).Select(a => a.NumberOfWorkingDays).SingleOrDefault();
            int? pastworkingdays = db.workingDays.Where(a => a.Month == pastmonth.Month && a.Year == pastmonth.Year).Select(a => a.NumberOfWorkingDays).SingleOrDefault();
            ExtendIdentityUser me = userManager.FindByIdAsync(userId).Result;
            int daysthismonth = DateTime.DaysInMonth(thismonth.Year, thismonth.Month);
            DateTime startthismonth = new DateTime(thismonth.Year, thismonth.Month, 1);
            DateTime endthismonth = new DateTime(thismonth.Year, thismonth.Month, daysthismonth);

            int dayspastmonth = DateTime.DaysInMonth(pastmonth.Year, pastmonth.Month);
            DateTime startpastmonth = new DateTime(pastmonth.Year, pastmonth.Month, 1);
            DateTime endpastmonth = new DateTime(pastmonth.Year, pastmonth.Month, dayspastmonth);

            List<CustomActualWorkingDays> res = new List<CustomActualWorkingDays>();
            CustomActualWorkingDays acc = new CustomActualWorkingDays();
            List<AccountSalesVisit> visits = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= startthismonth && a.VisitDate <= endthismonth),a=>a.VisitDate.Day).ToList();

            int UT = kpisRep.GetTimeOffDiff(userId, thismonth.Year, thismonth.Month, 1, thismonth.Year, thismonth.Month, daysthismonth);
            acc.ActualWorkingDays = visits.Count;
            acc.TimeOffTerritory = UT;
            acc.WorkingDays = thisworkingdays;
            acc.Role = 1;
            res.Add(acc);
            CustomActualWorkingDays accpast = new CustomActualWorkingDays();
            List<AccountSalesVisit> visitspast = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= startpastmonth && a.VisitDate <= endpastmonth), a => a.VisitDate.Day).ToList();

            int UTpast = kpisRep.GetTimeOffDiff(userId, pastmonth.Year, pastmonth.Month, 1, pastmonth.Year, pastmonth.Month, dayspastmonth);
            accpast.ActualWorkingDays = visitspast.Count;
            accpast.TimeOffTerritory = UTpast;
            accpast.WorkingDays = pastworkingdays;
            accpast.Role = 2;
            res.Add(accpast);
            return res;
        }

        public IEnumerable<CustomActualWorkingDays> CaculateActualWorkingDaysByMonth(int year, int month, string managerId)
        {

            int? workingdays = db.workingDays.Where(a => a.Month == month && a.Year == year).Select(a => a.NumberOfWorkingDays).SingleOrDefault();
            IEnumerable<ExtendIdentityUser> medicals = userManager.GetUsersInRoleAsync("Medical Representative").Result;
            List<ExtendIdentityUser> sales = userManager.GetUsersInRoleAsync("Sales Representative").Result.ToList();
            List<string> subids = db.UserSubmanager.Where(a => a.ManagerId == managerId).Select(a => a.RepId).ToList();
            foreach (var item in subids)
            {
                ExtendIdentityUser subrep = userManager.FindByIdAsync(item).Result;
                sales.Add(subrep);
            }
            IEnumerable<ExtendIdentityUser> supportive = userManager.GetUsersInRoleAsync("Supportive").Result;
            IEnumerable<ExtendIdentityUser> myteammedical = medicals.Where(a => a.extendidentityuserid == managerId);
            IEnumerable<ExtendIdentityUser> myteamsales = sales.Where(a => a.extendidentityuserid == managerId);
            IEnumerable<ExtendIdentityUser> myteamsupportive = supportive.Where(a => a.extendidentityuserid == managerId);
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            List<CustomActualWorkingDays> res = new List<CustomActualWorkingDays>();

            foreach (var item in myteammedical)
            {
                List<ContactMedicalVisit> visits = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(a => a.VisitDate.Day).ToList();
                int UT = kpisRep.GetTimeOffDiff(item.Id, year, month, 1, year, month, days);
                CustomActualWorkingDays x = new CustomActualWorkingDays();
                x.ActualWorkingDays = visits.Count;
                x.FullName = item.FullName;
                x.TimeOffTerritory = UT;
                x.WorkingDays = workingdays;
                x.Role = 1;
                res.Add(x);
            }

            foreach (var item in myteamsales)
            {
                List<AccountSalesVisit> visits = db.accountSalesVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(a => a.VisitDate.Day).ToList();
                int UT = kpisRep.GetTimeOffDiff(item.Id, year, month, 1, year, month, days);
                CustomActualWorkingDays x = new CustomActualWorkingDays();
                x.ActualWorkingDays = visits.Count;
                x.FullName = item.FullName;
                x.TimeOffTerritory = UT;
                x.WorkingDays = workingdays;
                x.Role = 2;
                res.Add(x);
            }

            foreach (var item in myteamsupportive)
            {
                List<AccountSupportiveVisit> visits = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(a => a.VisitDate.Day).ToList();
                int UT = kpisRep.GetTimeOffDiff(item.Id, year, month, 1, year, month, days);
                CustomActualWorkingDays x = new CustomActualWorkingDays();
                x.ActualWorkingDays = visits.Count;
                x.FullName = item.FullName;
                x.TimeOffTerritory = UT;
                x.WorkingDays = workingdays;
                x.Role = 3;
                res.Add(x);
            }




            return res.OrderBy(a => a.FullName);
        }

        public IEnumerable<CustomActualWorkingDays> CaculateActualWorkingDaysByMonthTopManager(int year, int month)
        {
            int? workingdays = db.workingDays.Where(a => a.Month == month && a.Year == year).Select(a => a.NumberOfWorkingDays).SingleOrDefault();
           
            List<ExtendIdentityUser> medicals = userManager.GetUsersInRoleAsync("Medical Representative").Result.ToList();
            List<ExtendIdentityUser> sales = userManager.GetUsersInRoleAsync("Sales Representative").Result.ToList();
            List<ExtendIdentityUser> supportive = userManager.GetUsersInRoleAsync("Supportive").Result.ToList();

            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            List<CustomActualWorkingDays> res = new List<CustomActualWorkingDays>();

            foreach (var item in medicals)
            {
                List<ContactMedicalVisit> visits = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end),x=>x.VisitDate.Day).ToList();
                    
               
                int UT = kpisRep.GetTimeOffDiff(item.Id, year, month, 1, year, month, days);
                CustomActualWorkingDays x = new CustomActualWorkingDays();
                x.ActualWorkingDays = visits.Count;
                x.FullName = item.FullName;
                x.TimeOffTerritory = UT;
                x.WorkingDays = workingdays;
                x.Role = 1;
                res.Add(x);
            }

            foreach (var item in sales)
            { 
                List<AccountSalesVisit> visits = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end), a => a.VisitDate.Day).ToList();
                    
                int UT = kpisRep.GetTimeOffDiff(item.Id, year, month, 1, year, month, days);
                CustomActualWorkingDays x = new CustomActualWorkingDays();
                x.ActualWorkingDays = visits.Count;
                x.FullName = item.FullName;
                x.TimeOffTerritory = UT;
                x.WorkingDays = workingdays;
                x.Role = 2;
                res.Add(x);
            }

            foreach (var item in supportive)
            {
                List<AccountSupportiveVisit> visits = DistinctByExtension.DistinctBy(db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end), a => a.VisitDate.Day).ToList();
       
                int UT = kpisRep.GetTimeOffDiff(item.Id, year, month, 1, year, month, days);
                CustomActualWorkingDays x = new CustomActualWorkingDays();
                x.ActualWorkingDays = visits.Count;
                x.FullName = item.FullName;
                x.TimeOffTerritory = UT;
                x.WorkingDays = workingdays;
                x.Role = 3;
                res.Add(x);
            }



            return res.OrderBy(a => a.FullName);
        }

        public bool DeleteTimeOffTerritory(int id)
        {
            UserTimeOff obj = db.userTimeOff.Find(id);
            db.userTimeOff.Remove(obj);
            db.SaveChanges();
            return true;
        }

        public bool EditWorkingDays(int Id, int WorkingDays)
        {
            WorkingDays obj = db.workingDays.Find(Id);
            obj.NumberOfWorkingDays = WorkingDays;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<CustomTimeOffTerritory> GetAllTimeOff()
        {
            DateTime start = ti.GetCurrentTime().AddYears(-1);

            TimeSpan diff = new TimeSpan(5, 0, 0);
           

            List<CustomTimeOffTerritory> res = db.Users.Join(db.userTimeOff, a => a.Id, b => b.ExtendIdentityUserId, (a, b) => new
            {
                Name = a.FullName,
                DateFrom = b.DateTimeFrom,
                Dateto = b.DateTimeTo,
                ReasonId = b.TimeOffTerritoryReasonsId,
                Description = b.Description,
                Accepted = b.Accepted
            }).Where(a=>a.Accepted == true).Join(db.timeOffTerrirtoryReasons, a => a.ReasonId, b => b.Id, (a, b) => new CustomTimeOffTerritory
            {
                Id = b.Id,
                FullName = a.Name,
                DateFrom = a.DateFrom,
                DateTo = a.Dateto,
                Reason = b.Reason,
                Description = a.Description,
                Accepted = a.Accepted
            }).Where(a => a.DateFrom >= start).ToList();

            foreach (var item in res)
            {
                if (item.DateTo.TimeOfDay - item.DateFrom.TimeOfDay >= diff)
                {
                    item.status = true;
                }
                else
                {
                    if (item.DateTo.TimeOfDay - item.DateFrom.TimeOfDay < diff)
                    {
                        item.status = false;
                    }
                }
                
            }
         

            return res;
        }

        public IEnumerable<CustomTimeOffTerritory> GetMyTeamTimeOff(string ManagerId)
        {
            DateTime start = ti.GetCurrentTime().AddYears(-1);
            TimeSpan diff = new TimeSpan(0, 5, 0, 0, 0);
            
            List<ExtendIdentityUser> MyTeam = db.Users.Where(a => a.extendidentityuserid == ManagerId || a.Id == ManagerId).ToList();

            List<string> subteam = db.UserSubmanager.Where(a => a.ManagerId == ManagerId).Select(a => a.RepId).ToList();

            foreach (var item in subteam)
            {
                ExtendIdentityUser subrep = userManager.FindByIdAsync(item).Result;
                MyTeam.Add(subrep);
            }

            List<CustomTimeOffTerritory> res = MyTeam.Join(db.userTimeOff, a => a.Id, b => b.ExtendIdentityUserId, (a, b) => new
            {
                Name = a.FullName,
                DateFrom = b.DateTimeFrom,
                Dateto = b.DateTimeTo,
                ReasonId = b.TimeOffTerritoryReasonsId,
                Description = b.Description,
                Accepted = b.Accepted
            }).Where(a=>a.Accepted == true).Join(db.timeOffTerrirtoryReasons, a => a.ReasonId, b => b.Id, (a, b) => new CustomTimeOffTerritory
            {
                Id = b.Id,
                FullName = a.Name,
                DateFrom = a.DateFrom,
                DateTo = a.Dateto,
                Reason = b.Reason,
                Description = a.Description,
                Accepted = a.Accepted
            }).Where(a => a.DateFrom >= start).ToList();

            foreach (var item in res)
            {
                if (item.DateTo.TimeOfDay - item.DateFrom.TimeOfDay >= diff)
                {
                    item.status = true;
                }
                else
                {
                    if (item.DateTo.TimeOfDay - item.DateFrom.TimeOfDay < diff)
                    {
                        item.status = false;
                    }
                }

            }

            return res;
        }

        public IEnumerable<CustomVacancyRequests> GetMyTeamVacanciesRequests(string ManagerId)
        {
            DateTime now = ti.GetCurrentTime().Date.AddMonths(-3);
            List<ExtendIdentityUser> myteam = db.Users.Where(a => a.extendidentityuserid == ManagerId).ToList();
            List<string> mysubids = db.UserSubmanager.Where(a => a.ManagerId == ManagerId).Select(a => a.RepId).ToList();
            foreach (var item in mysubids)
            {
                ExtendIdentityUser subrep = userManager.FindByIdAsync(item).Result;
                myteam.Add(subrep);
            }
            IEnumerable<CustomVacancyRequests> res = myteam.Join(db.vacancyRequests, a => a.Id, b => b.extendidentityuserid, (a, b) => new
            {
                Id = b.Id,
                Description = b.Description,
                VacancyDateTimeFrom = b.VacancyDateTimeFrom,
                VacancyDateTimeTo = b.VacancyDateTimeTo,
                RepName = a.FullName,
                ReasonId = b.TimeOffTerritoryReasonsId,
                Accepted = b.Accepted,
                Rejected = b.Rejected
            }).Join(db.timeOffTerrirtoryReasons, a => a.ReasonId, b => b.Id, (a, b) => new CustomVacancyRequests
            {
                Id = a.Id,
                Description = a.Description,
                VacancyDateTimeFrom = a.VacancyDateTimeFrom,
                VacancyDateTimeTo = a.VacancyDateTimeTo,
                RepName = a.RepName,
                Reason = b.Reason,
                Accepted = a.Accepted,
                Rejected = a.Rejected
            });
            return res;
        }

        public IEnumerable<CustomTimeOffTerritory> GetMyTimeOffData(string userId)
        {
            IEnumerable<CustomTimeOffTerritory> res = db.userTimeOff.Join(db.timeOffTerrirtoryReasons, a => a.TimeOffTerritoryReasonsId, b => b.Id, (a, b) =>
            new CustomTimeOffTerritory
            {
                Id = a.Id,
                DateFrom = a.DateTimeFrom,
                DateTo = a.DateTimeTo,
                Reason = b.Reason,
                Description = a.Description,
                UserId = a.ExtendIdentityUserId,
                Accepted = a.Accepted
            }).Where(a => a.UserId == userId && a.Accepted == true);

            return res.OrderByDescending(a=>a.DateFrom);
        }

        public IEnumerable<DateTime> Getmytimesoff(string userId)
        {
            IEnumerable<UserTimeOff> UT = db.userTimeOff.Where(a => a.ExtendIdentityUserId == userId && a.Accepted == true);

            List<DateTime> res = new List<DateTime>();

            foreach (var item in UT)
            {
                DateTime x = item.DateTimeFrom;
                res.Add(x);

            }

            return res;
        }

        public IEnumerable<CustomVacancyRequests> GetMyVacancies(string userId)
        {
            DateTime start = ti.GetCurrentTime().AddYears(-1);

            List<VacancyRequests> myrequests = db.vacancyRequests.Where(a => a.VacancyDateTimeTo >= start && a.extendidentityuserid == userId).ToList();

            List<CustomVacancyRequests> res = myrequests.Join(db.timeOffTerrirtoryReasons, a => a.TimeOffTerritoryReasonsId, b => b.Id, (a, b) =>
            new CustomVacancyRequests
            {
                Id = a.Id,
                Accepted = a.Accepted,
                Description = a.Description,
                Reason = b.Reason,
                Rejected = a.Rejected,
                VacancyDateTimeFrom = a.VacancyDateTimeFrom,
                VacancyDateTimeTo = a.VacancyDateTimeTo
            }).ToList();

            return res;
        }

        public IEnumerable<TimeOffTerritoryReasons> GetTimeOffTerritoryReasons()
        {
            IEnumerable<TimeOffTerritoryReasons> res = db.timeOffTerrirtoryReasons.Select(a => a);
            return res;
        }

        public IEnumerable<TimeOffTerritoryReasons> GetVacancyReasons()
        {
            IEnumerable<TimeOffTerritoryReasons> res = db.timeOffTerrirtoryReasons.Where(a => a.Id == 1|| a.Id == 7);
            return res;
        }

        public IEnumerable<WorkingDaysModel> GetWorkingDaysSet()
        {
            IEnumerable<WorkingDays> list = db.workingDays.Where(a => a.NumberOfWorkingDays.HasValue);
            List<WorkingDaysModel> res = new List<WorkingDaysModel>();
            foreach (var item in list)
            { 
                string date = new DateTime(item.Year, item.Month, 1).ToString("yyyy-MM");
                int? workingdays = item.NumberOfWorkingDays;
                WorkingDaysModel obj = new WorkingDaysModel();
                obj.Id = item.Id;
                obj.Month = date;
                obj.WorkingDays = workingdays;
                res.Add(obj);
            }
            return res.OrderByDescending(a=>a.Month);
        }

        public TimeOffMail MakeTimeOffTerritory(UserTimeOff obj)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(obj.ExtendIdentityUserId).Result;
            string UserRole = userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if (UserRole == "Medical Representative" || UserRole == "Sales Representative")
            {
                obj.Accepted = null;
                Notifications n = new Notifications();
                n.ExtendIdetityUserId = user.extendidentityuserid;
                n.NitificationDateTime = ti.GetCurrentTime();
                n.NotificationDetails = user.FullName + " requested time off territory on " + obj.DateTimeFrom.ToString("dddd dd/MM/yyyy");
                n.Url = "TimeOffRequests.html";
                db.notifications.Add(n);
            }
            else
            {

                if (UserRole == "First Line Manager" || UserRole == "System Admin" || UserRole == "System Adminn")
                {
                    obj.Accepted = null;
                    IEnumerable<string> topids = userManager.GetUsersInRoleAsync("Top Line Manager").Result.Select(a => a.Id);
                    foreach (var item in topids)
                    {
                        Notifications n = new Notifications();
                        n.ExtendIdetityUserId = item;
                        n.NitificationDateTime = ti.GetCurrentTime();
                        n.NotificationDetails = user.FullName + " requested time off territory on " + obj.DateTimeFrom.ToString("dddd dd/MM/yyyy");
                        n.Url = "TimeOffRequests.html";
                        db.notifications.Add(n);
                    }
                }
                else
                {
                    if (UserRole == "Top Line Manager")
                    {
                        obj.Accepted = true;
                    }
                }
               
            }
            db.userTimeOff.Add(obj);
            db.SaveChanges();
            TimeOffMail res = new TimeOffMail();
            res.Id = obj.Id;
            res.FullName = userManager.FindByIdAsync(obj.ExtendIdentityUserId).Result.FullName;
            res.UserId = obj.ExtendIdentityUserId;
            res.Date = obj.DateTimeFrom.ToString("dd MMMM yyyy");
            res.TimeFrom = obj.DateTimeFrom.ToString("hh:mm tt");
            res.TimeTo = obj.DateTimeTo.ToString("hh:mm tt");
            res.Reason = db.timeOffTerrirtoryReasons.Where(a => a.Id == obj.TimeOffTerritoryReasonsId).FirstOrDefault().Reason;
            res.Description = obj.Description;
            return res;
        }

        public bool MakeVacancyRequest(VacancyRequests obj)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(obj.extendidentityuserid).Result;
            Notifications n = new Notifications();
            n.NitificationDateTime = ti.GetCurrentTime();
            n.ExtendIdetityUserId = user.extendidentityuserid;
            n.NotificationDetails = user.FullName + " requested vacancy on " + obj.VacancyDateTimeFrom.ToString("dddd dd/MM/yyyy");
            n.Url = "vacancyrequest.html";
            db.notifications.Add(n);
            db.vacancyRequests.Add(obj);
            db.SaveChanges();
            return true;
        }

        public bool RejectVacancyRequest(int id)
        {

            VacancyRequests obj = db.vacancyRequests.Find(id);
            obj.Accepted = false;
            obj.Rejected = true;
            Notifications n = new Notifications();
            n.NitificationDateTime = ti.GetCurrentTime();
            n.ExtendIdetityUserId = obj.extendidentityuserid;
            n.NotificationDetails = "Your vacancy request from " + obj.VacancyDateTimeFrom.ToString("dddd dd/MM/yyyy") + " to " + obj.VacancyDateTimeTo.ToString("dddd dd/MM/yyyy") + " has been rejected";
            n.Url = "Vacancies.html";
            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }

        public bool SetWorkingDays(int month, int year, int workingdays)
        {
            WorkingDays obj = db.workingDays.Where(a => a.Month == month && a.Year == year).SingleOrDefault();
            obj.NumberOfWorkingDays = workingdays;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<CustomTimeOffTerritory> GetTimeOffRequestsToTakeAction(string userId)
        {
            DateTime start = ti.GetCurrentTime().AddMonths(-1).Date;
            DateTime end = ti.GetCurrentTime().AddMonths(3).Date;
            ExtendIdentityUser user = userManager.FindByIdAsync(userId).Result;
            string role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
            IEnumerable<CustomTimeOffTerritory> result = new List<CustomTimeOffTerritory>();
            if (role == "Top Line Manager")
            {
                IEnumerable<ExtendIdentityUser> FirstManagers = userManager.GetUsersInRoleAsync("First Line Manager").Result;
                 result = FirstManagers.Join(db.userTimeOff, a => a.Id, b => b.ExtendIdentityUserId, (a, b) => new
                {
                    Id = b.Id,
                    DateFrom = b.DateTimeFrom,
                    DateTo = b.DateTimeTo,
                    TimeOffTerritoryReasonsId = b.TimeOffTerritoryReasonsId,
                    Description = b.Description,
                    UserId = b.ExtendIdentityUserId,
                    FullName = a.FullName,
                    Accepted = b.Accepted
                }).Where(a => a.Accepted == null && a.DateFrom >= start && a.DateFrom <= end).Join(db.timeOffTerrirtoryReasons, a => a.TimeOffTerritoryReasonsId, b => b.Id, (a, b) => new CustomTimeOffTerritory
                {
                    Id = a.Id,
                    DateFrom = a.DateFrom,
                    DateTo = a.DateTo,
                    Reason = b.Reason,
                    Description = a.Description,
                    UserId = a.UserId,
                    FullName = a.FullName
                }).OrderBy(a=>a.DateFrom);
         
            }
            else
            {
                if (role == "First Line Manager")
                {
                    List<ExtendIdentityUser> MyTeam = new List<ExtendIdentityUser>();
                    List<ExtendIdentityUser> Medical = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a=>a.extendidentityuserid == userId).ToList();
                    List<ExtendIdentityUser> Sales = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.extendidentityuserid == userId).ToList();
                    List<ExtendIdentityUser> Supportive = userManager.GetUsersInRoleAsync("Supportive").Result.Where(a => a.extendidentityuserid == userId).ToList();
                    List<string> subids = db.UserSubmanager.Where(a => a.ManagerId == userId).Select(a => a.RepId).ToList();

                    foreach (var item in Medical)
                    {
                        MyTeam.Add(item);
                    }
                    foreach (var item in Sales)
                    {
                        MyTeam.Add(item);
                    }
                    foreach (var item in Supportive)
                    {
                        MyTeam.Add(item);
                    }
                    foreach (var item in subids)
                    {
                        ExtendIdentityUser subrep = userManager.FindByIdAsync(item).Result;
                        MyTeam.Add(subrep);
                    }

                    result = MyTeam.Join(db.userTimeOff, a => a.Id, b => b.ExtendIdentityUserId, (a, b) => new
                    {
                        Id = b.Id,
                        DateFrom = b.DateTimeFrom,
                        DateTo = b.DateTimeTo,
                        TimeOffTerritoryReasonsId = b.TimeOffTerritoryReasonsId,
                        Description = b.Description,
                        UserId = b.ExtendIdentityUserId,
                        FullName = a.FullName,
                        Accepted = b.Accepted
                    }).Where(a => a.Accepted == null && a.DateFrom >= start && a.DateFrom <= end).Join(db.timeOffTerrirtoryReasons, a => a.TimeOffTerritoryReasonsId, b => b.Id, (a, b) => new CustomTimeOffTerritory
                    {
                        Id = a.Id,
                        DateFrom = a.DateFrom,
                        DateTo = a.DateTo,
                        Reason = b.Reason,
                        Description = a.Description,
                        UserId = a.UserId,
                        FullName = a.FullName
                    }).OrderBy(a => a.DateFrom);
                  

                }
            }

            return result;

        }

        public IEnumerable<CustomTimeOffTerritory> GetMyTimeOffStatus(string userId)
        {
            DateTime start = ti.GetCurrentTime().AddMonths(-1).Date;
            DateTime end = ti.GetCurrentTime().AddMonths(2).Date;
            IEnumerable<CustomTimeOffTerritory> result = db.userTimeOff.Where(a => a.ExtendIdentityUserId == userId && a.DateTimeFrom >= start && a.DateTimeFrom <= end).Join(db.timeOffTerrirtoryReasons, a => a.TimeOffTerritoryReasonsId, b => b.Id, (a, b) => new CustomTimeOffTerritory
            {
                Id = a.Id,
                DateFrom = a.DateTimeFrom,
                DateTo = a.DateTimeTo,
                Reason = b.Reason,
                Description = a.Description,
                Accepted = a.Accepted
            }).OrderBy(a => a.DateFrom);
            return result;
        }

        public bool AcceptTimeOff(int id)
        {
            UserTimeOff UTF = db.userTimeOff.Find(id);
            UTF.Accepted = true;

            Notifications n = new Notifications();
            n.NitificationDateTime = ti.GetCurrentTime();
            n.ExtendIdetityUserId = UTF.ExtendIdentityUserId;
            n.NotificationDetails = "Your time off territory request on " + UTF.DateTimeFrom.ToString("dddd dd/MM/yyyy") + " has been accepted";
            n.Url = "MyTimeOffTerritory.html";
            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }

        public bool RejectTimeOff(int id)
        {
            UserTimeOff UTF = db.userTimeOff.Find(id);
            UTF.Accepted = false;
            Notifications n = new Notifications();
            n.NitificationDateTime = ti.GetCurrentTime();
            n.ExtendIdetityUserId = UTF.ExtendIdentityUserId;
            n.NotificationDetails = "Your time off territory request on " + UTF.DateTimeFrom.ToString("dddd dd/MM/yyyy") + " has been rejected";
            n.Url = "MyTimeOffTerritory.html";
            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }

        public async Task mail(TimeOffMail res)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(res.UserId).Result;
            string role = userManager.GetRolesAsync(user).Result.FirstOrDefault();

            if (role == "Medical Representative" || role == "Sales Representative")
            {
                string mail = userManager.FindByIdAsync(user.extendidentityuserid).Result.Email;
                string body = "<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n\r\n    <link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css\" rel=\"stylesheet\" integrity=\"sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3\" crossorigin=\"anonymous\">\r\n\r\n    <title>Time Off Request</title>\r\n</head>\r\n<body>\r\n\r\n\r\n\r\n    <div align=\"center\">\r\n\r\n        <h1><span style=\"color:#0d6efd ;\">[Name]</span> wants to have a time off territory</h1>\r\n\r\n        <div class=\"tabel-responsive\">\r\n            <table class=\"table table-borderless text-center\">\r\n                <thead>\r\n                    <tr><th>Date</th><th>[Date]</th></tr>\r\n                    <tr><th>From</th><th>[From]</th></tr>\r\n                    <tr><th>To</th><th>[To]</th></tr>\r\n                    <tr><th>Reason</th><th>[Reason]</th></tr>\r\n                    <tr><th>Description</th><th>[Description]</th></tr>\r\n                </thead>\r\n            </table>\r\n        </div>\r\n        <br><br>\r\n        <a href=\"https://{{DashboardURL}}/AcceptTimeOffRequest.html?[Id]\" role=\"button\" class=\"btn btn-success btn-lg\">Accept</a>&nbsp; &nbsp; &nbsp; &nbsp; <a href=\"https://{{DashboardURL}}/RejectTimeOffRequest.html?[Id]\" role=\"button\" class=\"btn btn-danger btn-lg\">Reject</a>\r\n    </div>\r\n\r\n\r\n\r\n    <script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js\" integrity=\"sha384-ka7Sk0Gln4gmtz2MlQnikT1wXgYsOg+OMhuP+IlRH9sENBO0LRn5q+8nbTov4+1p\" crossorigin=\"anonymous\"></script>\r\n    <script src=\"https://cdn.jsdelivr.net/npm/@popperjs/core@2.10.2/dist/umd/popper.min.js\" integrity=\"sha384-7+zCNj/IqJ95wo16oMtfsKbZ9ccEh31eOz1HGyDuCQ6wgnyJNSYdrPa03rtR1zdB\" crossorigin=\"anonymous\"></script>\r\n    <script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.min.js\" integrity=\"sha384-QJHtvGhmr9XOIpI6YVutG+2QOK9T+ZnN4kzFN1RtK3zEFEIsxhlmWl5/YESvpZ13\" crossorigin=\"anonymous\"></script>\r\n    <script src=\"https://code.jquery.com/jquery-3.6.0.min.js\" integrity=\"sha256-/xUj+3OJU5yExlq6GSYGSHk7tPXikynS7ogEvDej/m4=\" crossorigin=\"anonymous\"></script>\r\n    <script>\r\n        (function (document, window, $) {\r\n            'use strict';\r\n            $(document).ready(function () {\r\n\r\n\r\n\r\n            });\r\n\r\n\r\n        })(document, window, jQuery);\r\n    </script>\r\n</body>\r\n</html>";
           
                body = body.Replace("[Id]", res.Id.ToString()).Replace("[Name]", res.FullName).Replace("[Date]", res.Date).Replace("[From]", res.TimeFrom).Replace("[To]", res.TimeTo).Replace("[Reason]", res.Reason).Replace("[Description]", res.Description);

                MailMessage m = new MailMessage();
                m.To.Add(mail);
                m.Subject = res.FullName+" Time Off Territory Request on "+res.Date;
                m.From = new MailAddress(EmailModel.EmailAddress);
                m.Sender = new MailAddress(EmailModel.EmailAddress);
                m.To.Add(mail);
                m.Body = body;
                m.IsBodyHtml = true;
                m.Priority = MailPriority.High;
                SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
                smtp.Send(m);
            }
            else
            {
                if (role == "First Line Manager")
                {
                    IEnumerable<string> mail = userManager.GetUsersInRoleAsync("Top Line Manager").Result.Select(a => a.Email);

                    string body = "<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n\r\n    <link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css\" rel=\"stylesheet\" integrity=\"sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3\" crossorigin=\"anonymous\">\r\n\r\n    <title>Time Off Request</title>\r\n</head>\r\n<body>\r\n\r\n\r\n\r\n    <div align=\"center\">\r\n\r\n        <h1><span style=\"color:#0d6efd ;\">[Name]</span> wants to have a time off territory</h1>\r\n\r\n        <div class=\"tabel-responsive\">\r\n            <table class=\"table table-borderless text-center\">\r\n                <thead>\r\n                    <tr><th>Date</th><th>[Date]</th></tr>\r\n                    <tr><th>From</th><th>[From]</th></tr>\r\n                    <tr><th>To</th><th>[To]</th></tr>\r\n                    <tr><th>Reason</th><th>[Reason]</th></tr>\r\n                    <tr><th>Description</th><th>[Description]</th></tr>\r\n                </thead>\r\n            </table>\r\n        </div>\r\n        <br><br>\r\n        <a href=\"https://{{DashboardURL}}/AcceptTimeOffRequest.html?[Id]\" role=\"button\" class=\"btn btn-success btn-lg\">Accept</a>&nbsp; &nbsp; &nbsp; &nbsp; <a href=\"https://{{DashboardURL}}/RejectTimeOffRequest.html?[Id]\" role=\"button\" class=\"btn btn-danger btn-lg\">Reject</a>\r\n    </div>\r\n\r\n\r\n\r\n    <script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js\" integrity=\"sha384-ka7Sk0Gln4gmtz2MlQnikT1wXgYsOg+OMhuP+IlRH9sENBO0LRn5q+8nbTov4+1p\" crossorigin=\"anonymous\"></script>\r\n    <script src=\"https://cdn.jsdelivr.net/npm/@popperjs/core@2.10.2/dist/umd/popper.min.js\" integrity=\"sha384-7+zCNj/IqJ95wo16oMtfsKbZ9ccEh31eOz1HGyDuCQ6wgnyJNSYdrPa03rtR1zdB\" crossorigin=\"anonymous\"></script>\r\n    <script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.min.js\" integrity=\"sha384-QJHtvGhmr9XOIpI6YVutG+2QOK9T+ZnN4kzFN1RtK3zEFEIsxhlmWl5/YESvpZ13\" crossorigin=\"anonymous\"></script>\r\n    <script src=\"https://code.jquery.com/jquery-3.6.0.min.js\" integrity=\"sha256-/xUj+3OJU5yExlq6GSYGSHk7tPXikynS7ogEvDej/m4=\" crossorigin=\"anonymous\"></script>\r\n    <script>\r\n        (function (document, window, $) {\r\n            'use strict';\r\n            $(document).ready(function () {\r\n\r\n\r\n\r\n            });\r\n\r\n\r\n        })(document, window, jQuery);\r\n    </script>\r\n</body>\r\n</html>";

                    body = body.Replace("[Id]", res.Id.ToString()).Replace("[Name]", res.FullName).Replace("[Date]", res.Date).Replace("[From]", res.TimeFrom).Replace("[To]", res.TimeTo).Replace("[Reason]", res.Reason).Replace("[Description]", res.Description);

                    MailMessage m = new MailMessage();
                    foreach (var item in mail)
                    {
                        m.To.Add(item);
                    }
                    
                    m.Subject = res.FullName + " Time Off Territory Request on " + res.Date;
                    m.From = new MailAddress(EmailModel.EmailAddress);
                    m.Sender = new MailAddress(EmailModel.EmailAddress);
                    m.Body = body;
                    m.IsBodyHtml = true;
                    m.Priority = MailPriority.High;
                    SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
                    smtp.EnableSsl = false;
                    smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
                    smtp.Send(m);
                }
                else
                {
                    return;
                }
            }
           
            
        }

        public TimeOffMail GetTimeOffById(int id)
        {
            UserTimeOff x = db.userTimeOff.Find(id);
            TimeOffMail res = new TimeOffMail();
            res.Id = id;
            res.UserId = x.ExtendIdentityUserId;
            res.FullName = userManager.FindByIdAsync(x.ExtendIdentityUserId).Result.FullName;
            res.Date = x.DateTimeFrom.ToString("dddd dd MMMM yyyy");
            res.TimeFrom = x.DateTimeFrom.ToString("hh:mm tt");
            res.TimeTo = x.DateTimeTo.ToString("hh:mm tt");
            res.Reason = db.timeOffTerrirtoryReasons.Find(x.TimeOffTerritoryReasonsId).Reason;
            res.Description = x.Description;
            return res;
        }
      
    }
}

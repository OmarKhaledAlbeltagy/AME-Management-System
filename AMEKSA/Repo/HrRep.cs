using AMEKSA.Context;
using AMEKSA.Models;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class HrRep:IHrRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public HrRep(DbContainer db, ITimeRep ti, UserManager<ExtendIdentityUser> userManager)
        {
            this.db = db;
            this.ti = ti;
            this.userManager = userManager;
        }

        public IEnumerable<DateTime> GetActualWorkingDaysMedical(string userId)
        {
            DateTime now = ti.GetCurrentTime();
            DateTime yearstart = now.AddYears(-1);
            int days = DateTime.DaysInMonth(now.Year, now.Month);
            DateTime start = new DateTime(yearstart.Year, yearstart.Month, 1, 0, 0, 0);
            DateTime end = new DateTime(now.Year, now.Month, days, 23, 59, 59);
            List<DateTime> res = db.contactMedicalVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end).ToList().DistinctBy(x => x.VisitDate.DayOfYear).Select(a => a.VisitDate).ToList();
            return res;
        }

        public IEnumerable<DateTime> GetActualWorkingDaysSales(string userId)
        {
            DateTime now = ti.GetCurrentTime();
            DateTime yearstart = now.AddYears(-1);
            int days = DateTime.DaysInMonth(now.Year, now.Month);
            DateTime start = new DateTime(yearstart.Year, yearstart.Month, 1, 0, 0, 0);
            DateTime end = new DateTime(now.Year, now.Month, days, 23, 59, 59);
            List<DateTime> res = db.accountSalesVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end).ToList().DistinctBy(x => x.VisitDate.DayOfYear).Select(a => a.VisitDate).ToList();
            return res;
        }

        public IEnumerable<ExtendIdentityUser> GetAllMedicalRep()
        {
            IEnumerable<ExtendIdentityUser> res = userManager.GetUsersInRoleAsync("Medical Representative").Result;

            return res.OrderBy(a=>a.FullName);
        }

        public IEnumerable<ExtendIdentityUser> GetAllSalesRep()
        {
            IEnumerable<ExtendIdentityUser> res = userManager.GetUsersInRoleAsync("Sales Representative").Result;

            return res.OrderBy(a => a.FullName);
        }

        public TenClockReportModel GetTenClockReport()
        {
            DateTime now = ti.GetCurrentTime();
            DateTime start = new DateTime();
        
            switch(now.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    start = now.AddDays(-3);
                    break;
                case DayOfWeek.Monday:
                    start = now.AddDays(-1);
                    break;
                case DayOfWeek.Tuesday:
                    start = now.AddDays(-1);
                    break;
                case DayOfWeek.Wednesday:
                    start = now.AddDays(-1);
                    break;
                case DayOfWeek.Thursday:
                    start = now.AddDays(-1);
                    break;
                case DayOfWeek.Friday:
                    start = now.AddDays(-1);
                    break;
                case DayOfWeek.Saturday:
                    start = now.AddDays(-2);
                    break;
            }

            List<ExtendIdentityUser> MedicalNotReported = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a=>a.Active == true).ToList();
            List<ExtendIdentityUser> MedicalReported = new List<ExtendIdentityUser>();

            List<string> medicalreportedids = db.contactMedicalVisit.Where(a => a.VisitDate.Date == start.Date).Select(y => y.extendidentityuserid).Distinct().ToList();

            foreach (var item in medicalreportedids)
            {
                ExtendIdentityUser medical = userManager.FindByIdAsync(item).Result;

                MedicalNotReported.Remove(medical);
                MedicalReported.Add(medical);
            }

            List<ExtendIdentityUser> SalesNotReported = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.Active == true).ToList();
            List<ExtendIdentityUser> SalesReported = new List<ExtendIdentityUser>();

            List<string> salesreportedids = db.accountSalesVisit.Where(a => a.VisitDate.Date == start.Date).Select(y => y.extendidentityuserid).Distinct().ToList();

            foreach (var item in salesreportedids)
            {
                ExtendIdentityUser sales = userManager.FindByIdAsync(item).Result;

                SalesNotReported.Remove(sales);
                SalesReported.Add(sales);
            }

            TenClockReportModel res = new TenClockReportModel();
            res.MedicalReported = MedicalReported.OrderBy(a=>a.FullName);
            res.MedicalNotReported = MedicalNotReported.OrderBy(a => a.FullName);
            res.SalesReported = SalesReported.OrderBy(a => a.FullName);
            res.SalesNotReported = SalesNotReported.OrderBy(a => a.FullName);
            return res;

        }

        public IEnumerable<DateTime> GetTimeOffDays(string userId)
        {
            DateTime now = ti.GetCurrentTime();
            DateTime yearstart = now.AddYears(-1);
            int days = DateTime.DaysInMonth(now.Year, now.Month);
            DateTime start = new DateTime(yearstart.Year, yearstart.Month, 1, 0, 0, 0);
            DateTime end = new DateTime(now.Year, now.Month, days, 23, 59, 59);
            
            IEnumerable<DateTime> res = db.userTimeOff.Where(a => a.ExtendIdentityUserId == userId && a.DateTimeFrom >= start && a.DateTimeFrom <= end).Select(a => a.DateTimeFrom);

            return res;
        }
    }
}

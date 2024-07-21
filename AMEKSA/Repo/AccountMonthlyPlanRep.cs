using AMEKSA.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Entities;
using AMEKSA.MonthlyPlanModels;
using AMEKSA.Models;

namespace AMEKSA.Repo
{
    public class AccountMonthlyPlanRep:IAccountMonthlyPlanRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;

        public AccountMonthlyPlanRep(DbContainer db, ITimeRep ti)
        {
            this.db = db;
            this.ti = ti;
        }

        public bool DeletePlannedVisit(int id)
        {
            List<AccountMonthlyPlanCollection> col = db.accountMonthlyPlanCollection.Where(a => a.AccountMonthlyPlanId == id).ToList();
            if (col != null)
            {
                foreach (var item in col)
                {
                    
                    db.accountMonthlyPlanCollection.Remove(item);
                }
            }
 
            AccountMonthlyPlan del = db.accountMonthlyPlan.Find(id);
            db.accountMonthlyPlan.Remove(del);
            db.SaveChanges();
            return true;
        }

        public bool DeletePlannedVisitSales(int id)
        {
            AccountMonthlyPlan del = db.accountMonthlyPlan.Find(id);
            IEnumerable<AccountMonthlyPlanCollection> col = db.accountMonthlyPlanCollection.Where(a => a.AccountMonthlyPlanId == id);
            foreach (var item in col)
            {
                db.accountMonthlyPlanCollection.Remove(item);
            }
            db.accountMonthlyPlan.Remove(del);
            db.SaveChanges();
            return true;
        }

        public IEnumerable<AccountsForPlan> GetMyAccountsForPlan(string userid)
        {
            DateTime now = ti.GetCurrentTime();
            int days = DateTime.DaysInMonth(now.Year, now.Month);
            DateTime start = new DateTime(now.Year, now.Month, 1);
            DateTime end = new DateTime(now.Year, now.Month, days);
            List<Account> acc = db.userAccount.Where(a => a.extendidentityuserid == userid).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            Account
            {
                Id = b.Id,
                AccountName = b.AccountName,
                AccountTypeId = b.AccountTypeId,
                Address = b.Address,
                BestTimeFrom = b.BestTimeFrom,
                BestTimeTo = b.BestTimeTo,
                CategoryId = b.CategoryId,
                DistrictId = b.DistrictId,
                Email = b.Email,
                NumberOfDoctors = b.NumberOfDoctors,
                PaymentNote = b.PaymentNote,
                PhoneNumber = b.PhoneNumber,
                PurchaseTypeId = b.PurchaseTypeId,
                RelationshipNote = b.RelationshipNote
            }).OrderBy(b => b.AccountName).ToList();

            List<AccountsForPlan> res = new List<AccountsForPlan>();
            
            foreach (var item in acc)
            {
                AccountMonthlyPlan plan = db.accountMonthlyPlan.Where(a => a.PlannedDate >= start && a.PlannedDate <= end && a.ExtendIdentityUserId == userid && a.AccountId == item.Id).FirstOrDefault();
                AccountsForPlan obj = new AccountsForPlan();
                obj.Id = item.Id;
                obj.AccountName = item.AccountName;
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
            List<AccountMonthlyPlan> myplans = db.accountMonthlyPlan.Where(a => a.ExtendIdentityUserId == userId && a.PlannedDate >= start && a.PlannedDate <= end).ToList();

            List<CustomMonthlyPlan> res = myplans.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomMonthlyPlan
            {
                Id = a.Id,
                OrgId = b.Id,
                OrgName = b.AccountName,
                PlannedDate = a.PlannedDate,
                Status = a.Status,
                now = now
            }).ToList();
            return res;
        }

        public IEnumerable<CustomMonthlyPlanSales> GetMyPlanThisMonthSales(string userId)
        {
            DateTime now = ti.GetCurrentTime();

            DateTime monthstart = now.AddMonths(-1);
            DateTime monthend = now.AddMonths(1);
            int monthenddays = DateTime.DaysInMonth(monthend.Year, monthend.Month);
            DateTime start = new DateTime(monthstart.Year, monthstart.Month, 1, 0, 0, 0);
            DateTime end = new DateTime(monthend.Year, monthend.Month, monthenddays, 23, 59, 59);
            List<AccountMonthlyPlan> myplans = db.accountMonthlyPlan.Where(a => a.ExtendIdentityUserId == userId && a.PlannedDate >= start && a.PlannedDate <= end).ToList();

            List<CustomMonthlyPlanSales> res = myplans.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomMonthlyPlanSales
            {
                Id = a.Id,
                OrgId = b.Id,
                OrgName = b.AccountName,
                PlannedDate = a.PlannedDate,
                Status = a.Status,
                now = now,
                customMonthlyPlanCollection = db.accountMonthlyPlanCollection.Where(x => x.AccountMonthlyPlanId == a.Id).Join(db.brand, c => c.BrandId, d => d.Id, (c, d) => new CustomMonthlyPlanCollection { MonthlyPlanId = c.Id, BrandName = d.BrandName, Collection = c.Collection }).ToList()
            }).ToList();

            return res;
        }


     

        public AccountMonthlyPlan PlanVisit(AccountMonthlyPlan obj)
        {
            
            AccountMonthlyPlan x = new AccountMonthlyPlan();
            x.AccountId = obj.AccountId;
            x.ExtendIdentityUserId = obj.ExtendIdentityUserId;
            x.PlannedDate = obj.PlannedDate;
            x.Date = obj.Date;
            db.accountMonthlyPlan.Add(x);
            db.SaveChanges();
            return x;
        }

        public int PlanVisitSales(AccountMonthlyPlan obj)
        {
            AccountMonthlyPlan x = new AccountMonthlyPlan();
            x.AccountId = obj.AccountId;
            x.ExtendIdentityUserId = obj.ExtendIdentityUserId;
            x.PlannedDate = obj.PlannedDate;
            x.Date = obj.Date;
            db.accountMonthlyPlan.Add(x);
        
            foreach (var item in obj.accountMonthlyPlanCollection)
            {
                AccountMonthlyPlanCollection y = new AccountMonthlyPlanCollection();
                y.AccountMonthlyPlanId = x.Id;
                y.BrandId = item.BrandId;
                y.Collection = item.Collection;
                db.accountMonthlyPlanCollection.Add(y);
            }
            db.SaveChanges();
            return x.Id;
        }

        public int PlanVisitSupportive(AccountMonthlyPlan obj)
        {
            AccountMonthlyPlan x = new AccountMonthlyPlan();
            x.AccountId = obj.AccountId;
            x.ExtendIdentityUserId = obj.ExtendIdentityUserId;
            x.PlannedDate = obj.PlannedDate;
            x.Date = obj.Date;
            db.accountMonthlyPlan.Add(x);
            db.SaveChanges();
            return x.Id;
        }
    }
}

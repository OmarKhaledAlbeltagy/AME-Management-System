using AMEKSA.Context;
using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Models; 
namespace AMEKSA.Repo
{
    public class CategoryRep:ICategoryRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;

        public CategoryRep(DbContainer db,ITimeRep ti)
        {
            this.db = db;
            this.ti = ti;
        }

        public bool categ()
        {
            IEnumerable<Account> c = db.account.Select(a=>a);
            foreach (var item in c)
            {
                item.AccountName = item.AccountName.ToUpper();
            }
            db.SaveChanges();
            return true;
        }

        public bool date()
        {
            DateTime from = new DateTime(2021, 9, 1);
            IEnumerable<AccountMonthlyPlan> A = db.accountMonthlyPlan.Where(a => a.Date >= from && a.Status == false);
            IEnumerable<ContactMonthlyPlan> C = db.contactMonthlyPlan.Where(a => a.Date >= from && a.Status == false);
            IEnumerable<AccountSalesVisit> AS = db.accountSalesVisit.Where(a => a.VisitDate >= from);
            IEnumerable<AccountMedicalVisit> AM = db.accountMedicalVisit.Where(a => a.VisitDate >= from);
            IEnumerable<ContactMedicalVisit> CM = db.contactMedicalVisit.Where(a => a.VisitDate >= from);
            foreach (var item in A)
            {
                foreach (var S in AS)
                {
                    if (item.ExtendIdentityUserId == S.extendidentityuserid && item.AccountId == S.AccountId && item.Date == S.VisitDate)
                    {
                        item.Status = true;
                        item.AccountSalesVisitId = S.Id;
                    }
                }
                foreach (var M in AM)
                {
                    if (item.ExtendIdentityUserId == M.extendidentityuserid && item.AccountId == M.AccountId && item.Date == M.VisitDate)
                    {
                        item.Status = true;
                        item.AccountMedicalVisitId = M.Id;
                    }
                }
            }
            foreach (var item in C)
            {
                foreach (var M in CM)
                {
                    if (item.ExtendIdentityUserId == M.extendidentityuserid && item.ContactId == M.ContactId && item.Date == M.VisitDate)
                    {
                        item.Status = true;
                        item.ContactMedicalVisitId = M.Id;
                    }
                }
            }
            db.SaveChanges();
            return true;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return db.category.Select(a => a);
        }

        public DateTime GetTimeNow()
        {
            return ti.GetCurrentTime();
        }

        public IEnumerable<Contact> nonocateg()
        {
            IEnumerable<Contact> c = db.contact.Where(a => a.CategoryId == 7 || a.CategoryId == null);

            return c;
        }
    }
}

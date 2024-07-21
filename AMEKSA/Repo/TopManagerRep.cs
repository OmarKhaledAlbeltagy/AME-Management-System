using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.FirstManagerModels;
using AMEKSA.Models;
using AMEKSA.MonthlyPlanModels;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class TopManagerRep : ITopManagerRep
    {
        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly ITimeRep ti;

        public TopManagerRep(DbContainer db, UserManager<ExtendIdentityUser> userManager,ITimeRep ti)
        {
            this.db = db;
            this.userManager = userManager;
            this.ti = ti;
        }

        public IEnumerable<AccountVisitsPercentageByCategoryModel> AccountpastMonthPercentage()
        {
            

            int APlusId = db.category.Where(a => a.CategoryName == "A+").Select(a => a.Id).SingleOrDefault();
            int AId = db.category.Where(a => a.CategoryName == "A").Select(a => a.Id).SingleOrDefault();
            int BId = db.category.Where(a => a.CategoryName == "B").Select(a => a.Id).SingleOrDefault();
            int CId = db.category.Where(a => a.CategoryName == "C").Select(a => a.Id).SingleOrDefault();

           




            int APlusAccounts = db.account.Where(a => a.CategoryId == APlusId).Select(x => x.Id).Count();
            int AAccounts = db.account.Where(a => a.CategoryId == AId).Select(x => x.Id).Count();
            int BAccounts = db.account.Where(a => a.CategoryId == BId).Select(x => x.Id).Count();
            int CAccounts = db.account.Where(a => a.CategoryId == CId).Select(x => x.Id).Count();





           
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.AddMonths(-1).Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);


            int AMVAPlus = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int ASVAPlus = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsAPlus = AMVAPlus + ASVAPlus;

            int AMVA = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
               a => a.AccountId, b => b.Id, (a, b) => new
               {
                   Id = a.Id,
                   AccountId = b.Id,
                   CategoryId = b.CategoryId
               }).Where(x => x.CategoryId == AId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int ASVA = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == AId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsA = AMVA + ASVA;

            int AMVB = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
              a => a.AccountId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  AccountId = b.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == BId).DistinctBy(z => z.AccountId).Select(c => c.Id).Count();

            int ASVB = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == BId).DistinctBy(z => z.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsB = AMVB + ASVB;

            int AMVC = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
              a => a.AccountId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  AccountId = b.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == CId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int ASVC = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == CId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsC = AMVC + ASVC;


            List<AccountVisitsPercentageByCategoryModel> result = new List<AccountVisitsPercentageByCategoryModel>();
            AccountVisitsPercentageByCategoryModel APlusResult = new AccountVisitsPercentageByCategoryModel();
            if (APlusAccounts != 0)
            {
                APlusResult.CategoryName = "A+";
                float rateAPlus = (float)AccountsVisitsAPlus / (float)APlusAccounts;
                APlusResult.percentage = rateAPlus * 100;
                result.Add(APlusResult);
            }
            else
            {
                APlusResult.CategoryName = "A+";
                APlusResult.percentage = 0;
                result.Add(APlusResult);
            }

            AccountVisitsPercentageByCategoryModel AResult = new AccountVisitsPercentageByCategoryModel();
            if (AAccounts != 0)
            {
                AResult.CategoryName = "A";
                float rateA = (float)AccountsVisitsA / (float)AAccounts;
                AResult.percentage = rateA * 100;
                result.Add(AResult);
            }
            else
            {
                AResult.CategoryName = "A";
                AResult.percentage = 0;
                result.Add(AResult);
            }

            AccountVisitsPercentageByCategoryModel BResult = new AccountVisitsPercentageByCategoryModel();
            if (BAccounts != 0)
            {
                BResult.CategoryName = "B";
                float rateB = (float)AccountsVisitsB / (float)BAccounts;
                BResult.percentage = rateB * 100;
                result.Add(BResult);
            }
            else
            {
                BResult.CategoryName = "B";
                BResult.percentage = 0;
                result.Add(BResult);
            }

            AccountVisitsPercentageByCategoryModel CResult = new AccountVisitsPercentageByCategoryModel();
            if (CAccounts != 0)
            {
                CResult.CategoryName = "C";
                float rateC = (float)AccountsVisitsC / (float)CAccounts;
                CResult.percentage = rateC * 100;
                result.Add(CResult);
            }

            else
            {
                CResult.CategoryName = "C";
                CResult.percentage = 0;
                result.Add(CResult);
            }

            return result;
        }

        public IEnumerable<AccountVisitsPercentageByCategoryModel> AccountThisMonthPercentage()
        {

            int APlusId = db.category.Where(a => a.CategoryName == "A+").Select(a => a.Id).SingleOrDefault();
            int AId = db.category.Where(a => a.CategoryName == "A").Select(a => a.Id).SingleOrDefault();
            int BId = db.category.Where(a => a.CategoryName == "B").Select(a => a.Id).SingleOrDefault();
            int CId = db.category.Where(a => a.CategoryName == "C").Select(a => a.Id).SingleOrDefault();

            




            int APlusAccounts = db.account.Where(a => a.CategoryId == APlusId).Select(x => x.Id).Count();
            int AAccounts = db.account.Where(a => a.CategoryId == AId).Select(x => x.Id).Count();
            int BAccounts = db.account.Where(a => a.CategoryId == BId).Select(x => x.Id).Count();
            int CAccounts = db.account.Where(a => a.CategoryId == CId).Select(x => x.Id).Count();





           
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);


            int AMVAPlus = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int ASVAPlus = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsAPlus = AMVAPlus + ASVAPlus;

            int AMVA = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
               a => a.AccountId, b => b.Id, (a, b) => new
               {
                   Id = a.Id,
                   AccountId = b.Id,
                   CategoryId = b.CategoryId
               }).Where(x => x.CategoryId == AId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int ASVA = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == AId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsA = AMVA + ASVA;

            int AMVB = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
              a => a.AccountId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  AccountId = b.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == BId).DistinctBy(z => z.AccountId).Select(c => c.Id).Count();

            int ASVB = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == BId).DistinctBy(z => z.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsB = AMVB + ASVB;

            int AMVC = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
              a => a.AccountId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  AccountId = b.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == CId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int ASVC = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == CId).DistinctBy(a => a.AccountId).Select(c => c.Id).Count();

            int AccountsVisitsC = AMVC + ASVC;


            List<AccountVisitsPercentageByCategoryModel> result = new List<AccountVisitsPercentageByCategoryModel>();
            AccountVisitsPercentageByCategoryModel APlusResult = new AccountVisitsPercentageByCategoryModel();
            if (APlusAccounts != 0)
            {
                APlusResult.CategoryName = "A+";
                float rateAPlus = (float)AccountsVisitsAPlus / (float)APlusAccounts;
                APlusResult.percentage = rateAPlus * 100;
                result.Add(APlusResult);
            }
            else
            {
                APlusResult.CategoryName = "A+";
                APlusResult.percentage = 0;
                result.Add(APlusResult);
            }

            AccountVisitsPercentageByCategoryModel AResult = new AccountVisitsPercentageByCategoryModel();
            if (AAccounts != 0)
            {
                AResult.CategoryName = "A";
                float rateA = (float)AccountsVisitsA / (float)AAccounts;
                AResult.percentage = rateA * 100;
                result.Add(AResult);
            }
            else
            {
                AResult.CategoryName = "A";
                AResult.percentage = 0;
                result.Add(AResult);
            }

            AccountVisitsPercentageByCategoryModel BResult = new AccountVisitsPercentageByCategoryModel();
            if (BAccounts != 0)
            {
                BResult.CategoryName = "B";
                float rateB = (float)AccountsVisitsB / (float)BAccounts;
                BResult.percentage = rateB * 100;
                result.Add(BResult);
            }
            else
            {
                BResult.CategoryName = "B";
                BResult.percentage = 0;
                result.Add(BResult);
            }

            AccountVisitsPercentageByCategoryModel CResult = new AccountVisitsPercentageByCategoryModel();
            if (CAccounts != 0)
            {
                CResult.CategoryName = "C";
                float rateC = (float)AccountsVisitsC / (float)CAccounts;
                CResult.percentage = rateC * 100;
                result.Add(CResult);
            }

            else
            {
                CResult.CategoryName = "C";
                CResult.percentage = 0;
                result.Add(CResult);
            }

            return result;
        }

        public IEnumerable<AccountVisitsPercentageByCategoryModel> ContactPastMonthPercentage()
        {
            

            int APlusId = db.category.Where(a => a.CategoryName == "A+").Select(a => a.Id).SingleOrDefault();
            int AId = db.category.Where(a => a.CategoryName == "A").Select(a => a.Id).SingleOrDefault();
            int BId = db.category.Where(a => a.CategoryName == "B").Select(a => a.Id).SingleOrDefault();
            int CId = db.category.Where(a => a.CategoryName == "C").Select(a => a.Id).SingleOrDefault();



           


            int APlusContacts = db.userContact.Where(a => a.CategoryId == APlusId).Select(x => x.Id).Count();
            int AContacts = db.userContact.Where(a => a.CategoryId == AId).Select(x => x.Id).Count();
            int BContacts = db.userContact.Where(a => a.CategoryId == BId).Select(x => x.Id).Count();
            int CContacts = db.userContact.Where(a => a.CategoryId == CId).Select(x => x.Id).Count();

     
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.AddMonths(-1).Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);


            int CMVAPlus = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
                a => a.ContactId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactId = b.Id
                }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
                {
                    Id = a.Id,
                    ContactId = a.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId),a=>a.ContactId).Select(c => c.Id).Count();


        


            int CMVA = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
               a => a.ContactId, b => b.Id, (a, b) => new
               {
                   Id = a.Id,
                   ContactId = b.Id
               }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
               {
                   Id = a.Id,
                   ContactId = a.Id,
                   CategoryId = b.CategoryId
               }).Where(x => x.CategoryId == AId),a=>a.ContactId).Select(c => c.Id).Count();




            int CMVB = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
              a => a.ContactId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = b.Id
              }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = a.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == BId),a=>a.ContactId).Select(c => c.Id).Count();
            ;

            int CMVC = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
              a => a.ContactId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = b.Id
              }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = a.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == CId),a=>a.ContactId).Select(c => c.Id).Count();



            List<AccountVisitsPercentageByCategoryModel> result = new List<AccountVisitsPercentageByCategoryModel>();
            AccountVisitsPercentageByCategoryModel APlusResult = new AccountVisitsPercentageByCategoryModel();
            if (APlusContacts != 0)
            {
                APlusResult.CategoryName = "A+";
                float rateAPlus = (float)CMVAPlus / (float)APlusContacts;
                APlusResult.percentage = rateAPlus * 100;
                result.Add(APlusResult);
            }
            else
            {
                APlusResult.CategoryName = "A+";
                APlusResult.percentage = 0;
                result.Add(APlusResult);
            }

            AccountVisitsPercentageByCategoryModel AResult = new AccountVisitsPercentageByCategoryModel();
            if (AContacts != 0)
            {
                AResult.CategoryName = "A";
                float rateA = (float)CMVA / (float)AContacts;
                AResult.percentage = rateA * 100;
                result.Add(AResult);
            }
            else
            {
                AResult.CategoryName = "A";
                AResult.percentage = 0;
                result.Add(AResult);
            }

            AccountVisitsPercentageByCategoryModel BResult = new AccountVisitsPercentageByCategoryModel();
            if (BContacts != 0)
            {
                BResult.CategoryName = "B";
                float rateB = (float)CMVB / (float)BContacts;
                BResult.percentage = rateB * 100;
                result.Add(BResult);
            }
            else
            {
                BResult.CategoryName = "B";
                BResult.percentage = 0;
                result.Add(BResult);
            }

            AccountVisitsPercentageByCategoryModel CResult = new AccountVisitsPercentageByCategoryModel();
            if (CContacts != 0)
            {
                CResult.CategoryName = "C";
                float rateC = (float)CMVC / (float)CContacts;
                CResult.percentage = rateC * 100;
                result.Add(CResult);
            }

            else
            {
                CResult.CategoryName = "C";
                CResult.percentage = 0;
                result.Add(CResult);
            }

            return result;
        }

        public IEnumerable<AccountVisitsPercentageByCategoryModel> ContactThisMonthPercentage()
        {
            

            int APlusId = db.category.Where(a => a.CategoryName == "A+").Select(a => a.Id).SingleOrDefault();
            int AId = db.category.Where(a => a.CategoryName == "A").Select(a => a.Id).SingleOrDefault();
            int BId = db.category.Where(a => a.CategoryName == "B").Select(a => a.Id).SingleOrDefault();
            int CId = db.category.Where(a => a.CategoryName == "C").Select(a => a.Id).SingleOrDefault();



            


            int APlusContacts = db.userContact.Where(a => a.CategoryId == APlusId).Select(x => x.Id).Count();
            int AContacts = db.userContact.Where(a => a.CategoryId == AId).Select(x => x.Id).Count();
            int BContacts = db.userContact.Where(a => a.CategoryId == BId).Select(x => x.Id).Count();
            int CContacts = db.userContact.Where(a => a.CategoryId == CId).Select(x => x.Id).Count();

            
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);


            int CMVAPlus = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
                a => a.ContactId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactId = b.Id
                }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
                {
                    Id = a.Id,
                    ContactId = a.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId),a=>a.ContactId).Select(c => c.Id).Count();


            int CMVA = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
               a => a.ContactId, b => b.Id, (a, b) => new
               {
                   Id = a.Id,
                   ContactId = b.Id
               }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
               {
                   Id = a.Id,
                   ContactId = a.Id,
                   CategoryId = b.CategoryId
               }).Where(x => x.CategoryId == AId),a=>a.ContactId).Select(c => c.Id).Count();




            int CMVB = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
              a => a.ContactId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = b.Id
              }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = a.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == BId),a=>a.ContactId).Select(c => c.Id).Count();
            ;

            int CMVC = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,
              a => a.ContactId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = b.Id
              }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = a.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == CId),a=>a.ContactId).Select(c => c.Id).Count();



            List<AccountVisitsPercentageByCategoryModel> result = new List<AccountVisitsPercentageByCategoryModel>();
            AccountVisitsPercentageByCategoryModel APlusResult = new AccountVisitsPercentageByCategoryModel();
            if (APlusContacts != 0)
            {
                APlusResult.CategoryName = "A+";
                float rateAPlus = (float)CMVAPlus / (float)APlusContacts;
                APlusResult.percentage = rateAPlus * 100;
                result.Add(APlusResult);
            }
            else
            {
                APlusResult.CategoryName = "A+";
                APlusResult.percentage = 0;
                result.Add(APlusResult);
            }

            AccountVisitsPercentageByCategoryModel AResult = new AccountVisitsPercentageByCategoryModel();
            if (AContacts != 0)
            {
                AResult.CategoryName = "A";
                float rateA = (float)CMVA / (float)AContacts;
                AResult.percentage = rateA * 100;
                result.Add(AResult);
            }
            else
            {
                AResult.CategoryName = "A";
                AResult.percentage = 0;
                result.Add(AResult);
            }

            AccountVisitsPercentageByCategoryModel BResult = new AccountVisitsPercentageByCategoryModel();
            if (BContacts != 0)
            {
                BResult.CategoryName = "B";
                float rateB = (float)CMVB / (float)BContacts;
                BResult.percentage = rateB * 100;
                result.Add(BResult);
            }
            else
            {
                BResult.CategoryName = "B";
                BResult.percentage = 0;
                result.Add(BResult);
            }

            AccountVisitsPercentageByCategoryModel CResult = new AccountVisitsPercentageByCategoryModel();
            if (CContacts != 0)
            {
                CResult.CategoryName = "C";
                float rateC = (float)CMVC / (float)CContacts;
                CResult.percentage = rateC * 100;
                result.Add(CResult);
            }

            else
            {
                CResult.CategoryName = "C";
                CResult.percentage = 0;
                result.Add(CResult);
            }

            return result;
        }

        public IEnumerable<CustomAccountSalesVisit> GetAccountSalesVisitsByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ExtendIdentityUser> GetAllFirstManagers()
        {
            List<ExtendIdentityUser> res = new List<ExtendIdentityUser>();
            IEnumerable<ExtendIdentityUser> FirstManagers = userManager.GetUsersInRoleAsync("First Line Manager").Result;
            IEnumerable<ExtendIdentityUser> TopManagers = userManager.GetUsersInRoleAsync("Top Line Manager").Result;
            foreach (var f in FirstManagers)
            {
                res.Add(f);
            }

            foreach (var t in TopManagers)
            {
                res.Add(t);
            }
          
            return res.OrderBy(a=>a.FullName);
        }

        public IEnumerable<CustomAccountMedicalVisit> GetDetailedAMV(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);

            List<ExtendIdentityUser> team = new List<ExtendIdentityUser>();
            if (userid == "0")
            {
                team = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a=>a.Active == true).ToList();
            }
            else
            {
                team = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a => a.Active == true).Where(a => a.extendidentityuserid == userid).ToList();
            }
            List<AccountMedicalVisit> visits = new List<AccountMedicalVisit>();
            foreach (var item in team)
            {
                IEnumerable<AccountMedicalVisit> v = db.accountMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= from && a.VisitDate <= to);
                foreach (var vis in v)
                {
                    visits.Add(vis);
                }
            }

            List<CustomAccountMedicalVisitProducts> productlist = new List<CustomAccountMedicalVisitProducts>();
            List<CustomVisitPerson> personlist = new List<CustomVisitPerson>();
            foreach (var item in visits)
            {
                var s = db.accountMedicalVisitProducts.Where(a => a.AccountMedicalVisitId == item.Id).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
                {
                    ProductName = b.ProductName,
                    KeyId = a.AccountMedicalVisitId
                });
                foreach (var product in s)
                {
                    CustomAccountMedicalVisitProducts ad = new CustomAccountMedicalVisitProducts();
                    ad.ProductName = product.ProductName;
                    ad.AccountMedicalVisitId = product.KeyId;
                    productlist.Add(ad);
                }

                var ss = db.accountMedicalVisitPerson.Where(a => a.AccountMedicalVisitId == item.Id);
                foreach (var person in ss)
                {
                    CustomVisitPerson ab = new CustomVisitPerson();
                    ab.PersonName = person.PersonName;
                    ab.PersonPosition = person.PersonPosition;
                    ab.KeyId = person.AccountMedicalVisitId;
                    personlist.Add(ab);
                }

            }




            List<CustomAccountMedicalVisit> result = DistinctByExtension.DistinctBy(visits.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = b.AccountName,
                AccountTypeId = b.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = b.DistrictId,
                CategoryId = b.CategoryId,
                SubmittingDate = a.SubmittingDate,
                UserId = a.extendidentityuserid
            }).Join(db.accountMedicalVisitProducts, a => a.Id, b => b.AccountMedicalVisitId, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = a.DistrictId,
                CategoryId = a.CategoryId,
                SubmittingDate = a.SubmittingDate,
                UserId = a.UserId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = a.DistrictId,
                DistrictName = b.DistrictName,
                CategoryId = a.CategoryId,
                SubmittingDate = a.SubmittingDate,
                UserId = a.UserId
            }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new {

                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = a.DistrictId,
                DistrictName = a.DistrictName,
                CategoryName = b.CategoryName,
                SubmittingDate = a.SubmittingDate,
                UserId = a.UserId
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                CategoryName = a.CategoryName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = b.AccountTypeName,
                SubmittingDate = a.SubmittingDate,
                UserId = a.UserId
            }).Join(db.Users.Where(a => a.Active == true), a => a.UserId, b => b.Id, (a, b) => new CustomAccountMedicalVisit
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                product = productlist.Where(x => x.AccountMedicalVisitId == a.Id).ToList(),
                person = personlist.Where(c => c.KeyId == a.Id).ToList(),
                CategoryName = a.CategoryName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = a.AccountTypeName,
                SubmittingDate = a.SubmittingDate,
                UserName = b.FullName
            }),o=>o.Id).OrderByDescending(a => a.VisitDate).OrderBy(a => a.UserName).ToList();
                




            return result;
        }

        public IEnumerable<CustomAccountSupportiveVisit> GetDetailedASupportiveV(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);

            List<ExtendIdentityUser> team = new List<ExtendIdentityUser>();
            if (userid == "0")
            {
                team = userManager.GetUsersInRoleAsync("Supportive").Result.Where(a => a.Active == true).ToList();
            }
            else
            {
                team = userManager.GetUsersInRoleAsync("Supportive").Result.Where(a => a.extendidentityuserid == userid && a.Active == true).ToList();
            }

            List<AccountSupportiveVisit> visits = new List<AccountSupportiveVisit>();
            foreach (var item in team)
            {
                List<AccountSupportiveVisit> v = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= from && a.VisitDate <= to).ToList();
                foreach (var vis in v)
                {
                    visits.Add(vis);
                }
            }

            //var visitsids = db.accountSalesVisit.Where(a => a.extendidentityuserid == userid && a.VisitDate >= from && a.VisitDate <= to).Select(a => a.Id);

            List<CustomVisitProduct> productlist = new List<CustomVisitProduct>();
            List<CustomVisitPerson> personlist = new List<CustomVisitPerson>();
            foreach (var item in visits)
            {
                var s = db.AccountSupportiveVisitproduct.Where(a => a.AccountSupportVisitId == item.Id).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
                {
                    ProductName = b.ProductName,
                    KeyId = a.AccountSupportVisitId
                });
                foreach (var product in s)
                {
                    CustomVisitProduct ad = new CustomVisitProduct();
                    ad.ProductName = product.ProductName;
                    ad.KeyId = product.KeyId;
                    productlist.Add(ad);
                }

                var ss = db.AccountSupportiveVisitPerson.Where(a => a.AccountSupportiveVisitId == item.Id);
                foreach (var person in ss)
                {
                    CustomVisitPerson ab = new CustomVisitPerson();
                    ab.PersonName = person.PersonName;
                    ab.PersonPosition = person.PersonPosition;
                    ab.KeyId = person.AccountSupportiveVisitId;
                    personlist.Add(ab);
                }

            }




            List<CustomAccountSupportiveVisit> result = DistinctByExtension.DistinctBy(visits.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = b.AccountName,
                AccountTypeId = b.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.AdditionalNotes,
                DistrictId = b.DistrictId,
                CategoryId = b.CategoryId,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.extendidentityuserid
            }).Join(db.AccountSupportiveVisitproduct, a => a.Id, b => b.AccountSupportVisitId, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = a.DistrictId,
                CategoryId = a.CategoryId,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = a.DistrictId,
                DistrictName = b.DistrictName,
                CategoryId = a.CategoryId,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId
            }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new {

                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = a.DistrictId,
                DistrictName = a.DistrictName,
                CategoryName = b.CategoryName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                CategoryName = a.CategoryName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = b.AccountTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId
            }).Join(db.Users.Where(a => a.Active == true), a => a.UserId, b => b.Id, (a, b) => new CustomAccountSupportiveVisit
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                CategoryName = a.CategoryName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.PaymentNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = a.AccountTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserName = b.FullName
            }),o=>o.Id).OrderByDescending(a => a.VisitDate).OrderBy(a => a.UserName).ToList();


            foreach (var item in result)
            {
                item.products = productlist.Where(a => a.KeyId == item.Id);
                item.persons = personlist.Where(a => a.KeyId == item.Id);
            }

            return result;
        }

        public IEnumerable<CustomAccountSalesVisit> GetDetailedASV(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);

            List<ExtendIdentityUser> team = new List<ExtendIdentityUser>();
            if (userid == "0")
            {
                team = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.Active == true).ToList();
            }
            else
            {
                team = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.extendidentityuserid == userid && a.Active == true).ToList();

                List<string> subteam = db.Users.Where(a => a.Active == true).Join(db.UserSubmanager.Where(a => a.ManagerId == userid), a => a.Id, b => b.RepId, (a, b) => new UserSubmanager
                {
                    Id = b.Id,
                    RepId = b.RepId,
                    ManagerId = b.ManagerId
                }).Select(a => a.RepId).ToList();

                foreach (var item in subteam)
                {
                    ExtendIdentityUser subuser = userManager.FindByIdAsync(item).Result;
                    if (subuser.Active == true)
                    {
                        team.Add(subuser);
                    }
                    
                }
            }

            List<AccountSalesVisit> visits = new List<AccountSalesVisit>();
            foreach (var item in team)
            {
                List<AccountSalesVisit> v = db.accountSalesVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= from && a.VisitDate <= to).ToList();
                foreach (var vis in v)
                {
                    visits.Add(vis);
                }
            }

            //var visitsids = db.accountSalesVisit.Where(a => a.extendidentityuserid == userid && a.VisitDate >= from && a.VisitDate <= to).Select(a => a.Id);

            List<CustomVisitBrand> brandlist = new List<CustomVisitBrand>();
            List<CustomVisitPerson> personlist = new List<CustomVisitPerson>();
            foreach (var item in visits)
            {
                var s = db.accountSalesVisitBrand.Where(a => a.AccountSalesVisitId == item.Id).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new
                {
                    BrandName = b.BrandName,
                    KeyId = a.AccountSalesVisitId
                }).ToList();
                foreach (var brand in s)
                {
                    CustomVisitBrand ad = new CustomVisitBrand();
                    ad.BrandName = brand.BrandName;
                    ad.KeyId = brand.KeyId;
                    brandlist.Add(ad);
                }

                var ss = db.accountSalesVisitPerson.Where(a => a.AccountSalesVisitId == item.Id).ToList();
                foreach (var person in ss)
                {
                    CustomVisitPerson ab = new CustomVisitPerson();
                    ab.PersonName = person.PersonName;
                    ab.PersonPosition = person.PersonPosition;
                    ab.KeyId = person.AccountSalesVisitId;
                    personlist.Add(ab);
                }

            }




            List<CustomAccountSalesVisit> result = DistinctByExtension.DistinctBy(visits.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = b.AccountName,
                AccountTypeId = b.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = b.DistrictId,
                CategoryId = b.CategoryId,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.extendidentityuserid
            }).Join(db.accountSalesVisitBrand, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = a.DistrictId,
                CategoryId = a.CategoryId,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = a.DistrictId,
                DistrictName = b.DistrictName,
                CategoryId = a.CategoryId,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId
            }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new {

                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = a.DistrictId,
                DistrictName = a.DistrictName,
                CategoryName = b.CategoryName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                brands = brandlist.Where(x => x.KeyId == a.Id).ToList(),
                persons = personlist.Where(c => c.KeyId == a.Id).ToList(),
                CategoryName = a.CategoryName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = b.AccountTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId
            }).Join(db.Users.Where(a => a.Active == true), a => a.UserId, b => b.Id, (a, b) => new CustomAccountSalesVisit
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                brands = a.brands,
                persons = a.persons,
                CategoryName = a.CategoryName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = a.AccountTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserName = b.FullName
            }),o=>o.Id).OrderByDescending(a => a.VisitDate).OrderBy(a => a.UserName).ToList();

            return result;
        }



        public IEnumerable<CustomContactMedicalVisit> GetDetailedCMV(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);
            List<ExtendIdentityUser> team = new List<ExtendIdentityUser>();
            if (userid == "0")
            {
                team = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a => a.Active == true).ToList();
            }
            else
            {
                team = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a => a.extendidentityuserid == userid && a.Active == true).ToList();
            }
            

            List<ContactMedicalVisit> visits = new List<ContactMedicalVisit>();
            foreach (var item in team)
            {
                List<ContactMedicalVisit> v = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= from && a.VisitDate <= to).ToList();
                foreach (var vis in v)
                {
                    visits.Add(vis);
                }
            }


            

            List<CustomContactMedicalVisitProducts> productlist = new List<CustomContactMedicalVisitProducts>();
            List<CustomContactSalesAid> salesaidslist = new List<CustomContactSalesAid>();
            foreach (var item in visits)
            {
                var s = db.contactMedicalVisitProduct.Where(a => a.ContactMedicalVisitId == item.Id).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
                {
                    ProductName = b.ProductName,
                    ProductShare = a.ProductShare,
                    KeyId = a.ContactMedicalVisitId
                }).ToList();
                foreach (var product in s)
                {
                    CustomContactMedicalVisitProducts ad = new CustomContactMedicalVisitProducts();
                    ad.ProductName = product.ProductName;
                    ad.ProductShare = product.ProductShare;
                    ad.ContactMedicalVisitId = product.KeyId;
                    productlist.Add(ad);
                }

                var ss = db.contactSalesAid.Where(a => a.ContactMedicalVisitId == item.Id).Join(db.salesAid, a => a.SalesAidId, b => b.Id, (a, b) => new
                {
                    SalesAidName = b.SalesAidName,
                    ContactMedicalVisitId = a.ContactMedicalVisitId
                }).ToList();
                foreach (var aid in ss)
                {
                    CustomContactSalesAid ab = new CustomContactSalesAid();
                    ab.SalesAidName = aid.SalesAidName;
                    ab.ContactMedicalVisitId = aid.ContactMedicalVisitId;
                    salesaidslist.Add(ab);
                }

            }



            var xxx = visits.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = b.ContactName,
                ContactTypeId = b.ContactTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = b.DistrictId,
                SubmittingDate = a.SubmittingDate,
                UserId = a.extendidentityuserid,
                customcontactmedicalvisitproduct = productlist.Where(x => x.ContactMedicalVisitId == a.Id),
                customcontactsalesaid = salesaidslist.Where(c => c.ContactMedicalVisitId == a.Id)
            }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                ContactTypeId = a.ContactTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = a.DistrictId,
                SubmittingDate = a.SubmittingDate,
                UserId = a.UserId,
                customcontactmedicalvisitproduct = a.customcontactmedicalvisitproduct,
                customcontactsalesaid = a.customcontactsalesaid,
                CategoryId = b.CategoryId
            });





            List<CustomContactMedicalVisit> result = DistinctByExtension.DistinctBy(visits.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Email = b.Email,
                AccountId = b.AccountId,
                PhoneNumber = b.MobileNumber,
                LandLineNumber = b.LandLineNumber,
                ContactId = a.ContactId,
                ContactName = b.ContactName,
                ContactTypeId = b.ContactTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = b.DistrictId,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.extendidentityuserid,
                customcontactmedicalvisitproduct = productlist.Where(x => x.ContactMedicalVisitId == a.Id),
                customcontactsalesaid = salesaidslist.Where(c => c.ContactMedicalVisitId == a.Id)
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Email = a.Email,
                AccountId = a.AccountId,
                PhoneNumber = a.PhoneNumber,
                LandLineNumber = a.LandLineNumber,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                ContactTypeId = a.ContactTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = a.DistrictId,
                DistrictName = b.DistrictName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId,
                customcontactmedicalvisitproduct = a.customcontactmedicalvisitproduct,
                customcontactsalesaid = a.customcontactsalesaid
            }).Join(db.contactType, a => a.ContactTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Email = a.Email,
                AccountId = a.AccountId,
                PhoneNumber = a.PhoneNumber,
                LandLineNumber = a.LandLineNumber,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = b.ContactTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId
            }).Join(db.Users.Where(a => a.Active == true), a => a.UserId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Email = a.Email,
                AccountId = a.AccountId,
                PhoneNumber = a.PhoneNumber,
                LandLineNumber = a.LandLineNumber,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = a.ContactTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId,
                UserName = b.FullName
            }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
            {
                Id = a.Id,
                Email = a.Email,
                AccountId = a.AccountId,
                PhoneNumber = a.PhoneNumber,
                LandLineNumber = a.LandLineNumber,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = a.ContactTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId,
                UserName = a.UserName,
                CategoryId = b.CategoryId
            }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                Email = a.Email,
                AccountId = a.AccountId,
                PhoneNumber = a.PhoneNumber,
                LandLineNumber = a.LandLineNumber,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = a.ContactTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId,
                UserName = a.UserName,
                CategoryId = a.CategoryId,
                CategoryName = b.CategoryName
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContactMedicalVisit
            {
                Id = a.Id,
                Email = a.Email,
                AccountName = b.AccountName,
                PhoneNumber = a.PhoneNumber,
                LandLineNumber = a.LandLineNumber,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = a.ContactTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime,
                UserId = a.UserId,
                UserName = a.UserName,
                CategoryId = a.CategoryId,
                CategoryName = a.CategoryName,
                Requested = false
            }),x=>x.Id).OrderByDescending(a => a.VisitDate).OrderBy(a => a.UserName).ToList().ToList();


            foreach (var item in result)
            {
                item.customcontactmedicalvisitproduct = productlist.Where(x => x.ContactMedicalVisitId == item.Id);
                item.customcontactsalesaid = salesaidslist.Where(c => c.ContactMedicalVisitId == item.Id);
            }
              



            return result;
        }

        public IEnumerable<CustomTarget> GetTarget(SearchTargetModel obj)
        {
            if (obj.CategoryId == 0)
            {
                int days = DateTime.DaysInMonth(obj.year, obj.Month);
                DateTime start = new DateTime(obj.year, obj.Month, 1);
                DateTime end = new DateTime(obj.year, obj.Month, days);

                var x = db.userContact.Join(db.Users.Where(a => a.Active == true), a => a.extendidentityuserid, b => b.Id, (a, b) => new
                {
                    UserId = b.Id,
                    ContactId = a.ContactId,
                    FullName = b.FullName,
                    ManagerId = b.extendidentityuserid,
                    MonthlyTarget = a.MonthlyTarget,
                    CategoryId = a.CategoryId
                }).Where(u => u.ManagerId == obj.ManagerId).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
                {
                    FullName = a.FullName,
                    ContactName = b.ContactName,
                    MonthlyTarget = a.MonthlyTarget,
                    CategoryId = a.CategoryId,
                    ContactId = b.Id,
                    UserId = a.UserId
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    FullName = a.FullName,
                    ContactName = a.ContactName,
                    MonthlyTarget = a.MonthlyTarget,
                    CategoryId = a.CategoryId,
                    ContactId = a.ContactId,
                    UserId = a.UserId,
                    CategoryName = b.CategoryName
                });

                List<CustomTarget> result = new List<CustomTarget>();
                foreach (var item in x)
                {
                    CustomTarget t = new CustomTarget();
                    int count = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end && a.extendidentityuserid == item.UserId && a.ContactId == item.ContactId).Select(a => a.Id).Count();
                    t.ContactName = item.ContactName;
                    t.FullName = item.FullName;
                    t.CurrentVisits = count;
                    t.MonthlyTarget = item.MonthlyTarget;
                    t.CategoryName = item.CategoryName;
                    result.Add(t);
                }

                return result;
            }

            else
            {
                int days = DateTime.DaysInMonth(obj.year, obj.Month);
                DateTime start = new DateTime(obj.year, obj.Month, 1);
                DateTime end = new DateTime(obj.year, obj.Month, days);

                var x = db.userContact.Join(db.Users.Where(a => a.Active == true), a => a.extendidentityuserid, b => b.Id, (a, b) => new
                {
                    UserId = b.Id,
                    ContactId = a.ContactId,
                    FullName = b.FullName,
                    ManagerId = b.extendidentityuserid,
                    MonthlyTarget = a.MonthlyTarget,
                    CategoryId = a.CategoryId
                }).Where(u => u.ManagerId == obj.ManagerId).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
                {
                    FullName = a.FullName,
                    ContactName = b.ContactName,
                    MonthlyTarget = a.MonthlyTarget,
                    CategoryId = a.CategoryId,
                    ContactId = b.Id,
                    UserId = a.UserId
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    FullName = a.FullName,
                    ContactName = a.ContactName,
                    MonthlyTarget = a.MonthlyTarget,
                    CategoryId = a.CategoryId,
                    ContactId = a.ContactId,
                    UserId = a.UserId,
                    CategoryName = b.CategoryName
                });

                List<CustomTarget> result = new List<CustomTarget>();
                foreach (var item in x)
                {
                    CustomTarget t = new CustomTarget();
                    int count = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end && a.extendidentityuserid == item.UserId && a.ContactId == item.ContactId).Select(a => a.Id).Count();
                    t.ContactName = item.ContactName;
                    t.FullName = item.FullName;
                    t.CurrentVisits = count;
                    t.MonthlyTarget = item.MonthlyTarget;
                    t.CategoryName = item.CategoryName;
                    result.Add(t);
                }

                return result;
            }
            
        }

        public IEnumerable<MyTeamModel> GetTeamMedical()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MyTeamModel> GetTeamSales()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VisitsCountModel> GetVisitsCountDamamByMonth(int year, int month)
        {
          
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            List<VisitsCountModel> result = new List<VisitsCountModel>();
            double accountmedical = DistinctByExtension.DistinctBy(db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }),a=>a.Id).Where(x => x.CityId == 2).Select(a => a.Id).Count();

           

            VisitsCountModel accmed = new VisitsCountModel();
            accmed.VisitType = "Medical To Accounts";
            accmed.VisitCount = accountmedical;
            result.Add(accmed);

            double contactmedical = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }),a=>a.Id).Where(x => x.CityId == 2).Select(a => a.Id).Count();

           


            VisitsCountModel conmed = new VisitsCountModel();
            conmed.VisitType = "Medical To Contacts";
            conmed.VisitCount = contactmedical;
            result.Add(conmed);

            double accountsales = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }),a=>a.Id).Where(x => x.CityId == 2).Select(a => a.Id).Count();

            VisitsCountModel accsal = new VisitsCountModel();
            accsal.VisitType = "Sales To Accounts";
            accsal.VisitCount = accountsales;
            result.Add(accsal);

            double accountsupportive = DistinctByExtension.DistinctBy(db.AccountSupportiveVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }),a=>a.Id).Where(x => x.CityId == 2).Select(a => a.Id).Count();

            VisitsCountModel accsup = new VisitsCountModel();
            accsup.VisitType = "Supportive To Accounts";
            accsup.VisitCount = accountsupportive;
            result.Add(accsup);

            return result;
        }

        public IEnumerable<VisitsCountModel> GetVisitsCountJeddahByMonth(int year, int month)
        {
            
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            List<VisitsCountModel> result = new List<VisitsCountModel>();
            double accountmedical = DistinctByExtension.DistinctBy(db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }),a=>a.Id).Where(x => x.CityId == 3).Select(a => a.Id).Count();
               

     



            VisitsCountModel accmed = new VisitsCountModel();
            accmed.VisitType = "Medical To Accounts";
            accmed.VisitCount = accountmedical;
            result.Add(accmed);

            double contactmedical = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }), a => a.Id).Where(x => x.CityId == 3).Select(a => a.Id).Count();
            
   
            VisitsCountModel conmed = new VisitsCountModel();
            conmed.VisitType = "Medical To Contacts";
            conmed.VisitCount = contactmedical;

            result.Add(conmed);

            double accountsales = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate < end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }), a => a.Id).Where(x => x.CityId == 3).Select(a => a.Id).Count();
             

            VisitsCountModel accsal = new VisitsCountModel();
            accsal.VisitType = "Sales To Accounts";
            accsal.VisitCount = accountsales;
            result.Add(accsal);

            double accountsupportive = DistinctByExtension.DistinctBy(db.AccountSupportiveVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }), a => a.Id).Where(x => x.CityId == 3).Select(a => a.Id).Count();
               

            VisitsCountModel accsup = new VisitsCountModel();
            accsup.VisitType = "Supportive To Accounts";
            accsup.VisitCount = accountsupportive;
            result.Add(accsup);

            return result;
        }

        public IEnumerable<VisitsCountModel> GetVisitsCountReyadByMonth(int year, int month)
        {
            
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            List<VisitsCountModel> result = new List<VisitsCountModel>();
            double accountmedical = DistinctByExtension.DistinctBy(db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }),a=>a.Id).Where(x => x.CityId == 1).Select(a => a.Id).Count();


     

     

            VisitsCountModel accmed = new VisitsCountModel();
            accmed.VisitType = "Medical To Accounts";
            accmed.VisitCount = accountmedical;
            result.Add(accmed);

            double contactmedical = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }), a => a.Id).Where(x => x.CityId == 1).Select(a => a.Id).Count();
                
        


            VisitsCountModel conmed = new VisitsCountModel();
            conmed.VisitType = "Medical To Contacts";
            conmed.VisitCount = contactmedical;
            result.Add(conmed);

            double accountsales = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }),a=>a.Id).Where(x => x.CityId == 1).Select(a => a.Id).Count();
                

            VisitsCountModel accsal = new VisitsCountModel();
            accsal.VisitType = "Sales To Accounts";
            accsal.VisitCount = accountsales;
            result.Add(accsal);



            double accountsupportive = DistinctByExtension.DistinctBy(db.AccountSupportiveVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }),a=>a.Id).Where(x => x.CityId == 1).Select(a => a.Id).Count();
                
             

            VisitsCountModel accsup = new VisitsCountModel();
            accsup.VisitType = "Supportive To Accounts";
            accsup.VisitCount = accountsupportive;
            result.Add(accsup);

            return result;
        }

        public IEnumerable<VisitsCountModel> GetVisitsCountThisMonthDamam()
        {

            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            List<VisitsCountModel> result = new List<VisitsCountModel>();
            int accountmedical = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a=>a.Id).Where(x => x.CityId == 2).Select(a => a.Id).Count();


            VisitsCountModel accmed = new VisitsCountModel();
            accmed.VisitType = "Medical To Accounts";
            accmed.VisitCount = accountmedical;
            result.Add(accmed);

            int contactmedical = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 2).Select(a => a.Id).Count();
 

            VisitsCountModel conmed = new VisitsCountModel();
            conmed.VisitType = "Medical To Contacts";
            conmed.VisitCount = contactmedical;
            result.Add(conmed);

            int accountsales = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 2).Select(a => a.Id).Count();

            VisitsCountModel accsal = new VisitsCountModel();
            accsal.VisitType = "Sales To Accounts";
            accsal.VisitCount = accountsales;
            result.Add(accsal);

            double accountsupportive = db.AccountSupportiveVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 2).Select(a => a.Id).Count();

            VisitsCountModel accsup = new VisitsCountModel();
            accsup.VisitType = "Supportive To Accounts";
            accsup.VisitCount = accountsupportive;
            result.Add(accsup);

            return result;
        }

        public IEnumerable<VisitsCountModel> GetVisitsCountThisMonthJeddah()
        {
            
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            List<VisitsCountModel> result = new List<VisitsCountModel>();
            int accountmedical = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 3).Select(a => a.Id).Count();
           

            VisitsCountModel accmed = new VisitsCountModel();
            accmed.VisitType = "Medical To Accounts";
            accmed.VisitCount = accountmedical;
            result.Add(accmed);

            int contactmedical = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 3).Select(a => a.Id).Count();


            VisitsCountModel conmed = new VisitsCountModel();
            conmed.VisitType = "Medical To Contacts";
            conmed.VisitCount = contactmedical;
            result.Add(conmed);

            int accountsales = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate < end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 3).Select(a => a.Id).Count();

            VisitsCountModel accsal = new VisitsCountModel();
            accsal.VisitType = "Sales To Accounts";
            accsal.VisitCount = accountsales;
            result.Add(accsal);

            double accountsupportive = db.AccountSupportiveVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 1).Select(a => a.Id).Count();

            VisitsCountModel accsup = new VisitsCountModel();
            accsup.VisitType = "Supportive To Accounts";
            accsup.VisitCount = accountsupportive;
            result.Add(accsup);

            return result;
        }

        public IEnumerable<VisitsCountModel> GetVisitsCountThisMonthReyad()
        {
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            List<VisitsCountModel> result = new List<VisitsCountModel>();
            int accountmedical = db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account,a=>a.AccountId,b=>b.Id,(a,b)=>new
            {
            Id = a.Id,
            DistrictId = b.DistrictId
            }).Join(db.district,a=>a.DistrictId,b=>b.Id,(a,b)=>new
            {
            Id = a.Id,
            CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x=>x.CityId == 1).Select(a => a.Id).Count();


            VisitsCountModel accmed = new VisitsCountModel();
            accmed.VisitType = "Medical To Accounts";
            accmed.VisitCount = accountmedical;
            result.Add(accmed);

            int contactmedical = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.contact,a=>a.ContactId,b=>b.Id,(a,b)=>new
            {
            Id = a.Id,
            DistrictId= b.DistrictId
            }).Join(db.district,a=>a.DistrictId,b=>b.Id,(a,b)=>new
            {
            Id = a.Id,
            CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x=>x.CityId == 1).Select(a => a.Id).Count();

       

            VisitsCountModel conmed = new VisitsCountModel();
            conmed.VisitType = "Medical To Contacts";
            conmed.VisitCount = contactmedical;
            result.Add(conmed);

            int accountsales = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 1).Select(a => a.Id).Count();

            VisitsCountModel accsal = new VisitsCountModel();
            accsal.VisitType = "Sales To Accounts";
            accsal.VisitCount = accountsales;
            result.Add(accsal);

            double accountsupportive = db.AccountSupportiveVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CityId = b.CityId
            }).DistinctBy(a => a.Id).Where(x => x.CityId == 1).Select(a => a.Id).Count();

            VisitsCountModel accsup = new VisitsCountModel();
            accsup.VisitType = "Supportive To Accounts";
            accsup.VisitCount = accountsupportive;
            result.Add(accsup);

            return result;

        }

        public List<CustomContactPlanExcel> MedicalPlansExcel(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto)
        {
            DateTime now = ti.GetCurrentTime();
            DateTime start = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime end = new DateTime(yearto, monthto, dayto);

            List<ExtendIdentityUser> medicals = userManager.GetUsersInRoleAsync("Medical Representative").Result.ToList();

            List<CustomContactPlanExcel> res = new List<CustomContactPlanExcel>();

            foreach (var item in medicals)
            {
                CustomContactPlanExcel obj = new CustomContactPlanExcel();
                List<CustomMonthlyPlan> planss = new List<CustomMonthlyPlan>();
                obj.UserName = item.FullName;
                List<ContactMonthlyPlan> myplans = db.contactMonthlyPlan.Where(a => a.ExtendIdentityUserId == item.Id && a.PlannedDate >= start && a.PlannedDate <= end).ToList();


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
                }).OrderBy(a=>a.PlannedDate).ToList();

                List<CustomMonthlyPlan> withoutaff = myplans.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new CustomMonthlyPlan
                {
                    Id = a.Id,
                    OrgId = b.Id,
                    OrgName = b.ContactName,
                    PlannedDate = a.PlannedDate,
                    Status = a.Status,
                    now = now
                }).OrderBy(a => a.PlannedDate).ToList();

                foreach (var itemm in withaff)
                {
                    planss.Add(itemm);
                }

                foreach (var itemm in withoutaff)
                {
                    planss.Add(itemm);
                }
                List<CustomMonthlyPlan> ress = DistinctByExtension.DistinctBy(planss, a => a.Id).ToList();
                    
                obj.planlist = ress;
                res.Add(obj);
            }

            
            return res.OrderBy(a=>a.UserName).ToList();
        }


        public List<CustomAccountPlanExcel> SalesPlansExcel(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto)
        {
            DateTime now = ti.GetCurrentTime();

            DateTime start = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime end = new DateTime(yearto, monthto, dayto);
            List<ExtendIdentityUser> supportives = userManager.GetUsersInRoleAsync("Sales Representative").Result.ToList();
            List<CustomAccountPlanExcel> res = new List<CustomAccountPlanExcel>();

            foreach (var item in supportives)
            {
                CustomAccountPlanExcel obj = new CustomAccountPlanExcel();

                List<AccountMonthlyPlan> myplans = db.accountMonthlyPlan.Where(a => a.ExtendIdentityUserId == item.Id && a.PlannedDate >= start && a.PlannedDate <= end).ToList();
                obj.UserName = item.FullName;

                List<CustomMonthlyPlanSales> ress = myplans.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomMonthlyPlanSales
                {
                    Id = a.Id,
                    OrgId = b.Id,
                    OrgName = b.AccountName,
                    PlannedDate = a.PlannedDate,
                    Status = a.Status,
                    now = now,
                    customMonthlyPlanCollection = db.accountMonthlyPlanCollection.Where(x => x.AccountMonthlyPlanId == a.Id).Join(db.brand, c => c.BrandId, d => d.Id, (c, d) => new CustomMonthlyPlanCollection { MonthlyPlanId = c.Id, BrandName = d.BrandName, Collection = c.Collection }).ToList()
                }).OrderBy(a => a.PlannedDate).ToList();

                obj.planlist = ress;

                res.Add(obj);

            }





            return res;
        }

        public List<CustomAccountPlanExcel> SupportivePlansExcel(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto)
        {
            DateTime now = ti.GetCurrentTime();

            DateTime start = new DateTime(yearfrom,monthfrom,dayfrom);
            DateTime end = new DateTime(yearto, monthto, dayto);
            List<ExtendIdentityUser> supportives = userManager.GetUsersInRoleAsync("Supportive").Result.ToList();
            List<CustomAccountPlanExcel> res = new List<CustomAccountPlanExcel>();

            foreach (var item in supportives)
            {
                CustomAccountPlanExcel obj = new CustomAccountPlanExcel();
               
                List<AccountMonthlyPlan> myplans = db.accountMonthlyPlan.Where(a => a.ExtendIdentityUserId == item.Id && a.PlannedDate >= start && a.PlannedDate <= end).ToList();
                obj.UserName = item.FullName;

                List<CustomMonthlyPlanSales> ress = myplans.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomMonthlyPlanSales
                {
                    Id = a.Id,
                    OrgId = b.Id,
                    OrgName = b.AccountName,
                    PlannedDate = a.PlannedDate,
                    Status = a.Status,
                    now = now,
                    customMonthlyPlanCollection = db.accountMonthlyPlanCollection.Where(x => x.AccountMonthlyPlanId == a.Id).Join(db.brand, c => c.BrandId, d => d.Id, (c, d) => new CustomMonthlyPlanCollection { MonthlyPlanId = c.Id, BrandName = d.BrandName, Collection = c.Collection }).ToList()
                }).OrderBy(a => a.PlannedDate).ToList();

                obj.planlist = ress;

                res.Add(obj);

            }

            

           

            return res;
        }


        public IEnumerable<TopManagerMorrisLine> MorrisLine()
        {
           DateTime now = ti.GetCurrentTime();
           int month = now.Month;
            List<TopManagerMorrisLine> res = new List<TopManagerMorrisLine>();
            for (int i = 1; i <= month; i++)
            {
                int days = DateTime.DaysInMonth(now.Year, i);
                DateTime start = new DateTime(now.Year, i, 1);
                DateTime end = new DateTime(now.Year, i, days);
                int medicalcontact = db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Count();
                int salesaccount = db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Count();
                int supportiveaccount = db.AccountSupportiveVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Count();
                string yearmonth = start.ToString("yyyy-MM");
                TopManagerMorrisLine obj = new TopManagerMorrisLine();
                obj.y = yearmonth;
                obj.a = salesaccount;
                obj.b = medicalcontact;
                obj.c = supportiveaccount;
                res.Add(obj);
            }
            return res;

        }

        public IEnumerable<ExtendIdentityUser> GetAllFirstManagersAndTopManagers()
        {
            List<ExtendIdentityUser> res = new List<ExtendIdentityUser>();

            List<ExtendIdentityUser> f = userManager.GetUsersInRoleAsync("First Line Manager").Result.ToList();

            List<ExtendIdentityUser> t = userManager.GetUsersInRoleAsync("Top Line Manager").Result.ToList();

            foreach (var item in f)
            {
                res.Add(item);
            }
            foreach (var item in t)
            {
                res.Add(item);
            }

            return res.OrderBy(a=>a.FullName);
        }
    }
}

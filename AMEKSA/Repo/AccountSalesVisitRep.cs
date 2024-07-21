using AMEKSA.AccountSalesVisitModels;
using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AMEKSA.Repo
{
    public class AccountSalesVisitRep:IAccountSalesVisitRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public AccountSalesVisitRep(DbContainer db,ITimeRep ti, UserManager<ExtendIdentityUser> userManager)
        {
            this.db = db;
            this.ti = ti;
            this.userManager = userManager;
        }

        public void AddPersonsToExistVisit(IEnumerable<AccountSalesVisitPerson> list)
        {
            db.accountSalesVisitPerson.AddRange(list);
            db.SaveChanges();
        }

        public void AddBrandsToExistVisit(IEnumerable<AccountSalesVisitBrand> list)
        {
            db.accountSalesVisitBrand.AddRange(list);
            db.SaveChanges();
        }

        public void EditVisit(AccountSalesVisit obj)
        {
            AccountSalesVisit OldVisit = db.accountSalesVisit.Find(obj.Id);
            OldVisit.PaymentNotes = obj.PaymentNotes;
            OldVisit.VisitDate = obj.VisitDate;
            OldVisit.VisitTime = obj.VisitTime;
            OldVisit.VisitNotes = obj.VisitNotes;
            db.SaveChanges();
            foreach (var item in obj.accountsalesvisitperson)
            {
                AccountSalesVisitPerson old = db.accountSalesVisitPerson.Find(item.Id);
                old.PersonName = item.PersonName;
                old.PersonPosition = item.PersonPosition;
                db.SaveChanges();
                
            }
            foreach (var item in obj.accountsalesvisitbrand)
            {
                AccountSalesVisitBrand old = db.accountSalesVisitBrand.Find(item.Id);
                old.BrandId = item.BrandId;
                db.SaveChanges();
            }
            
        }

        //public CustomAccountSalesVisit GetAccountSalesVisitById(int id)
        //{
        //    IEnumerable<CustomAccountSalesVisitBrand> brandlist = db.brand
        //       .Join(db.accountSalesVisitBrand,
        //       a => a.Id,
        //       b => b.BrandId,
        //       (a, b) => new CustomAccountSalesVisitBrand
        //       {
        //           AccountSalesVisitId = b.AccountSalesVisitId,
        //           BrandId = a.Id,
        //           BrandName = a.BrandName
        //       });

        //    IEnumerable<AccountSalesVisitPerson> personlist = db.accountSalesVisitPerson.Select(a => a);

        //    CustomAccountSalesVisit visit = db.accountSalesVisit.Where(a => a.Id == id)
        //        .Join(brandlist,
        //        a => a.Id,
        //        b => b.AccountSalesVisitId,
        //        (a, b) => new
        //        {
        //            AccountId = a.AccountId,
        //            accountsalesvisitbrands = brandlist.Where(x => x.AccountSalesVisitId == a.Id),
        //            Id = a.Id,
        //            PaymentNotes = a.PaymentNotes,
        //            VisitNotes = a.VisitNotes,
        //            VisitDate = a.VisitDate,
        //            VisitTime = a.VisitTime

        //        }).Join(db.account,
        //        a => a.AccountId,
        //        b => b.Id,
        //        (a, b) => new
        //        {
        //            AccountId = a.AccountId,
        //            accountsalesvisitbrands = a.accountsalesvisitbrands,
        //            Id = a.Id,
        //            PaymentNotes = a.PaymentNotes,
        //            VisitNotes = a.VisitNotes,
        //            VisitDate = a.VisitDate,
        //            VisitTime = a.VisitTime,
        //            AccountName = b.AccountName
        //        }).Join(personlist, a => a.Id,
        //        b => b.AccountSalesVisitId,
        //        (a, b) => new CustomAccountSalesVisit
        //        {
        //            Id = a.Id,
        //            AccountId = a.AccountId,
        //            AccountName = a.AccountName,
        //            VisitNotes = a.VisitNotes,
        //            PaymentNotes = a.PaymentNotes,
        //            VisitDate = a.VisitDate,
        //            VisitTime = a.VisitTime,
        //            accountsalesvisitbrands = a.accountsalesvisitbrands,
        //            accountsalesvisitperson = personlist.Where(x => x.AccountSalesVisitId == a.Id)
        //        }).SingleOrDefault();

        //    return visit;
        //}

       // public IEnumerable<CustomAccountSalesVisit> GetAllMyAccountSalesVisits(string userId)
        //{
        //    IEnumerable<CustomAccountSalesVisitBrand> brandlist = db.brand
        //        .Join(db.accountSalesVisitBrand,
        //        a => a.Id,
        //        b => b.BrandId,
        //        (a, b) => new CustomAccountSalesVisitBrand
        //        {
        //            AccountSalesVisitId = b.AccountSalesVisitId,
        //            BrandId = a.Id,
        //            BrandName = a.BrandName
        //        });

        //    IEnumerable<AccountSalesVisitPerson> personlist = db.accountSalesVisitPerson.Select(a => a);

        //    IEnumerable<CustomAccountSalesVisit> visitlist = db.accountSalesVisit.Where(a=>a.extendidentityuser.Id==userId)
        //        .Join(brandlist,
        //        a => a.Id,
        //        b => b.AccountSalesVisitId,
        //        (a, b) => new
        //        {
        //            AccountId = a.AccountId,
        //            accountsalesvisitbrands = brandlist.Where(x => x.AccountSalesVisitId == a.Id),
        //            Id = a.Id,
        //            PaymentNotes = a.PaymentNotes,
        //            VisitNotes = a.VisitNotes,
        //            VisitDate = a.VisitDate,
        //            VisitTime = a.VisitTime

        //        }).Join(db.account,
        //        a => a.AccountId,
        //        b => b.Id,
        //        (a, b) => new 
        //        {
        //            AccountId = a.AccountId,
        //            accountsalesvisitbrands = a.accountsalesvisitbrands,
        //            Id = a.Id,
        //            PaymentNotes = a.PaymentNotes,
        //            VisitNotes = a.VisitNotes,
        //            VisitDate = a.VisitDate,
        //            VisitTime = a.VisitTime,
        //            AccountName = b.AccountName
        //        }).Join(personlist, a => a.Id,
        //        b => b.AccountSalesVisitId,
        //        (a, b) => new CustomAccountSalesVisit
        //        {
        //            Id = a.Id,
        //            AccountId = a.AccountId,
        //            AccountName = a.AccountName,
        //            VisitNotes = a.VisitNotes,
        //            PaymentNotes = a.PaymentNotes,                 
        //            VisitDate = a.VisitDate,
        //            VisitTime = a.VisitTime,
        //            accountsalesvisitbrands = a.accountsalesvisitbrands,
        //            accountsalesvisitperson = personlist.Where(x => x.AccountSalesVisitId == a.Id)
        //        });

        //    return visitlist;
            //IEnumerable<AccountSalesVisit> VisitList = db.accountSalesVisit.Where(a => a.extendidentityuser.Id == userId);

            //List<AccountSalesVisitPerson> personlist = new List<AccountSalesVisitPerson>();
            //List<AccountSalesVisitBrand> brandlist = new List<AccountSalesVisitBrand>();

            //foreach (var visit in VisitList)
            //{
            //    IEnumerable<AccountSalesVisitPerson> person = db.accountSalesVisitPerson.Where(a => a.AccountSalesVisitId == visit.Id);
            //    personlist.AddRange(person);
            //}

            //foreach (var visit in VisitList)
            //{
            //    IEnumerable<AccountSalesVisitBrand> brand = db.accountSalesVisitBrand.Where(a => a.AccountSalesVisitId == visit.Id);
            //    brandlist.AddRange(brand);
            //}

            //return VisitList
            //    .Join(personlist,
            //    a => a.Id,
            //    b => b.AccountSalesVisitId,
            //    (a, b) => new
            //    {
            //        Id = a.Id,
            //        AccountId = a.AccountId,
            //        VisitDate = a.VisitDate,
            //        VisitTime = a.VisitTime,
            //        VisitNotes = a.VisitNotes,
            //        PaymentNotes = a.PaymentNotes,
            //        AccountSalesVisitPerson = personlist,
            //    }).Join(brandlist,
            //    a => a.Id,
            //    b => b.AccountSalesVisitId,
            //    (a, b) => new
            //    {
            //        Id = a.Id,
            //        AccountId = a.AccountId,
            //        VisitDate = a.VisitDate,
            //        VisitTime = a.VisitTime,
            //        VisitNotes = a.VisitNotes,
            //        PaymentNotes = a.PaymentNotes,
            //        AccountSalesVisitPerson = a.AccountSalesVisitPerson,
            //        AccountSalesVisitBrand = brandlist
            //    }).Join(db.account,
            //    a => a.AccountId,
            //    b => b.Id,
            //    (a, b) => new CustomAccountSalesVisit
            //    {
            //        Id = a.Id,
            //        AccountId = a.AccountId,
            //        AccountName = b.AccountName,
            //        VisitDate = a.VisitDate,
            //        VisitTime = a.VisitTime,
            //        VisitNotes = a.VisitNotes,
            //        PaymentNotes = a.PaymentNotes,
            //        accountsalesvisitperson = a.AccountSalesVisitPerson,
            //        accountsalesvisitbrands = a.AccountSalesVisitBrand
            //    });

        //}

        public void MakeACollection(AccountBrandPayment obj)
        {
            AccountBrandPayment payment = db.accountBrandPayment.Find(obj.Id);
            payment.Collection = obj.Collection;
            payment.Balance = 0;
            db.SaveChanges();
        }

        public bool MakeVisit(AccountSalesVisitModel obj)
        {

            DateTime datenow = ti.GetCurrentTime();
            int monthdays = DateTime.DaysInMonth(obj.VisitDate.Year, obj.VisitDate.Month);
            DateTime planstart = new DateTime(obj.VisitDate.Year, obj.VisitDate.Month, 1);
            DateTime planend = ti.GetCurrentTime().Date;

            AccountSalesVisit visit = new AccountSalesVisit();
            visit.VisitDate = obj.VisitDate;
            visit.VisitTime = obj.VisitTime;
            visit.SubmittingDate = datenow;
            visit.SubmittingTime = datenow;
            visit.AccountId = obj.AccountId;
            visit.extendidentityuserid = obj.extendidentityuserid;
            visit.VisitNotes = obj.VisitNotes;
            visit.PaymentNotes = obj.PaymentNotes;
            visit.ManagerId = obj.ManagerId;
            db.accountSalesVisit.Add(visit);
            db.SaveChanges();

            foreach (var item in obj.BrandsIds)
            {
                AccountSalesVisitBrand brands = new AccountSalesVisitBrand();
                brands.BrandId = item;
                brands.AccountSalesVisitId = visit.Id;
                db.accountSalesVisitBrand.Add(brands);
                db.SaveChanges();
            }

            foreach (var item in obj.PersonModel)
            {
                AccountSalesVisitPerson persons = new AccountSalesVisitPerson();
                persons.PersonName = item.PersonName;
                persons.PersonPosition = item.PersonPosition;
                persons.AccountSalesVisitId = visit.Id;
                db.accountSalesVisitPerson.Add(persons);
                db.SaveChanges();
            }

            List<BalanceCalculationModel> calc = new List<BalanceCalculationModel>();

            if (obj.CollectionModel != null)
            {
                foreach (var item in obj.CollectionModel)
                {
                    AccountSalesVisitCollection col = new AccountSalesVisitCollection();
                    col.BrandId = item.BrandId;
                    col.Collection = item.Collection;
                    col.AccountSalesVisitId = visit.Id;
                    db.accountSalesVisitCollection.Add(col);
                    db.SaveChanges();
                    BalanceCalculationModel o = new BalanceCalculationModel();
                    o.BrandId = item.BrandId;
                    o.Collection = item.Collection;
                    calc.Add(o);
                }
            }


            if (obj.BalanceModel != null)
            {

                    foreach (var item in obj.BalanceModel)
                    {
                        AccountBalance bal = db.accountBalance.Where(a => a.AccountId == obj.AccountId && a.BrandId == item.BrandId).FirstOrDefault();
                        float currentbalance = bal.Balance;
                         float currentcollection = 0;
                        BalanceCalculationModel currenctbrand = calc.Where(a => a.BrandId == item.BrandId).SingleOrDefault();
                    if (currenctbrand == null)
                    {

                    }
                    else
                    {
                        currentcollection = currenctbrand.Collection;
                    }
                        float newbalance = currentbalance + item.InvoiceValue - currentcollection;
                    bal.Balance = newbalance;
                    db.SaveChanges();

                    }
               
            }


            AccountMonthlyPlan todayplan = db.accountMonthlyPlan.Where(a => a.ExtendIdentityUserId == obj.extendidentityuserid && a.AccountId == obj.AccountId && a.Date == obj.VisitDate && a.Status == false).FirstOrDefault();
            if (todayplan != null)
            {
                todayplan.Status = true;
                todayplan.AccountSalesVisitId = visit.Id;
                db.SaveChanges();
                return true;
            }
            else
            {
                AccountMonthlyPlan plan = db.accountMonthlyPlan.Where(a => a.ExtendIdentityUserId == obj.extendidentityuserid && a.AccountId == obj.AccountId && a.Date >= planstart && a.Date <= planend && a.Status == false).OrderBy(b => b.PlannedDate).FirstOrDefault();
                if (plan != null)
                {
                    plan.Status = true;
                    plan.AccountSalesVisitId = visit.Id;
                    db.SaveChanges();
                }
                return true;
            }



        }

        public IEnumerable<CustomAccount> GetAccountsToVisitNow(string userId)
        {
            

            TimeSpan now = ti.GetCurrentTime().TimeOfDay;

            List<int> Ids = db.userAccount.Where(a => a.extendidentityuserid == userId).Select(a => a.AccountId).ToList();

            List<Account> acc = new List<Account>();

            foreach (var id in Ids)
            {
                Account checkaccount = db.account.Where(a => a.Id == id && a.BestTimeFrom.HasValue && a.BestTimeTo.HasValue).SingleOrDefault();
                if (checkaccount != null)
                {
                    acc.Add(checkaccount);
                }
            }
            
            List<Account> result = new List<Account>();
            foreach (var item in acc)
            {
                DateTime ff = DateTime.Parse(item.BestTimeFrom.ToString());
                DateTime tt = DateTime.Parse(item.BestTimeTo.ToString());

                string from = ff.ToString(@"yyyy\:MM\:dd\:HH\:mm\:ss\:fffffff").Substring(11,5);
               
                 string to = tt.ToString(@"yyyy\:MM\:dd\:HH\:mm\:ss\:fffffff").Substring(11,5);

               TimeSpan f =  TimeSpan.Parse(from);
               TimeSpan t = TimeSpan.Parse(to);
                if (f <= now && t >= now)
                {
                   result.Add(item);
                }
            }

            IEnumerable<CustomAccount> res = result.Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new CustomAccount
            {
                Id = a.Id,
                AccountName = a.AccountName,
                DistrictName = b.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo
            });

            return res;
        }

        public IEnumerable<TimeSpan> test()
        {
            TimeSpan now = DateTime.Now.TimeOfDay;

            IEnumerable<Account> acc = db.account.Where(a => a.BestTimeFrom.HasValue && a.BestTimeTo.HasValue);
            List<TimeSpan> result = new List<TimeSpan>();

            foreach (var item in acc)
            {
                DateTime ff = DateTime.Parse(item.BestTimeFrom.ToString());
                DateTime tt = DateTime.Parse(item.BestTimeTo.ToString());



                string from = ff.ToString(@"yyyy\:MM\:dd\:HH\:mm\:ss\:fffffff").Substring(11, 5);

                string to = tt.ToString(@"yyyy\:MM\:dd\:HH\:mm\:ss\:fffffff").Substring(11, 5);

                TimeSpan f = TimeSpan.Parse(from);
                TimeSpan t = TimeSpan.Parse(to);

                result.Add(f);
                result.Add(t);
               
            }

            return result;
        }

        public bool MakeACollection(MakeCollection obj)
        {
            AccountBrandPayment result = db.accountBrandPayment.Find(obj.Id);

            result.Collection = result.Collection + obj.Collection;
            result.Balance = result.Openning - result.Collection;
            db.SaveChanges();
            return true;
        }

        public bool RequestNewOpenning(OpenningRequest obj)
        {
            db.openningRequest.Add(obj);
            db.SaveChanges();
            return true;
        }

        public IEnumerable<CustomOpenningRequest> GetMyRequests(string userId)
        {
            IEnumerable<CustomOpenningRequest> result = db.accountBrandPayment.Join(db.openningRequest.Where(u => u.ExtendIdentityUserId == userId), a => a.Id, b => b.AccountBrandPaymentId, (a, b) => new
            {
                Id = b.Id,
                BrandId = a.BrandId,
                AccountBrandPaymentId = a.Id,
                AccountId = a.AccountId,
                CurrentOpenning = a.Openning,
                CurrentCollection = a.Collection,
                CurrentBalance = a.Balance,
                RequestedOpenning = b.RequestedOpenning,
                NewOpenning = a.Openning + b.RequestedOpenning,
                NewCollection = a.Collection,
                ExtendIdentityUserId = b.ExtendIdentityUserId,
                Confirmed = b.Confirmed,
                Rejected = b.Rejected
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountBrandPaymentId = a.AccountBrandPaymentId,
                AccountId = a.AccountId,
                AccountName = b.AccountName,
                BrandId = a.BrandId,
                CurrentOpenning = a.CurrentOpenning,
                CurrentCollection = a.CurrentCollection,
                CurrentBalance = a.CurrentBalance,
                RequestedOpenning = a.RequestedOpenning,
                NewOpenning = a.NewOpenning,
                NewCollection = a.NewCollection,
                NewBalance = a.NewOpenning - a.NewCollection,
                ExtendIdentityUserId = a.ExtendIdentityUserId,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected
            }).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountBrandPaymentId = a.AccountBrandPaymentId,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                BrandId = a.BrandId,
                BrandName = b.BrandName,
                CurrentOpenning = a.CurrentOpenning,
                CurrentCollection = a.CurrentCollection,
                CurrentBalance = a.CurrentBalance,
                RequestedOpenning = a.RequestedOpenning,
                NewOpenning = a.NewOpenning,
                NewCollection = a.NewCollection,
                NewBalance = a.NewOpenning - a.NewCollection,
                ExtendIdentityUserId = a.ExtendIdentityUserId,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected
            }).Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new CustomOpenningRequest
            {
                Id = a.Id,
                AccountBrandPaymentId = a.AccountBrandPaymentId,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                BrandId = a.BrandId,
                BrandName = a.BrandName,
                CurrentOpenning = a.CurrentOpenning,
                CurrentCollection = a.CurrentCollection,
                CurrentBalance = a.CurrentBalance,
                RequestedOpenning = a.RequestedOpenning,
                NewOpenning = a.NewOpenning,
                NewCollection = a.NewCollection,
                NewBalance = a.NewOpenning - a.NewCollection,
                ExtendIdentityUserId = a.ExtendIdentityUserId,
                Confirmed = a.Confirmed,
                Rejected = a.Rejected,
                UserName = b.FullName
            });

            return result;
            
        }

        public IEnumerable<CustomAccount> GetMyAccounts(string userId)
        {

            List<int> Ids = db.userAccount.Where(a => a.extendidentityuserid == userId).Select(a => a.AccountId).ToList();

            List<Account> acc = new List<Account>();

            foreach (var id in Ids)
            {
                Account checkaccount = db.account.Find(id);
                acc.Add(checkaccount);       
            }



            List<CustomAccount> res = acc.Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CategoryId = a.CategoryId,
                PurchaseTypeId = a.PurchaseTypeId,
                AccountName = a.AccountName,
                DistrictName = b.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                AccountTypeId = a.AccountTypeId,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                CategoryId = a.CategoryId,
                AccountName = a.AccountName,
                PurchaseTypeId = a.PurchaseTypeId,
                DistrictName = a.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                AccountTypeId = a.AccountTypeId,
                AccountTypeName = b.AccountTypeName,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email
            }).Join(db.category,a=>a.CategoryId,b=>b.Id,(a,b)=> new {
                Id = a.Id,
                CategoryName = b.CategoryName,
                AccountName = a.AccountName,
                PurchaseTypeId = a.PurchaseTypeId,
                DistrictName = a.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                AccountTypeId = a.AccountTypeId,
                AccountTypeName = a.AccountTypeName,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email
            }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new CustomAccount
            {
                Id = a.Id,
                CategoryName = a.CategoryName,
                AccountName = a.AccountName,
                PurchaseTypeId = a.PurchaseTypeId,
                PurchaseTypeName = b.PurchaseTypeName,
                DistrictName = a.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                AccountTypeId = a.AccountTypeId,
                AccountTypeName = a.AccountTypeName,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email

            }).ToList();

            return res;
        }

        public IEnumerable<CustomAccountSalesVisit> GetMyVisits(string userId)
        {
            var visitsids = db.accountSalesVisit.Where(a => a.extendidentityuserid == userId).Select(a => a.Id);

            List<CustomVisitBrand> brandlist = new List<CustomVisitBrand>();
            List<CustomVisitPerson> personlist = new List<CustomVisitPerson>();
            foreach (var item in visitsids)
            {
                var s = db.accountSalesVisitBrand.Where(a => a.AccountSalesVisitId == item).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new
                {
                    BrandName = b.BrandName,
                    KeyId = a.AccountSalesVisitId
                });
                foreach (var brand in s)
                {
                    CustomVisitBrand ad = new CustomVisitBrand();
                    ad.BrandName = brand.BrandName;
                    ad.KeyId = brand.KeyId;
                    brandlist.Add(ad);
                }

                var ss = db.accountSalesVisitPerson.Where(a => a.AccountSalesVisitId == item);
                foreach (var person in ss)
                {
                    CustomVisitPerson ab = new CustomVisitPerson();
                    ab.PersonName = person.PersonName;
                    ab.PersonPosition = person.PersonPosition;
                    ab.KeyId = person.AccountSalesVisitId;
                    personlist.Add(ab);
                }

            }




            IEnumerable<CustomAccountSalesVisit> result = db.accountSalesVisit.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = b.AccountName,
                AccountTypeId = b.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = b.DistrictId
            }).Join(db.accountSalesVisitBrand, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                brands = brandlist.Where(x => x.KeyId == a.Id).ToList(),
                persons = personlist.Where(c => c.KeyId == a.Id).ToList(),
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = a.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                brands = a.brands,
                persons = a.persons,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = a.DistrictId,
                DistrictName = b.DistrictName
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new CustomAccountSalesVisit
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                brands = a.brands,
                persons = a.persons,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = b.AccountTypeName
            }).DistinctBy(o => o.Id).OrderByDescending(a => a.VisitDate);


            
            return result;
        }

        public CustomAccountSalesVisit GetVisitById(int visitId)
        {
            var visitsids = db.accountSalesVisit.Where(a => a.Id == visitId).Select(a => a.Id).ToList();

            List<CustomVisitBrand> brandlist = new List<CustomVisitBrand>();
            List<CustomVisitPerson> personlist = new List<CustomVisitPerson>();
            foreach (var item in visitsids)
            {
                var s = db.accountSalesVisitBrand.Where(a => a.AccountSalesVisitId == item).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new
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

                var ss = db.accountSalesVisitPerson.Where(a => a.AccountSalesVisitId == item);
                foreach (var person in ss)
                {
                    CustomVisitPerson ab = new CustomVisitPerson();
                    ab.PersonName = person.PersonName;
                    ab.PersonPosition = person.PersonPosition;
                    ab.KeyId = person.AccountSalesVisitId;
                    personlist.Add(ab);
                }

            }




            CustomAccountSalesVisit result = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(n => n.Id == visitId).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = b.AccountName,
                AccountTypeId = b.AccountTypeId,
                Address = b.Address,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = b.DistrictId,
                PhoneNumber = b.PhoneNumber,
                Email = b.Email,
                NumberOfDoctors = b.NumberOfDoctors,
                PurchaseTypeId = b.PurchaseTypeId,
                CategoryId = b.CategoryId
            }).Join(db.accountSalesVisitBrand, a => a.Id, b => b.AccountSalesVisitId, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                Address = a.Address,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                NumberOfDoctors = a.NumberOfDoctors,
                PaymentNotes = a.PaymentNotes,
                DistrictId = a.DistrictId,
                PurchaseTypeId = a.PurchaseTypeId,
                CategoryId = a.CategoryId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                Address = a.Address,
                AccountTypeId = a.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                NumberOfDoctors = a.NumberOfDoctors,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = a.DistrictId,
                DistrictName = b.DistrictName,
                PurchaseTypeId = a.PurchaseTypeId,
                CategoryId = a.CategoryId
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                Address = a.Address,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictName = a.DistrictName,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                NumberOfDoctors = a.NumberOfDoctors,
                AccountTypeName = b.AccountTypeName,
                PurchaseTypeId = a.PurchaseTypeId,
                CategoryId = a.CategoryId
            }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                Address = a.Address,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictName = a.DistrictName,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                NumberOfDoctors = a.NumberOfDoctors,
                AccountTypeName = a.AccountTypeName,
                PurchaseTypeId = a.PurchaseTypeId,
                CategoryId = a.CategoryId,
                CategoryName = b.CategoryName
            }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new CustomAccountSalesVisit
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                Address = a.Address,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictName = a.DistrictName,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                NumberOfDoctors = a.NumberOfDoctors,
                AccountTypeName = a.AccountTypeName,
                CategoryName = a.CategoryName,
                PurchaseTypeName = b.PurchaseTypeName
            }),o=>o.Id).SingleOrDefault();
                
       


            result.brands = brandlist.Where(x => x.KeyId == result.Id).ToList();
            result.persons = personlist.Where(c => c.KeyId == result.Id).ToList();




            return result;
        }

        public IEnumerable<CustomAccountSalesVisit> GetMyVisitsByDate(AccountSalesVisitByDateModel obj)
        {
            List<int> visitsids = db.accountSalesVisit.Where(a => a.extendidentityuserid == obj.UserId && a.VisitDate <= obj.Start && a.VisitDate >= obj.End).Select(a => a.Id).ToList();

            List<CustomVisitBrand> brandlist = new List<CustomVisitBrand>();
            List<CustomVisitPerson> personlist = new List<CustomVisitPerson>();
            foreach (var item in visitsids)
            {
                var s = db.accountSalesVisitBrand.Where(a => a.AccountSalesVisitId == item).Join(db.brand, a => a.BrandId, b => b.Id, (a, b) => new
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

                var ss = db.accountSalesVisitPerson.Where(a => a.AccountSalesVisitId == item).ToList();
                foreach (var person in ss)
                {
                    CustomVisitPerson ab = new CustomVisitPerson();
                    ab.PersonName = person.PersonName;
                    ab.PersonPosition = person.PersonPosition;
                    ab.KeyId = person.AccountSalesVisitId;
                    personlist.Add(ab);
                }

            }




            List<CustomAccountSalesVisit> result = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.extendidentityuserid == obj.UserId && a.VisitDate >= obj.Start && a.VisitDate <= obj.End).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = b.AccountName,
                AccountTypeId = b.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictId = b.DistrictId
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
                DistrictId = a.DistrictId
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
                DistrictName = b.DistrictName
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new CustomAccountSalesVisit
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = b.AccountTypeName
            }), o => o.Id).OrderByDescending(a => a.VisitDate).ToList();

            List<CustomAccountSalesVisit> res = new List<CustomAccountSalesVisit>();
            foreach (var item in result)
            {

                item.brands = brandlist.Where(a=>a.KeyId == item.Id).ToList();
                item.persons = personlist.Where(a=>a.KeyId == item.Id).ToList();


                RequestDeleteAccountSales check = db.requestDeleteAccountSales.Where(a => a.AccountSalesVisitId == item.Id).FirstOrDefault();
                if (check == null)
                {
                    item.Requested = false;
                }
                else
                {
                    item.Requested = true;
                }
                res.Add(item);
            }


            return res;
        }

        public bool RequestDeleteAccountSales(int VisitId)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(db.accountSalesVisit.Find(VisitId).extendidentityuserid).Result;
            IEnumerable<ExtendIdentityUser> admins = userManager.GetUsersInRoleAsync("System Admin").Result;
            foreach (var item in admins)
            {
                Notifications n = new Notifications();
                n.NitificationDateTime = ti.GetCurrentTime();
                n.ExtendIdetityUserId = item.Id;
                n.NotificationDetails = user.FullName + " requested to delete visit to contact " + db.account.Find(db.accountSalesVisit.Find(VisitId).AccountId).AccountName;
                n.Url = "VisitsDeletingRequests.html";
                db.notifications.Add(n);
            }

            RequestDeleteAccountSales obj = new RequestDeleteAccountSales();

            obj.AccountSalesVisitId = VisitId;
            db.requestDeleteAccountSales.Add(obj);
            db.SaveChanges();
            return true;
        }

        public IEnumerable<CustomAccountSalesVisit> GetASVDeleteRequests()
        {

            IEnumerable<CustomAccountSalesVisit> results = db.requestDeleteAccountSales.Join(db.accountSalesVisit, a => a.AccountSalesVisitId, b => b.Id, (a, b) => new
            {
                Id = b.Id,
                VisitDate = b.VisitDate,
                AccountId = b.AccountId,
                UserId = b.extendidentityuserid
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                VisitDate = a.VisitDate,
                AccountName = b.AccountName,
                UserId = a.UserId
            }).Join(db.Users, a => a.UserId, b => b.Id, (a, b) => new CustomAccountSalesVisit
            {
                Id = a.Id,
                VisitDate = a.VisitDate,
                AccountName = a.AccountName,
                UserName = b.FullName
            });

            return results;
        }

        public bool ConfirmASVDeleting(int visitid)
        {
            AccountMonthlyPlan plan = db.accountMonthlyPlan.Where(a => a.AccountSalesVisitId == visitid).FirstOrDefault();

            if (plan == null)
            {

            }
            else
            {
                plan.AccountSalesVisitId = null;
                plan.Status = false;
                db.SaveChanges();
            }

            IEnumerable<AccountSalesVisitChat> chat = db.accountSalesVisitChat.Where(a => a.AccountSalesVisitId == visitid);

            foreach (var item in chat)
            {
                db.accountSalesVisitChat.Remove(item);
            }

            IEnumerable<AccountSalesVisitBrand> visitbrands = db.accountSalesVisitBrand.Where(a => a.AccountSalesVisitId == visitid);

            foreach (var item in visitbrands)
            {
                db.accountSalesVisitBrand.Remove(item);
            }
            IEnumerable<AccountSalesVisitPerson> visitpersons = db.accountSalesVisitPerson.Where(a => a.AccountSalesVisitId == visitid);
            foreach (var item in visitpersons)
            {
                db.accountSalesVisitPerson.Remove(item);
            }
            IEnumerable<RequestDeleteAccountSales> request = db.requestDeleteAccountSales.Where(a => a.AccountSalesVisitId == visitid);

            foreach (var item in request)
            {
                db.requestDeleteAccountSales.Remove(item);
            }
            IEnumerable<AccountSalesVisitCollection> col = db.accountSalesVisitCollection.Where(a => a.AccountSalesVisitId == visitid);
            foreach (var item in col)
            {
                db.accountSalesVisitCollection.Remove(item);
            }

            AccountSalesVisit visit = db.accountSalesVisit.Find(visitid);
            Account a = db.account.Find(visit.AccountId);
            Notifications n = new Notifications();
            n.NitificationDateTime = ti.GetCurrentTime();
            n.ExtendIdetityUserId = visit.extendidentityuserid;
            n.NotificationDetails = "Your request to delete visit to " + a.AccountName + " on " + visit.VisitDate.ToString("dddd dd/MM/yyyy") + " has been accepted and the visit was deleted";
            n.Url = "MyVisits.html";
            db.notifications.Add(n);
            db.accountSalesVisit.Remove(visit);
            db.SaveChanges();
            return true;
        }

        public bool RejectASVDeleting(int visitid)
        {
            IEnumerable<RequestDeleteAccountSales> requests = db.requestDeleteAccountSales.Where(a => a.AccountSalesVisitId == visitid);
            foreach (var item in requests)
            {
                db.requestDeleteAccountSales.Remove(item);
            }
            AccountSalesVisit visit = db.accountSalesVisit.Find(visitid);
            Account a = db.account.Find(visit.AccountId);
            Notifications n = new Notifications();
            n.NitificationDateTime = ti.GetCurrentTime();
            n.ExtendIdetityUserId = visit.extendidentityuserid;
            n.NotificationDetails = "Your request to delete visit to " + a.AccountName + " on " + visit.VisitDate.ToString("dddd dd/MM/yyyy") + " has been rejected";
            n.Url = "MyVisits.html";
            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }
    }
}

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
    public class AccountSupportiveVisitRep : IAccountSupportiveVisitRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public AccountSupportiveVisitRep(DbContainer db, ITimeRep ti, UserManager<ExtendIdentityUser> userManager)
        {
            this.db = db;
            this.ti = ti;
            this.userManager = userManager;
        }

        public bool ConfirmAccountSupportiveVDeleting(int visitid)
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

            IEnumerable<AccountSupportiveVisitProduct> visitbrands = db.AccountSupportiveVisitproduct.Where(a => a.AccountSupportVisitId == visitid);

            foreach (var item in visitbrands)
            {
                db.AccountSupportiveVisitproduct.Remove(item);
            }
            IEnumerable<AccountSupportiveVisitPerson> visitpersons = db.AccountSupportiveVisitPerson.Where(a => a.AccountSupportiveVisitId == visitid);
            foreach (var item in visitpersons)
            {
                db.AccountSupportiveVisitPerson.Remove(item);
            }
            IEnumerable<RequestDeleteAccountSupportive> request = db.RequestDeleteAccountSupportive.Where(a => a.AccountSupportiveVisitId == visitid);

            foreach (var item in request)
            {
                db.RequestDeleteAccountSupportive.Remove(item);
            }
          
            

            AccountSupportiveVisit visit = db.AccountSupportiveVisit.Find(visitid);
            Account a = db.account.Find(visit.AccountId);
            Notifications n = new Notifications();
            n.NitificationDateTime = ti.GetCurrentTime();
            n.ExtendIdetityUserId = visit.extendidentityuserid;
            n.NotificationDetails = "Your request to delete visit to " + a.AccountName + " on " + visit.VisitDate.ToString("dddd dd/MM/yyyy") + " has been accepted and the visit was deleted";
            n.Url = "MyAccountsVisits.html";
            db.notifications.Add(n);
            db.AccountSupportiveVisit.Remove(visit);
            db.SaveChanges();
            return true;
        }

        public IEnumerable<CustomAccountSupportiveVisit> GetAccountSupportiveDeleteRequests()
        {
            IEnumerable<CustomAccountSupportiveVisit> results = db.RequestDeleteAccountSupportive.Join(db.AccountSupportiveVisit, a => a.AccountSupportiveVisitId, b => b.Id, (a, b) => new
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
            }).Join(db.Users, a => a.UserId, b => b.Id, (a, b) => new CustomAccountSupportiveVisit
            {
                Id = a.Id,
                VisitDate = a.VisitDate,
                AccountName = a.AccountName,
                UserName = b.FullName
            });

            return results;
        }

        public IEnumerable<CustomAccountSupportiveVisit> GetMyVisitsByDate(AccountSalesVisitByDateModel obj)
        {
            var visitsids = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == obj.UserId && a.VisitDate <= obj.Start && a.VisitDate >= obj.End).Select(a => a.Id);

            List<CustomVisitProduct> productlist = new List<CustomVisitProduct>();
            List<CustomVisitPerson> personlist = new List<CustomVisitPerson>();
            foreach (var item in visitsids)
            {
                var s = db.AccountSupportiveVisitproduct.Where(a => a.AccountSupportVisitId == item).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
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

                var ss = db.AccountSupportiveVisitPerson.Where(a => a.AccountSupportiveVisitId == item);
                foreach (var person in ss)
                {
                    CustomVisitPerson ab = new CustomVisitPerson();
                    ab.PersonName = person.PersonName;
                    ab.PersonPosition = person.PersonPosition;
                    ab.KeyId = person.AccountSupportiveVisitId;
                    personlist.Add(ab);
                }

            }




            List<CustomAccountSupportiveVisit> result = DistinctByExtension.DistinctBy(db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == obj.UserId && a.VisitDate >= obj.Start && a.VisitDate <= obj.End).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = b.AccountName,
                AccountTypeId = b.AccountTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.AdditionalNotes,
                DistrictId = b.DistrictId
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
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new CustomAccountSupportiveVisit
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.PaymentNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = b.AccountTypeName
            }),o=>o.Id).OrderByDescending(a => a.VisitDate).ToList();

            List<CustomAccountSupportiveVisit> res = new List<CustomAccountSupportiveVisit>();
            foreach (var item in result)
            {
                item.products = productlist.Where(x => x.KeyId == item.Id);
                item.persons = personlist.Where(x => x.KeyId == item.Id);



                RequestDeleteAccountSupportive check = db.RequestDeleteAccountSupportive.Where(a => a.AccountSupportiveVisitId == item.Id).FirstOrDefault();
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

        public CustomAccountSupportiveVisit GetVisitById(int visitId)
        {
            var visitsids = db.AccountSupportiveVisit.Where(a => a.Id == visitId).Select(a => a.Id).ToList();

            List<CustomVisitProduct> productlist = new List<CustomVisitProduct>();
            List<CustomVisitPerson> personlist = new List<CustomVisitPerson>();
            foreach (var item in visitsids)
            {
                var s = db.AccountSupportiveVisitproduct.Where(a => a.AccountSupportVisitId == item).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
                {
                    ProductName = b.ProductName,
                    AccountSupportVisitId = a.AccountSupportVisitId,
                }).ToList();

                foreach (var product in s)
                {
                    CustomVisitProduct ad = new CustomVisitProduct();
                    ad.ProductName = product.ProductName;
                    ad.KeyId = product.AccountSupportVisitId;
                    productlist.Add(ad);
                }

                var ss = db.AccountSupportiveVisitPerson.Where(a => a.AccountSupportiveVisitId == item);
                foreach (var person in ss)
                {
                    CustomVisitPerson ab = new CustomVisitPerson();
                    ab.PersonName = person.PersonName;
                    ab.PersonPosition = person.PersonPosition;
                    ab.KeyId = person.AccountSupportiveVisitId;
                    personlist.Add(ab);
                }

            }




            CustomAccountSupportiveVisit result = DistinctByExtension.DistinctBy(db.AccountSupportiveVisit.Where(n => n.Id == visitId).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                UserId = a.extendidentityuserid,
                AccountId = a.AccountId,
                AccountName = b.AccountName,
                AccountTypeId = b.AccountTypeId,
                Address = b.Address,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = b.DistrictId,
                PhoneNumber = b.PhoneNumber,
                Email = b.Email,
                NumberOfDoctors = b.NumberOfDoctors,
                PurchaseTypeId = b.PurchaseTypeId,
                CategoryId = b.CategoryId
            }).Join(db.AccountSupportiveVisitproduct, a => a.Id, b => b.AccountSupportVisitId, (a, b) => new
            {
                Id = a.Id,
                UserId = a.UserId,
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
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = a.DistrictId,
                PurchaseTypeId = a.PurchaseTypeId,
                CategoryId = a.CategoryId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                UserId = a.UserId,
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
                AdditionalNotes = a.AdditionalNotes,
                DistrictId = a.DistrictId,
                DistrictName = b.DistrictName,
                PurchaseTypeId = a.PurchaseTypeId,
                CategoryId = a.CategoryId
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                UserId = a.UserId,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                Address = a.Address,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
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
                UserId = a.UserId,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                Address = a.Address,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictName = a.DistrictName,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                NumberOfDoctors = a.NumberOfDoctors,
                AccountTypeName = a.AccountTypeName,
                PurchaseTypeId = a.PurchaseTypeId,
                CategoryId = a.CategoryId,
                CategoryName = b.CategoryName
            }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new CustomAccountSupportiveVisit
            {
                Id = a.Id,
                AccountId = a.AccountId,
                UserId = a.UserId,
                AccountName = a.AccountName,
                Address = a.Address,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.AdditionalNotes,
                DistrictName = a.DistrictName,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                NumberOfDoctors = a.NumberOfDoctors,
                AccountTypeName = a.AccountTypeName,
                CategoryName = a.CategoryName,
                PurchaseTypeName = b.PurchaseTypeName
            }),a=>a.Id).SingleOrDefault();

            result.products = productlist.Where(x => x.KeyId == result.Id);
            result.persons = personlist.Where(c => c.KeyId == result.Id);
            return result;

        }

        public bool MakeVisit(AccountSupportiveVisitModel obj)
        {
            DateTime datenow = ti.GetCurrentTime();
            int monthdays = DateTime.DaysInMonth(obj.VisitDate.Year, obj.VisitDate.Month);
            DateTime planstart = new DateTime(obj.VisitDate.Year, obj.VisitDate.Month, 1);
            DateTime planend = ti.GetCurrentTime().Date;

            AccountSupportiveVisit visit = new AccountSupportiveVisit();
            visit.VisitDate = obj.VisitDate;
            visit.VisitTime = obj.VisitTime;
            visit.SubmittingDate = datenow;
            visit.SubmittingTime = datenow;
            visit.AccountId = obj.AccountId;
            visit.extendidentityuserid = obj.extendidentityuserid;
            visit.VisitNotes = obj.VisitNotes;
            visit.AdditionalNotes = obj.AdditionalNotes;
            visit.ManagerId = obj.ManagerId;
            db.AccountSupportiveVisit.Add(visit);
            db.SaveChanges();

            foreach (var item in obj.ProductsIds)
            {
                AccountSupportiveVisitProduct products = new AccountSupportiveVisitProduct();
                products.ProductId = item;
                products.AccountSupportVisitId = visit.Id;
                db.AccountSupportiveVisitproduct.Add(products);
                db.SaveChanges();
            }

            foreach (var item in obj.PersonModel)
            {
                AccountSupportiveVisitPerson persons = new AccountSupportiveVisitPerson();
                persons.PersonName = item.PersonName;
                persons.PersonPosition = item.PersonPosition;
                persons.AccountSupportiveVisitId = visit.Id;
                db.AccountSupportiveVisitPerson.Add(persons);
                db.SaveChanges();
            }
            AccountMonthlyPlan todayplan = db.accountMonthlyPlan.Where(a => a.ExtendIdentityUserId == obj.extendidentityuserid && a.AccountId == obj.AccountId && a.Date == obj.VisitDate && a.Status == false).FirstOrDefault();
            if (todayplan != null)
            {
                todayplan.Status = true;
                todayplan.AccountMedicalVisitId = visit.Id;
                db.SaveChanges();
                return true;
            }
            else
            {
                AccountMonthlyPlan plan = db.accountMonthlyPlan.Where(a => a.ExtendIdentityUserId == obj.extendidentityuserid && a.AccountId == obj.AccountId && a.Date >= planstart && a.Date <= planend && a.Status == false).OrderBy(b => b.PlannedDate).FirstOrDefault();
                if (plan != null)
                {
                    plan.Status = true;
                    plan.AccountMedicalVisitId = visit.Id;
                    db.SaveChanges();
                }
                return true;
            }
         
     

        }

        public bool RejectAccountSupportiveVDeleting(int visitid)
        {
            IEnumerable<RequestDeleteAccountSupportive> requests = db.RequestDeleteAccountSupportive.Where(a => a.AccountSupportiveVisitId == visitid);
            foreach (var item in requests)
            {
                db.RequestDeleteAccountSupportive.Remove(item);
            }
            AccountSupportiveVisit visit = db.AccountSupportiveVisit.Find(visitid);
            Account a = db.account.Find(visit.AccountId);
            Notifications n = new Notifications();
            n.NitificationDateTime = ti.GetCurrentTime();
            n.ExtendIdetityUserId = visit.extendidentityuserid;
            n.NotificationDetails = "Your request to delete visit to " + a.AccountName + " on " + visit.VisitDate.ToString("dddd dd/MM/yyyy") + " has been rejected";
            n.Url = "MyAccountsVisits.html";
            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }

        public bool RequestDeleteAccountSupportive(int VisitId)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(db.AccountSupportiveVisit.Find(VisitId).extendidentityuserid).Result;
            IEnumerable<ExtendIdentityUser> admins = userManager.GetUsersInRoleAsync("System Admin").Result;
           

            RequestDeleteAccountSupportive obj = new RequestDeleteAccountSupportive();
            obj.AccountSupportiveVisitId = VisitId;
            db.RequestDeleteAccountSupportive.Add(obj);

            foreach (var item in admins)
            {
                Notifications n = new Notifications();
                n.NitificationDateTime = ti.GetCurrentTime();
                n.ExtendIdetityUserId = item.Id;
                n.NotificationDetails = user.FullName + " requested to delete visit to account " + db.account.Find(db.AccountSupportiveVisit.Find(VisitId).AccountId).AccountName;
                n.Url = "VisitsDeletingRequests.html";
                db.notifications.Add(n);
            }


            db.SaveChanges();
            return true;
        }
    }
}

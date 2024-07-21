using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AMEKSA.ContactMedicalVisitModels;
using AMEKSA.Models;
using Microsoft.AspNetCore.Identity;
using AMEKSA.Privilage;
using MoreLinq.Extensions;

namespace AMEKSA.Repo
{
    public class ContactMedicalVisitRep:IContactMedicalVisitRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public ContactMedicalVisitRep(DbContainer db,ITimeRep ti, UserManager<ExtendIdentityUser> userManager)
        {
            this.db = db;
            this.ti = ti;
            this.userManager = userManager;
        }

        public void AddAidsToExistVisit(IEnumerable<ContactSalesAid> list)
        {
            db.contactSalesAid.AddRange(list);
            db.SaveChanges();
        }

        public void AddProductsToExistVisit(IEnumerable<ContactMedicalVisitProduct> list)
        {
            db.contactMedicalVisitProduct.AddRange(list);
            db.SaveChanges();
        }

        public bool ConfirmCMVDeleting(int visitid)
        {
            ContactMonthlyPlan plan = db.contactMonthlyPlan.Where(a => a.ContactMedicalVisitId == visitid).FirstOrDefault();

            if (plan == null)
            {

            }
            else
            {
                plan.ContactMedicalVisitId = null;
                plan.Status = false;
                db.SaveChanges();
            }

            IEnumerable<ContactMedicalVisitChat> chat = db.contactMedicalVisitChat.Where(a => a.ContactMedicalVisitId == visitid);
            foreach (var item in chat)
            {
                db.contactMedicalVisitChat.Remove(item);
            }

            IEnumerable<ContactMedicalVisitProduct> visitproducts = db.contactMedicalVisitProduct.Where(a => a.ContactMedicalVisitId == visitid);

            foreach (var item in visitproducts)
            {
                db.contactMedicalVisitProduct.Remove(item);
            }
            IEnumerable<ContactSalesAid> visitaids = db.contactSalesAid.Where(a => a.ContactMedicalVisitId == visitid);
            foreach (var item in visitaids)
            {
                db.contactSalesAid.Remove(item);
            }
            IEnumerable<RequestDeleteContactMedical> request = db.requestDeleteContactMedical.Where(a => a.ContactMedicalVisitId == visitid);

            foreach (var item in request)
            {
                db.requestDeleteContactMedical.Remove(item);
            }

            ContactMedicalVisit visit = db.contactMedicalVisit.Find(visitid);
            Contact c = db.contact.Find(visit.ContactId);
            Notifications n = new Notifications();
            n.NitificationDateTime = ti.GetCurrentTime();
            n.ExtendIdetityUserId = visit.extendidentityuserid;
            n.NotificationDetails = "Your request to delete visit to " + c.ContactName + " on " + visit.VisitDate.ToString("dddd dd/MM/yyyy") + " has been accepted and the visit was deleted";
            n.Url = "MyContactsVisits.html";
            db.contactMedicalVisit.Remove(visit);
            db.notifications.Add(n);

            db.SaveChanges();
            return true;
        }

        public void EditVisit(ContactMedicalVisit obj)
        {
            ContactMedicalVisit old = db.contactMedicalVisit.Find(obj.Id);
            old.Requests = obj.Requests;
            old.VisitDate = obj.VisitDate;
            old.VisitTime = obj.VisitTime;
            old.VisitNotes = obj.VisitNotes;
            db.SaveChanges();

            foreach (var item in obj.contactmedicalvisitproduct)
            {
                ContactMedicalVisitProduct oldproduct = db.contactMedicalVisitProduct.Find(item.Id);
                oldproduct.NumberOfSamples = item.NumberOfSamples;
                oldproduct.ProductId = item.ProductId;
                oldproduct.ProductShare = item.ProductShare;
                db.SaveChanges();
            }

            foreach (var item in obj.contactsalesaid)
            {
                ContactSalesAid oldaid = db.contactSalesAid.Find(item.Id);
                oldaid.SalesAidId = item.SalesAidId;
                db.SaveChanges();
            }
        }

        public CustomContactMedicalVisit GetAccountMedicalVisitById(int id)
        {

            IEnumerable<CustomContactSalesAid> aidlist = db.contactSalesAid
                .Join(db.salesAid,
                a => a.SalesAidId,
                b => b.Id,
                (a, b) => new CustomContactSalesAid
                {
                    ContactMedicalVisitId = a.ContactMedicalVisitId,
                    SalesAidId = b.Id,
                    SalesAidName = b.SalesAidName
                });

            IEnumerable<CustomContactMedicalVisitProducts> productlist = db.product
                .Join(db.contactMedicalVisitProduct,
                a => a.Id,
                b => b.ProductId,
                (a, b) => new CustomContactMedicalVisitProducts
                {
                    ContactMedicalVisitId = b.ContactMedicalVisitId,
                    ProductId = a.Id,
                    ProductName = a.ProductName,
                    ProductShare = b.ProductShare
                });

            CustomContactMedicalVisit visit = db.contactMedicalVisit.Where(a => a.Id == id)
                .Join(aidlist,
                a => a.Id,
                b => b.SalesAidId,
                (a, b) => new
                {
                    ContactId = a.ContactId,
                    customcontactsalesaid = aidlist.Where(x => x.ContactMedicalVisitId == a.Id),
                    Id = a.Id,
                    Requests = a.Requests,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    VisitNotes = a.VisitNotes
                }).Join(productlist,
                a => a.Id,
                b => b.ContactMedicalVisitId,
                (a, b) => new
                {
                    ContactId = a.ContactId,
                    customcontactsalesaid = a.customcontactsalesaid,
                    Id = a.Id,
                    Requests = a.Requests,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    VisitNotes = a.VisitNotes,
                    customcontactmedicalvisitproduct = productlist.Where(x => x.ContactMedicalVisitId == a.Id)
                }).Join(db.contact,
                a => a.ContactId,
                b => b.Id,
                (a, b) => new CustomContactMedicalVisit
                {
                    Id = a.Id,
                    ContactId = a.ContactId,
                    ContactName = b.ContactName,
                    Requests = a.Requests,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    VisitNotes = a.VisitNotes,
                    customcontactsalesaid = a.customcontactsalesaid,
                    customcontactmedicalvisitproduct = a.customcontactmedicalvisitproduct
                }).SingleOrDefault();

            return visit;
        }

        public IEnumerable<CustomContactMedicalVisit> GetCMVDeleteRequests()
        {
            IEnumerable<CustomContactMedicalVisit> results = db.requestDeleteContactMedical.Join(db.contactMedicalVisit, a => a.ContactMedicalVisitId, b => b.Id, (a, b) => new
            {
                Id = b.Id,
                VisitDate = b.VisitDate,
                ContactId = b.ContactId,
                UserId = b.extendidentityuserid
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                VisitDate = a.VisitDate,
                ContactName = b.ContactName,
                UserId = a.UserId
            }).Join(db.Users, a => a.UserId, b => b.Id, (a, b) => new CustomContactMedicalVisit
            {
                Id = a.Id,
                VisitDate = a.VisitDate,
                ContactName = a.ContactName,
                UserName = b.FullName
            });

            return results;
        }

        //public IEnumerable<CustomContactMedicalVisit> GetAllMyAccountMedicalVisits(string userid)
        //{
        //    IEnumerable<CustomContactSalesAid> aidlist = db.contactSalesAid
        //        .Join(db.salesAid,
        //        a => a.SalesAidId,
        //        b => b.Id,
        //        (a, b) => new CustomContactSalesAid
        //        {
        //            ContactMedicalVisitId = a.ContactMedicalVisitId,
        //            SalesAidId = b.Id,
        //            SalesAidName = b.SalesAidName
        //        });

        //    IEnumerable<CustomContactMedicalVisitProducts> productlist = db.product
        //        .Join(db.contactMedicalVisitProduct,
        //        a => a.Id,
        //        b => b.ProductId,
        //        (a, b) => new CustomContactMedicalVisitProducts
        //        {
        //            ContactMedicalVisitId = b.ContactMedicalVisitId,
        //            ProductId = a.Id,
        //            ProductName = a.ProductName,
        //            ProductShare = b.ProductShare
        //        });

        //    IEnumerable<CustomContactMedicalVisit> visitlist = db.contactMedicalVisit.Where(a=>a.extendidentityuser.Id==userid)
        //        .Join(aidlist,
        //        a => a.Id,
        //        b => b.SalesAidId,
        //        (a, b) => new
        //        {
        //            ContactId = a.ContactId,
        //            customcontactsalesaid = aidlist.Where(x => x.ContactMedicalVisitId == a.Id),
        //            Id = a.Id,
        //            Requests = a.Requests,
        //            VisitDate = a.VisitDate,
        //            VisitTime = a.VisitTime,
        //            VisitNotes = a.VisitNotes
        //        }).Join(productlist,
        //        a => a.Id,
        //        b => b.ContactMedicalVisitId,
        //        (a, b) => new
        //        {
        //            ContactId = a.ContactId,
        //            customcontactsalesaid = a.customcontactsalesaid,
        //            Id = a.Id,
        //            Requests = a.Requests,
        //            VisitDate = a.VisitDate,
        //            VisitTime = a.VisitTime,
        //            VisitNotes = a.VisitNotes,
        //            customcontactmedicalvisitproduct = productlist.Where(x => x.ContactMedicalVisitId == a.Id)
        //        }).Join(db.contact,
        //        a => a.ContactId,
        //        b => b.Id,
        //        (a, b) => new CustomContactMedicalVisit
        //        {
        //            Id = a.Id,
        //            ContactId = a.ContactId,
        //            ContactName = b.ContactName,
        //            Requests = a.Requests,
        //            VisitDate = a.VisitDate,
        //            VisitTime = a.VisitTime,
        //            VisitNotes = a.VisitNotes,
        //            customcontactsalesaid = a.customcontactsalesaid,
        //            customcontactmedicalvisitproduct = a.customcontactmedicalvisitproduct
        //        });

        //    return visitlist;
        //}

        public IEnumerable<CustomContact> GetContactsToVisitNow(string userId)
        {
          

            TimeSpan now = ti.GetCurrentTime().TimeOfDay;

            List<int> Ids = db.userContact.Where(a => a.extendidentityuserid == userId).Select(a => a.ContactId).ToList();

            List<Contact> acc = new List<Contact>();

            foreach (var id in Ids)
            {
                Contact checkaccount = db.contact.Where(a => a.Id == id && a.BestTimeFrom.HasValue && a.BestTimeTo.HasValue).SingleOrDefault();
                if (checkaccount != null)
                {
                    acc.Add(checkaccount);
                }
            }

            List<Contact> result = new List<Contact>();
            foreach (var item in acc)
            {
                DateTime ff = DateTime.Parse(item.BestTimeFrom.ToString());
                DateTime tt = DateTime.Parse(item.BestTimeTo.ToString());

                string from = ff.ToString(@"yyyy\:MM\:dd\:HH\:mm\:ss\:fffffff").Substring(11, 5);

                string to = tt.ToString(@"yyyy\:MM\:dd\:HH\:mm\:ss\:fffffff").Substring(11, 5);

                TimeSpan f = TimeSpan.Parse(from);
                TimeSpan t = TimeSpan.Parse(to);
                if (f <= now && t >= now)
                {
                    result.Add(item);
                }
            }

            List<CustomContact> res = result.Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                DistrictName = b.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                AccountId = a.AccountId
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContact
            {
                Id = a.Id,
                ContactName = a.ContactName,
                DistrictName = a.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                AccountName = b.AccountName
            }).ToList();

            return res;
        }

        public IEnumerable<CustomContact> GetMyContacts(string userId)
        {
           
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year,month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year,month,days);

            List<UserContact> UC = db.userContact.Where(a => a.extendidentityuserid == userId).ToList();

            List<Contact> acc = new List<Contact>();

            foreach (var contact in UC)
            {
                Contact checkaccount = db.contact.Find(contact.ContactId);
                acc.Add(checkaccount);
            }

            List<CustomContact> res = new List<CustomContact>();

            List<CustomContact> contactswithaff = UC.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new 
            {
                Id = b.Id,
                ContactName = b.ContactName,
                Gender = b.Gender,
                Address = b.Address,
                PurchaseTypeId = b.PurchaseTypeId,
                AccountId = b.AccountId,
                DistrictId = b.DistrictId,
                BestTimeFrom = b.BestTimeFrom,
                BestTimeTo = b.BestTimeTo,
                ContactTypeId = b.ContactTypeId,
                MobileNumber = b.MobileNumber,
                Email = b.Email,
                CategoryId = a.CategoryId,
                MonthlyTarget = a.MonthlyTarget,
                Hsan = b.Hsan
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                Gender = a.Gender,
                Address = a.Address,
                PurchaseTypeId = a.PurchaseTypeId,
                AccountId = a.AccountId,
                DistrictName = b.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                ContactTypeId = a.ContactTypeId,
                MobileNumber = a.MobileNumber,
                Email = a.Email,
                CategoryId = a.CategoryId,
                MonthlyTarget = a.MonthlyTarget,
                Hsan = a.Hsan
            }).Join(db.contactType, a => a.ContactTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                Gender = a.Gender,
                Address = a.Address,
                PurchaseTypeId = a.PurchaseTypeId,
                AccountId = a.AccountId,
                DistrictName = a.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                ContactTypeName = b.ContactTypeName,
                MobileNumber = a.MobileNumber,
                Email = a.Email,
                CategoryId = a.CategoryId,
                MonthlyTarget = a.MonthlyTarget,
                Hsan = a.Hsan
            }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                Gender = a.Gender,
                Address = a.Address,
                PurchaseTypeName = b.PurchaseTypeName,
                AccountId = a.AccountId,
                DistrictName = a.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                ContactTypeName = a.ContactTypeName,
                MobileNumber = a.MobileNumber,
                Email = a.Email,
                CategoryId = a.CategoryId,
                MonthlyTarget = a.MonthlyTarget,
                Hsan = a.Hsan
            }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new {
                Id = a.Id,
                CategoryName = b.CategoryName,
                ContactName = a.ContactName,
                Gender = a.Gender,
                Address = a.Address,
                PurchaseTypeName = a.PurchaseTypeName,
                AccountId = a.AccountId,
                DistrictName = a.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                ContactTypeName = a.ContactTypeName,
                MobileNumber = a.MobileNumber,
                Email = a.Email,
                MonthlyTarget = a.MonthlyTarget,
                Hsan = a.Hsan
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContact
            {
                Id = a.Id,
                CategoryName = a.CategoryName,
                ContactName = a.ContactName,
                Gender = a.Gender,
                Address = a.Address,
                PurchaseTypeName = a.PurchaseTypeName,
                AccountName = b.AccountName,
                DistrictName = a.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                ContactTypeName = a.ContactTypeName,
                MobileNumber = a.MobileNumber,
                Email = a.Email,
                MonthlyTarget = a.MonthlyTarget,
                CurrentVisits = db.contactMedicalVisit.Where(x => x.ContactId == a.Id && x.VisitDate >= start && x.VisitDate <= end && x.extendidentityuserid == userId).Select(z => z.Id).Count(),
                Hsan = a.Hsan
            }).ToList();




   
            foreach (var item in contactswithaff)
            {
                res.Add(item);
            }

            List<CustomContact> contactswithoutaff = UC.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = b.Id,
                ContactName = b.ContactName,
                Gender = b.Gender,
                Address = b.Address,
                PurchaseTypeId = b.PurchaseTypeId,
                AccountId = b.AccountId,
                DistrictId = b.DistrictId,
                BestTimeFrom = b.BestTimeFrom,
                BestTimeTo = b.BestTimeTo,
                ContactTypeId = b.ContactTypeId,
                MobileNumber = b.MobileNumber,
                Email = b.Email,
                CategoryId = a.CategoryId,
                MonthlyTarget = a.MonthlyTarget,
                Hsan = b.Hsan
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                Gender = a.Gender,
                Address = a.Address,
                PurchaseTypeId = a.PurchaseTypeId,
                AccountId = a.AccountId,
                DistrictName = b.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                ContactTypeId = a.ContactTypeId,
                MobileNumber = a.MobileNumber,
                Email = a.Email,
                CategoryId = a.CategoryId,
                MonthlyTarget = a.MonthlyTarget,
                Hsan = a.Hsan
            }).Join(db.contactType, a => a.ContactTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                Gender = a.Gender,
                Address = a.Address,
                PurchaseTypeId = a.PurchaseTypeId,
                AccountId = a.AccountId,
                DistrictName = a.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                ContactTypeName = b.ContactTypeName,
                MobileNumber = a.MobileNumber,
                Email = a.Email,
                CategoryId = a.CategoryId,
                MonthlyTarget = a.MonthlyTarget,
                Hsan = a.Hsan
            }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                Gender = a.Gender,
                Address = a.Address,
                PurchaseTypeName = b.PurchaseTypeName,
                AccountId = a.AccountId,
                DistrictName = a.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                ContactTypeName = a.ContactTypeName,
                MobileNumber = a.MobileNumber,
                Email = a.Email,
                CategoryId = a.CategoryId,
                MonthlyTarget = a.MonthlyTarget,
                Hsan = a.Hsan
            }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new CustomContact
            {
                Id = a.Id,
                CategoryName = b.CategoryName,
                ContactName = a.ContactName,
                Gender = a.Gender,
                Address = a.Address,
                PurchaseTypeName = a.PurchaseTypeName,
                AccountId = a.AccountId,
                DistrictName = a.DistrictName,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                ContactTypeName = a.ContactTypeName,
                MobileNumber = a.MobileNumber,
                Email = a.Email,
                MonthlyTarget = a.MonthlyTarget,
                CurrentVisits = db.contactMedicalVisit.Where(x => x.ContactId == a.Id && x.VisitDate >= start && x.VisitDate <= end && x.extendidentityuserid == userId).Select(z => z.Id).Count(),
                Hsan = a.Hsan
            }).ToList();
            foreach (var item in contactswithoutaff)
            {
                res.Add(item);
            }
            List<CustomContact> ress = DistinctByExtension.DistinctBy(res, a => a.Id).ToList();
              


            foreach (var item in ress)
            {
                RequestChangeContactCategory r = db.requestChangeCategory.Where(a => a.ExtendIdentityUserId == userId && a.ContactId == item.Id && a.Confirmed == false && a.Rejected == false).FirstOrDefault();

                if (r == null)
                {
                    item.CategoryRequest = false;
                }
                else
                {
                    item.CategoryRequest = true;
                }

                List<RequestChangeContactTarget> t = db.requestChangeContactTargets.Where(a => a.RepId == userId && a.ContactId == item.Id).ToList();
                if (t.Count() == 0)
                {
                    item.TargetRequest = false;
                }
                else
                {
                    foreach (var req in t)
                    {
                        if (req.Status == null)
                        {
                            item.TargetRequest = true;
                            break;
                        }
                        else
                        {
                            item.TargetRequest = false;
                        }
                    }
                }
                
               
               
            }


            return ress.OrderBy(a=>a.ContactName);
        }

        public TotalTargetModel GetMyTotalTarget(string id)
        {
            DateTime now = ti.GetCurrentTime();
            int monthdays = DateTime.DaysInMonth(now.Year,now.Month);
            DateTime start = new DateTime(now.Year, now.Month, 1);
            DateTime end = new DateTime(now.Year, now.Month, monthdays);
            List<UserContact> UC = db.userContact.Where(a => a.extendidentityuserid == id).ToList();
            int visits = 0;
            int? totaltaregt = UC.Select(a => a.MonthlyTarget).Sum();
            foreach (var item in UC)
            {
                int thisvisits = db.contactMedicalVisit.Where(a => a.extendidentityuserid == id && a.ContactId == item.ContactId && a.VisitDate >= start && a.VisitDate <= end).Count();
                visits = visits + thisvisits;
                
            }
            TotalTargetModel res = new TotalTargetModel();
            if(totaltaregt == null)
            {
                res.TotalTarget = 0;
            }
            else
            {
                res.TotalTarget = (int)totaltaregt;
            }
            
            res.Visited = visits;

            return res;
        }

        public IEnumerable<CustomContactMedicalVisit> GetMyVisits(string userId)
        {
            var visitsids = db.contactMedicalVisit.Where(a => a.extendidentityuserid == userId).Select(a => a.Id);

            List<CustomContactMedicalVisitProducts> productlist = new List<CustomContactMedicalVisitProducts>();
            List<CustomContactSalesAid> aidlist = new List<CustomContactSalesAid>();
            foreach (var item in visitsids)
            {
                var s = db.contactMedicalVisitProduct.Where(a => a.ContactMedicalVisitId == item).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
                {
                    ProductName = b.ProductName,
                    ContactMedicalVisitId = a.ContactMedicalVisitId,
                    ProductShare = a.ProductShare
                });
                foreach (var product in s)
                {
                    CustomContactMedicalVisitProducts ad = new CustomContactMedicalVisitProducts();
                    ad.ProductName = product.ProductName;
                    ad.ProductShare = product.ProductShare;
                    ad.ContactMedicalVisitId = product.ContactMedicalVisitId;
                    productlist.Add(ad);
                }

                var ss = db.contactSalesAid.Where(a => a.ContactMedicalVisitId == item).Join(db.salesAid,
                    a => a.SalesAidId, b => b.Id, (a, b) => new CustomContactSalesAid
                    {
                        ContactMedicalVisitId = a.ContactMedicalVisitId,
                        SalesAidId = b.Id,
                        SalesAidName = b.SalesAidName
                    });
               
                
                foreach (var aid in ss)
                {
                    aidlist.Add(aid);
                }

            }




            IEnumerable<CustomContactMedicalVisit> result = db.contactMedicalVisit.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = b.ContactName,
                ContactTypeId = b.ContactTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = b.DistrictId
            }).Join(db.contactMedicalVisitProduct, a => a.Id, b => b.ContactMedicalVisitId, (a, b) => new
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                ContactTypeId = a.ContactTypeId,
                customcontactmedicalvisitproduct = productlist.Where(x => x.ContactMedicalVisitId == a.Id),
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = a.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                ContactTypeId = a.ContactTypeId,
                customcontactmedicalvisitproduct = a.customcontactmedicalvisitproduct,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = a.DistrictId,
                DistrictName = b.DistrictName
            }).Join(db.contactType, a => a.ContactTypeId, b => b.Id, (a, b) => new CustomContactMedicalVisit
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                customcontactmedicalvisitproduct = a.customcontactmedicalvisitproduct,
                customcontactsalesaid = aidlist.Where(c => c.ContactMedicalVisitId == a.Id),
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = b.ContactTypeName
            }).DistinctBy(o => o.Id).OrderByDescending(a => a.VisitDate);



            return result;
        }

        public CustomContactMedicalVisit GetVisitById(int visitId)
        {
            //var visitsids = db.contactMedicalVisit.Where(a => a.Id == visitId).Select(a => a.Id);

            List<CustomContactMedicalVisitProducts> productlist = new List<CustomContactMedicalVisitProducts>();
            List<CustomContactSalesAid> aidlist = new List<CustomContactSalesAid>();
            //foreach (var item in visitsids)
            //{
                var s = db.contactMedicalVisitProduct.Where(a => a.ContactMedicalVisitId == visitId).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
                {
                    ProductName = b.ProductName,
                    ContactMedicalVisitId = a.ContactMedicalVisitId,
                    ProductShare = a.ProductShare
                });
                foreach (var product in s)
                {
                    CustomContactMedicalVisitProducts ad = new CustomContactMedicalVisitProducts();
                    ad.ProductName = product.ProductName;
                    ad.ProductShare = product.ProductShare;
                    ad.ContactMedicalVisitId = product.ContactMedicalVisitId;
                    productlist.Add(ad);
                }

                var ss = db.contactSalesAid.Where(a => a.ContactMedicalVisitId == visitId).Join(db.salesAid,
                    a => a.SalesAidId, b => b.Id, (a, b) => new CustomContactSalesAid
                    {
                        ContactMedicalVisitId = a.ContactMedicalVisitId,
                        SalesAidId = b.Id,
                        SalesAidName = b.SalesAidName
                    });


                foreach (var aid in ss)
                {
                    aidlist.Add(aid);
                }

            //}


            CustomContactMedicalVisit result = db.userContact.Join(db.contactMedicalVisit,a=>a.ContactId,b=>b.ContactId,(a,b)=>new {
                Id = b.Id,
                ContactId = b.ContactId,
                VisitDate = b.VisitDate,
                VisitTime = b.VisitTime,
                VisitNotes = b.VisitNotes,
                Requests = b.Requests,
                CategoryId = a.CategoryId
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
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
                AccountId = b.AccountId,
                CategoryId = a.CategoryId,
                PurchaseTypeId = b.PurchaseTypeId,
                PhoneNumber = b.MobileNumber
            }).Join(db.contactMedicalVisitProduct, a => a.Id, b => b.ContactMedicalVisitId, (a, b) => new
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
                AccountId = a.AccountId,
                CategoryId = a.CategoryId,
                PurchaseTypeId = a.PurchaseTypeId,
                PhoneNumber = a.PhoneNumber
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
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
                DistrictName = b.DistrictName,
                AccountId = a.AccountId,
                CategoryId = a.CategoryId,
                PurchaseTypeId = a.PurchaseTypeId,
                PhoneNumber = a.PhoneNumber
            }).Join(db.contactType, a => a.ContactTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = b.ContactTypeName,
                AccountId = a.AccountId,
                CategoryId = a.CategoryId,
                PurchaseTypeId = a.PurchaseTypeId,
                PhoneNumber = a.PhoneNumber
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = a.ContactTypeName,
                AccountName = b.AccountName,
                CategoryId = a.CategoryId,
                PurchaseTypeId = a.PurchaseTypeId,
                PhoneNumber = a.PhoneNumber
            }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = a.ContactTypeName,
                AccountName = a.AccountName,
                CategoryName = b.CategoryName,
                PurchaseTypeId = a.PurchaseTypeId,
                PhoneNumber = a.PhoneNumber
            }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new CustomContactMedicalVisit
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = a.ContactTypeName,
                AccountName = a.AccountName,
                CategoryName = a.CategoryName,
                PurchaseTypeName = b.PurchaseTypeName,
                PhoneNumber = a.PhoneNumber
            }).Where(a => a.Id == visitId).ToList().FirstOrDefault();

            result.customcontactmedicalvisitproduct = productlist.Where(x => x.ContactMedicalVisitId == result.Id);
            result.customcontactsalesaid = aidlist.Where(c => c.ContactMedicalVisitId == result.Id);
            return result;
        }

        public IEnumerable<CustomContactMedicalVisit> GetVisitsByDate(AccountSalesVisitByDateModel obj)
        {

            var visitsids = db.contactMedicalVisit.Where(a => a.extendidentityuserid == obj.UserId && a.VisitDate >= obj.Start && a.VisitDate <= obj.End).Select(a => a.Id).ToList();

            List<CustomContactMedicalVisitProducts> productlist = new List<CustomContactMedicalVisitProducts>();
            List<CustomContactSalesAid> aidlist = new List<CustomContactSalesAid>();
            foreach (var item in visitsids)
            {
                var s = db.contactMedicalVisitProduct.Where(a => a.ContactMedicalVisitId == item).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
                {
                    ProductName = b.ProductName,
                    ContactMedicalVisitId = a.ContactMedicalVisitId,
                    ProductShare = a.ProductShare
                }).ToList();
                foreach (var product in s)
                {
                    CustomContactMedicalVisitProducts ad = new CustomContactMedicalVisitProducts();
                    ad.ProductName = product.ProductName;
                    ad.ProductShare = product.ProductShare;
                    ad.ContactMedicalVisitId = product.ContactMedicalVisitId;
                    productlist.Add(ad);
                }

                var ss = db.contactSalesAid.Where(a => a.ContactMedicalVisitId == item).Join(db.salesAid,
                    a => a.SalesAidId, b => b.Id, (a, b) => new CustomContactSalesAid
                    {
                        ContactMedicalVisitId = a.ContactMedicalVisitId,
                        SalesAidId = b.Id,
                        SalesAidName = b.SalesAidName
                    }).ToList();


                foreach (var aid in ss)
                {
                    aidlist.Add(aid);
                }

            }




            List<CustomContactMedicalVisit> result = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.extendidentityuserid == obj.UserId && a.VisitDate >= obj.Start && a.VisitDate <= obj.End).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = b.ContactName,
                ContactTypeId = b.ContactTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = b.DistrictId
            }).Join(db.contactMedicalVisitProduct, a => a.Id, b => b.ContactMedicalVisitId, (a, b) => new
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                ContactTypeId = a.ContactTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = a.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
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
                DistrictName = b.DistrictName
            }).Join(db.contactType, a => a.ContactTypeId, b => b.Id, (a, b) => new CustomContactMedicalVisit
            {
                Id = a.Id,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = b.ContactTypeName
            }), o => o.Id).OrderByDescending(a => a.VisitDate).ToList();
               

            List<CustomContactMedicalVisit> res = new List<CustomContactMedicalVisit>();
            foreach (var item in result)
            {
                item.customcontactmedicalvisitproduct = productlist.Where(x => x.ContactMedicalVisitId == item.Id);
                item.customcontactsalesaid = aidlist.Where(c => c.ContactMedicalVisitId == item.Id);

                RequestDeleteContactMedical check = db.requestDeleteContactMedical.Where(a => a.ContactMedicalVisitId == item.Id).FirstOrDefault();
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

        public bool MakeVisit(ContactMedicalVisitModel obj)
        {

            DateTime datenow = ti.GetCurrentTime();
            int monthdays = DateTime.DaysInMonth(obj.VisitDate.Year, obj.VisitDate.Month);
            DateTime planstart = new DateTime(obj.VisitDate.Year, obj.VisitDate.Month, 1);
            DateTime planend = ti.GetCurrentTime().Date;

            ContactMedicalVisit visit = new ContactMedicalVisit();
            visit.VisitDate = obj.VisitDate;
            visit.VisitTime = obj.VisitTime;
            visit.SubmittingDate = datenow;
            visit.SubmittingTime = datenow;
            visit.ContactId = obj.ContactId;
            visit.extendidentityuserid = obj.extendidentityuserid;
            visit.VisitNotes = obj.VisitNotes;
            visit.Requests = obj.Requests;
            visit.ManagerId = obj.ManagerId;
            db.contactMedicalVisit.Add(visit);
            db.SaveChanges();

            List<int> newaidsids = new List<int>();

            foreach (var item in obj.newaids)
            {
                SalesAid newsalesaid = new SalesAid();
                newsalesaid.SalesAidName = item.SalesAidName;
                db.salesAid.Add(newsalesaid);
                db.SaveChanges();
                int newsalesaidid = newsalesaid.Id;
                newaidsids.Add(newsalesaidid);
            }

            foreach (var item in newaidsids)
            {
                ContactSalesAid x = new ContactSalesAid();
                x.SalesAidId = item;
                x.ContactMedicalVisitId = visit.Id;
                db.contactSalesAid.Add(x);
                db.SaveChanges();
            }

            foreach (var item in obj.aids)
            {
                ContactSalesAid x = new ContactSalesAid();
                x.SalesAidId = item.SalesAidId;
                x.ContactMedicalVisitId = visit.Id;
                db.contactSalesAid.Add(x);
                db.SaveChanges();

            }

            foreach (var item in obj.products)
            {
                ContactMedicalVisitProduct x = new ContactMedicalVisitProduct();
                x.ProductId = item.ProductId;
                x.ProductShare = item.ProductShare;
                x.NumberOfSamples = item.NumberOfSamples;
                x.ContactMedicalVisitId = visit.Id;
                db.contactMedicalVisitProduct.Add(x);
                db.SaveChanges();
            }

            ContactMonthlyPlan todayplan = db.contactMonthlyPlan.Where(a => a.ExtendIdentityUserId == obj.extendidentityuserid && a.ContactId == obj.ContactId && a.Date == obj.VisitDate && a.Status == false).FirstOrDefault();
            if (todayplan != null)
            {
                todayplan.Status = true;
                todayplan.ContactMedicalVisitId = visit.Id;
                db.SaveChanges();
                return true;
            }
            else
            {
                ContactMonthlyPlan plan = db.contactMonthlyPlan.Where(a => a.ExtendIdentityUserId == obj.extendidentityuserid && a.ContactId == obj.ContactId && a.Date >= planstart && a.Date <= planend && a.Status == false).OrderBy(b => b.PlannedDate).FirstOrDefault();
                if (plan != null)
                {
                    plan.Status = true;
                    plan.ContactMedicalVisitId = visit.Id;
                    db.SaveChanges();
                }
                return true;
            }
        }

        public bool RejectCMVDeleting(int visitid)
        {
            IEnumerable<RequestDeleteContactMedical> requests = db.requestDeleteContactMedical.Where(a => a.ContactMedicalVisitId == visitid);
            foreach (var item in requests)
            {
                db.requestDeleteContactMedical.Remove(item);
            }
            ContactMedicalVisit visit = db.contactMedicalVisit.Find(visitid);
            Contact c = db.contact.Find(visit.ContactId);
            Notifications n = new Notifications();
            n.NitificationDateTime = ti.GetCurrentTime();
            n.ExtendIdetityUserId = visit.extendidentityuserid;
            n.NotificationDetails = "Your request to delete visit to " + c.ContactName + " on " + visit.VisitDate.ToString("dddd dd/MM/yyyy") + " has been rejected";
            n.Url = "MyContactsVisits.html";
            db.notifications.Add(n);
            db.SaveChanges();
            return true;

        }

        public bool RequestDeleteContactMedical(int VisitId)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(db.contactMedicalVisit.Find(VisitId).extendidentityuserid).Result;
            
            List<ExtendIdentityUser> admin = userManager.GetUsersInRoleAsync("System Admin").Result.ToList();
            List<ExtendIdentityUser> adminn = userManager.GetUsersInRoleAsync("System Adminn").Result.ToList();

            foreach (var item in admin)
            {
                Notifications n = new Notifications();
                n.NitificationDateTime = ti.GetCurrentTime();
                n.ExtendIdetityUserId = item.Id;
                n.NotificationDetails = user.FullName + " requested to delete visit to contact " + db.contact.Find(db.contactMedicalVisit.Find(VisitId).ContactId).ContactName;
                n.Url = "VisitsDeletingRequests.html";
                db.notifications.Add(n);
            }
            foreach (var item in adminn)
            {
                Notifications n = new Notifications();
                n.NitificationDateTime = ti.GetCurrentTime();
                n.ExtendIdetityUserId = item.Id;
                n.NotificationDetails = user.FullName + " requested to delete visit to contact " + db.contact.Find(db.contactMedicalVisit.Find(VisitId).ContactId).ContactName;
                n.Url = "VisitsDeletingRequests.html";
                db.notifications.Add(n);
            }
            RequestDeleteContactMedical obj = new RequestDeleteContactMedical();
            obj.ContactMedicalVisitId = VisitId;
            db.requestDeleteContactMedical.Add(obj);
            db.SaveChanges();
            return true;
        }

     
    }
}

using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class ContactRep:IContactRep
    {
        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public ContactRep(DbContainer db, UserManager<ExtendIdentityUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public bool AddContact(Contact obj)
        {
            if (obj.AccountId == null || obj.AccountId == 0)
            {
                obj.AccountId = 738;
            }
            obj.Guidd = Guid.NewGuid();
            db.contact.Add(obj);
            db.SaveChanges();
            return true;
        }

        public bool AddHsan(AddHsanModel obj)
        {
            EventTravelRequest etr = db.EventTravelRequest.Find(obj.Id);

            Contact c = db.contact.Find(etr.ContactId);
            c.Hsan = obj.hsan;
            db.SaveChanges();
            return true;
        }

        public bool AddHsanByContactId(AddHsanModel obj)
        {
            Contact c = db.contact.Find(obj.Id);
            c.Hsan = obj.hsan;
            db.SaveChanges();
            return true;
        }

        public bool AddHsanDirect(AddHsanModel obj)
        {
            Contact c = db.contact.Find(obj.Id);
            c.Hsan = obj.hsan;
            db.SaveChanges();
            return true;
        }

        public bool DeleteContact(int id)
        {
            List<ContactMonthlyPlan> cmp = db.contactMonthlyPlan.Where(a => a.ContactId == id).ToList();
            db.contactMonthlyPlan.RemoveRange(cmp);
            List<ContactMedicalVisit> MV = db.contactMedicalVisit.Where(a => a.ContactId == id).ToList();
            foreach (var item in MV)
            {
                List<ContactMedicalVisitProduct> MVP = db.contactMedicalVisitProduct.Where(a => a.ContactMedicalVisitId == item.Id).ToList();
                List<ContactSalesAid> MVPs = db.contactSalesAid.Where(a => a.ContactMedicalVisitId == item.Id).ToList();
                db.contactMedicalVisitProduct.RemoveRange(MVP);
                db.contactSalesAid.RemoveRange(MVPs);
                //foreach (var product in MVP)
                //{
                //    db.contactMedicalVisitProduct.Remove(product);

                //}
                //db.SaveChanges();
                //foreach (var aid in MVPs)
                //{
                //    db.contactSalesAid.Remove(aid);

                //}
               
                db.contactMedicalVisit.Remove(item);
                db.SaveChanges();
            }
      

            List<UserContact> UA = db.userContact.Where(a => a.ContactId == id).ToList();
            foreach (var item in UA)
            {
                db.userContact.Remove(item);

            }
            db.SaveChanges();


            Contact acc = db.contact.Find(id);

            db.contact.Remove(acc);
            db.SaveChanges();

            return true;
        }

        public bool EditContactContactinfo(ContactContactInfo obj)
        {
            Contact old = db.contact.Find(obj.Id);
            old.MobileNumber = obj.MobileNumber;
            old.LandLineNumber = obj.LandLineNumber;
            old.Email = obj.Email;
            db.SaveChanges();
            return true;
        }

        public bool EditContactGeneralInfo(ContactGeneralInfo obj)
        {
            Contact old = db.contact.Find(obj.Id);
            old.Gender = obj.Gender;
            old.ContactName = obj.ContactName;
            old.ContactTypeId = obj.ContactTypeId;
            old.PurchaseTypeId = obj.PurchaseTypeId;
            old.AccountId = obj.AccountId;
            db.SaveChanges();
            return true;
        }

        public bool EditContactLocationInfo(ContactLocationInfo obj)
        {
            Contact old = db.contact.Find(obj.Id);
            old.DistrictId = obj.DistrictId;
            old.Address = obj.Address;
            db.SaveChanges();
            return true;
        }

        public bool EditContactNotesInfo(ContactNoteInfo obj)
        {
            Contact old = db.contact.Find(obj.Id);
            old.PaymentNotes = obj.PaymentNotes;
            old.RelationshipNote = obj.RelationshipNote;
            db.SaveChanges();
            return true;
        }

        public bool EditContactTimeInfo(ContactTimeInfo obj)
        {
            Contact old = db.contact.Find(obj.Id);
            old.BestTimeFrom = obj.BestTimeFrom;
            old.BestTimeTo = obj.BestTimeTo;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<CustomContact> GetAllContacts()
        {
            


                List<CustomContact> res = db.contact
                .Join(db.district,
                a => a.DistrictId,
                b => b.Id,
                (a, b) =>
                   new
                   {
                       Id = a.Id,
                       ContactName = a.ContactName,
                       DistrictId = b.Id,
                       DistrictName = b.DistrictName,
                       CityId = b.CityId,
                       Address = a.Address,
                       LandLineNumber = a.LandLineNumber,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       ContactTypeId = a.ContactTypeId,
                       PaymentNotes = a.PaymentNotes,
                       RelationshipNotes = a.RelationshipNote,
                       BestTimeFrom = a.BestTimeFrom,
                       BestTimeTo = a.BestTimeTo,
                       PurchaseTypeId = a.PurchaseTypeId,
                       AccountId = a.AccountId,
                       Gender = a.Gender
                   }).Join(db.city,
                   a => a.CityId,
                   b => b.Id,
                   (a, b) => new
                   {
                       Id = a.Id,
                       ContactName = a.ContactName,
                       DistrictId = a.DistrictId,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       CityName = b.CityName,
                       Address = a.Address,
                       LandLineNumber = a.LandLineNumber,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       ContactTypeId = a.ContactTypeId,
                       PaymentNotes = a.PaymentNotes,
                       RelationshipNotes = a.RelationshipNotes,
                       BestTimeFrom = a.BestTimeFrom,
                       BestTimeTo = a.BestTimeTo,
                       PurchaseTypeId = a.PurchaseTypeId,
                       AccountId = a.AccountId,
                       Gender = a.Gender
                   }).Join(db.contactType,
                   a => a.ContactTypeId,
                   b => b.Id,
                   (a, b) =>
                   new
                   {
                       Id = a.Id,
                       ContactName = a.ContactName,
                       DistrictId = a.DistrictId,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       CityName = a.CityName,
                       Address = a.Address,
                       LandLineNumber = a.LandLineNumber,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       ContactTypeId = a.ContactTypeId,
                       ContactTypeName = b.ContactTypeName,
                       PaymentNotes = a.PaymentNotes,
                       RelationshipNotes = a.RelationshipNotes,
                       BestTimeFrom = a.BestTimeFrom,
                       BestTimeTo = a.BestTimeTo,
                       PurchaseTypeId = a.PurchaseTypeId,
                       AccountId = a.AccountId,
                       Gender = a.Gender
                   }
                   ).Join(db.purchaseType,
                   a => a.PurchaseTypeId,
                   b => b.Id,
                   (a, b) => new CustomContact
                   {
                       Id = a.Id,
                       ContactName = a.ContactName,
                       DistrictId = a.DistrictId,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       CityName = a.CityName,
                       Address = a.Address,
                       LandLineNumber = a.LandLineNumber,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       ContactTypeId = a.ContactTypeId,
                       ContactTypeName = a.ContactTypeName,
                       PaymentNotes = a.PaymentNotes,
                       RelationshipNote = a.RelationshipNotes,
                       BestTimeFrom = a.BestTimeFrom,
                       BestTimeTo = a.BestTimeTo,
                       PurchaseTypeId = a.PurchaseTypeId,
                       PurchaseTypeName = b.PurchaseTypeName,
                       AccountId = a.AccountId,
                       Gender = a.Gender
                   }).OrderBy(a => a.ContactName).ToList();
            return res;

            
        }

        public IEnumerable<CustomContact> GetAllContactsFiltered(FilteringContactsModel obj)
        {

            if (obj.DistrictId == 0 && obj.ContactTypeId == 0)
            {
                List<CustomContact> cus = new List<CustomContact>();
                IEnumerable<CustomContact> one = db.contact
                .Join(db.district,
                a => a.DistrictId,
                b => b.Id,
                (a, b) =>
                   new
                   {
                       Id = a.Id,
                       ContactName = a.ContactName,
                       DistrictName = b.DistrictName,
                       CityId = b.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountId = a.AccountId,
                       Hsan = a.Hsan,
                       CategoryId = a.CategoryId,
                       ContactTypeId = a.ContactTypeId
                   }).Join(db.account,a=>a.AccountId,b=>b.Id,(a,b)=>new CustomContact
                   {
                       Id = a.Id,
                       ContactName = a.ContactName,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountName = b.AccountName,
                       Hsan = a.Hsan,
                       CategoryId = a.CategoryId,
                       ContactTypeId = a.ContactTypeId
                   }).Where(a => a.CityId == obj.CityId);

               
                IEnumerable<CustomContact> two = db.contact
                .Join(db.district,
                a => a.DistrictId,
                b => b.Id,
                (a, b) =>
                   new CustomContact
                   {
                       Id = a.Id,
                       ContactName = a.ContactName,
                       DistrictName = b.DistrictName,
                       CityId = b.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountId = a.AccountId,
                       Hsan = a.Hsan,
                       CategoryId = a.CategoryId,
                       ContactTypeId = a.ContactTypeId
                   }).Where(a => a.CityId == obj.CityId && a.AccountId == null);

                foreach (var item in one)
                {
                    cus.Add(item);
                }
                foreach (var item in two)
                {
                    cus.Add(item);
                }
         
                return cus.DistinctBy(a => a.Id).OrderBy(a => a.ContactName);
                 
            }
            else
            {

                if (obj.DistrictId == 0)
                {
                    List<CustomContact> cus = new List<CustomContact>();

                    IEnumerable<CustomContact> one = db.contact
                .Join(db.district,
                a => a.DistrictId,
                b => b.Id,
                (a, b) =>
                   new
                   {
                       Id = a.Id,
                       ContactTypeId = a.ContactTypeId,
                       ContactName = a.ContactName,
                       DistrictName = b.DistrictName,
                       CityId = b.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountId = a.AccountId,
                       Hsan = a.Hsan,
                       CategoryId = a.CategoryId
                   }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContact
                   {
                       Id = a.Id,
                       ContactTypeId = a.ContactTypeId,
                       ContactName = a.ContactName,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountName = b.AccountName,
                       Hsan = a.Hsan,
                       CategoryId = a.CategoryId
                   }).Where(a => a.CityId == obj.CityId && a.ContactTypeId == obj.ContactTypeId);

                    IEnumerable<CustomContact> two = db.contact
               .Join(db.district,
               a => a.DistrictId,
               b => b.Id,
               (a, b) =>
                  new CustomContact
                  {
                      Id = a.Id,
                      ContactTypeId = a.ContactTypeId,
                      ContactName = a.ContactName,
                      DistrictName = b.DistrictName,
                      CityId = b.CityId,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      AccountId = a.AccountId,
                      Hsan = a.Hsan,
                      CategoryId = a.CategoryId
                  }).Where(a => a.CityId == obj.CityId && a.ContactTypeId == obj.ContactTypeId && a.AccountId == null);

                    foreach (var item in one)
                    {
                        cus.Add(item);
                    }
                    foreach (var item in two)
                    {
                        cus.Add(item);
                    }
                    return cus.DistinctBy(a => a.Id).OrderBy(a => a.ContactName);
                }

                else
                {
                    if (obj.ContactTypeId == 0)
                    {
                        List<CustomContact> cus = new List<CustomContact>();

                        IEnumerable<CustomContact> one = db.contact
                .Join(db.district,
                a => a.DistrictId,
                b => b.Id,
                (a, b) =>
                   new
                   {
                       Id = a.Id,
                       ContactTypeId = a.ContactTypeId,
                       ContactName = a.ContactName,
                       DistrictId = a.DistrictId,
                       DistrictName = b.DistrictName,
                       CityId = b.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountId = a.AccountId,
                       Hsan = a.Hsan,
                       CategoryId = a.CategoryId
                   }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContact
                   {
                       Id = a.Id,
                       DistrictId = a.DistrictId,
                       ContactTypeId = a.ContactTypeId,
                       ContactName = a.ContactName,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountName = b.AccountName,
                       Hsan = a.Hsan,
                       CategoryId = a.CategoryId
                   }).Where(a => a.DistrictId == obj.DistrictId);

                        IEnumerable<CustomContact> two = db.contact
               .Join(db.district,
               a => a.DistrictId,
               b => b.Id,
               (a, b) =>
                  new CustomContact
                  {
                      Id = a.Id,
                      ContactTypeId = a.ContactTypeId,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = b.DistrictName,
                      CityId = b.CityId,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      AccountId = a.AccountId,
                      Hsan = a.Hsan,
                      CategoryId = a.CategoryId
                  }).Where(a => a.DistrictId == obj.DistrictId && a.AccountId == null) ;

                        foreach (var item in one)
                        {
                            cus.Add(item);
                        }
                        foreach (var item in two)
                        {
                            cus.Add(item);
                        }

                        return cus.DistinctBy(a => a.Id).OrderBy(a => a.ContactName);
                    }
                    else
                    {
                        List<CustomContact> cus = new List<CustomContact>();

                        IEnumerable<CustomContact> one = db.contact
                .Join(db.district,
                a => a.DistrictId,
                b => b.Id,
                (a, b) =>
                   new
                   {
                       Id = a.Id,
                       ContactTypeId = a.ContactTypeId,
                       DistrictId = a.DistrictId,
                       ContactName = a.ContactName,
                       DistrictName = b.DistrictName,
                       CityId = b.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountId = a.AccountId,
                       Hsan = a.Hsan,
                       CategoryId = a.CategoryId
                   }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContact
                   {
                       Id = a.Id,
                       ContactTypeId = a.ContactTypeId,
                       ContactName = a.ContactName,
                       DistrictId = a.DistrictId,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountName = b.AccountName,
                       Hsan = a.Hsan,
                       CategoryId = a.CategoryId
                   }).Where(a => a.DistrictId == obj.DistrictId && a.ContactTypeId == obj.ContactTypeId);

                        IEnumerable<CustomContact> two = db.contact
               .Join(db.district,
               a => a.DistrictId,
               b => b.Id,
               (a, b) =>
                  new CustomContact
                  {
                      Id = a.Id,
                      ContactTypeId = a.ContactTypeId,
                      DistrictId = a.DistrictId,
                      ContactName = a.ContactName,
                      DistrictName = b.DistrictName,
                      CityId = b.CityId,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      AccountId = a.AccountId,
                      Hsan = a.Hsan,
                      CategoryId = a.CategoryId
                  }).Where(a => a.DistrictId == obj.DistrictId && a.ContactTypeId == obj.ContactTypeId && a.AccountId == null);

                        foreach (var item in one)
                        {
                            cus.Add(item);
                        }
                        foreach (var item in two)
                        {
                            cus.Add(item);
                        }

                        return cus.DistinctBy(a => a.Id).OrderBy(a => a.ContactName);
                    }
                }
            }








            
        }

        public CustomContact GetContactByIdWithAccount(int id)
        {
            Contact con = db.contact.Find(id);
            int CityId = db.district.Where(a => a.Id == con.DistrictId).Select(a => a.CityId).First();
            int medicalvisits = db.contactMedicalVisit.Where(a => a.ContactId == id).Count();
            List<string> firstlinenames = db.userContact.Where(a => a.ContactId == id).Join(db.Users, a => a.extendidentityuserid, b => b.Id, (a, b) => new
            {
                ManagerId = b.extendidentityuserid
            }).Join(db.Users, a => a.ManagerId, b => b.Id, (a, b) => new
            {
                Name = b.FullName
            }).Select(a => a.Name).ToList();
            List<string> medicalids = db.userContact.Where(a => a.ContactId == id).Select(a => a.extendidentityuserid).ToList();
            List<string> medicalnames = new List<string>();
            foreach (var item in medicalids)
            {
                string username = db.Users.Where(a => a.Id == item).Select(a => a.FullName).SingleOrDefault();
                medicalnames.Add(username);
            }

          


            CustomContact result = db.contact
               .Where(x => x.Id == id).Join(db.district,
               a => a.DistrictId,
               b => b.Id,
               (a, b) =>
                  new
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = b.Id,
                      DistrictName = b.DistrictName,
                      CityId = b.CityId,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNotes = a.RelationshipNote,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      AccountId = a.AccountId,
                      Gender = a.Gender
                  }).Join(db.city,
                  a => a.CityId,
                  b => b.Id,
                  (a, b) => new
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = a.DistrictName,
                      CityId = a.CityId,
                      CityName = b.CityName,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNotes = a.RelationshipNotes,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      AccountId = a.AccountId,
                      Gender = a.Gender
                  }).Join(db.contactType,
                  a => a.ContactTypeId,
                  b => b.Id,
                  (a, b) =>
                  new
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = a.DistrictName,
                      CityId = a.CityId,
                      CityName = a.CityName,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      ContactTypeName = b.ContactTypeName,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNotes = a.RelationshipNotes,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      AccountId = a.AccountId,
                      Gender = a.Gender
                  }
                  ).Join(db.purchaseType,
                  a => a.PurchaseTypeId,
                  b => b.Id,
                  (a, b) => new
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = a.DistrictName,
                      CityId = a.CityId,
                      CityName = a.CityName,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      ContactTypeName = a.ContactTypeName,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNotes = a.RelationshipNotes,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      PurchaseTypeName = b.PurchaseTypeName,
                      AccountId = a.AccountId,
                      Gender = a.Gender
                  }).Join(db.account,
                  a => a.AccountId,
                  b => b.Id,
                  (a, b) => new CustomContact
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = a.DistrictName,
                      CityId = a.CityId,
                      CityName = a.CityName,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      ContactTypeName = a.ContactTypeName,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNote = a.RelationshipNotes,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      PurchaseTypeName = a.PurchaseTypeName,
                      AccountId = a.AccountId,
                      AccountName = b.AccountName,
                      Gender = a.Gender,
                      NumberOfMedicalVisits = medicalvisits,
                      FirstLineNames = firstlinenames,
                      MedicalNames = medicalnames
                  }).SingleOrDefault();


            List<Event> ev = new List<Event>();
            List<EventTravelRequest> etr = db.EventTravelRequest.Where(a=>a.ContactId == result.Id).ToList();

            foreach (var item in etr)
            {
                Event e = db.Event.Find(item.EventId);
                ev.Add(e);
            }


            result.events = ev;

            return result;
        }

        public CustomContact GetContactByIdWithoutAccount(int id)
        {
            Contact con = db.contact.Find(id);
            int CityId = db.district.Where(a => a.Id == con.DistrictId).Select(a => a.CityId).First();
            int medicalvisits = db.contactMedicalVisit.Where(a => a.ContactId == id).Count();
            IEnumerable<string> firstlinenames = db.userContact.Where(a => a.ContactId == id).Join(db.Users, a => a.extendidentityuserid, b => b.Id, (a, b) => new
            {
                ManagerId = b.extendidentityuserid
            }).Join(db.Users, a => a.ManagerId, b => b.Id, (a, b) => new
            {
                Name = b.FullName
            }).Select(a => a.Name);
            IEnumerable<string> medicalids = db.contactMedicalVisit.Where(a => a.ContactId == id).Select(a => a.extendidentityuserid);
            List<string> medicalnames = new List<string>();
            foreach (var item in medicalids)
            {
                string username = db.Users.Where(a => a.Id == item).Select(a => a.FullName).SingleOrDefault();
                medicalnames.Add(username);
            }




            CustomContact result = db.contact
               .Where(x => x.Id == id).Join(db.district,
               a => a.DistrictId,
               b => b.Id,
               (a, b) =>
                  new
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = b.Id,
                      DistrictName = b.DistrictName,
                      CityId = b.CityId,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNotes = a.RelationshipNote,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      Gender = a.Gender
                  }).Join(db.city,
                  a => a.CityId,
                  b => b.Id,
                  (a, b) => new
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = a.DistrictName,
                      CityId = a.CityId,
                      CityName = b.CityName,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNotes = a.RelationshipNotes,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      Gender = a.Gender
                  }).Join(db.contactType,
                  a => a.ContactTypeId,
                  b => b.Id,
                  (a, b) =>
                  new
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = a.DistrictName,
                      CityId = a.CityId,
                      CityName = a.CityName,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      ContactTypeName = b.ContactTypeName,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNotes = a.RelationshipNotes,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      Gender = a.Gender
                  }
                  ).Join(db.purchaseType,
                  a => a.PurchaseTypeId,
                  b => b.Id,
                  (a, b) => new CustomContact
                  {
                      Id = a.Id,
                      ContactName = a.ContactName,
                      DistrictId = a.DistrictId,
                      DistrictName = a.DistrictName,
                      CityId = a.CityId,
                      CityName = a.CityName,
                      Address = a.Address,
                      LandLineNumber = a.LandLineNumber,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      ContactTypeId = a.ContactTypeId,
                      ContactTypeName = a.ContactTypeName,
                      PaymentNotes = a.PaymentNotes,
                      RelationshipNote = a.RelationshipNotes,
                      BestTimeFrom = a.BestTimeFrom,
                      BestTimeTo = a.BestTimeTo,
                      PurchaseTypeId = a.PurchaseTypeId,
                      PurchaseTypeName = b.PurchaseTypeName,
                      Gender = a.Gender,
                      NumberOfMedicalVisits = medicalvisits,
                      FirstLineNames = firstlinenames,
                      MedicalNames = medicalnames
                  }).SingleOrDefault();



            List<Event> ev = new List<Event>();
            List<EventTravelRequest> etr = db.EventTravelRequest.Where(a => a.ContactId == result.Id).ToList();

            foreach (var item in etr)
            {
                Event e = db.Event.Find(item.EventId);
                ev.Add(e);
            }

            result.events = ev;

            return result;
        }

        public IEnumerable<CustomContact> SearchContact(SearchByWord contactName)
        {
            string normalized = contactName.Word.Normalize().ToUpper();
            List<CustomContact> res = new List<CustomContact>();
            IEnumerable<CustomContact> one = db.contact.Where(a => a.ContactName.Contains(normalized) || a.Email.Contains(normalized) || a.MobileNumber.Contains(normalized))
                .Join(db.district,
                a => a.DistrictId,
                b => b.Id,
                (a, b) =>
                   new
                   {
                       Id = a.Id,
                       ContactTypeId = a.ContactTypeId,
                       DistrictId = a.DistrictId,
                       ContactName = a.ContactName,
                       DistrictName = b.DistrictName,
                       CityId = b.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountId = a.AccountId,
                       Hsan = a.Hsan
                   }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContact
                   {
                       Id = a.Id,
                       ContactTypeId = a.ContactTypeId,
                       ContactName = a.ContactName,
                       DistrictId = a.DistrictId,
                       DistrictName = a.DistrictName,
                       CityId = a.CityId,
                       MobileNumber = a.MobileNumber,
                       Email = a.Email,
                       AccountName = b.AccountName,
                       Hsan = a.Hsan
                   });
            IEnumerable<CustomContact> two = db.contact.Where(a => a.ContactName.Contains(normalized))
               .Join(db.district,
               a => a.DistrictId,
               b => b.Id,
               (a, b) =>
                  new CustomContact
                  {
                      Id = a.Id,
                      ContactTypeId = a.ContactTypeId,
                      DistrictId = a.DistrictId,
                      ContactName = a.ContactName,
                      DistrictName = b.DistrictName,
                      CityId = b.CityId,
                      MobileNumber = a.MobileNumber,
                      Email = a.Email,
                      AccountId = a.AccountId,
                      Hsan = a.Hsan
                  });
            foreach (var item in one)
            {
                res.Add(item);
            }
            foreach (var item in two)
            {
                res.Add(item);
            }
            return res.DistinctBy(a=>a.Id).OrderBy(a=>a.AccountName);
        }
    }
}

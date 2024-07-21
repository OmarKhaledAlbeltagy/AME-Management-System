using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using MoreLinq;
using MoreLinq.Extensions;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq.Experimental;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;

namespace AMEKSA.Repo
{
    public class SystemAdminRep:ISystemAdminRep
    {
        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly ITimeRep ti;

        public SystemAdminRep(DbContainer db, UserManager<ExtendIdentityUser> userManager, ITimeRep ti)
        {
            this.db = db;
            this.userManager = userManager;
            this.ti = ti;
        }

        public IEnumerable<SystemAdminQuickVisitsReport> GetAccountMedicalVisitByAccountId(int id)
        {
            IEnumerable<SystemAdminQuickVisitsReport> result = db.accountMedicalVisit.Where(a => a.AccountId == id).Join(db.Users, a => a.extendidentityuserid, b => b.Id, (a, b) => new SystemAdminQuickVisitsReport
            {
                Id = a.Id,
                UserName = b.FullName,
                Date = a.VisitDate,
                VisitDate = a.VisitDate.ToString("dd MMMM yyyy"),
                VisitTime = a.VisitTime.ToString("hh:mm tt"),
                SubmittingDate = a.SubmittingDate.ToString("dd MMMM yyyy"),
                SubmittingTime = a.SubmittingTime.ToString("hh:mm tt")
            }).OrderByDescending(a => a.Date);

            return result;
        }

        public IEnumerable<SystemAdminQuickVisitsReport> GetAccountSalesVisitByAccountId(int id)
        {
            IEnumerable<SystemAdminQuickVisitsReport> result = db.accountSalesVisit.Where(a => a.AccountId == id).Join(db.Users, a => a.extendidentityuserid, b => b.Id, (a, b) => new SystemAdminQuickVisitsReport
            {
                Id = a.Id,
                UserName = b.FullName,
                Date = a.VisitDate,
                VisitDate = a.VisitDate.ToString("dd MMMM yyyy"),
                VisitTime = a.VisitTime.ToString("hh:mm tt"),
                SubmittingDate = a.SubmittingDate.ToString("dd MMMM yyyy"),
                SubmittingTime = a.SubmittingTime.ToString("hh:mm tt")
            }).OrderByDescending(a=>a.Date);

            return result;
        }

        public IEnumerable<SystemAdminQuickVisitsReport> GetAccountSupportiveVisitByAccountId(int id)
        {
            IEnumerable<SystemAdminQuickVisitsReport> result = db.AccountSupportiveVisit.Where(a => a.AccountId == id).Join(db.Users, a => a.extendidentityuserid, b => b.Id, (a, b) => new SystemAdminQuickVisitsReport
            {
                Id = a.Id,
                UserName = b.FullName,
                Date = a.VisitDate,
                VisitDate = a.VisitDate.ToString("dd MMMM yyyy"),
                VisitTime = a.VisitTime.ToString("hh:mm tt"),
                SubmittingDate = a.SubmittingDate.ToString("dd MMMM yyyy"),
                SubmittingTime = a.SubmittingTime.ToString("hh:mm tt")
            }).OrderByDescending(a => a.Date);

            return result;
        }

        public IEnumerable<AccountsPerUserModel> GetAllMedicalAccounts()
        {
            
            
            IEnumerable<ExtendIdentityUser> medical = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a=>a.Active == true);

            List <AccountsPerUserModel> res = new List<AccountsPerUserModel>();
            foreach (var item in medical)
            {
                AccountsPerUserModel obj = new AccountsPerUserModel();
                obj.FullName = item.FullName;
                
                List<CustomAccount> acc = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
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
                    PaymentNote = b.PaymentNote,
                    RelationshipNote = b.RelationshipNote,
                    PhoneNumber = b.PhoneNumber,
                    PurchaseTypeId = b.PurchaseTypeId
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = b.CategoryName,
                    DistrictId = a.DistrictId,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeId = a.AccountTypeId,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new 
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = b.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeId = a.AccountTypeId,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.accountType,a=>a.AccountTypeId,b=>b.Id,(a,b)=>new 
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeName = b.AccountTypeName,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.purchaseType,a=>a.PurchaseTypeId,b=>b.Id,(a,b)=>new CustomAccount
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeName = a.AccountTypeName,
                    PurchaseTypeName = b.PurchaseTypeName
                }).ToList();
                obj.Accounts = acc;
                res.Add(obj);
            }
            return res.OrderBy(a => a.FullName);
        }

        public IEnumerable<ContactsPerUserModel> GetAllMedicalContacts()
        {
            IEnumerable<ExtendIdentityUser> users = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a=>a.Active == true);
            List<ContactsPerUserModel> res = new List<ContactsPerUserModel>();

            foreach (var item in users)
            {
                ContactsPerUserModel obj = new ContactsPerUserModel();
                obj.FullName = item.FullName;
                var x = db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
                {
                    Id = b.Id,
                    ContactName = b.ContactName,
                    ContactTypeId = b.ContactTypeId,
                    Address = b.Address,
                    BestTimeFrom = b.BestTimeFrom,
                    BestTimeTo = b.BestTimeTo,
                    CategoryId = a.CategoryId,
                    DistrictId = b.DistrictId,
                    Email = b.Email,
                    PaymentNotes = b.PaymentNotes,
                    RelationshipNote = b.RelationshipNote,
                    LandLineNumber = b.LandLineNumber,
                    MobileNumber = b.MobileNumber,
                    PurchaseTypeId = b.PurchaseTypeId,
                    AccountId = b.AccountId,
                    Gender = b.Gender
                }).Join(db.contactType, a => a.ContactTypeId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactName = a.ContactName,
                    ContactTypeName = b.ContactTypeName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryId = a.CategoryId,
                    DistrictId = a.DistrictId,
                    Email = a.Email,
                    PaymentNotes = a.PaymentNotes,
                    RelationshipNote = a.RelationshipNote,
                    LandLineNumber = a.LandLineNumber,
                    MobileNumber = a.MobileNumber,
                    PurchaseTypeId = a.PurchaseTypeId,
                    AccountId = a.AccountId,
                    Gender = a.Gender
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactName = a.ContactName,
                    ContactTypeName = a.ContactTypeName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = b.CategoryName,
                    DistrictId = a.DistrictId,
                    Email = a.Email,
                    PaymentNotes = a.PaymentNotes,
                    RelationshipNote = a.RelationshipNote,
                    LandLineNumber = a.LandLineNumber,
                    MobileNumber = a.MobileNumber,
                    PurchaseTypeId = a.PurchaseTypeId,
                    AccountId = a.AccountId,
                    Gender = a.Gender
                }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactName = a.ContactName,
                    ContactTypeName = a.ContactTypeName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = b.DistrictName,
                    Email = a.Email,
                    PaymentNotes = a.PaymentNotes,
                    RelationshipNote = a.RelationshipNote,
                    LandLineNumber = a.LandLineNumber,
                    MobileNumber = a.MobileNumber,
                    PurchaseTypeId = a.PurchaseTypeId,
                    AccountId = a.AccountId,
                    Gender = a.Gender
                }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactName = a.ContactName,
                    ContactTypeName = a.ContactTypeName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNotes = a.PaymentNotes,
                    RelationshipNote = a.RelationshipNote,
                    LandLineNumber = a.LandLineNumber,
                    MobileNumber = a.MobileNumber,
                    PurchaseTypeName = b.PurchaseTypeName,
                    AccountId = a.AccountId,
                    Gender = a.Gender
                }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContact
                {
                    Id = a.Id,
                    ContactName = a.ContactName,
                    ContactTypeName = a.ContactTypeName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNotes = a.PaymentNotes,
                    RelationshipNote = a.RelationshipNote,
                    LandLineNumber = a.LandLineNumber,
                    MobileNumber = a.MobileNumber,
                    PurchaseTypeName = a.PurchaseTypeName,
                    AccountName = b.AccountName,
                    Gender = a.Gender
                }).ToList();

              List<CustomContact> y = DistinctByExtension.DistinctBy(x, a => a.Id).OrderBy(a => a.ContactName).ToList();

                obj.Contacts = y;
                res.Add(obj);
            }
            return res.OrderBy(a=>a.FullName);

        }

        public IEnumerable<AccountsPerUserModel> GetAllSalesAccounts()
        {
            IEnumerable<ExtendIdentityUser> sales = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.Active == true);

            List<AccountsPerUserModel> res = new List<AccountsPerUserModel>();
            foreach (var item in sales)
            {
                AccountsPerUserModel obj = new AccountsPerUserModel();
                obj.FullName = item.FullName;

                List<CustomAccount> acc = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
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
                    PaymentNote = b.PaymentNote,
                    RelationshipNote = b.RelationshipNote,
                    PhoneNumber = b.PhoneNumber,
                    PurchaseTypeId = b.PurchaseTypeId
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = b.CategoryName,
                    DistrictId = a.DistrictId,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeId = a.AccountTypeId,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = b.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeId = a.AccountTypeId,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeName = b.AccountTypeName,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new CustomAccount
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeName = a.AccountTypeName,
                    PurchaseTypeName = b.PurchaseTypeName
                }).ToList();
                obj.Accounts = acc;
                res.Add(obj);
            }
            return res.OrderBy(a => a.FullName);
        }

        public IEnumerable<AccountsPerUserModel> GetAllSupportiveAccounts()
        {
            IEnumerable<ExtendIdentityUser> sales = userManager.GetUsersInRoleAsync("Supportive").Result.Where(a => a.Active == true);

            List<AccountsPerUserModel> res = new List<AccountsPerUserModel>();
            foreach (var item in sales)
            {
                AccountsPerUserModel obj = new AccountsPerUserModel();
                obj.FullName = item.FullName;

                List<CustomAccount> acc = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
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
                    PaymentNote = b.PaymentNote,
                    RelationshipNote = b.RelationshipNote,
                    PhoneNumber = b.PhoneNumber,
                    PurchaseTypeId = b.PurchaseTypeId
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = b.CategoryName,
                    DistrictId = a.DistrictId,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeId = a.AccountTypeId,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = b.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeId = a.AccountTypeId,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeName = b.AccountTypeName,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new CustomAccount
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeName = a.AccountTypeName,
                    PurchaseTypeName = b.PurchaseTypeName
                }).ToList();
                obj.Accounts = acc;
                res.Add(obj);
            }
            return res.OrderBy(a => a.FullName);
        }

        public IEnumerable<SystemAdminQuickVisitsReport> GetContactMedicalVisitByContactId(int id)
        {
            IEnumerable<SystemAdminQuickVisitsReport> result = db.contactMedicalVisit.Where(a => a.ContactId == id).Join(db.Users, a => a.extendidentityuserid, b => b.Id, (a, b) => new SystemAdminQuickVisitsReport
            {
                Id = a.Id,
                Date = a.VisitDate,
                UserName = b.FullName,
                VisitDate = a.VisitDate.ToString("dd MMMM yyyy"),
                VisitTime = a.VisitTime.ToString("hh:mm tt"),
                SubmittingDate = a.SubmittingDate.ToString("dd MMMM yyyy"),
                SubmittingTime = a.SubmittingTime.ToString("hh:mm tt")
            }).OrderByDescending(a => a.Date);

            return result;
        }

        public AccountsPerUserModel GetRepAccounts(string UserId)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(UserId).Result;

        

            AccountsPerUserModel res = new AccountsPerUserModel();


            res.FullName = user.FullName;
            res.Accounts = db.userAccount.Where(a => a.extendidentityuserid == user.Id).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
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
                    PaymentNote = b.PaymentNote,
                    RelationshipNote = b.RelationshipNote,
                    PhoneNumber = b.PhoneNumber,
                    PurchaseTypeId = b.PurchaseTypeId
                }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    AccountTypeName = b.AccountTypeName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryId = a.CategoryId,
                    DistrictId = a.DistrictId,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    AccountTypeName = a.AccountTypeName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = b.CategoryName,
                    DistrictId = a.DistrictId,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    AccountTypeName = a.AccountTypeName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = b.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new CustomAccount
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    AccountTypeName = a.AccountTypeName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    PurchaseTypeName = b.PurchaseTypeName
                }).ToList();
                
           
            return res;
        }

        public ContactsPerUserModel GetRepContacts(string UserId)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(UserId).Result;
            ContactsPerUserModel res = new ContactsPerUserModel();



            res.FullName = user.FullName;
            res.Contacts = DistinctByExtension.DistinctBy(db.userContact.Where(a => a.extendidentityuserid == user.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
            {
                Id = b.Id,
                ContactName = b.ContactName,
                ContactTypeId = b.ContactTypeId,
                Address = b.Address,
                BestTimeFrom = b.BestTimeFrom,
                BestTimeTo = b.BestTimeTo,
                CategoryId = a.CategoryId,
                DistrictId = b.DistrictId,
                Email = b.Email,
                PaymentNotes = b.PaymentNotes,
                RelationshipNote = b.RelationshipNote,
                LandLineNumber = b.LandLineNumber,
                MobileNumber = b.MobileNumber,
                PurchaseTypeId = b.PurchaseTypeId,
                AccountId = b.AccountId,
                Gender = b.Gender
            }).Join(db.contactType, a => a.ContactTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                ContactTypeName = b.ContactTypeName,
                Address = a.Address,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                CategoryId = a.CategoryId,
                DistrictId = a.DistrictId,
                Email = a.Email,
                PaymentNotes = a.PaymentNotes,
                RelationshipNote = a.RelationshipNote,
                LandLineNumber = a.LandLineNumber,
                MobileNumber = a.MobileNumber,
                PurchaseTypeId = a.PurchaseTypeId,
                AccountId = a.AccountId,
                Gender = a.Gender
            }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                ContactTypeName = a.ContactTypeName,
                Address = a.Address,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                CategoryName = b.CategoryName,
                DistrictId = a.DistrictId,
                Email = a.Email,
                PaymentNotes = a.PaymentNotes,
                RelationshipNote = a.RelationshipNote,
                LandLineNumber = a.LandLineNumber,
                MobileNumber = a.MobileNumber,
                PurchaseTypeId = a.PurchaseTypeId,
                AccountId = a.AccountId,
                Gender = a.Gender
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                ContactTypeName = a.ContactTypeName,
                Address = a.Address,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                CategoryName = a.CategoryName,
                DistrictName = b.DistrictName,
                Email = a.Email,
                PaymentNotes = a.PaymentNotes,
                RelationshipNote = a.RelationshipNote,
                LandLineNumber = a.LandLineNumber,
                MobileNumber = a.MobileNumber,
                PurchaseTypeId = a.PurchaseTypeId,
                AccountId = a.AccountId,
                Gender = a.Gender
            }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                ContactTypeName = a.ContactTypeName,
                Address = a.Address,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                CategoryName = a.CategoryName,
                DistrictName = a.DistrictName,
                Email = a.Email,
                PaymentNotes = a.PaymentNotes,
                RelationshipNote = a.RelationshipNote,
                LandLineNumber = a.LandLineNumber,
                MobileNumber = a.MobileNumber,
                PurchaseTypeName = b.PurchaseTypeName,
                AccountId = a.AccountId,
                Gender = a.Gender
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContact
            {
                Id = a.Id,
                ContactName = a.ContactName,
                ContactTypeName = a.ContactTypeName,
                Address = a.Address,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                CategoryName = a.CategoryName,
                DistrictName = a.DistrictName,
                Email = a.Email,
                PaymentNotes = a.PaymentNotes,
                RelationshipNote = a.RelationshipNote,
                LandLineNumber = a.LandLineNumber,
                MobileNumber = a.MobileNumber,
                PurchaseTypeName = a.PurchaseTypeName,
                AccountName = b.AccountName,
                Gender = a.Gender
            }),a=>a.Id).ToList();
                
           
            
            return res;
        }

        public IEnumerable<UserSubmanagerModel> GetRepLinkedSubManagers(string RepId)
        {
            IEnumerable<UserSubmanagerModel> res = db.UserSubmanager.Where(a => a.RepId == RepId).Join(db.Users, a => a.ManagerId, b => b.Id, (a, b) => new
              UserSubmanagerModel
            {
                Id = a.Id,
                ManagerId = b.Id,
                ManagerName = b.FullName
            }).OrderBy(a => a.ManagerName);

            return res;
        }

        public IEnumerable<UserSubmanagerModel> GetRepNotLinkedSubManagers(string RepId)
        {
            string MainmanagerId = userManager.FindByIdAsync(RepId).Result.extendidentityuserid;
            IEnumerable<ExtendIdentityUser> allmanagers = userManager.GetUsersInRoleAsync("First Line manager").Result;
            IEnumerable<UserSubmanager> linked = db.UserSubmanager.Where(a => a.RepId == RepId);

            List<UserSubmanagerModel> res = new List<UserSubmanagerModel>();
            foreach (var item in allmanagers)
            {
                UserSubmanager x = linked.Where(a => a.ManagerId == item.Id).FirstOrDefault();

                if(x == null)
                {
                    UserSubmanagerModel obj = new UserSubmanagerModel();
                    obj.Id = 0;
                    obj.ManagerId = item.Id;
                    obj.ManagerName = item.FullName;
                    res.Add(obj);
                }
            }

            UserSubmanagerModel rem = res.Where(a => a.ManagerId == MainmanagerId).FirstOrDefault();
            res.Remove(rem);

            return res.OrderBy(a => a.ManagerName);

        }

        public IEnumerable<RepByCategory> GetRepsByCategory(int x)
        {
            List<RepByCategory> res = new List<RepByCategory>();
            if (x == 1)
            {
                IEnumerable<ExtendIdentityUser> Medicals = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a=>a.Active == true);
               
                foreach (var item in Medicals)
                {
             
                    RepByCategory o = new RepByCategory();
                    o.RepName = item.FullName;
                    o.TotalContacts = db.userContact.Where(a => a.extendidentityuserid == item.Id).Count();
                    if (o.TotalContacts == 0)
                    {
                        o.APlusContacts = 0;
                        o.AContacts = 0;
                        o.BContacts = 0;
                        o.CContacts = 0;
                        o.APlusContactsPercentage = 0;
                        o.AContactsPercentage = 0;
                        o.BContactsPercentage = 0;
                        o.CContactsPercentage = 0;
                        o.APlusContactsTarget = 0;
                        o.AContactsTarget = 0;
                        o.BContactsTarget = 0;
                        o.CContactsTarget = 0;
                        o.APlusContactsPercentage = 0;
                        o.AContactsPercentage = 0;
                        o.BContactsPercentage = 0;
                        o.CContactsPercentage = 0;
                        o.TotalTarget = 0;
                        res.Add(o);
                    }
                    else { 
                    o.APlusContacts = db.userContact.Where(a => a.extendidentityuserid == item.Id && a.CategoryId == 1).Count();
                    o.AContacts = db.userContact.Where(a => a.extendidentityuserid == item.Id && a.CategoryId == 2).Count();
                    o.BContacts = db.userContact.Where(a => a.extendidentityuserid == item.Id && a.CategoryId == 3).Count();
                    o.CContacts = db.userContact.Where(a => a.extendidentityuserid == item.Id && a.CategoryId == 4).Count();
                    o.APlusContactsPercentage = ((float)o.APlusContacts / (float)o.TotalContacts) * 100;
                    o.AContactsPercentage = ((float)o.AContacts / (float)o.TotalContacts) * 100;
                    o.BContactsPercentage = ((float)o.BContacts / (float)o.TotalContacts) * 100;
                    o.CContactsPercentage = ((float)o.CContacts / (float)o.TotalContacts) * 100;
                    o.APlusContactsTarget = db.userContact.Where(a => a.extendidentityuserid == item.Id && a.CategoryId == 1).Select(a=>a.MonthlyTarget).Sum();
                    o.AContactsTarget = db.userContact.Where(a => a.extendidentityuserid == item.Id && a.CategoryId == 2).Select(a => a.MonthlyTarget).Sum();
                    o.BContactsTarget = db.userContact.Where(a => a.extendidentityuserid == item.Id && a.CategoryId == 3).Select(a => a.MonthlyTarget).Sum();
                    o.CContactsTarget = db.userContact.Where(a => a.extendidentityuserid == item.Id && a.CategoryId == 4).Select(a => a.MonthlyTarget).Sum();
                    o.TotalTarget = db.userContact.Where(a => a.extendidentityuserid == item.Id).Select(a => a.MonthlyTarget).Sum();
                        if(o.APlusContactsTarget == 0)
                        {
                            o.APlusContactsTargetPercentage = 0;
                        }
                        else
                        {
                            o.APlusContactsTargetPercentage = ((float)o.APlusContactsTarget / (float)o.TotalTarget) * 100;
                        }

                        if(o.AContactsTarget == 0)
                        {
                            o.AContactsTargetPercentage = 0;
                        }
                        else
                        {
                            o.AContactsTargetPercentage = ((float)o.APlusContactsTarget / (float)o.TotalTarget) * 100;
                        }

                        if (o.BContactsTarget == 0)
                        {
                            o.BContactsTargetPercentage = 0;
                        }
                        else
                        {
                            o.BContactsTargetPercentage = ((float)o.APlusContactsTarget / (float)o.TotalTarget) * 100;
                        }
                        if (o.CContactsTarget == 0)
                        {
                            o.CContactsTargetPercentage = 0;
                        }
                        else
                        {
                            o.CContactsTargetPercentage = ((float)o.APlusContactsTarget / (float)o.TotalTarget) * 100;
                        }
                      
                        res.Add(o);
                    }
                }

            }

            else
            {
                if (x == 2)
                {
                    IEnumerable<ExtendIdentityUser> Sales = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.Active == true);
                    foreach (var item in Sales)
                    {
                        RepByCategory o = new RepByCategory();
                        o.RepName = item.FullName;
                        o.TotalContacts = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Count();
                        if (o.TotalContacts == 0)
                        {
                            o.APlusContacts = 0;
                            o.AContacts = 0;
                            o.BContacts = 0;
                            o.CContacts = 0;
                            o.APlusContactsPercentage = 0;
                            o.AContactsPercentage = 0;
                            o.BContactsPercentage = 0;
                            o.CContactsPercentage = 0;
                            res.Add(o);
                        }
                        else
                        {
                            o.APlusContacts = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
                            {
                                Id = a.AccountId,
                                CategoryId = b.CategoryId
                            }).Where(c => c.CategoryId == 1).Count();
                            o.AContacts = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
                            {
                                Id = a.AccountId,
                                CategoryId = b.CategoryId
                            }).Where(c => c.CategoryId == 2).Count();
                            o.BContacts = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
                            {
                                Id = a.AccountId,
                                CategoryId = b.CategoryId
                            }).Where(c => c.CategoryId == 3).Count();
                            o.CContacts = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
                            {
                                Id = a.AccountId,
                                CategoryId = b.CategoryId
                            }).Where(c => c.CategoryId == 4).Count();
                            o.APlusContactsPercentage = ((float)o.APlusContacts / (float)o.TotalContacts) * 100;
                            o.AContactsPercentage = ((float)o.AContacts / (float)o.TotalContacts) * 100;
                            o.BContactsPercentage = ((float)o.BContacts / (float)o.TotalContacts) * 100;
                            o.CContactsPercentage = ((float)o.CContacts / (float)o.TotalContacts) * 100;
                            res.Add(o);
                        }

                    }
   
                }

                else
                {
                    if (x == 3)
                    {
                        IEnumerable<ExtendIdentityUser> Supportive = userManager.GetUsersInRoleAsync("Supportive").Result.Where(a => a.Active == true);
                        foreach (var item in Supportive)
                        {
                            RepByCategory o = new RepByCategory();
                            o.RepName = item.FullName;
                            o.TotalContacts = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Count();
                            if (o.TotalContacts == 0)
                            {
                                o.APlusContacts = 0;
                                o.AContacts = 0;
                                o.BContacts = 0;
                                o.CContacts = 0;
                                o.APlusContactsPercentage = 0;
                                o.AContactsPercentage = 0;
                                o.BContactsPercentage = 0;
                                o.CContactsPercentage = 0;
                                res.Add(o);
                            }
                            else
                            {
                                o.APlusContacts = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
                                {
                                    Id = a.AccountId,
                                    CategoryId = b.CategoryId
                                }).Where(c => c.CategoryId == 1).Count();
                                o.AContacts = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
                                {
                                    Id = a.AccountId,
                                    CategoryId = b.CategoryId
                                }).Where(c => c.CategoryId == 2).Count();
                                o.BContacts = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
                                {
                                    Id = a.AccountId,
                                    CategoryId = b.CategoryId
                                }).Where(c => c.CategoryId == 3).Count();
                                o.CContacts = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
                                {
                                    Id = a.AccountId,
                                    CategoryId = b.CategoryId
                                }).Where(c => c.CategoryId == 4).Count();
                                o.APlusContactsPercentage = ((float)o.APlusContacts / (float)o.TotalContacts) * 100;
                                o.AContactsPercentage = ((float)o.AContacts / (float)o.TotalContacts) * 100;
                                o.BContactsPercentage = ((float)o.BContacts / (float)o.TotalContacts) * 100;
                                o.CContactsPercentage = ((float)o.CContacts / (float)o.TotalContacts) * 100;
                                res.Add(o);
                            }

                        }
                        
                    }
                }
              
            }
            return res.OrderBy(a => a.RepName);
        }

        public IEnumerable<AccountsPerUserModel> GetTeamAccounts(string ManagerId)
        {
            List<ExtendIdentityUser> users = new List<ExtendIdentityUser>();
            IEnumerable<ExtendIdentityUser> sales = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a=>a.extendidentityuserid == ManagerId);
            IEnumerable<ExtendIdentityUser> medical = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a => a.extendidentityuserid == ManagerId);
            IEnumerable<ExtendIdentityUser> supportive = userManager.GetUsersInRoleAsync("Supportive").Result.Where(a => a.extendidentityuserid == ManagerId);


            foreach (var item in sales)
            {
                users.Add(item);
            }
            foreach (var item in medical)
            {
                users.Add(item);
            }
            foreach (var item in supportive)
            {
                users.Add(item);
            }

            List<AccountsPerUserModel> res = new List<AccountsPerUserModel>();
            foreach (var item in users)
            {
                AccountsPerUserModel obj = new AccountsPerUserModel();
                obj.FullName = item.FullName;

                List<CustomAccount> acc = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
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
                    PaymentNote = b.PaymentNote,
                    RelationshipNote = b.RelationshipNote,
                    PhoneNumber = b.PhoneNumber,
                    PurchaseTypeId = b.PurchaseTypeId
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = b.CategoryName,
                    DistrictId = a.DistrictId,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeId = a.AccountTypeId,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = b.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeId = a.AccountTypeId,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeName = b.AccountTypeName,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new CustomAccount
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeName = a.AccountTypeName,
                    PurchaseTypeName = b.PurchaseTypeName
                }).ToList();
                obj.Accounts = acc;
                res.Add(obj);
            }
            return res.OrderBy(a => a.FullName);
        }

        public IEnumerable<ContactsPerUserModel> GetTeamContacts(string ManagerId)
        {
            IEnumerable<ExtendIdentityUser> users = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a=>a.extendidentityuserid == ManagerId);
            List<ContactsPerUserModel> res = new List<ContactsPerUserModel>();

            foreach (var item in users)
            {
                ContactsPerUserModel obj = new ContactsPerUserModel();
                obj.FullName = item.FullName;
                obj.Contacts = DistinctByExtension.DistinctBy(db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
                {
                    Id = b.Id,
                    ContactName = b.ContactName,
                    ContactTypeId = b.ContactTypeId,
                    Address = b.Address,
                    BestTimeFrom = b.BestTimeFrom,
                    BestTimeTo = b.BestTimeTo,
                    CategoryId = a.CategoryId,
                    DistrictId = b.DistrictId,
                    Email = b.Email,
                    PaymentNotes = b.PaymentNotes,
                    RelationshipNote = b.RelationshipNote,
                    LandLineNumber = b.LandLineNumber,
                    MobileNumber = b.MobileNumber,
                    PurchaseTypeId = b.PurchaseTypeId,
                    AccountId = b.AccountId,
                    Gender = b.Gender
                }).Join(db.contactType, a => a.ContactTypeId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactName = a.ContactName,
                    ContactTypeName = b.ContactTypeName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryId = a.CategoryId,
                    DistrictId = a.DistrictId,
                    Email = a.Email,
                    PaymentNotes = a.PaymentNotes,
                    RelationshipNote = a.RelationshipNote,
                    LandLineNumber = a.LandLineNumber,
                    MobileNumber = a.MobileNumber,
                    PurchaseTypeId = a.PurchaseTypeId,
                    AccountId = a.AccountId,
                    Gender = a.Gender
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactName = a.ContactName,
                    ContactTypeName = a.ContactTypeName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = b.CategoryName,
                    DistrictId = a.DistrictId,
                    Email = a.Email,
                    PaymentNotes = a.PaymentNotes,
                    RelationshipNote = a.RelationshipNote,
                    LandLineNumber = a.LandLineNumber,
                    MobileNumber = a.MobileNumber,
                    PurchaseTypeId = a.PurchaseTypeId,
                    AccountId = a.AccountId,
                    Gender = a.Gender
                }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactName = a.ContactName,
                    ContactTypeName = a.ContactTypeName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = b.DistrictName,
                    Email = a.Email,
                    PaymentNotes = a.PaymentNotes,
                    RelationshipNote = a.RelationshipNote,
                    LandLineNumber = a.LandLineNumber,
                    MobileNumber = a.MobileNumber,
                    PurchaseTypeId = a.PurchaseTypeId,
                    AccountId = a.AccountId,
                    Gender = a.Gender
                }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactName = a.ContactName,
                    ContactTypeName = a.ContactTypeName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNotes = a.PaymentNotes,
                    RelationshipNote = a.RelationshipNote,
                    LandLineNumber = a.LandLineNumber,
                    MobileNumber = a.MobileNumber,
                    PurchaseTypeName = b.PurchaseTypeName,
                    AccountId = a.AccountId,
                    Gender = a.Gender
                }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContact
                {
                    Id = a.Id,
                    ContactName = a.ContactName,
                    ContactTypeName = a.ContactTypeName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNotes = a.PaymentNotes,
                    RelationshipNote = a.RelationshipNote,
                    LandLineNumber = a.LandLineNumber,
                    MobileNumber = a.MobileNumber,
                    PurchaseTypeName = a.PurchaseTypeName,
                    AccountName = b.AccountName,
                    Gender = a.Gender
                }),a=>a.Id).ToList();

                res.Add(obj);
            }
            return res.OrderBy(a => a.FullName);
        }

        public IEnumerable<AccountsPerUserModel> GetTeamSalesAccounts(string ManagerId)
        {
            List<ExtendIdentityUser> users = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.extendidentityuserid == ManagerId).ToList();

            List<string> subids = db.UserSubmanager.Where(a => a.ManagerId == ManagerId).Select(a => a.RepId).ToList();

            foreach (var item in subids)
            {
                ExtendIdentityUser subrep = userManager.FindByIdAsync(item).Result;
                users.Add(subrep);
            }
          

            List<AccountsPerUserModel> res = new List<AccountsPerUserModel>();
            foreach (var item in users)
            {
                AccountsPerUserModel obj = new AccountsPerUserModel();
                obj.FullName = item.FullName;

                List<CustomAccount> acc = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
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
                    PaymentNote = b.PaymentNote,
                    RelationshipNote = b.RelationshipNote,
                    PhoneNumber = b.PhoneNumber,
                    PurchaseTypeId = b.PurchaseTypeId
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = b.CategoryName,
                    DistrictId = a.DistrictId,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeId = a.AccountTypeId,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = b.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeId = a.AccountTypeId,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeName = b.AccountTypeName,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new CustomAccount
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeName = a.AccountTypeName,
                    PurchaseTypeName = b.PurchaseTypeName
                }).ToList();
                obj.Accounts = acc;
                res.Add(obj);
            }
            return res.OrderBy(a => a.FullName);
        }



        public IEnumerable<AccountsPerUserModel> GetTeamSupportiveAccounts(string ManagerId)
        {
            List<ExtendIdentityUser> users = userManager.GetUsersInRoleAsync("Supportive").Result.Where(a => a.extendidentityuserid == ManagerId).ToList();

            List<AccountsPerUserModel> res = new List<AccountsPerUserModel>();
            foreach (var item in users)
            {
                AccountsPerUserModel obj = new AccountsPerUserModel();
                obj.FullName = item.FullName;

                List<CustomAccount> acc = db.userAccount.Where(a => a.extendidentityuserid == item.Id).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
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
                    PaymentNote = b.PaymentNote,
                    RelationshipNote = b.RelationshipNote,
                    PhoneNumber = b.PhoneNumber,
                    PurchaseTypeId = b.PurchaseTypeId
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = b.CategoryName,
                    DistrictId = a.DistrictId,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeId = a.AccountTypeId,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = b.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeId = a.AccountTypeId,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeName = b.AccountTypeName,
                    PurchaseTypeId = a.PurchaseTypeId
                }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new CustomAccount
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    Address = a.Address,
                    BestTimeFrom = a.BestTimeFrom,
                    BestTimeTo = a.BestTimeTo,
                    CategoryName = a.CategoryName,
                    DistrictName = a.DistrictName,
                    Email = a.Email,
                    PaymentNote = a.PaymentNote,
                    RelationshipNote = a.RelationshipNote,
                    PhoneNumber = a.PhoneNumber,
                    AccountTypeName = a.AccountTypeName,
                    PurchaseTypeName = b.PurchaseTypeName
                }).ToList();
                obj.Accounts = acc;
                res.Add(obj);
            }
            return res.OrderBy(a => a.FullName);
        }

        public IEnumerable<CustomAccount> GetUnAssignedAccounts()
        {
            List<Account> accs = db.account.Select(a => a).ToList();
            
            List<Account> uas = db.userAccount.Join(db.account,a=>a.AccountId,b=>b.Id,(a,b)=>new Account
            {
                Id = b.Id,
                AccountName = b.AccountName,
                Address = b.Address,
                BestTimeFrom = b.BestTimeFrom,
                BestTimeTo = b.BestTimeTo,
                CategoryId = b.CategoryId,
                DistrictId = b.DistrictId,
                Email = b.Email,
                PaymentNote = b.PaymentNote,
                RelationshipNote = b.RelationshipNote,
                PhoneNumber = b.PhoneNumber,
                AccountTypeId = b.AccountTypeId,
                PurchaseTypeId = b.PurchaseTypeId
            }).ToList();

            foreach (var item in uas)
            {
                Account obj = accs.Where(a=>a.Id == item.Id).SingleOrDefault();

             accs.Remove(obj);
                
            }

            List<CustomAccount> res = accs.Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountName = a.AccountName,
                Address = a.Address,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                CategoryName = b.CategoryName,
                DistrictId = a.DistrictId,
                Email = a.Email,
                PaymentNote = a.PaymentNote,
                RelationshipNote = a.RelationshipNote,
                PhoneNumber = a.PhoneNumber,
                AccountTypeId = a.AccountTypeId,
                PurchaseTypeId = a.PurchaseTypeId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountName = a.AccountName,
                Address = a.Address,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                CategoryName = a.CategoryName,
                DistrictName = b.DistrictName,
                Email = a.Email,
                PaymentNote = a.PaymentNote,
                RelationshipNote = a.RelationshipNote,
                PhoneNumber = a.PhoneNumber,
                AccountTypeId = a.AccountTypeId,
                PurchaseTypeId = a.PurchaseTypeId
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountName = a.AccountName,
                Address = a.Address,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                CategoryName = a.CategoryName,
                DistrictName = a.DistrictName,
                Email = a.Email,
                PaymentNote = a.PaymentNote,
                RelationshipNote = a.RelationshipNote,
                PhoneNumber = a.PhoneNumber,
                AccountTypeName = b.AccountTypeName,
                PurchaseTypeId = a.PurchaseTypeId
            }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new CustomAccount
            {
                Id = a.Id,
                AccountName = a.AccountName,
                Address = a.Address,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                CategoryName = a.CategoryName,
                DistrictName = a.DistrictName,
                Email = a.Email,
                PaymentNote = a.PaymentNote,
                RelationshipNote = a.RelationshipNote,
                PhoneNumber = a.PhoneNumber,
                AccountTypeName = a.AccountTypeName,
                PurchaseTypeName = b.PurchaseTypeName
            }).ToList();

            return res.OrderBy(a => a.AccountName);
        }

        public IEnumerable<CustomContact> GetUnAssignedContacts()
        {
            List<Contact> cons = db.contact.Select(a => a).ToList();
       
            List<ContactWithCategory> ucs = db.userContact.Join(db.contact,a=>a.ContactId,b=>b.Id,(a,b)=>new ContactWithCategory
            {
                Id = b.Id,
                ContactName = b.ContactName,
                ContactTypeId = b.ContactTypeId,
                Address = b.Address,
                BestTimeFrom = b.BestTimeFrom,
                BestTimeTo = b.BestTimeTo,
                CategoryId = a.CategoryId,
                DistrictId = b.DistrictId,
                Email = b.Email,
                PaymentNotes = b.PaymentNotes,
                RelationshipNote = b.RelationshipNote,
                LandLineNumber = b.LandLineNumber,
                MobileNumber = b.MobileNumber,
                PurchaseTypeId = b.PurchaseTypeId,
                AccountId = b.AccountId,
                Gender = b.Gender
            }).ToList();

            foreach (var item in ucs)
            {
                Contact obj = cons.Where(a => a.Id == item.Id).SingleOrDefault();
                cons.Remove(obj);
            }

            List<CustomContact> res = cons.Join(db.contactType, a => a.ContactTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                ContactTypeName = b.ContactTypeName,
                Address = a.Address,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                DistrictId = a.DistrictId,
                Email = a.Email,
                PaymentNotes = a.PaymentNotes,
                RelationshipNote = a.RelationshipNote,
                LandLineNumber = a.LandLineNumber,
                MobileNumber = a.MobileNumber,
                PurchaseTypeId = a.PurchaseTypeId,
                AccountId = a.AccountId,
                Gender = a.Gender
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                ContactTypeName = a.ContactTypeName,
                Address = a.Address,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                DistrictName = b.DistrictName,
                Email = a.Email,
                PaymentNotes = a.PaymentNotes,
                RelationshipNote = a.RelationshipNote,
                LandLineNumber = a.LandLineNumber,
                MobileNumber = a.MobileNumber,
                PurchaseTypeId = a.PurchaseTypeId,
                AccountId = a.AccountId,
                Gender = a.Gender
            }).Join(db.purchaseType, a => a.PurchaseTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                ContactTypeName = a.ContactTypeName,
                Address = a.Address,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                DistrictName = a.DistrictName,
                Email = a.Email,
                PaymentNotes = a.PaymentNotes,
                RelationshipNote = a.RelationshipNote,
                LandLineNumber = a.LandLineNumber,
                MobileNumber = a.MobileNumber,
                PurchaseTypeName = b.PurchaseTypeName,
                AccountId = a.AccountId,
                Gender = a.Gender
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContact
            {
                Id = a.Id,
                ContactName = a.ContactName,
                ContactTypeName = a.ContactTypeName,
                Address = a.Address,
                BestTimeFrom = a.BestTimeFrom,
                BestTimeTo = a.BestTimeTo,
                DistrictName = a.DistrictName,
                Email = a.Email,
                PaymentNotes = a.PaymentNotes,
                RelationshipNote = a.RelationshipNote,
                LandLineNumber = a.LandLineNumber,
                MobileNumber = a.MobileNumber,
                PurchaseTypeName = a.PurchaseTypeName,
                AccountName = b.AccountName,
                Gender = a.Gender
            }).ToList();

            return res.OrderBy(a=>a.ContactName);
        }

        public bool LinkSubManager(string RepId, string ManagerId)
        {
            UserSubmanager obj = new UserSubmanager();
            obj.RepId = RepId;
            obj.ManagerId = ManagerId;
            db.UserSubmanager.Add(obj);
            db.SaveChanges();
            return true;
        }

        public bool UnLinkSubManager(string RepId, string ManagerId)
        {
            IEnumerable<UserSubmanager> l = db.UserSubmanager.Where(a => a.RepId == RepId && a.ManagerId == ManagerId);

            foreach (var item in l)
            {
                db.UserSubmanager.Remove(item);
            }
            db.SaveChanges();
            return true;
        }

        public bool WorkingDaysRemidner()
        {
            int year = ti.GetCurrentTime().AddMonths(1).Year;
            int month = ti.GetCurrentTime().AddMonths(1).Month;

            WorkingDays w = db.workingDays.Where(a => a.Month == month && a.Year == year).FirstOrDefault();

            if (w.NumberOfWorkingDays == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

using AMEKSA.Context;
using AMEKSA.FirstManagerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Entities;
using AMEKSA.CustomEntities;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;

using AMEKSA.Models;
using MoreLinq.Extensions;

namespace AMEKSA.Repo
{
    public class FirstManagerRep : IFirstManagerRep
    {
        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly ITimeRep ti;

        public FirstManagerRep(DbContainer db,UserManager<ExtendIdentityUser> userManager,ITimeRep ti)
        {
            this.db = db;
            this.userManager = userManager;
            this.ti = ti;
        }

        public IEnumerable<MyTeamModel> GetMyTeamMedical(string managerId)
        {

            
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int pastmonth = datenow.AddMonths(-1).Month;
            int year = datenow.Year;
            int pastmonthyear = datenow.AddMonths(-1).Year;
            int days = DateTime.DaysInMonth(year, month);
            int pastdays = DateTime.DaysInMonth(pastmonthyear, pastmonth);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);
            DateTime startpastmonth = new DateTime(year, pastmonth, 1);
            DateTime endpastmonth = new DateTime(year, pastmonth, pastdays);
            IEnumerable<ExtendIdentityUser> myteam = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a => a.extendidentityuserid == managerId && a.Active == true);
            
            List<MyTeamModel> result = new List<MyTeamModel>();
            foreach (var item in myteam)
            {
                int accountvisits = db.accountMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();
                int contactsvisits = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();
                int? contactstarget = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new { MonthlyTarget = b.MonthlyTarget }).Select(s => s.MonthlyTarget).Sum();
                int accountvisitspastmonth = db.accountMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= startpastmonth && a.VisitDate <= endpastmonth).Select(a => a.Id).Count();
                int contactsvisitspastmonth = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= startpastmonth && a.VisitDate <= endpastmonth).Select(a => a.Id).Count();
                int? contactstargetpastmonth = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= startpastmonth && a.VisitDate <= endpastmonth).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new { MonthlyTarget = b.MonthlyTarget }).Select(s => s.MonthlyTarget).Sum();
                MyTeamModel x = new MyTeamModel();
                x.AccountsVisits = accountvisits;
                x.ContactsVisits = contactsvisits;
                x.AccountsVisitsPastMonth = accountvisitspastmonth;
                x.ContactsVisitsPastMonth = contactsvisitspastmonth;
                x.FullName = item.FullName;
                x.PhoneNumber = item.PhoneNumber;
                x.Email = item.Email;
                x.userId = item.Id;
                x.ContactTarget = contactstarget;
                x.ContactTargetPastMonth = contactstargetpastmonth;
                result.Add(x);
            }
            return result;
        }

        public IEnumerable<CustomAccountMedicalVisit> GetMyTeamAMVisits(VisitsSearchModel obj)
        {
            List<string> myteamids = new List<string>();
            if (obj.ManagerId == "0")
            {
                myteamids = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a => a.Active == true).Select(a => a.Id).ToList();
            }
            else
            {
                myteamids = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a=>a.extendidentityuserid == obj.ManagerId && a.Active == true).Select(a => a.Id).ToList();
            }


            List<AccountMedicalVisit> myteamvisits = new List<AccountMedicalVisit>();
            foreach (var id in myteamids)
            {
                IEnumerable<AccountMedicalVisit> visits = db.accountMedicalVisit.Where(a => a.extendidentityuserid == id && a.VisitDate >= obj.Start && a.VisitDate <= obj.End);

                foreach (var item in visits)
                {
                    myteamvisits.Add(item);
                }
            }

            List<CustomAccountMedicalVisit> result = DistinctByExtension.DistinctBy(myteamvisits.Join(db.account,
                a => a.AccountId,
                b => b.Id,
                (a, b) => new
                {
                    Id = a.Id,
                    AccountName = b.AccountName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserId = a.extendidentityuserid,
                    PhoneNumber = b.PhoneNumber,
                    Email = b.Email,
                    AccountTypeId = b.AccountTypeId,
                    CategoryId = b.CategoryId
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserId = a.UserId,
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    AccountTypeId = a.AccountTypeId,
                    CategoryName = b.CategoryName
                }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserId = a.UserId,
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    AccountTypeName = b.AccountTypeName,
                    CategoryName = a.CategoryName
                }).Join(db.Users.Where(a => a.Active == true), a => a.UserId, b => b.Id, (a, b) => new CustomAccountMedicalVisit
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserName = b.FullName,
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    AccountTypeName = a.AccountTypeName,
                    CategoryName = a.CategoryName
                }), o => o.Id).OrderByDescending(a => a.VisitDate).OrderBy(a => a.UserName).ToList();
                


            return result;
        }

        public IEnumerable<CustomAccountSalesVisit> GetMyTeamASVisits(VisitsSearchModel obj)
        {
            List<string> myteamids = new List<string>();
            if (obj.ManagerId == "0")
            {
                myteamids = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.Active == true).Select(a => a.Id).ToList();
            }
            else
            {
                myteamids = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.extendidentityuserid == obj.ManagerId && a.Active == true).Select(a => a.Id).ToList();

                List<string> subteamids = db.Users.Where(a => a.Active == true).Join(db.UserSubmanager.Where(a => a.ManagerId == obj.ManagerId), a => a.Id, b => b.RepId, (a, b) => new UserSubmanager
                {
                    Id = b.Id,
                    RepId = b.RepId,
                    ManagerId = b.ManagerId
                }).Select(a => a.RepId).ToList();


                foreach (var item in subteamids)
                {
                    myteamids.Add(item);
                }
            }

            List<AccountSalesVisit> myteamvisits = new List<AccountSalesVisit>();
            foreach (var id in myteamids)
            {
                List<AccountSalesVisit> visits = db.accountSalesVisit.Where(a => a.extendidentityuserid == id && a.VisitDate >= obj.Start && a.VisitDate <= obj.End).ToList();

                foreach (var item in visits)
                {
                    myteamvisits.Add(item);
                }
            }

            List<CustomAccountSalesVisit> result = DistinctByExtension.DistinctBy(myteamvisits.Join(db.account,
                a => a.AccountId,
                b => b.Id,
                (a, b) => new
                {
                    Id = a.Id,
                    AccountName = b.AccountName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserId = a.extendidentityuserid,
                    PhoneNumber = b.PhoneNumber,
                    Email = b.Email,
                    AccountTypeId = b.AccountTypeId,
                    CategoryId = b.CategoryId
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserId = a.UserId,
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    AccountTypeId = a.AccountTypeId,
                    CategoryName = b.CategoryName
                }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserId = a.UserId,
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    AccountTypeName = b.AccountTypeName,
                    CategoryName = a.CategoryName
                }).Join(db.Users.Where(a => a.Active == true), a => a.UserId, b => b.Id, (a, b) => new CustomAccountSalesVisit
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserName = b.FullName,
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    AccountTypeName = a.AccountTypeName,
                    CategoryName = a.CategoryName
                }),o=>o.Id).OrderByDescending(a => a.VisitDate).OrderBy(a => a.UserName).ToList();


            return result;
        }

        public IEnumerable<CustomContactMedicalVisit> GetMyTeamCMVisits(VisitsSearchModel obj)
        {
            List<string> myteamids = new List<string>();
            if (obj.ManagerId == "0")
            {
                myteamids = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a => a.Active == true).Select(a => a.Id).ToList();
            }
            else
            {
                myteamids = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a => a.extendidentityuserid == obj.ManagerId && a.Active == true).Select(a => a.Id).ToList();
            }

            List<ContactMedicalVisit> myteamvisits = new List<ContactMedicalVisit>();
            foreach (var id in myteamids)
            {
                List<ContactMedicalVisit> visits = db.contactMedicalVisit.Where(a => a.extendidentityuserid == id && a.VisitDate >= obj.Start && a.VisitDate <= obj.End).ToList();

                foreach (var item in visits)
                {
                    myteamvisits.Add(item);
                }
            }
           
            IEnumerable<CustomContactMedicalVisit> result = DistinctByExtension.DistinctBy(myteamvisits.Join(db.contact,
                a => a.ContactId,
                b => b.Id,
                (a, b) => new
                {
                    Id = a.Id,
                    ContactId = b.Id,
                    ContactName = b.ContactName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserId = a.extendidentityuserid,
                    LandLineNumber = b.LandLineNumber,
                    MobileNumber = b.MobileNumber,
                    Email = b.Email,
                    ContactTypeId = b.ContactTypeId,
                    AccountId = b.AccountId
                }).Join(db.userContact, a => a.ContactId, b => b.ContactId, (a, b) => new
                {
                    Id = a.Id,
                    CategoryId = b.CategoryId,
                    ContactName = a.ContactName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserId = a.UserId,
                    UsrId = b.extendidentityuserid,
                    LandLineNumber = a.LandLineNumber,
                    MobileNumber = a.MobileNumber,
                    Email = a.Email,
                    ContactTypeId = a.ContactTypeId,
                    AccountId = a.AccountId
                }).Where(a => a.UserId == a.UsrId).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactName = a.ContactName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserId = a.UserId,
                    LandLineNumber = a.LandLineNumber,
                    MobileNumber = a.MobileNumber,
                    Email = a.Email,
                    ContactTypeId = a.ContactTypeId,
                    CategoryName = b.CategoryName,
                    AccountId = a.AccountId
                }).Join(db.contactType, a => a.ContactTypeId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactName = a.ContactName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserId = a.UserId,
                    LandLineNumber = a.LandLineNumber,
                    MobileNumber = a.MobileNumber,
                    Email = a.Email,
                    ContactTypeName = b.ContactTypeName,
                    CategoryName = a.CategoryName,
                    AccountId = a.AccountId
                }).Join(db.Users.Where(a => a.Active == true), a => a.UserId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactName = a.ContactName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserName = b.FullName,
                    LandLineNumber = a.LandLineNumber,
                    MobileNumber = a.MobileNumber,
                    Email = a.Email,
                    ContactTypeName = a.ContactTypeName,
                    CategoryName = a.CategoryName,
                    AccountId = a.AccountId
                }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContactMedicalVisit
                {
                    Id = a.Id,
                    ContactName = a.ContactName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserName = a.UserName,
                    LandLineNumber = a.LandLineNumber,
                    MobileNumber = a.MobileNumber,
                    Email = a.Email,
                    ContactTypeName = a.ContactTypeName,
                    CategoryName = a.CategoryName,
                    AccountName = b.AccountName
                }), o => o.Id).OrderByDescending(a => a.VisitDate).OrderBy(a => a.UserName);
                
             

            return result;
        }

        public IEnumerable<VisitsCountModel> GetVisitsCountByMonth(int year, int month, string myId)
        {

            List<string> myteamids = db.Users.Where(a => a.extendidentityuserid == myId && a.Active == true).Select(a=>a.Id).ToList();

            List<AccountMedicalVisit> accmedvisit = new List<AccountMedicalVisit>();
            List<ContactMedicalVisit> conmedvisit = new List<ContactMedicalVisit>();
            List<AccountSalesVisit> accsalvisit = new List<AccountSalesVisit>();
            List<AccountSupportiveVisit> accsupvisit = new List<AccountSupportiveVisit>();
            foreach (var item in myteamids)
            {
                List<AccountMedicalVisit> a = db.accountMedicalVisit.Where(ss => ss.extendidentityuserid == item).ToList();
                foreach (var b in a)
                {
                    accmedvisit.Add(b);
                }
                List<ContactMedicalVisit> c = db.contactMedicalVisit.Where(zz => zz.extendidentityuserid == item).ToList();
                foreach (var d in c)
                {
                    conmedvisit.Add(d);
                }
                List<AccountSalesVisit> x = db.accountSalesVisit.Where(bb => bb.extendidentityuserid == item).ToList();
                foreach (var y in x)
                {
                    accsalvisit.Add(y);
                }
                List<AccountSupportiveVisit> h = db.AccountSupportiveVisit.Where(kk => kk.extendidentityuserid == item).ToList();
                foreach (var g in h)
                {
                    accsupvisit.Add(g);
                }
            }




           
            DateTime datenow = ti.GetCurrentTime();
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);
            List<VisitsCountModel> result = new List<VisitsCountModel>();
            int accountmedical = accmedvisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();
            VisitsCountModel accmed = new VisitsCountModel();
            accmed.VisitType = "Medical To Accounts";
            accmed.VisitCount = accountmedical;
            result.Add(accmed);

            int contactmedical = conmedvisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();
            VisitsCountModel conmed = new VisitsCountModel();
            conmed.VisitType = "Medical To Contacts";
            conmed.VisitCount = contactmedical;
            result.Add(conmed);

            int accountsales = accsalvisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();
            VisitsCountModel accsal = new VisitsCountModel();
            accsal.VisitType = "Sales To Accounts";
            accsal.VisitCount = accountsales;
            result.Add(accsal);

            int accountsupportive = accsupvisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();
            VisitsCountModel accsup = new VisitsCountModel();
            accsup.VisitType = "Supportive To Accounts";
            accsup.VisitCount = accountsupportive;
            result.Add(accsup);

            return result;




        }

        public IEnumerable<AccountVisitsPercentageByCategoryModel> AccountThisMonthPercentage(string myId)
        {
            int? mycity = userManager.FindByIdAsync(myId).Result.CityId;
            
            int APlusId = db.category.Where(a => a.CategoryName == "A+").Select(a=>a.Id).SingleOrDefault();
            int AId = db.category.Where(a => a.CategoryName == "A").Select(a => a.Id).SingleOrDefault();
            int BId = db.category.Where(a => a.CategoryName == "B").Select(a => a.Id).SingleOrDefault();
            int CId = db.category.Where(a => a.CategoryName == "C").Select(a => a.Id).SingleOrDefault();

            var myaccs = db.account.Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountName = a.AccountName,
                CategoryId = a.CategoryId,
                DistrictId = a.DistrictId,
                CityId = b.CityId
            }).Where(x => x.CityId == mycity);




            int APlusAccounts = myaccs.Where(a => a.CategoryId == APlusId).Select(x => x.Id).Count();
            int AAccounts = myaccs.Where(a => a.CategoryId == AId).Select(x => x.Id).Count();
            int BAccounts = myaccs.Where(a => a.CategoryId == BId).Select(x => x.Id).Count();
            int CAccounts = myaccs.Where(a => a.CategoryId == CId).Select(x => x.Id).Count();






            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            
                int AMVAPlus = DistinctByExtension.DistinctBy(db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
                    a => a.AccountId, b => b.Id, (a, b) => new
                    {
                        Id = a.Id,
                        AccountId = b.Id,
                        CategoryId = b.CategoryId
                    }).Where(x => x.CategoryId == APlusId), a => a.AccountId).Select(c => c.Id).Count();
                

                
                int ASVAPlus = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
                    a => a.AccountId, b => b.Id, (a, b) => new
                    {
                        Id = a.Id,
                        AccountId = b.Id,
                        CategoryId = b.CategoryId
                    }).Where(x => x.CategoryId == APlusId), a => a.AccountId).Select(c => c.Id).Count();
                


              int AccountsVisitsAPlus = AMVAPlus + ASVAPlus;

                int AMVA = DistinctByExtension.DistinctBy(db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
                   a => a.AccountId, b => b.Id, (a, b) => new
                   {
                       Id = a.Id,
                       AccountId = b.Id,
                       CategoryId = b.CategoryId
                   }).Where(x => x.CategoryId == AId), a => a.AccountId).Select(c => c.Id).Count();
              

                int ASVA = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
                    a => a.AccountId, b => b.Id, (a, b) => new
                    {
                        Id = a.Id,
                        AccountId = b.Id,
                        CategoryId = b.CategoryId
                    }).Where(x => x.CategoryId == AId), a => a.AccountId).Select(c => c.Id).Count();


            int AccountsVisitsA = AMVA + ASVA;

                int AMVB = DistinctByExtension.DistinctBy(db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
                  a => a.AccountId, b => b.Id, (a, b) => new
                  {
                      Id = a.Id,
                      AccountId = b.Id,
                      CategoryId = b.CategoryId
                  }).Where(x => x.CategoryId == BId), z => z.AccountId).Select(c => c.Id).Count();
                
           

                int ASVB = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
                    a => a.AccountId, b => b.Id, (a, b) => new
                    {
                        Id = a.Id,
                        AccountId = b.Id,
                        CategoryId = b.CategoryId
                    }).Where(x => x.CategoryId == BId), z => z.AccountId).Select(c => c.Id).Count();
               

            int AccountsVisitsB = AMVB + ASVB;

                int AMVC = DistinctByExtension.DistinctBy(db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
                  a => a.AccountId, b => b.Id, (a, b) => new
                  {
                      Id = a.Id,
                      AccountId = b.Id,
                      CategoryId = b.CategoryId
                  }).Where(x => x.CategoryId == CId), a => a.AccountId).Select(c => c.Id).Count();
              

                int ASVC = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
                    a => a.AccountId, b => b.Id, (a, b) => new
                    {
                        Id = a.Id,
                        AccountId = b.Id,
                        CategoryId = b.CategoryId
                    }).Where(x => x.CategoryId == CId), a => a.AccountId).Select(c => c.Id).Count();
             

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

        public IEnumerable<MyTeamModel> GetMyTeamSales(string managerId)
        {
            
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int pastmonth = datenow.AddMonths(-1).Month;
            int year = datenow.Year;
            int pastmonthyear = datenow.AddMonths(-1).Year;
            int days = DateTime.DaysInMonth(year,month);
            int pastdays = DateTime.DaysInMonth(pastmonthyear, pastmonth);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);
            DateTime startpastomonth = new DateTime(pastmonthyear, pastmonth, 1);
            DateTime endpastomonth = new DateTime(pastmonthyear, pastmonth, pastdays);
            List<ExtendIdentityUser> myteam = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.extendidentityuserid == managerId && a.Active == true).ToList();
            List<string> mysubteamids = db.UserSubmanager.Where(a => a.ManagerId == managerId).Select(a => a.RepId).ToList();
            foreach (var item in mysubteamids)
            {
                ExtendIdentityUser sub = userManager.FindByIdAsync(item).Result;
                myteam.Add(sub);
            }
            List<MyTeamModel> result = new List<MyTeamModel>();
            foreach (var item in myteam)
            {
                int accountvisits = db.accountSalesVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();
                int accountvisitspast = db.accountSalesVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= startpastomonth && a.VisitDate <= endpastomonth).Select(a => a.Id).Count();
                MyTeamModel x = new MyTeamModel();
                x.AccountsVisits = accountvisits;
                x.AccountsVisitsPastMonth = accountvisitspast;
                x.FullName = item.FullName;
                x.PhoneNumber = item.PhoneNumber;
                x.Email = item.Email;
                x.userId = item.Id;
                result.Add(x);
            }
            return result;
        }

        public IEnumerable<CustomOpenningRequest> GetMyTeamRequests(string managerId)
        {
            IEnumerable<string> myteamids = userManager.Users.Where(a => a.extendidentityuserid == managerId && a.Active == true).Select(a => a.Id);


            List<OpenningRequest> requests = new List<OpenningRequest>();
            foreach (var item in myteamids)
            {
                IEnumerable<OpenningRequest> x = db.openningRequest.Where(a => a.ExtendIdentityUserId == item);

                foreach (var xx in x)
                {
                    requests.Add(xx);
                }
            }


            IEnumerable<CustomOpenningRequest> result = db.accountBrandPayment.Join(requests, a => a.Id, b => b.AccountBrandPaymentId, (a, b) => new
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
            }).Join(db.Users.Where(a => a.Active == true), a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new CustomOpenningRequest
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
            }).Where(x=>x.Confirmed == false && x.Rejected == false);

            return result;
        }

        public bool DeclineRequest(int id)
        {
            OpenningRequest obj = db.openningRequest.Find(id);
            obj.Rejected = true;
            db.SaveChanges();
            return true;
        }

        public bool AcceptRequest(int id)
        {
            OpenningRequest obj = db.openningRequest.Find(id);
            AccountBrandPayment pay = db.accountBrandPayment.Find(obj.AccountBrandPaymentId);
            pay.Openning = pay.Openning + obj.RequestedOpenning;
            pay.Balance = pay.Openning - pay.Collection;
            obj.Confirmed = true;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<CustomAccountSalesVisit> GetAccountSalesVisitsByUserId(string userId)
        {
            
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);

            var visitsids = db.accountSalesVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id);

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




            IEnumerable<CustomAccountSalesVisit> result = db.accountSalesVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <=end).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
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
                UserId = a.extendidentityuserid
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
                DistrictId = a.DistrictId,
                UserId = a.UserId
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
                DistrictName = b.DistrictName,
                UserId = a.UserId
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new CustomAccountSalesVisit
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                brands = a.brands,
                persons = a.persons,
                UserId = a.UserId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                PaymentNotes = a.PaymentNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = b.AccountTypeName
            }).DistinctBy(o => o.Id).OrderByDescending(a => a.VisitDate);



            return result;
        }

        public IEnumerable<CustomAccountSalesVisit> GetASDetailedVisits(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom,monthfrom,dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);
            

            var visitsids = db.accountSalesVisit.Where(a => a.extendidentityuserid == userid && a.VisitDate >= from && a.VisitDate <= to).Select(a => a.Id);

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




            IEnumerable<CustomAccountSalesVisit> result = db.accountSalesVisit.Where(a => a.extendidentityuserid == userid && a.VisitDate >= from && a.VisitDate <= to).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
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
                CategoryId= b.CategoryId,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime
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
                SubmittingTime = a.SubmittingTime
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
                SubmittingTime = a.SubmittingTime
            }).Join(db.category,a=>a.CategoryId,b=>b.Id,(a,b)=>new {

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
                SubmittingTime = a.SubmittingTime
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new CustomAccountSalesVisit
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
                SubmittingTime = a.SubmittingTime
            }).DistinctBy(o => o.Id).OrderByDescending(a => a.VisitDate);



            return result;
        } 

        public List<CustomAccountMedicalVisit> GetAMDetailedVisits(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);

            var visitsids = db.accountMedicalVisit.Where(a => a.extendidentityuserid == userid && a.VisitDate >= from && a.VisitDate <= to).Select(a => a.Id);

            List<CustomAccountMedicalVisitProducts> productlist = new List<CustomAccountMedicalVisitProducts>();
            List<CustomVisitPerson> personlist = new List<CustomVisitPerson>();
            foreach (var item in visitsids)
            {
                var s = db.accountMedicalVisitProducts.Where(a => a.AccountMedicalVisitId == item).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
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

                var ss = db.accountMedicalVisitPerson.Where(a => a.AccountMedicalVisitId == item);
                foreach (var person in ss)
                {
                    CustomVisitPerson ab = new CustomVisitPerson();
                    ab.PersonName = person.PersonName;
                    ab.PersonPosition = person.PersonPosition;
                    ab.KeyId = person.AccountMedicalVisitId;
                    personlist.Add(ab);
                }

            }




            List<CustomAccountMedicalVisit> result = DistinctByExtension.DistinctBy(db.accountMedicalVisit.Where(a => a.extendidentityuserid == userid && a.VisitDate >= from && a.VisitDate <= to).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
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
                SubmittingDate = a.SubmittingDate
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
                SubmittingDate = a.SubmittingDate
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
                SubmittingDate = a.SubmittingDate
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
                SubmittingDate = a.SubmittingDate
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new CustomAccountMedicalVisit
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
                SubmittingDate = a.SubmittingDate
            }), a => a.Id).OrderByDescending(a => a.VisitDate).ToList();



            foreach (var item in result)
            {
                item.product = productlist.Where(x => x.AccountMedicalVisitId == item.Id).ToList();
                item.person = personlist.Where(c => c.KeyId == item.Id).ToList();
            }
         

            return result;
        }
     

        public List<CustomContactMedicalVisit> GetCMDetailedVisits(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);
            var visitsids = db.contactMedicalVisit.Where(a => a.extendidentityuserid == userid && a.VisitDate >= from && a.VisitDate <= to).Select(a => a.Id);

            List<CustomContactMedicalVisitProducts> productlist = new List<CustomContactMedicalVisitProducts>();
            List<CustomContactSalesAid> salesaidslist = new List<CustomContactSalesAid>();
            foreach (var item in visitsids)
            {
                var s = db.contactMedicalVisitProduct.Where(a => a.ContactMedicalVisitId == item).Join(db.product, a => a.ProductId, b => b.Id, (a, b) => new
                {
                    ProductName = b.ProductName,
                    ProductShare = a.ProductShare,
                    KeyId = a.ContactMedicalVisitId
                });
                foreach (var product in s)
                {
                    CustomContactMedicalVisitProducts ad = new CustomContactMedicalVisitProducts();
                    ad.ProductName = product.ProductName;
                    ad.ProductShare = product.ProductShare;
                    ad.ContactMedicalVisitId = product.KeyId;
                    productlist.Add(ad);
                }

                var ss = db.contactSalesAid.Where(a => a.ContactMedicalVisitId == item).Join(db.salesAid,a=>a.SalesAidId,b=>b.Id,(a,b)=>new 
                {
                    SalesAidName = b.SalesAidName,
                    ContactMedicalVisitId = a.ContactMedicalVisitId
                });
                foreach (var aid in ss)
                {
                    CustomContactSalesAid ab = new CustomContactSalesAid();
                    ab.SalesAidName = aid.SalesAidName;
                    ab.ContactMedicalVisitId = aid.ContactMedicalVisitId;
                    salesaidslist.Add(ab);
                }

            }




            List<CustomContactMedicalVisit> result = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.extendidentityuserid == userid && a.VisitDate >= from && a.VisitDate <= to).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = b.AccountId,
                ContactId = a.ContactId,
                ContactName = b.ContactName,
                ContactTypeId = b.ContactTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = b.DistrictId,
                SubmittingDate = a.SubmittingDate,
                extendidentityuserid = a.extendidentityuserid
            }).Join(db.contactMedicalVisitProduct, a => a.Id, b => b.ContactMedicalVisitId, (a, b) => new
            {
                Id = a.Id,
                ContactId = a.ContactId,
                AccountId = a.AccountId,
                ContactName = a.ContactName,
                ContactTypeId = a.ContactTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = a.DistrictId,
                SubmittingDate = a.SubmittingDate,
                extendidentityuserid = a.extendidentityuserid
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
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
                extendidentityuserid = a.extendidentityuserid
            }).Join(db.userContact, a => a.extendidentityuserid, b => b.extendidentityuserid, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                ContactTypeId = a.ContactTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = a.DistrictId,
                DistrictName = a.DistrictName,
                CategoryId = b.CategoryId,
                SubmittingDate = a.SubmittingDate
            }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new {

                Id = a.Id,
                AccountId = a.AccountId,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                ContactTypeId = a.ContactTypeId,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictId = a.DistrictId,
                DistrictName = a.DistrictName,
                CategoryName = b.CategoryName,
                SubmittingDate = a.SubmittingDate
            }).Join(db.contactType, a => a.ContactTypeId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                CategoryName = a.CategoryName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = b.ContactTypeName,
                SubmittingDate = a.SubmittingDate
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new CustomContactMedicalVisit
            {
                Id = a.Id,
                AccountName = b.AccountName,
                ContactId = a.ContactId,
                ContactName = a.ContactName,
                CategoryName = a.CategoryName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                Requests = a.Requests,
                DistrictName = a.DistrictName,
                ContactTypeName = a.ContactTypeName,
                SubmittingDate = a.SubmittingDate
            }),a=>a.Id).OrderByDescending(a => a.VisitDate).ToList();


            foreach (var item in result)
            {
                item.customcontactmedicalvisitproduct = productlist.Where(x => x.ContactMedicalVisitId == item.Id);
                item.customcontactsalesaid = salesaidslist.Where(c => c.ContactMedicalVisitId == item.Id);
            }
              
            return result;
        }

        

        public IEnumerable<AccountVisitsPercentageByCategoryModel> ContactThisMonthPercentage(string myId)
        {
            int? mycity = userManager.FindByIdAsync(myId).Result.CityId;
            IEnumerable<ContactCategoryModel> mycon = db.Users.Where(a => a.extendidentityuserid == myId).Join(db.userContact, a => a.Id, b => b.extendidentityuserid, (a, b) => new ContactCategoryModel
            {
                ContactId = b.ContactId,
                CategoryId = b.CategoryId
            });
            int APlusId = db.category.Where(a => a.CategoryName == "A+").Select(a => a.Id).SingleOrDefault();
            int AId = db.category.Where(a => a.CategoryName == "A").Select(a => a.Id).SingleOrDefault();
            int BId = db.category.Where(a => a.CategoryName == "B").Select(a => a.Id).SingleOrDefault();
            int CId = db.category.Where(a => a.CategoryName == "C").Select(a => a.Id).SingleOrDefault();



            IEnumerable<MyConsModel> mycons = db.contact.Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new 
            {
                Id = a.Id,
                ContactName = a.ContactName,
                DistrictId = a.DistrictId,
                CityId = b.CityId
            }).Join(mycon,a=>a.Id,b=>b.ContactId,(a,b)=> new MyConsModel
            {
                Id = a.Id,
                ContactName = a.ContactName,
                DistrictId = a.DistrictId,
                CityId = a.CityId,
                CategoryId = b.CategoryId
            }).Where(x => x.CityId == mycity);


            int APlusContacts = mycons.Where(a => a.CategoryId == APlusId).Select(x => x.Id).Count();
            int AContacts = mycons.Where(a => a.CategoryId == AId).Select(x => x.Id).Count();
            int BContacts = mycons.Where(a => a.CategoryId == BId).Select(x => x.Id).Count();
            int CContacts = mycons.Where(a => a.CategoryId == CId).Select(x => x.Id).Count();

           
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int year = datenow.Year;
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);


            int CMVAPlus = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(mycons,
                a => a.ContactId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId), a => a.ContactId).Select(c => c.Id).Count();



            int CMVA = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(mycons,
               a => a.ContactId, b => b.Id, (a, b) => new
               {
                   Id = a.Id,
                   ContactId = b.Id,
                   CategoryId = b.CategoryId
               }).Where(x => x.CategoryId == AId), a => a.ContactId).Select(c => c.Id).Count();
        


     

            int CMVB = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(mycons,
              a => a.ContactId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = b.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == BId), z => z.ContactId).Select(c => c.Id).Count();
;

            int CMVC = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(mycons,
              a => a.ContactId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = b.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == CId), a => a.ContactId).Select(c => c.Id).Count();
              



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

        public IEnumerable<AccountVisitsPercentageByCategoryModel> AccountpastMonthPercentage(string myId)
        {
            int? mycity = userManager.FindByIdAsync(myId).Result.CityId;

            int APlusId = db.category.Where(a => a.CategoryName == "A+").Select(a => a.Id).SingleOrDefault();
            int AId = db.category.Where(a => a.CategoryName == "A").Select(a => a.Id).SingleOrDefault();
            int BId = db.category.Where(a => a.CategoryName == "B").Select(a => a.Id).SingleOrDefault();
            int CId = db.category.Where(a => a.CategoryName == "C").Select(a => a.Id).SingleOrDefault();

            var myaccs = db.account.Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountName = a.AccountName,
                CategoryId = a.CategoryId,
                DistrictId = a.DistrictId,
                CityId = b.CityId
            }).Where(x => x.CityId == mycity);




            int APlusAccounts = myaccs.Where(a => a.CategoryId == APlusId).Select(x => x.Id).Count();
            int AAccounts = myaccs.Where(a => a.CategoryId == AId).Select(x => x.Id).Count();
            int BAccounts = myaccs.Where(a => a.CategoryId == BId).Select(x => x.Id).Count();
            int CAccounts = myaccs.Where(a => a.CategoryId == CId).Select(x => x.Id).Count();
  




        
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.AddMonths(-1).Month;
            int year = datenow.Year;
            
            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);


            int AMVAPlus = DistinctByExtension.DistinctBy(db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId), a => a.AccountId).Select(c => c.Id).Count();


            int ASVAPlus = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId), a => a.AccountId).Select(c => c.Id).Count();
                


            int AccountsVisitsAPlus = AMVAPlus + ASVAPlus;

            int AMVA = DistinctByExtension.DistinctBy(db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
               a => a.AccountId, b => b.Id, (a, b) => new
               {
                   Id = a.Id,
                   AccountId = b.Id,
                   CategoryId = b.CategoryId
               }).Where(x => x.CategoryId == AId), a => a.AccountId).Select(c => c.Id).Count();
        

            int ASVA = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == AId), a => a.AccountId).Select(c => c.Id).Count();
           

            int AccountsVisitsA = AMVA + ASVA;

            int AMVB = DistinctByExtension.DistinctBy(db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
              a => a.AccountId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  AccountId = b.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == BId), z => z.AccountId).Select(c => c.Id).Count();
           

            int ASVB = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == BId), z => z.AccountId).Select(c => c.Id).Count();
    

            int AccountsVisitsB = AMVB + ASVB;

            int AMVC = DistinctByExtension.DistinctBy(db.accountMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
              a => a.AccountId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  AccountId = b.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == CId), a => a.AccountId).Select(c => c.Id).Count();
          

            int ASVC = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(myaccs,
                a => a.AccountId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == CId), a => a.AccountId).Select(c => c.Id).Count();
           

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

        public IEnumerable<AccountVisitsPercentageByCategoryModel> ContactPastMonthPercentage(string myId)
        {
            int? mycity = userManager.FindByIdAsync(myId).Result.CityId;

            int APlusId = db.category.Where(a => a.CategoryName == "A+").Select(a => a.Id).SingleOrDefault();
            int AId = db.category.Where(a => a.CategoryName == "A").Select(a => a.Id).SingleOrDefault();
            int BId = db.category.Where(a => a.CategoryName == "B").Select(a => a.Id).SingleOrDefault();
            int CId = db.category.Where(a => a.CategoryName == "C").Select(a => a.Id).SingleOrDefault();


            IEnumerable<ContactCategoryModel> mycon = db.Users.Where(a => a.extendidentityuserid == myId).Join(db.userContact, a => a.Id, b => b.extendidentityuserid, (a, b) => new ContactCategoryModel
            {
                ContactId = b.ContactId,
                CategoryId = b.CategoryId
            });

            var mycons = db.contact.Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                DistrictId = a.DistrictId,
                CityId = b.CityId
            }).Join(mycon, a => a.Id, b => b.ContactId, (a, b) => new
            {
                Id = a.Id,
                ContactName = a.ContactName,
                DistrictId = a.DistrictId,
                CityId = a.CityId,
                CategoryId = b.CategoryId
            }).Where(x => x.CityId == mycity);


            int APlusContacts = mycons.Where(a => a.CategoryId == APlusId).Select(x => x.Id).Count();
            int AContacts = mycons.Where(a => a.CategoryId == AId).Select(x => x.Id).Count();
            int BContacts = mycons.Where(a => a.CategoryId == BId).Select(x => x.Id).Count();
            int CContacts = mycons.Where(a => a.CategoryId == CId).Select(x => x.Id).Count();

            
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.AddMonths(-1).Month;
            int year = datenow.Year;

            if (month == 12)
            {
                year = year - 1;
            }
            if (month == 0)
            {
                month = 12;
                year = year - 1;
            }

            int days = DateTime.DaysInMonth(year, month);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);


            int CMVAPlus = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(mycons,
                a => a.ContactId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    ContactId = b.Id,
                    CategoryId = b.CategoryId
                }).Where(x => x.CategoryId == APlusId), a => a.ContactId).Select(c => c.Id).Count();
    


            int CMVA = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(mycons,
               a => a.ContactId, b => b.Id, (a, b) => new
               {
                   Id = a.Id,
                   ContactId = b.Id,
                   CategoryId = b.CategoryId
               }).Where(x => x.CategoryId == AId),a=>a.ContactId).Select(c => c.Id).Count();
      




            int CMVB = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(mycons,
              a => a.ContactId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = b.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == BId), z => z.ContactId).Select(c => c.Id).Count();
       
            ;

            int CMVC = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.VisitDate >= start && a.VisitDate <= end).Join(mycons,
              a => a.ContactId, b => b.Id, (a, b) => new
              {
                  Id = a.Id,
                  ContactId = b.Id,
                  CategoryId = b.CategoryId
              }).Where(x => x.CategoryId == CId), a => a.ContactId).Select(c => c.Id).Count();
          



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

        public IEnumerable<MyTeamNamesModel> GetMyTeamNames(string ManagerId)
        {
            IEnumerable<ExtendIdentityUser> users = db.Users.Where(a => a.extendidentityuserid == ManagerId && a.Active == true);

            List<MyTeamNamesModel> res = new List<MyTeamNamesModel>();
            foreach (var item in users)
            {
                MyTeamNamesModel obj = new MyTeamNamesModel();
                obj.Id = item.Id;
                obj.FullName = item.FullName;
                res.Add(obj);
            }

            return res;
        }

        public IEnumerable<Account> GetFirstLineManagerAccounts(SearchByWord obj)
        {
            string normalized = obj.Word.Normalize().ToUpper();
            IEnumerable<Account> accounts = db.Users.Where(a => a.extendidentityuserid == obj.ManagerId && a.Active == true).Join(db.userAccount, a => a.Id, b => b.extendidentityuserid, (a, b) => new
            {
                AccountId = b.AccountId
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new Account
            {
                Id = b.Id,
                AccountName = b.AccountName,
                CreditLimit = b.CreditLimit
            });

            IEnumerable<Account> res = accounts.Where(a => a.AccountName.Contains(normalized));

            return res;
        }

        public IEnumerable<Contact> GetMyTeamContacts(string ManagerId)
        {
            List<int> ContactIds = db.Users.Where(a => a.extendidentityuserid == ManagerId && a.Active == true).Join(db.userContact, a => a.Id, b => b.extendidentityuserid, (a, b) => new
            {
                ContactId = b.ContactId
            }).Select(a=>a.ContactId).ToList();

            List<Contact> res = new List<Contact>();
            foreach(var item in ContactIds)
            {
                Contact x = db.contact.Find(item);
                res.Add(x);
            }

            return res.OrderBy(a=>a.ContactName);
          

        }

        public IEnumerable<MyTeamModel> GetMyTeamSupportive(string managerId)
        {
            DateTime datenow = ti.GetCurrentTime();
            int month = datenow.Month;
            int pastmonth = datenow.AddMonths(-1).Month;
            int year = datenow.Year;
            int pastmonthyear = datenow.AddMonths(-1).Year;
            int days = DateTime.DaysInMonth(year, month);
            int pastdays = DateTime.DaysInMonth(pastmonthyear, pastmonth);
            DateTime start = new DateTime(year, month, 1);
            DateTime end = new DateTime(year, month, days);
            DateTime startpastomonth = new DateTime(pastmonthyear, pastmonth, 1);
            DateTime endpastomonth = new DateTime(pastmonthyear, pastmonth, pastdays);
            IEnumerable<ExtendIdentityUser> myteam = userManager.GetUsersInRoleAsync("Supportive").Result.Where(a => a.extendidentityuserid == managerId && a.Active == true);

            List<MyTeamModel> result = new List<MyTeamModel>();
            foreach (var item in myteam)
            {
                int accountvisits = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();
                int accountvisitspast = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= startpastomonth && a.VisitDate <= endpastomonth).Select(a => a.Id).Count();
                MyTeamModel x = new MyTeamModel();
                x.AccountsVisits = accountvisits;
                x.AccountsVisitsPastMonth = accountvisitspast;
                x.FullName = item.FullName;
                x.PhoneNumber = item.PhoneNumber;
                x.Email = item.Email;
                x.userId = item.Id;
                result.Add(x);
            }
            return result;
        }

        public IEnumerable<CustomAccountSupportiveVisit> GetSupportiveDetailedVisits(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            DateTime from = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime to = new DateTime(yearto, monthto, dayto);


            var visitsids = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == userid && a.VisitDate >= from && a.VisitDate <= to).Select(a => a.Id);

            List<CustomVisitProduct> Productlist = new List<CustomVisitProduct>();
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
                    Productlist.Add(ad);
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




            IEnumerable<CustomAccountSupportiveVisit> result = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == userid && a.VisitDate >= from && a.VisitDate <= to).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
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
                SubmittingTime = a.SubmittingTime
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
                SubmittingTime = a.SubmittingTime
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
                SubmittingTime = a.SubmittingTime
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
                SubmittingTime = a.SubmittingTime
            }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new CustomAccountSupportiveVisit
            {
                Id = a.Id,
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                products = Productlist.Where(x => x.KeyId == a.Id),
                persons = personlist.Where(c => c.KeyId == a.Id),
                CategoryName = a.CategoryName,
                VisitDate = a.VisitDate,
                VisitTime = a.VisitTime,
                VisitNotes = a.VisitNotes,
                AdditionalNotes = a.PaymentNotes,
                DistrictName = a.DistrictName,
                AccountTypeName = b.AccountTypeName,
                SubmittingDate = a.SubmittingDate,
                SubmittingTime = a.SubmittingTime
            }).DistinctBy(o => o.Id).OrderByDescending(a => a.VisitDate);



            return result;
        }

        public IEnumerable<CustomAccountSupportiveVisit> GetMyTeamASupportiveVisits(VisitsSearchModel obj)
        {
            List<string> myteamids = new List<string>();
            if (obj.ManagerId == "0")
            {
                myteamids = userManager.GetUsersInRoleAsync("Supportive").Result.Where(a => a.Active == true).Select(a => a.Id).ToList();
            }
            else
            {
                myteamids = userManager.GetUsersInRoleAsync("Supportive").Result.Where(a => a.extendidentityuserid == obj.ManagerId && a.Active == true).Select(a => a.Id).ToList();
            }

            List<AccountSupportiveVisit> myteamvisits = new List<AccountSupportiveVisit>();
            foreach (var id in myteamids)
            {
                IEnumerable<AccountSupportiveVisit> visits = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == id && a.VisitDate >= obj.Start && a.VisitDate <= obj.End);

                foreach (var item in visits)
                {
                    myteamvisits.Add(item);
                }
            }

            List<CustomAccountSupportiveVisit> result = DistinctByExtension.DistinctBy(myteamvisits.Join(db.account,
                a => a.AccountId,
                b => b.Id,
                (a, b) => new
                {
                    Id = a.Id,
                    AccountName = b.AccountName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserId = a.extendidentityuserid,
                    PhoneNumber = b.PhoneNumber,
                    Email = b.Email,
                    AccountTypeId = b.AccountTypeId,
                    CategoryId = b.CategoryId
                }).Join(db.category, a => a.CategoryId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserId = a.UserId,
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    AccountTypeId = a.AccountTypeId,
                    CategoryName = b.CategoryName
                }).Join(db.accountType, a => a.AccountTypeId, b => b.Id, (a, b) => new
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserId = a.UserId,
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    AccountTypeName = b.AccountTypeName,
                    CategoryName = a.CategoryName
                }).Join(db.Users.Where(a => a.Active == true), a => a.UserId, b => b.Id, (a, b) => new CustomAccountSupportiveVisit
                {
                    Id = a.Id,
                    AccountName = a.AccountName,
                    VisitDate = a.VisitDate,
                    VisitTime = a.VisitTime,
                    UserName = b.FullName,
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    AccountTypeName = a.AccountTypeName,
                    CategoryName = a.CategoryName
                }), o => o.Id).OrderByDescending(a => a.VisitDate).OrderBy(a => a.UserName).ToList();


            return result;
        }
    }
}

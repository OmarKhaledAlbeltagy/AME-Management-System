using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Constraints;
using MoreLinq.Extensions;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AMEKSA.Repo
{
    public class KpisRep:IKpisRep
    {
        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly ITimeRep ti;


        public KpisRep(DbContainer db, UserManager<ExtendIdentityUser> userManager,ITimeRep ti)
        {
            this.db = db;
            this.userManager = userManager;
            this.ti = ti;
        }

        public bool EditProperty(int id, int value)
        {
            Entities.Properties obj = db.properties.Find(id);
            obj.Value = value;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<MedicalKpiModel> GetAllMedicalKpi(int yearfrom, int monthfrom, int yearto, int monthto)
        {
            int daysto = DateTime.DaysInMonth(yearto, monthto);
            DateTime start = new DateTime(yearfrom, monthfrom, 1, 0, 0, 0);
            DateTime end = new DateTime(yearto, monthto, daysto, 23, 59, 59);
            
            int? workingdaysdata = db.workingDays.Where(a => a.Month >= start.Month && a.Year >= start.Year && a.Month <= end.Month && a.Year <= end.Year).Select(a => a.NumberOfWorkingDays).Sum();
            IEnumerable<ExtendIdentityUser> users = userManager.GetUsersInRoleAsync("Medical Representative").Result.Where(a => a.Active == true);
            List<MedicalKpiModel> ress = new List<MedicalKpiModel>();
            foreach (var item in users)
            {
                int? timeoffdays = GetTimeOffDiff(item.Id, yearfrom, monthfrom, 1, yearto, monthto, daysto);
                   

             
               
                List<ContactWithCategory> UserAPlusAndA = DistinctByExtension.DistinctBy(db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
                {
                    Id = b.Id,
                    ContactName = b.ContactName,
                    Gender = b.Gender,
                    Address = b.Address,
                    LandLineNumber = b.LandLineNumber,
                    MobileNumber = b.MobileNumber,
                    Email = b.Email,
                    PaymentNotes = b.PaymentNotes,
                    RelationshipNote = b.RelationshipNote,
                    AccountId = b.AccountId,
                    CategoryId = a.CategoryId
                }),x=>x.Id).Where(a => a.CategoryId == 1 || a.CategoryId == 2).ToList();



                List<ContactWithCategory> UserB = DistinctByExtension.DistinctBy(db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
                {
                    Id = b.Id,
                    ContactName = b.ContactName,
                    Gender = b.Gender,
                    Address = b.Address,
                    LandLineNumber = b.LandLineNumber,
                    MobileNumber = b.MobileNumber,
                    Email = b.Email,
                    PaymentNotes = b.PaymentNotes,
                    RelationshipNote = b.RelationshipNote,
                    AccountId = b.AccountId,
                    CategoryId = a.CategoryId
                }), a => a.Id).Where(a => a.CategoryId == 3).ToList();


                List<ContactWithCategory> UserC = DistinctByExtension.DistinctBy(db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
                {
                    Id = b.Id,
                    ContactName = b.ContactName,
                    Gender = b.Gender,
                    Address = b.Address,
                    LandLineNumber = b.LandLineNumber,
                    MobileNumber = b.MobileNumber,
                    Email = b.Email,
                    PaymentNotes = b.PaymentNotes,
                    RelationshipNote = b.RelationshipNote,
                    AccountId = b.AccountId,
                    CategoryId = a.CategoryId
                }),a=>a.Id).Where(a => a.CategoryId == 4).ToList();
                    

                int AverageVisitPerDayKpiTarget = db.properties.Where(a => a.Id == 7).Select(a => a.Value).FirstOrDefault();
                int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 8).Select(a => a.Value).FirstOrDefault();
                int VisitsTargerAchievmentWeight = db.properties.Where(a => a.Id == 9).Select(a => a.Value).FirstOrDefault();
                int AAndAPlusWegiht = db.properties.Where(a => a.Id == 10).Select(a => a.Value).FirstOrDefault();
                int BWeight = db.properties.Where(a => a.Id == 11).Select(a => a.Value).FirstOrDefault();
                int CWeight = db.properties.Where(a => a.Id == 12).Select(a => a.Value).FirstOrDefault();
                int SellingDaysInTheFieldWegiht = db.properties.Where(a => a.Id == 13).Select(a => a.Value).FirstOrDefault();

                MedicalKpiModel res = new MedicalKpiModel();
                res.FullName = item.FullName;
                res.CityName = db.city.Where(a => a.Id == item.CityId).Select(a => a.CityName).FirstOrDefault();
                res.RoleName = "Medical Representative";
                res.Month = start.ToString("MMMM - yyyy")+" to "+end.ToString("MMMM - yyyy");
                res.ActualTotalNumberOfVisits = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();
                res.AplusAndAListed = UserAPlusAndA.Select(a => a.Id).Count();
                res.BListed = UserB.Select(a => a.Id).Count();
                res.CListed = UserC.Select(a => a.Id).Count();
                int APLusAndAVisited = 0;
                int BVisited = 0;
                int CVisited = 0;
                foreach (var aa in UserAPlusAndA)
                {
                    ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == aa.Id).FirstOrDefault();
                    if (x != null)
                    {
                        APLusAndAVisited++;
                    }
                }
                foreach (var bb in UserB)
                {
                    ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == bb.Id).FirstOrDefault();
                    if (x != null)
                    {
                        BVisited++;
                    }
                }
                foreach (var cc in UserC)
                {
                    ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == cc.Id).FirstOrDefault();
                    if (x != null)
                    {
                        CVisited++;
                    }
                }
                res.APlusAndAVisited = APLusAndAVisited;
                res.BVisited = BVisited;
                res.CVisited = CVisited;
                res.SellingDaysInTheField = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end),a=>a.VisitDate.Date).Select(a => a.Id).Count();

                res.WorkingDays = workingdaysdata - timeoffdays;
                res.AverageVisitsPerDayKpiTarget = AverageVisitPerDayKpiTarget;
                res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
                res.VisitsTargetAchievmentWeight = VisitsTargerAchievmentWeight;
                res.APlusAndAWeight = AAndAPlusWegiht;
                res.BWeight = BWeight;
                res.CWeight = CWeight;
                res.SellingDaysInTheFieldWeight = SellingDaysInTheFieldWegiht;
                res.TimeOffDays = timeoffdays;
                ress.Add(res);
            }
            return ress.OrderBy(a => a.FullName);
        }

        public List<KPIScoreModel> GetAllMedicalKpiChart(int yearfrom, int monthfrom, int yearto, int monthto)
        {
            List<MedicalKpiModel> Kpi = GetAllMedicalKpi(yearfrom, monthfrom, yearto, monthto).ToList();
            List<KPIScoreModel> res = new List<KPIScoreModel>();

            foreach (var item in Kpi)
            {
                int TargetedNumberOfVisitsKPITarget = (int)(item.WorkingDays * item.AverageVisitsPerDayKpiTarget);


                int VisitTaregAcheivmentKPITarget = TargetedNumberOfVisitsKPITarget;

                //zero
                float AverageVisitsPerDayActual = 0;
                if (item.SellingDaysInTheField != 0)
                {
                    AverageVisitsPerDayActual = (float)item.ActualTotalNumberOfVisits / (float)item.SellingDaysInTheField;
                }

                //zero
                float AverageVisitsPerDayRate = ((float)AverageVisitsPerDayActual / (float)item.AverageVisitsPerDayKpiTarget) * 100;

                //zero
                float VisitsTargetAcheivmentRate = ((float)item.ActualTotalNumberOfVisits / (float)VisitTaregAcheivmentKPITarget) * 100;

                //zero
                float ARate = 0;
                if (item.AplusAndAListed == 0)
                {

                }
                else
                {
                    float ARatee = (float)item.APlusAndAVisited / (float)item.AplusAndAListed;
                    ARate = ARatee * 100;
                }


                //zero
                float BRate = 0;
                if (item.BListed != 0)
                {
                    BRate = ((float)item.BVisited / (float)item.BListed) * 100;
                }


                float CRate = 0;
                if (item.CListed != 0)
                {
                    CRate = ((float)item.CVisited / (float)item.CListed) * 100;
                }

                float SellingDaysinTheFieldRate = ((float)item.SellingDaysInTheField / (float)item.WorkingDays) * 100;
                float AverageVisitsPerDayScore = ((float)AverageVisitsPerDayRate * (float)item.AverageVisitsPerDayWeight) / 100;
                float VisitsTargetAcheivmentScore = ((float)VisitsTargetAcheivmentRate * (float)item.VisitsTargetAchievmentWeight) / 100;
                float AScore = ((float)ARate * (float)item.APlusAndAWeight) / 100;
                float BScore = ((float)BRate * (float)item.BWeight) / 100;
                float CScore = ((float)CRate * (float)item.CWeight) / 100;
                float SellingDaysinTheFiledScore = ((float)SellingDaysinTheFieldRate * (float)item.SellingDaysInTheFieldWeight) / 100;
                float TotalScore = AverageVisitsPerDayScore + VisitsTargetAcheivmentScore + AScore + BScore + CScore + SellingDaysinTheFiledScore;
                float SCORE = (float)Math.Round(TotalScore, 0);

                KPIScoreModel obj = new KPIScoreModel();
                obj.Name = item.FullName;
                obj.Score = SCORE;
                res.Add(obj);
            }

            return res.OrderBy(a => a.Score).ToList();
        }

      
     

        public IEnumerable<Entities.Properties> GetAllProperties()
        {
            IEnumerable<Entities.Properties> res = db.properties.Select(a => a);
            return res;
        }

        public IEnumerable<SalesKpiModel> GetAllSalesKpi(int yearfrom,int monthfrom, int yearto, int monthto)
        {
            int daysto = DateTime.DaysInMonth(yearto, monthto);
            DateTime start = new DateTime(yearfrom, monthfrom, 1, 0, 0, 0);
            DateTime end = new DateTime(yearto, monthto, daysto, 23, 59, 59);
          
            int? WorkingDaysData = db.workingDays.Where(a => a.Month >= start.Month && a.Year >= start.Year && a.Month <= end.Month && a.Year <= end.Year).Select(a => a.NumberOfWorkingDays).Sum();

         


            IEnumerable<ExtendIdentityUser> list = userManager.GetUsersInRoleAsync("Sales Representative").Result.Where(a => a.Active == true);
            List<SalesKpiModel> result = new List<SalesKpiModel>();

            foreach (var obj in list)
            {
                int? timeoffdays = GetTimeOffDiff(obj.Id, start.Year, start.Month, 1, end.Year, end.Month, daysto);
                int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 3).Select(x => x.Value).FirstOrDefault();
                int VisitsTargetAchievmentWeight = db.properties.Where(a => a.Id == 2).Select(x => x.Value).FirstOrDefault();
                int CoverageForListedAccountsWeight = db.properties.Where(a => a.Id == 5).Select(x => x.Value).FirstOrDefault();
                int SellingDaysInTheFieldKpiWeight = db.properties.Where(a => a.Id == 6).Select(x => x.Value).FirstOrDefault();

                int AverageVisitsPerDayKpiTarget = db.properties.Where(a => a.Id == 1).Select(a => a.Value).SingleOrDefault();

                double? WorkingDays = WorkingDaysData - timeoffdays;

                string FullName = obj.FullName;

                string office = db.city.Where(a => a.Id == obj.CityId).Select(a => a.CityName).FirstOrDefault();

                string date = start.ToString("MMMM - yyyy")+" to "+end.ToString("MMMM - yyyy");

                double ActualTotalNumberOfVisits = db.accountSalesVisit.Where(a => a.extendidentityuserid == obj.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();

                double ListedAccounts = db.userAccount.Where(a => a.extendidentityuserid == obj.Id).DistinctBy(x => x.AccountId).Count();

                double SellingDaysInTheField = db.accountSalesVisit.Where(a => a.extendidentityuserid == obj.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(x => x.VisitDate.Date).Select(a => a.Id).Count();

                double VisitedAccounts = db.accountSalesVisit.Where(a => a.extendidentityuserid == obj.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(a => a.AccountId).Select(a => a.Id).Count();
                                

                SalesKpiModel res = new SalesKpiModel();
                res.FullName = FullName;
                res.Month = date;
                res.RoleName = "Sales Representative";

                res.CityName = office;
                res.ActualTotalNumberOfVisits = ActualTotalNumberOfVisits;
                res.ListedAccounts = ListedAccounts;
                res.SellingDaysInTheField = SellingDaysInTheField;
                //res.TargetedNumberOfVisits = TargetedNumberOfVisits;
                res.VisitedAccounts = VisitedAccounts;
                res.WorkingDays = WorkingDays;
                res.AverageVisitsPerDayKpiTarget = AverageVisitsPerDayKpiTarget;
               //res.AverageVisitsPerDayActual = AverageVisitsPerDayActual;
                //res.AverageVisitsPerDayRate = (AverageVisitsPerDayActual / AverageVisitsPerDayKpiTarget) * 100;
                //res.VisitsTargetAchievmentRate = (double)(ActualTotalNumberOfVisits / TargetedNumberOfVisits) * 100;
                //res.CoverageForListedAccountsRate = (VisitedAccounts / ListedAccounts) * 100;
                //res.SellingDaysInTheFieldKpiRate = (double)(SellingDaysInTheField / WorkingDays) * 100;
                res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
                res.VisitsTargetAchievmentWeight = VisitsTargetAchievmentWeight;
                res.CoverageForListedAccountsWeight = CoverageForListedAccountsWeight;
                res.SellingDaysInTheFieldKpiWeight = SellingDaysInTheFieldKpiWeight;
                res.TimeOffDays = timeoffdays;
                //res.AverageVisitsPerDayScore = (res.AverageVisitsPerDayRate * AverageVisitsPerDayWeight) / 100;
                //res.VisitsTargetAchievmentScore = (res.VisitsTargetAchievmentRate * VisitsTargetAchievmentWeight) / 100;
                //res.CoverageForListedAccountsScore = (res.CoverageForListedAccountsRate * CoverageForListedAccountsWeight) / 100;
                //res.SellingDaysInTheFieldScore = (res.SellingDaysInTheFieldKpiRate * SellingDaysInTheFieldKpiWeight) / 100;
                //res.TotalScore = res.AverageVisitsPerDayScore + res.VisitsTargetAchievmentScore + res.CoverageForListedAccountsScore + res.SellingDaysInTheFieldScore;
                result.Add(res);
            }
            return result.OrderBy(a => a.FullName);
        }

        public IEnumerable<SalesKpiModel> GetAllSupportiveKpi(int yearfrom, int monthfrom, int yearto, int monthto)
        {
            int daysto = DateTime.DaysInMonth(yearto, monthto);
            DateTime start = new DateTime(yearfrom, monthfrom, 1, 0, 0, 0);
            DateTime end = new DateTime(yearto, monthto, daysto, 23, 59, 59);

            int? WorkingDaysData = db.workingDays.Where(a => a.Month >= start.Month && a.Year >= start.Year && a.Month <= end.Month && a.Year <= end.Year).Select(a => a.NumberOfWorkingDays).Sum();
            IEnumerable<ExtendIdentityUser> list = userManager.GetUsersInRoleAsync("Supportive").Result.Where(a => a.Active == true);
            List<SalesKpiModel> result = new List<SalesKpiModel>();

            foreach (var obj in list)
            {
                int? timeoffdays = GetTimeOffDiff(obj.Id, yearfrom, monthfrom, 1, yearto, monthto, daysto);
                int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 3).Select(x => x.Value).FirstOrDefault();
                int VisitsTargetAchievmentWeight = db.properties.Where(a => a.Id == 2).Select(x => x.Value).FirstOrDefault();
                int CoverageForListedAccountsWeight = db.properties.Where(a => a.Id == 5).Select(x => x.Value).FirstOrDefault();
                int SellingDaysInTheFieldKpiWeight = db.properties.Where(a => a.Id == 6).Select(x => x.Value).FirstOrDefault();

                int AverageVisitsPerDayKpiTarget = db.properties.Where(a => a.Id == 1).Select(a => a.Value).SingleOrDefault();

                double? WorkingDays = WorkingDaysData - timeoffdays;

                string FullName = obj.FullName;

                string office = db.city.Where(a => a.Id == obj.CityId).Select(a => a.CityName).FirstOrDefault();

                string date = start.ToString("MMMM - yyyy")+" to "+end.ToString("MMMM - yyyy");

                double ActualTotalNumberOfVisits = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == obj.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();

                double ListedAccounts = db.userAccount.Where(a => a.extendidentityuserid == obj.Id).DistinctBy(x => x.AccountId).Count();

                double SellingDaysInTheField = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == obj.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(x => x.VisitDate.Date).Select(a => a.Id).Count();

                //double? TargetedNumberOfVisits = WorkingDays * AverageVisitsPerDayKpiTarget;

                double VisitedAccounts = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == obj.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(a => a.AccountId).Select(a => a.Id).Count();

                //double AverageVisitsPerDayActual = (double)ActualTotalNumberOfVisits / (double)SellingDaysInTheField;

                SalesKpiModel res = new SalesKpiModel();
                res.FullName = FullName;
                res.Month = date;
                res.RoleName = "Supportive Representative";

                res.CityName = office;
                res.ActualTotalNumberOfVisits = ActualTotalNumberOfVisits;
                res.ListedAccounts = ListedAccounts;
                res.SellingDaysInTheField = SellingDaysInTheField;
                //res.TargetedNumberOfVisits = TargetedNumberOfVisits;
                res.VisitedAccounts = VisitedAccounts;
                res.WorkingDays = WorkingDays;
                res.AverageVisitsPerDayKpiTarget = AverageVisitsPerDayKpiTarget;
                //res.AverageVisitsPerDayActual = AverageVisitsPerDayActual;
                //res.AverageVisitsPerDayRate = (AverageVisitsPerDayActual / AverageVisitsPerDayKpiTarget) * 100;
                //res.VisitsTargetAchievmentRate = (double)(ActualTotalNumberOfVisits / TargetedNumberOfVisits) * 100;
                //res.CoverageForListedAccountsRate = (VisitedAccounts / ListedAccounts) * 100;
                //res.SellingDaysInTheFieldKpiRate = (double)(SellingDaysInTheField / WorkingDays) * 100;
                res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
                res.VisitsTargetAchievmentWeight = VisitsTargetAchievmentWeight;
                res.CoverageForListedAccountsWeight = CoverageForListedAccountsWeight;
                res.SellingDaysInTheFieldKpiWeight = SellingDaysInTheFieldKpiWeight;
                res.TimeOffDays = timeoffdays;
                //res.AverageVisitsPerDayScore = (res.AverageVisitsPerDayRate * AverageVisitsPerDayWeight) / 100;
                //res.VisitsTargetAchievmentScore = (res.VisitsTargetAchievmentRate * VisitsTargetAchievmentWeight) / 100;
                //res.CoverageForListedAccountsScore = (res.CoverageForListedAccountsRate * CoverageForListedAccountsWeight) / 100;
                //res.SellingDaysInTheFieldScore = (res.SellingDaysInTheFieldKpiRate * SellingDaysInTheFieldKpiWeight) / 100;
                //res.TotalScore = res.AverageVisitsPerDayScore + res.VisitsTargetAchievmentScore + res.CoverageForListedAccountsScore + res.SellingDaysInTheFieldScore;
                result.Add(res);
            }
            return result.OrderBy(a => a.FullName);
        }

        public MedicalKpiModel GetMedicalKpi(int yearfrom, int monthfrom, int yearto, int monthto, string userId)
        {
            int daysto = DateTime.DaysInMonth(yearto, monthto);
            DateTime start = new DateTime(yearfrom, monthfrom, 1, 0, 0, 0);
            DateTime end = new DateTime(yearto, monthto, daysto, 23, 59, 59);
            
            int? WorkingDaysData = db.workingDays.Where(a => a.Month >= start.Month && a.Year >= start.Year && a.Month <= end.Month && a.Year <= end.Year).Select(a => a.NumberOfWorkingDays).Sum();
            ExtendIdentityUser user = userManager.FindByIdAsync(userId).Result;

            int? timeoffdays = GetTimeOffDiff(userId, yearfrom, monthfrom, 1, yearto, monthto, daysto);

            List<ContactWithCategory> UserAPlusAndA = DistinctByExtension.DistinctBy(db.userContact.Where(a => a.extendidentityuserid == user.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
            {
                Id = b.Id,
                ContactName = b.ContactName,
                Gender = b.Gender,
                Address = b.Address,
                LandLineNumber = b.LandLineNumber,
                MobileNumber = b.MobileNumber,
                Email = b.Email,
                PaymentNotes = b.PaymentNotes,
                RelationshipNote = b.RelationshipNote,
                AccountId = b.AccountId,
                CategoryId = a.CategoryId
            }),x=>x.Id).Where(a => a.CategoryId == 1 || a.CategoryId == 2).ToList();


            List<ContactWithCategory> UserB = DistinctByExtension.DistinctBy(db.userContact.Where(a => a.extendidentityuserid == user.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
            {
                Id = b.Id,
                ContactName = b.ContactName,
                Gender = b.Gender,
                Address = b.Address,
                LandLineNumber = b.LandLineNumber,
                MobileNumber = b.MobileNumber,
                Email = b.Email,
                PaymentNotes = b.PaymentNotes,
                RelationshipNote = b.RelationshipNote,
                AccountId = b.AccountId,
                CategoryId = a.CategoryId
            }),x=>x.Id).Where(a => a.CategoryId == 3).ToList();
                
                
        

            List<ContactWithCategory> UserC = DistinctByExtension.DistinctBy(db.userContact.Where(a => a.extendidentityuserid == user.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
            {
                Id = b.Id,
                ContactName = b.ContactName,
                Gender = b.Gender,
                Address = b.Address,
                LandLineNumber = b.LandLineNumber,
                MobileNumber = b.MobileNumber,
                Email = b.Email,
                PaymentNotes = b.PaymentNotes,
                RelationshipNote = b.RelationshipNote,
                AccountId = b.AccountId,
                CategoryId = a.CategoryId
            }), x => x.Id).Where(a => a.CategoryId == 4).ToList();

            int AverageVisitPerDayKpiTarget = db.properties.Where(a => a.Id == 7).Select(a => a.Value).FirstOrDefault();
            int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 8).Select(a => a.Value).FirstOrDefault();
            int VisitsTargerAchievmentWeight = db.properties.Where(a => a.Id == 9).Select(a => a.Value).FirstOrDefault();
            int AAndAPlusWegiht = db.properties.Where(a => a.Id == 10).Select(a => a.Value).FirstOrDefault();
            int BWeight = db.properties.Where(a => a.Id == 11).Select(a => a.Value).FirstOrDefault();
            int CWeight = db.properties.Where(a => a.Id == 12).Select(a => a.Value).FirstOrDefault();
            int SellingDaysInTheFieldWegiht = db.properties.Where(a => a.Id == 13).Select(a => a.Value).FirstOrDefault();

            MedicalKpiModel res = new MedicalKpiModel();
            res.FullName = user.FullName;
            res.CityName = db.city.Where(a => a.Id == user.CityId).Select(a => a.CityName).FirstOrDefault();
            res.RoleName = "Medical Representative";
            res.Month = start.ToString("MMMM - yyyy")+" to "+end.ToString("MMMM - yyyy");
            res.ActualTotalNumberOfVisits = db.contactMedicalVisit.Where(a => a.extendidentityuserid == user.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a=>a.Id).Count();
            res.AplusAndAListed = UserAPlusAndA.Select(a => a.Id).Count();
            res.BListed = UserB.Select(a => a.Id).Count();
            res.CListed = UserC.Select(a => a.Id).Count();
            int APLusAndAVisited = 0;
            int BVisited = 0;
            int CVisited = 0;
            foreach (var item in UserAPlusAndA)
            {
                ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == user.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == item.Id).FirstOrDefault();
                if (x != null)
                {
                    APLusAndAVisited++;
                }
            }
            foreach (var item in UserB)
            {
                ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == user.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == item.Id).FirstOrDefault();
                if (x != null)
                {
                    BVisited++;
                }
            }
            foreach (var item in UserC)
            {
                ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == user.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == item.Id).FirstOrDefault();
                if (x != null)
                {
                    CVisited++;
                }
            }
            res.APlusAndAVisited = APLusAndAVisited;
            res.BVisited = BVisited;
            res.CVisited = CVisited;
            res.SellingDaysInTheField = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.extendidentityuserid == user.Id && a.VisitDate >= start && a.VisitDate <= end),x=>x.VisitDate.Date).Select(a => a.Id).Count();
            res.WorkingDays = WorkingDaysData - timeoffdays;
            res.AverageVisitsPerDayKpiTarget = AverageVisitPerDayKpiTarget;
            res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
            res.VisitsTargetAchievmentWeight = VisitsTargerAchievmentWeight;
            res.APlusAndAWeight = AAndAPlusWegiht;
            res.BWeight = BWeight;
            res.CWeight = CWeight;
            res.SellingDaysInTheFieldWeight = SellingDaysInTheFieldWegiht;
            res.TimeOffDays = timeoffdays;
            return res;
        }

        public KPIScoreModel GetMedicalKpiChart(int yearfrom, int monthfrom, int yearto, int monthto, string userId)
        {
            MedicalKpiModel Kpi = GetMedicalKpi(yearfrom, monthfrom, yearto, monthto, userId);
            KPIScoreModel res = new KPIScoreModel();


            int TargetedNumberOfVisitsKPITarget = (int)(Kpi.WorkingDays * Kpi.AverageVisitsPerDayKpiTarget);


            int VisitTaregAcheivmentKPITarget = TargetedNumberOfVisitsKPITarget;

            //zero
            float AverageVisitsPerDayActual = 0;
            if (Kpi.SellingDaysInTheField != 0)
            {
                AverageVisitsPerDayActual = (float)Kpi.ActualTotalNumberOfVisits / (float)Kpi.SellingDaysInTheField;
            }

            //zero
            float AverageVisitsPerDayRate = ((float)AverageVisitsPerDayActual / (float)Kpi.AverageVisitsPerDayKpiTarget) * 100;

            //zero
            float VisitsTargetAcheivmentRate = ((float)Kpi.ActualTotalNumberOfVisits / (float)VisitTaregAcheivmentKPITarget) * 100;

            //zero
            float ARate = 0;
            if (Kpi.AplusAndAListed == 0)
            {
                
            }
            else
            {
                float ARatee = (float)Kpi.APlusAndAVisited / (float)Kpi.AplusAndAListed;
                ARate = ARatee * 100;
            }


            //zero
            float BRate = 0;
            if (Kpi.BListed != 0)
            {
                BRate = ((float)Kpi.BVisited / (float)Kpi.BListed) * 100;
            }


            float CRate = 0;
            if (Kpi.CListed != 0)
            {
                CRate = ((float)Kpi.CVisited / (float)Kpi.CListed) * 100;
            }

            float SellingDaysinTheFieldRate = ((float)Kpi.SellingDaysInTheField / (float)Kpi.WorkingDays) * 100;
            float AverageVisitsPerDayScore = ((float)AverageVisitsPerDayRate * (float)Kpi.AverageVisitsPerDayWeight) / 100;
            float VisitsTargetAcheivmentScore = ((float)VisitsTargetAcheivmentRate * (float)Kpi.VisitsTargetAchievmentWeight) / 100;
            float AScore = ((float)ARate * (float)Kpi.APlusAndAWeight) / 100;
            float BScore = ((float)BRate * (float)Kpi.BWeight) / 100;
            float CScore = ((float)CRate * (float)Kpi.CWeight) / 100;
            float SellingDaysinTheFiledScore = ((float)SellingDaysinTheFieldRate * (float)Kpi.SellingDaysInTheFieldWeight) / 100;
            float TotalScore = AverageVisitsPerDayScore + VisitsTargetAcheivmentScore + AScore + BScore + CScore + SellingDaysinTheFiledScore;
            float SCORE = (float)Math.Round(TotalScore, 0);

            res.Name = Kpi.FullName;
            res.Score = SCORE;

            return res;
        }

        public SalesKpiModel GetSalesKpi(int yearfrom, int monthfrom, int yearto, int monthto, string userId)
        {
            int daysto = DateTime.DaysInMonth(yearto, monthto);
            DateTime start = new DateTime(yearfrom, monthfrom, 1, 0, 0, 0);
            DateTime end = new DateTime(yearto, monthto, daysto, 23, 59, 59);
           
            int? WorkingDaysData = db.workingDays.Where(a => a.Month >= start.Month && a.Year >= start.Year && a.Month <= end.Month && a.Year <= end.Year).Select(a => a.NumberOfWorkingDays).Sum();
            ExtendIdentityUser user = userManager.FindByIdAsync(userId).Result;
            int? timeoffdays = GetTimeOffDiff(userId, yearfrom, monthfrom, 1, yearto, monthto, daysto);
            int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 3).Select(x => x.Value).FirstOrDefault();
            int VisitsTargetAchievmentWeight = db.properties.Where(a => a.Id == 2).Select(x => x.Value).FirstOrDefault();
            int CoverageForListedAccountsWeight = db.properties.Where(a => a.Id == 5).Select(x => x.Value).FirstOrDefault();
            int SellingDaysInTheFieldKpiWeight = db.properties.Where(a => a.Id == 6).Select(x => x.Value).FirstOrDefault();

            int AverageVisitsPerDayKpiTarget = db.properties.Where(a=>a.Id == 1).Select(a=>a.Value).SingleOrDefault();

           

            string FullName = user.FullName;

            string office = db.city.Where(a => a.Id == user.CityId).Select(a => a.CityName).FirstOrDefault();

            string date = start.ToString("MMMM - yyyy")+" to "+end.ToString("MMMM - yyyy");

            double ActualTotalNumberOfVisits = db.accountSalesVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end).Select(a=>a.Id).Count();

            double ListedAccounts = DistinctByExtension.DistinctBy(db.userAccount.Where(a => a.extendidentityuserid == userId),x=>x.AccountId).Count();
            

            double SellingDaysInTheField = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end),x=>x.VisitDate.Date).Select(a => a.Id).Count();

            //double? TargetedNumberOfVisits = WorkingDays * AverageVisitsPerDayKpiTarget;

            double VisitedAccounts = DistinctByExtension.DistinctBy(db.accountSalesVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end),x=>x.AccountId).Select(a => a.Id).Count();
         

           //double AverageVisitsPerDayActual =(double)ActualTotalNumberOfVisits / (double)SellingDaysInTheField;

            SalesKpiModel res = new SalesKpiModel();
            res.FullName = FullName;
            res.Month = date;
            res.RoleName = "Sales Representative";

            res.CityName = office;
            res.ActualTotalNumberOfVisits = ActualTotalNumberOfVisits;
            res.ListedAccounts = ListedAccounts;
            res.SellingDaysInTheField = SellingDaysInTheField;
            //res.TargetedNumberOfVisits = TargetedNumberOfVisits;
            res.VisitedAccounts = VisitedAccounts;
            res.WorkingDays = WorkingDaysData - timeoffdays;
            res.AverageVisitsPerDayKpiTarget = AverageVisitsPerDayKpiTarget;
            //res.AverageVisitsPerDayActual = AverageVisitsPerDayActual;
            //res.AverageVisitsPerDayRate = (AverageVisitsPerDayActual / AverageVisitsPerDayKpiTarget) * 100;
            //res.VisitsTargetAchievmentRate = (double)(ActualTotalNumberOfVisits / TargetedNumberOfVisits) * 100;
            //res.CoverageForListedAccountsRate = (VisitedAccounts / ListedAccounts) * 100;
            //res.SellingDaysInTheFieldKpiRate = (double)(SellingDaysInTheField / WorkingDays) * 100;
            res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
            res.VisitsTargetAchievmentWeight = VisitsTargetAchievmentWeight;
            res.CoverageForListedAccountsWeight = CoverageForListedAccountsWeight;
            res.SellingDaysInTheFieldKpiWeight = SellingDaysInTheFieldKpiWeight;
            res.TimeOffDays = timeoffdays;
            //res.AverageVisitsPerDayScore = (res.AverageVisitsPerDayRate * AverageVisitsPerDayWeight) / 100;
            //res.VisitsTargetAchievmentScore = (res.VisitsTargetAchievmentRate * VisitsTargetAchievmentWeight) / 100;
            //res.CoverageForListedAccountsScore = (res.CoverageForListedAccountsRate * CoverageForListedAccountsWeight) / 100;
            //res.SellingDaysInTheFieldScore = (res.SellingDaysInTheFieldKpiRate * SellingDaysInTheFieldKpiWeight) / 100;
            //res.TotalScore = res.AverageVisitsPerDayScore + res.VisitsTargetAchievmentScore + res.CoverageForListedAccountsScore + res.SellingDaysInTheFieldScore;
            return res;
        }

        public SalesKpiModel GetSupportiveKpi(int yearfrom, int monthfrom, int yearto, int monthto, string userId)
        {
            int daysto = DateTime.DaysInMonth(yearto, monthto);
            DateTime start = new DateTime(yearfrom, monthfrom, 1, 0, 0, 0);
            DateTime end = new DateTime(yearto, monthto, daysto, 23, 59, 59);

            int? WorkingDaysData = db.workingDays.Where(a => a.Month >= start.Month && a.Year >= start.Year && a.Month <= end.Month && a.Year <= end.Year).Select(a => a.NumberOfWorkingDays).Sum();
            ExtendIdentityUser user = userManager.FindByIdAsync(userId).Result;
            int? timeoffdays = GetTimeOffDiff(userId, yearfrom, monthfrom, 1, yearto, monthto, daysto);
            int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 3).Select(x => x.Value).FirstOrDefault();
            int VisitsTargetAchievmentWeight = db.properties.Where(a => a.Id == 2).Select(x => x.Value).FirstOrDefault();
            int CoverageForListedAccountsWeight = db.properties.Where(a => a.Id == 5).Select(x => x.Value).FirstOrDefault();
            int SellingDaysInTheFieldKpiWeight = db.properties.Where(a => a.Id == 6).Select(x => x.Value).FirstOrDefault();

            int AverageVisitsPerDayKpiTarget = db.properties.Where(a => a.Id == 1).Select(a => a.Value).SingleOrDefault();



            string FullName = user.FullName;

            string office = db.city.Where(a => a.Id == user.CityId).Select(a => a.CityName).FirstOrDefault();

            string date = start.ToString("MMMM - yyyy")+" to "+end.ToString("MMMM - yyyy");

            double ActualTotalNumberOfVisits = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();

            double ListedAccounts = DistinctByExtension.DistinctBy(db.userAccount.Where(a => a.extendidentityuserid == userId), x => x.AccountId).Count();
        

            double SellingDaysInTheField = DistinctByExtension.DistinctBy(db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end),a=>a.VisitDate.Date).Select(a => a.Id).Count();
   
            //double? TargetedNumberOfVisits = WorkingDays * AverageVisitsPerDayKpiTarget;

            double VisitedAccounts = DistinctByExtension.DistinctBy(db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == userId && a.VisitDate >= start && a.VisitDate <= end),a=>a.AccountId).Select(a => a.Id).Count();
        

            //double AverageVisitsPerDayActual =(double)ActualTotalNumberOfVisits / (double)SellingDaysInTheField;

            SalesKpiModel res = new SalesKpiModel();
            res.FullName = FullName;
            res.Month = date;
            res.RoleName = "Supportive Representative";

            res.CityName = office;
            res.ActualTotalNumberOfVisits = ActualTotalNumberOfVisits;
            res.ListedAccounts = ListedAccounts;
            res.SellingDaysInTheField = SellingDaysInTheField;
            //res.TargetedNumberOfVisits = TargetedNumberOfVisits;
            res.VisitedAccounts = VisitedAccounts;
            res.WorkingDays = WorkingDaysData - timeoffdays;
            res.AverageVisitsPerDayKpiTarget = AverageVisitsPerDayKpiTarget;
            //res.AverageVisitsPerDayActual = AverageVisitsPerDayActual;
            //res.AverageVisitsPerDayRate = (AverageVisitsPerDayActual / AverageVisitsPerDayKpiTarget) * 100;
            //res.VisitsTargetAchievmentRate = (double)(ActualTotalNumberOfVisits / TargetedNumberOfVisits) * 100;
            //res.CoverageForListedAccountsRate = (VisitedAccounts / ListedAccounts) * 100;
            //res.SellingDaysInTheFieldKpiRate = (double)(SellingDaysInTheField / WorkingDays) * 100;
            res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
            res.VisitsTargetAchievmentWeight = VisitsTargetAchievmentWeight;
            res.CoverageForListedAccountsWeight = CoverageForListedAccountsWeight;
            res.SellingDaysInTheFieldKpiWeight = SellingDaysInTheFieldKpiWeight;
            res.TimeOffDays = timeoffdays;
            //res.AverageVisitsPerDayScore = (res.AverageVisitsPerDayRate * AverageVisitsPerDayWeight) / 100;
            //res.VisitsTargetAchievmentScore = (res.VisitsTargetAchievmentRate * VisitsTargetAchievmentWeight) / 100;
            //res.CoverageForListedAccountsScore = (res.CoverageForListedAccountsRate * CoverageForListedAccountsWeight) / 100;
            //res.SellingDaysInTheFieldScore = (res.SellingDaysInTheFieldKpiRate * SellingDaysInTheFieldKpiWeight) / 100;
            //res.TotalScore = res.AverageVisitsPerDayScore + res.VisitsTargetAchievmentScore + res.CoverageForListedAccountsScore + res.SellingDaysInTheFieldScore;
            return res;
        }

        public IEnumerable<MedicalKpiModel> GetTeamMedicalKpi(int yearfrom, int monthfrom, int yearto, int monthto, string managerId)
        {
            int daysto = DateTime.DaysInMonth(yearto, monthto);
            DateTime start = new DateTime(yearfrom, monthfrom, 1, 0, 0, 0);
            DateTime end = new DateTime(yearto, monthto, daysto, 23, 59, 59);
            
            int? WorkingDaysdata = db.workingDays.Where(a => a.Month >= start.Month && a.Year >= start.Year && a.Month <= end.Month && a.Year <= end.Year).Select(a => a.NumberOfWorkingDays).Sum();
            List<ExtendIdentityUser> users = new List<ExtendIdentityUser>();


            List<ExtendIdentityUser> u = userManager.Users.Where(a=>a.extendidentityuserid == managerId && a.Active == true).ToList();
            foreach (var item in u)
            {
              var x =  userManager.IsInRoleAsync(item, "Medical Representative").Result;
                if (x)
                {
                    users.Add(item);
                }
            }
            List<MedicalKpiModel> ress = new List<MedicalKpiModel>();
            foreach (var item in users)
            {
                int? timeoffdays = GetTimeOffDiff(item.Id, yearfrom, monthfrom, 1, yearto, monthto, daysto);
                List<ContactWithCategory> UserAPlusAndA = DistinctByExtension.DistinctBy(db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
                {
                    Id = b.Id,
                    ContactName = b.ContactName,
                    Gender = b.Gender,
                    Address = b.Address,
                    LandLineNumber = b.LandLineNumber,
                    MobileNumber = b.MobileNumber,
                    Email = b.Email,
                    PaymentNotes = b.PaymentNotes,
                    RelationshipNote = b.RelationshipNote,
                    AccountId = b.AccountId,
                    CategoryId = a.CategoryId
                }),x=>x.Id).Where(a => a.CategoryId == 1 || a.CategoryId == 2).ToList();                   

                List<ContactWithCategory> UserB = DistinctByExtension.DistinctBy(db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
                {
                    Id = b.Id,
                    ContactName = b.ContactName,
                    Gender = b.Gender,
                    Address = b.Address,
                    LandLineNumber = b.LandLineNumber,
                    MobileNumber = b.MobileNumber,
                    Email = b.Email,
                    PaymentNotes = b.PaymentNotes,
                    RelationshipNote = b.RelationshipNote,
                    AccountId = b.AccountId,
                    CategoryId = a.CategoryId
                }), x=>x.Id).Where(a => a.CategoryId == 3).ToList();
                 
                List<ContactWithCategory> UserC = DistinctByExtension.DistinctBy(db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
                {
                    Id = b.Id,
                    ContactName = b.ContactName,
                    Gender = b.Gender,
                    Address = b.Address,
                    LandLineNumber = b.LandLineNumber,
                    MobileNumber = b.MobileNumber,
                    Email = b.Email,
                    PaymentNotes = b.PaymentNotes,
                    RelationshipNote = b.RelationshipNote,
                    AccountId = b.AccountId,
                    CategoryId = a.CategoryId
                }),x=>x.Id).Where(a => a.CategoryId == 4).ToList();

                int AverageVisitPerDayKpiTarget = db.properties.Where(a => a.Id == 7).Select(a => a.Value).FirstOrDefault();
                int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 8).Select(a => a.Value).FirstOrDefault();
                int VisitsTargerAchievmentWeight = db.properties.Where(a => a.Id == 9).Select(a => a.Value).FirstOrDefault();
                int AAndAPlusWegiht = db.properties.Where(a => a.Id == 10).Select(a => a.Value).FirstOrDefault();
                int BWeight = db.properties.Where(a => a.Id == 11).Select(a => a.Value).FirstOrDefault();
                int CWeight = db.properties.Where(a => a.Id == 12).Select(a => a.Value).FirstOrDefault();
                int SellingDaysInTheFieldWegiht = db.properties.Where(a => a.Id == 13).Select(a => a.Value).FirstOrDefault();

                MedicalKpiModel res = new MedicalKpiModel();
                res.FullName = item.FullName;
                res.CityName = db.city.Where(a => a.Id == item.CityId).Select(a => a.CityName).FirstOrDefault();
                res.RoleName = "Medical Representative";
                res.Month = start.ToString("MMMM - yyyy")+" to "+end.ToString("MMMM - yyyy");
                res.ActualTotalNumberOfVisits = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();
                res.AplusAndAListed = UserAPlusAndA.Select(a => a.Id).Count();
                res.BListed = UserB.Select(a => a.Id).Count();
                res.CListed = UserC.Select(a => a.Id).Count();
                int APLusAndAVisited = 0;
                int BVisited = 0;
                int CVisited = 0;
                foreach (var aa in UserAPlusAndA)
                {
                    ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == aa.Id).FirstOrDefault();
                    if (x != null)
                    {
                        APLusAndAVisited++;
                    }
                }
                foreach (var bb in UserB)
                {
                    ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == bb.Id).FirstOrDefault();
                    if (x != null)
                    {
                        BVisited++;
                    }
                }
                foreach (var cc in UserC)
                {
                    ContactMedicalVisit x = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end && a.ContactId == cc.Id).FirstOrDefault();
                    if (x != null)
                    {
                        CVisited++;
                    }
                }
                res.APlusAndAVisited = APLusAndAVisited;
                res.BVisited = BVisited;
                res.CVisited = CVisited;
                res.SellingDaysInTheField = DistinctByExtension.DistinctBy(db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate >= start && a.VisitDate <= end),x=>x.VisitDate.Date).Select(a => a.Id).Count();
                res.WorkingDays = WorkingDaysdata - timeoffdays;
                res.AverageVisitsPerDayKpiTarget = AverageVisitPerDayKpiTarget;
                res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
                res.VisitsTargetAchievmentWeight = VisitsTargerAchievmentWeight;
                res.APlusAndAWeight = AAndAPlusWegiht;
                res.BWeight = BWeight;
                res.CWeight = CWeight;
                res.SellingDaysInTheFieldWeight = SellingDaysInTheFieldWegiht;
                res.TimeOffDays = timeoffdays;
                ress.Add(res);
            }
            return ress.OrderBy(a => a.FullName);
        }

        public List<KPIScoreModel> GetTeamMedicalKpiChart(int yearfrom, int monthfrom, int yearto, int monthto, string managerId)
        {
            List<MedicalKpiModel> Kpi = GetTeamMedicalKpi(yearfrom, monthfrom, yearto, monthto, managerId).ToList();
            List<KPIScoreModel> res = new List<KPIScoreModel>();

            foreach (var item in Kpi)
            {
                int TargetedNumberOfVisitsKPITarget = (int)(item.WorkingDays * item.AverageVisitsPerDayKpiTarget);


                int VisitTaregAcheivmentKPITarget = TargetedNumberOfVisitsKPITarget;

                //zero
                float AverageVisitsPerDayActual = 0;
                if (item.SellingDaysInTheField != 0)
                {
                    AverageVisitsPerDayActual = (float)item.ActualTotalNumberOfVisits / (float)item.SellingDaysInTheField;
                }

                //zero
                float AverageVisitsPerDayRate = ((float)AverageVisitsPerDayActual / (float)item.AverageVisitsPerDayKpiTarget) * 100;

                //zero
                float VisitsTargetAcheivmentRate = ((float)item.ActualTotalNumberOfVisits / (float)VisitTaregAcheivmentKPITarget) * 100;

                //zero
                float ARate = 0;
                if (item.AplusAndAListed == 0)
                {

                }
                else
                {
                    float ARatee = (float)item.APlusAndAVisited / (float)item.AplusAndAListed;
                    ARate = ARatee * 100;
                }


                //zero
                float BRate = 0;
                if (item.BListed != 0)
                {
                    BRate = ((float)item.BVisited / (float)item.BListed) * 100;
                }


                float CRate = 0;
                if (item.CListed != 0)
                {
                    CRate = ((float)item.CVisited / (float)item.CListed) * 100;
                }

                float SellingDaysinTheFieldRate = ((float)item.SellingDaysInTheField / (float)item.WorkingDays) * 100;
                float AverageVisitsPerDayScore = ((float)AverageVisitsPerDayRate * (float)item.AverageVisitsPerDayWeight) / 100;
                float VisitsTargetAcheivmentScore = ((float)VisitsTargetAcheivmentRate * (float)item.VisitsTargetAchievmentWeight) / 100;
                float AScore = ((float)ARate * (float)item.APlusAndAWeight) / 100;
                float BScore = ((float)BRate * (float)item.BWeight) / 100;
                float CScore = ((float)CRate * (float)item.CWeight) / 100;
                float SellingDaysinTheFiledScore = ((float)SellingDaysinTheFieldRate * (float)item.SellingDaysInTheFieldWeight) / 100;
                float TotalScore = AverageVisitsPerDayScore + VisitsTargetAcheivmentScore + AScore + BScore + CScore + SellingDaysinTheFiledScore;
                float SCORE = (float)Math.Round(TotalScore, 0);

                KPIScoreModel obj = new KPIScoreModel();
                obj.Name = item.FullName;
                obj.Score = SCORE;
                res.Add(obj);
            }

            return res.OrderBy(a => a.Score).ToList();
        }

        public IEnumerable<SalesKpiModel> GetTeamSalesKpi(int yearfrom, int monthfrom, int yearto, int monthto, string managerId)
        {
            int daysto = DateTime.DaysInMonth(yearto, monthto);
            DateTime start = new DateTime(yearfrom, monthfrom, 1, 0, 0, 0);
            DateTime end = new DateTime(yearto, monthto, daysto, 23, 59, 59);
            
            int? WorkingDaysData = db.workingDays.Where(a => a.Month >= start.Month && a.Year >= start.Year && a.Month <= end.Month && a.Year <= end.Year).Select(a => a.NumberOfWorkingDays).Sum();
            List<ExtendIdentityUser> list = new List<ExtendIdentityUser>();


            IEnumerable<ExtendIdentityUser> u = userManager.Users.Where(a => a.extendidentityuserid == managerId && a.Active == true);
            foreach (var item in u)
            {
                var x = userManager.IsInRoleAsync(item, "Sales Representative").Result;
                if (x)
                {
                    list.Add(item);
                }
            }

            List<string> subids = db.UserSubmanager.Where(a => a.ManagerId == managerId).Select(a => a.RepId).ToList();

            foreach (var item in subids)
            {
                ExtendIdentityUser subrep = userManager.FindByIdAsync(item).Result;
                list.Add(subrep);
            }

            List<SalesKpiModel> result = new List<SalesKpiModel>();

            foreach (var ress in list)
            {
                int? timeoffdays = GetTimeOffDiff(ress.Id, yearfrom, monthfrom, 1, yearto, monthto, daysto);
                int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 3).Select(x => x.Value).FirstOrDefault();
                int VisitsTargetAchievmentWeight = db.properties.Where(a => a.Id == 2).Select(x => x.Value).FirstOrDefault();
                int CoverageForListedAccountsWeight = db.properties.Where(a => a.Id == 5).Select(x => x.Value).FirstOrDefault();
                int SellingDaysInTheFieldKpiWeight = db.properties.Where(a => a.Id == 6).Select(x => x.Value).FirstOrDefault();

                int AverageVisitsPerDayKpiTarget = db.properties.Where(a => a.Id == 1).Select(a => a.Value).SingleOrDefault();

                

                string FullName = ress.FullName;

                string office = db.city.Where(a => a.Id == ress.CityId).Select(a => a.CityName).FirstOrDefault();

                string date = start.ToString("MMMM - yyyy") + " to " + end.ToString("MMMM - yyyy");

                double ActualTotalNumberOfVisits = db.accountSalesVisit.Where(a => a.extendidentityuserid == ress.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();

                double ListedAccounts = db.userAccount.Where(a => a.extendidentityuserid == ress.Id).DistinctBy(x => x.AccountId).Count();

                double SellingDaysInTheField = db.accountSalesVisit.Where(a => a.extendidentityuserid == ress.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(x => x.VisitDate.Date).Select(a => a.Id).Count();

                //double? TargetedNumberOfVisits = WorkingDays * AverageVisitsPerDayKpiTarget;

                double VisitedAccounts = db.accountSalesVisit.Where(a => a.extendidentityuserid == ress.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(a => a.AccountId).Select(a => a.Id).Count();

                //double AverageVisitsPerDayActual = (double)ActualTotalNumberOfVisits / (double)SellingDaysInTheField;

                SalesKpiModel res = new SalesKpiModel();
                res.FullName = FullName;
                res.Month = date;
                res.RoleName = "Sales Representative";

                res.CityName = office;
                res.ActualTotalNumberOfVisits = ActualTotalNumberOfVisits;
                res.ListedAccounts = ListedAccounts;
                res.SellingDaysInTheField = SellingDaysInTheField;
                //res.TargetedNumberOfVisits = TargetedNumberOfVisits;
                res.VisitedAccounts = VisitedAccounts;
                res.WorkingDays = WorkingDaysData - timeoffdays;
                res.AverageVisitsPerDayKpiTarget = AverageVisitsPerDayKpiTarget;
                //res.AverageVisitsPerDayActual = AverageVisitsPerDayActual;
                //res.AverageVisitsPerDayRate = (AverageVisitsPerDayActual / AverageVisitsPerDayKpiTarget) * 100;
                //res.VisitsTargetAchievmentRate = (double)(ActualTotalNumberOfVisits / TargetedNumberOfVisits) * 100;
                //res.CoverageForListedAccountsRate = (VisitedAccounts / ListedAccounts) * 100;
                //res.SellingDaysInTheFieldKpiRate = (double)(SellingDaysInTheField / WorkingDays) * 100;
                res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
                res.VisitsTargetAchievmentWeight = VisitsTargetAchievmentWeight;
                res.CoverageForListedAccountsWeight = CoverageForListedAccountsWeight;
                res.SellingDaysInTheFieldKpiWeight = SellingDaysInTheFieldKpiWeight;
                res.TimeOffDays = timeoffdays;
                //res.AverageVisitsPerDayScore = (res.AverageVisitsPerDayRate * AverageVisitsPerDayWeight) / 100;
                //res.VisitsTargetAchievmentScore = (res.VisitsTargetAchievmentRate * VisitsTargetAchievmentWeight) / 100;
                //res.CoverageForListedAccountsScore = (res.CoverageForListedAccountsRate * CoverageForListedAccountsWeight) / 100;
                //res.SellingDaysInTheFieldScore = (res.SellingDaysInTheFieldKpiRate * SellingDaysInTheFieldKpiWeight) / 100;
                //res.TotalScore = res.AverageVisitsPerDayScore + res.VisitsTargetAchievmentScore + res.CoverageForListedAccountsScore + res.SellingDaysInTheFieldScore;
                result.Add(res);
            }
            return result.OrderBy(a => a.FullName);
        }

        public IEnumerable<SalesKpiModel> GetTeamSupportiveKpi(int yearfrom, int monthfrom, int yearto, int monthto, string managerId)
        {
            int daysto = DateTime.DaysInMonth(yearto, monthto);
            DateTime start = new DateTime(yearfrom, monthfrom, 1, 0, 0, 0);
            DateTime end = new DateTime(yearto, monthto, daysto, 23, 59, 59);
            
            int? WorkingDaysData = db.workingDays.Where(a => a.Month >= start.Month && a.Year >= start.Year && a.Month <= end.Month && a.Year <= end.Year).Select(a => a.NumberOfWorkingDays).Sum();
            List<ExtendIdentityUser> list = new List<ExtendIdentityUser>();


            IEnumerable<ExtendIdentityUser> u = userManager.Users.Where(a => a.extendidentityuserid == managerId && a.Active == true);
            foreach (var item in u)
            {
                var x = userManager.IsInRoleAsync(item, "Supportive").Result;
                if (x)
                {
                    list.Add(item);
                }
            }
            List<SalesKpiModel> result = new List<SalesKpiModel>();

            foreach (var ress in list)
            {
                int? timeoffdays = GetTimeOffDiff(ress.Id, yearfrom, monthfrom, 1, yearto, monthto, daysto);
                int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 3).Select(x => x.Value).FirstOrDefault();
                int VisitsTargetAchievmentWeight = db.properties.Where(a => a.Id == 2).Select(x => x.Value).FirstOrDefault();
                int CoverageForListedAccountsWeight = db.properties.Where(a => a.Id == 5).Select(x => x.Value).FirstOrDefault();
                int SellingDaysInTheFieldKpiWeight = db.properties.Where(a => a.Id == 6).Select(x => x.Value).FirstOrDefault();

                int AverageVisitsPerDayKpiTarget = db.properties.Where(a => a.Id == 1).Select(a => a.Value).SingleOrDefault();

                

                string FullName = ress.FullName;

                string office = db.city.Where(a => a.Id == ress.CityId).Select(a => a.CityName).FirstOrDefault();

                string date = start.ToString("MMMM - yyyy")+" to "+end.ToString("MMMM - yyyy");

                double ActualTotalNumberOfVisits = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == ress.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();

                double ListedAccounts = db.userAccount.Where(a => a.extendidentityuserid == ress.Id).DistinctBy(x => x.AccountId).Count();

                double SellingDaysInTheField = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == ress.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(x => x.VisitDate.Date).Select(a => a.Id).Count();

                //double? TargetedNumberOfVisits = WorkingDays * AverageVisitsPerDayKpiTarget;

                double VisitedAccounts = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == ress.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(a => a.AccountId).Select(a => a.Id).Count();

                //double AverageVisitsPerDayActual = (double)ActualTotalNumberOfVisits / (double)SellingDaysInTheField;

                SalesKpiModel res = new SalesKpiModel();
                res.FullName = FullName;
                res.Month = date;
                res.RoleName = "Supportive Representative";

                res.CityName = office;
                res.ActualTotalNumberOfVisits = ActualTotalNumberOfVisits;
                res.ListedAccounts = ListedAccounts;
                res.SellingDaysInTheField = SellingDaysInTheField;
                //res.TargetedNumberOfVisits = TargetedNumberOfVisits;
                res.VisitedAccounts = VisitedAccounts;
                res.WorkingDays = WorkingDaysData - timeoffdays;
                res.AverageVisitsPerDayKpiTarget = AverageVisitsPerDayKpiTarget;
                //res.AverageVisitsPerDayActual = AverageVisitsPerDayActual;
                //res.AverageVisitsPerDayRate = (AverageVisitsPerDayActual / AverageVisitsPerDayKpiTarget) * 100;
                //res.VisitsTargetAchievmentRate = (double)(ActualTotalNumberOfVisits / TargetedNumberOfVisits) * 100;
                //res.CoverageForListedAccountsRate = (VisitedAccounts / ListedAccounts) * 100;
                //res.SellingDaysInTheFieldKpiRate = (double)(SellingDaysInTheField / WorkingDays) * 100;
                res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
                res.VisitsTargetAchievmentWeight = VisitsTargetAchievmentWeight;
                res.CoverageForListedAccountsWeight = CoverageForListedAccountsWeight;
                res.SellingDaysInTheFieldKpiWeight = SellingDaysInTheFieldKpiWeight;
                res.TimeOffDays = timeoffdays;
                //res.AverageVisitsPerDayScore = (res.AverageVisitsPerDayRate * AverageVisitsPerDayWeight) / 100;
                //res.VisitsTargetAchievmentScore = (res.VisitsTargetAchievmentRate * VisitsTargetAchievmentWeight) / 100;
                //res.CoverageForListedAccountsScore = (res.CoverageForListedAccountsRate * CoverageForListedAccountsWeight) / 100;
                //res.SellingDaysInTheFieldScore = (res.SellingDaysInTheFieldKpiRate * SellingDaysInTheFieldKpiWeight) / 100;
                //res.TotalScore = res.AverageVisitsPerDayScore + res.VisitsTargetAchievmentScore + res.CoverageForListedAccountsScore + res.SellingDaysInTheFieldScore;
                result.Add(res);
            }
            return result.OrderBy(a => a.FullName);
        }

        public int GetTimeOffDiff(string id,int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto)
        {
           
            DateTime start = new DateTime(yearfrom, monthfrom, dayfrom);
            DateTime end = new DateTime(yearto, monthto, dayto);
            var s = end - start;
            int days = s.Days;
            TimeSpan diff = new TimeSpan(0, 5, 0, 0, 0);
            int timeoffdays = DistinctByExtension.DistinctBy(db.userTimeOff.ToList().Where(a => a.ExtendIdentityUserId == id).Where(a => a.DateTimeFrom >= start).Where(a => a.DateTimeFrom <= end).Where(a => a.DateTimeTo - a.DateTimeFrom >= diff).Where(a => a.Accepted == true), b => b.DateTimeFrom.Date).Select(a => a.Id).ToList().Count();
          
            
            for (int indx = 0; indx <= days; indx++)
            {
                DateTime d = start.AddDays(indx).Date;
                List<UserTimeOff> t = DistinctByExtension.DistinctBy(db.userTimeOff.ToList().Where(a => a.ExtendIdentityUserId == id && a.DateTimeFrom - a.DateTimeTo < diff && a.DateTimeFrom.Date == d.Date && a.Accepted == true), a => a.DateTimeFrom.Date).ToList();


                var tt = t.Count();
                if (tt > 1)
                {
                    
                    TimeSpan x = new TimeSpan(0, 0, 0, 0, 0);
                    foreach (var itemm in t)
                    {
                        x = x + (itemm.DateTimeTo - itemm.DateTimeFrom);
                    }
                    if (x >= diff)
                    {
                        timeoffdays++;
                    }


                }
             
            }
      
            return timeoffdays;
        }
    }
}

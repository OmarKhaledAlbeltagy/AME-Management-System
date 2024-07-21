using AMEKSA.Context;
using AMEKSA.DevModels;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class DevRep : IDevRep
    {
        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly IKpisRep kpisRep;

        public DevRep(DbContainer db, UserManager<ExtendIdentityUser> userManager, IKpisRep kpisRep)
        {
            this.db = db;
            this.userManager = userManager;
            this.kpisRep = kpisRep;
        }

        public int Addcad(AddCad m)
        {
            int err = 0;
            foreach (var item in m.ContactName)
            {
                Contact c = db.contact.Where(a => a.ContactName == item).FirstOrDefault();

                if (c == null)
                {
                    err++;
                }
                else
                {
                    EventTravelRequest ev = new EventTravelRequest();
                    ev.ContactId = c.Id;
                    ev.EventId = m.EventId;
                    ev.Accumpained = 0;
                    ev.Rejected = false;
                    ev.Confirmed = false;
                    ev.TopAction = true;
                    ev.TopConfirmed = true;
                    ev.TopRejected = false;
                    ev.TopActionUserId = "eaaddc1d-e5f2-4594-abc3-cb545bbde63f";
                    ev.ExtendIdentityUserId = "34bbdf68-cc97-4edd-b2be-8dd1593962b7";
                    db.EventTravelRequest.Add(ev);
                }
            }
            db.SaveChanges();
            return err;
        }

        public bool changeuserpassword(string id)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(id).Result;

            string token =   userManager.GeneratePasswordResetTokenAsync(user).Result;

           IdentityResult res = userManager.ResetPasswordAsync(user, token,"123456789").Result;
            if(res.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }  
        }

        public bool configurelimit()
        {
            List<ExtendIdentityUser> all = new List<ExtendIdentityUser>();
            List<ExtendIdentityUser> medical = userManager.GetUsersInRoleAsync("Medical Representative").Result.ToList();
            List<ExtendIdentityUser> sales = userManager.GetUsersInRoleAsync("Sales Representative").Result.ToList();
            List<ExtendIdentityUser> Supportive = userManager.GetUsersInRoleAsync("Supportive").Result.ToList();

            foreach (var item in medical)
            {
                all.Add(item);
            }

            foreach (var item in sales)
            {
                all.Add(item);
            }

            foreach (var item in Supportive)
            {
                all.Add(item);
            }

            foreach (var item in all)
            {
                UserVisitsLimit x = new UserVisitsLimit();
                x.Days = 3;
                x.ExtendIdentityUserId = item.Id;
                db.usersVisitsLimit.Add(x);
            }
            db.SaveChanges();
            return true;
        }

        public IEnumerable<MedicalKpiModel> GetAllMedicalKpi()
        {
           
            DateTime start = new DateTime(2022, 1, 1, 0, 0, 0);
            DateTime end = new DateTime(2022, 6, 15, 23, 59, 59);

            int? workingdaysdata = 113;
            IEnumerable<ExtendIdentityUser> users = userManager.GetUsersInRoleAsync("Medical Representative").Result;
            List<MedicalKpiModel> ress = new List<MedicalKpiModel>();
            foreach (var item in users)
            {
                int? timeoffdays = kpisRep.GetTimeOffDiff(item.Id, 2022, 01, 1, 2022, 06, 15);




                IEnumerable<ContactWithCategory> UserAPlusAndA = db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
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
                }).DistinctBy(x => x.Id).Where(a => a.CategoryId == 1 || a.CategoryId == 2);

                IEnumerable<ContactWithCategory> UserB = db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
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
                }).DistinctBy(x => x.Id).Where(a => a.CategoryId == 3);

                IEnumerable<ContactWithCategory> UserC = db.userContact.Where(a => a.extendidentityuserid == item.Id).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new ContactWithCategory
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
                }).DistinctBy(x => x.Id).Where(a => a.CategoryId == 4);

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
                res.Month = start.ToString("MMMM - yyyy");
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
                res.SellingDaysInTheField = db.contactMedicalVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate.Date >= start.Date && a.VisitDate.Date <= end.Date).DistinctBy(x => x.VisitDate.Date).Select(a => a.Id).Count();
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

        public IEnumerable<SalesKpiModel> GetAllSalesKpi()
        {
            DateTime start = new DateTime(2022, 1, 1, 0, 0, 0);
            DateTime end = new DateTime(2022, 6, 15, 23, 59, 59);

            int? WorkingDaysData = 113;
            IEnumerable<ExtendIdentityUser> list = userManager.GetUsersInRoleAsync("Sales Representative").Result;
            List<SalesKpiModel> result = new List<SalesKpiModel>();

            foreach (var obj in list)
            {
                int? timeoffdays = kpisRep.GetTimeOffDiff(obj.Id, 2022, 01, 01, 2022, 06, 15);
                int AverageVisitsPerDayWeight = db.properties.Where(a => a.Id == 3).Select(x => x.Value).FirstOrDefault();
                int VisitsTargetAchievmentWeight = db.properties.Where(a => a.Id == 2).Select(x => x.Value).FirstOrDefault();
                int CoverageForListedAccountsWeight = db.properties.Where(a => a.Id == 5).Select(x => x.Value).FirstOrDefault();
                int SellingDaysInTheFieldKpiWeight = db.properties.Where(a => a.Id == 6).Select(x => x.Value).FirstOrDefault();

                int AverageVisitsPerDayKpiTarget = db.properties.Where(a => a.Id == 1).Select(a => a.Value).SingleOrDefault();

                double? WorkingDays = WorkingDaysData - timeoffdays;

                string FullName = obj.FullName;

                string office = db.city.Where(a => a.Id == obj.CityId).Select(a => a.CityName).FirstOrDefault();

                string date = start.ToString("MMMM - yyyy");

                double ActualTotalNumberOfVisits = db.accountSalesVisit.Where(a => a.extendidentityuserid == obj.Id && a.VisitDate >= start && a.VisitDate <= end).Select(a => a.Id).Count();

                double ListedAccounts = db.userAccount.Where(a => a.extendidentityuserid == obj.Id).DistinctBy(x => x.AccountId).Count();

                double SellingDaysInTheField = db.accountSalesVisit.Where(a => a.extendidentityuserid == obj.Id && a.VisitDate.Date >= start.Date && a.VisitDate.Date <= end.Date).DistinctBy(x => x.VisitDate.Date).Select(a => a.Id).Count();

                

                double VisitedAccounts = db.accountSalesVisit.Where(a => a.extendidentityuserid == obj.Id && a.VisitDate >= start && a.VisitDate <= end).DistinctBy(a => a.AccountId).Select(a => a.Id).Count();

                

                SalesKpiModel res = new SalesKpiModel();
                res.FullName = FullName;
                res.Month = date;
                res.RoleName = "Sales Representative";

                res.CityName = office;
                res.ActualTotalNumberOfVisits = ActualTotalNumberOfVisits;
                res.ListedAccounts = ListedAccounts;
                res.SellingDaysInTheField = SellingDaysInTheField;
                res.VisitedAccounts = VisitedAccounts;
                res.WorkingDays = WorkingDays;
                res.AverageVisitsPerDayKpiTarget = AverageVisitsPerDayKpiTarget;
                res.AverageVisitsPerDayWeight = AverageVisitsPerDayWeight;
                res.VisitsTargetAchievmentWeight = VisitsTargetAchievmentWeight;
                res.CoverageForListedAccountsWeight = CoverageForListedAccountsWeight;
                res.SellingDaysInTheFieldKpiWeight = SellingDaysInTheFieldKpiWeight;
                res.TimeOffDays = timeoffdays;
                result.Add(res);
            }
            return result.OrderBy(a => a.FullName);
        }

        public int? GetWorkingDays(int yearfrom, int monthfrom, int yearto, int monthto)
        {

            int daysto = DateTime.DaysInMonth(yearto, monthto);
            DateTime start = new DateTime(yearfrom, monthfrom, 1, 0, 0, 0);
            DateTime end = new DateTime(yearto, monthto, daysto, 23, 59, 59);
            List<int> ids = db.workingDays.Where(a => a.Month >= start.Month && a.Year >= start.Year && a.Month <= end.Month && a.Year <= end.Year).Select(a => a.Id).ToList();
            int? WorkingDaysData = db.workingDays.Where(a => a.Month >= start.Month && a.Year >= start.Year && a.Month <= end.Month && a.Year <= end.Year).Select(a => a.NumberOfWorkingDays).Sum();

            return WorkingDaysData;
        }

        public bool Maher()
        {
            IEnumerable<UserAccount> del = db.userAccount.Where(a => a.extendidentityuserid == "e038ca96-665b-42db-a52c-aa29523a23b7");

            foreach (var item in del)
            {
                db.userAccount.Remove(item);
            }
            db.SaveChanges();
            List<int> accids = db.account.Select(a=>a.Id).ToList();

            foreach (var item in accids)
            {
                UserAccount obj = new UserAccount();
                obj.AccountId = item;
                obj.extendidentityuserid = "e038ca96-665b-42db-a52c-aa29523a23b7";
                db.userAccount.Add(obj);
            }
            db.SaveChanges();
            return true;
        }

        public bool mail()
        {
            //string path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "EmailTemp.html");
            //var body = File.ReadAllText(path);

         
            string body = "<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">\r\n<head>\r\n    <!--[if gte mso 9]>\r\n    <xml>\r\n      <o:OfficeDocumentSettings>\r\n        <o:AllowPNG/>\r\n        <o:PixelsPerInch>96</o:PixelsPerInch>\r\n      </o:OfficeDocumentSettings>\r\n    </xml>\r\n    <![endif]-->\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <meta name=\"x-apple-disable-message-reformatting\">\r\n    <!--[if !mso]><!-->\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\"><!--<![endif]-->\r\n    <title></title>\r\n\r\n    <style type=\"text/css\">\r\n        @media only screen and (min-width: 620px) {\r\n            .u-row {\r\n                width: 600px !important;\r\n            }\r\n\r\n                .u-row .u-col {\r\n                    vertical-align: top;\r\n                }\r\n\r\n                .u-row .u-col-50 {\r\n                    width: 300px !important;\r\n                }\r\n\r\n                .u-row .u-col-100 {\r\n                    width: 600px !important;\r\n                }\r\n        }\r\n\r\n        @media (max-width: 620px) {\r\n            .u-row-container {\r\n                max-width: 100% !important;\r\n                padding-left: 0px !important;\r\n                padding-right: 0px !important;\r\n            }\r\n\r\n            .u-row .u-col {\r\n                min-width: 320px !important;\r\n                max-width: 100% !important;\r\n                display: block !important;\r\n            }\r\n\r\n            .u-row {\r\n                width: calc(100% - 40px) !important;\r\n            }\r\n\r\n            .u-col {\r\n                width: 100% !important;\r\n            }\r\n\r\n                .u-col > div {\r\n                    margin: 0 auto;\r\n                }\r\n        }\r\n\r\n        body {\r\n            margin: 0;\r\n            padding: 0;\r\n        }\r\n\r\n        table,\r\n        tr,\r\n        td {\r\n            vertical-align: top;\r\n            border-collapse: collapse;\r\n        }\r\n\r\n        p {\r\n            margin: 0;\r\n        }\r\n\r\n        .ie-container table,\r\n        .mso-container table {\r\n            table-layout: fixed;\r\n        }\r\n\r\n        * {\r\n            line-height: inherit;\r\n        }\r\n\r\n        a[x-apple-data-detectors='true'] {\r\n            color: inherit !important;\r\n            text-decoration: none !important;\r\n        }\r\n\r\n        table, td {\r\n            color: #000000;\r\n        }\r\n\r\n        #u_body a {\r\n            color: #0000ee;\r\n            text-decoration: underline;\r\n        }\r\n\r\n        #u_content_text_4 a {\r\n            color: #f1c40f;\r\n        }\r\n\r\n        @media (max-width: 480px) {\r\n            /* #u_content_image_1 .v-src-width {\r\n                width: auto !important;\r\n            }\r\n\r\n            #u_content_image_1 .v-src-max-width {\r\n                max-width: 25% !important;\r\n            } */\r\n\r\n            #u_content_text_3 .v-container-padding-padding {\r\n                padding: 10px 20px 20px !important;\r\n            }\r\n\r\n            #u_content_button_1 .v-size-width {\r\n                width: 65% !important;\r\n            }\r\n\r\n            #u_content_text_2 .v-container-padding-padding {\r\n                padding: 20px 20px 60px !important;\r\n            }\r\n\r\n            #u_content_text_4 .v-container-padding-padding {\r\n                padding: 60px 20px !important;\r\n            }\r\n\r\n            #u_content_heading_2 .v-container-padding-padding {\r\n                padding: 30px 10px 0px !important;\r\n            }\r\n\r\n            #u_content_heading_2 .v-text-align {\r\n                text-align: center !important;\r\n            }\r\n\r\n            #u_content_social_1 .v-container-padding-padding {\r\n                padding: 10px 10px 10px 98px !important;\r\n            }\r\n\r\n            #u_content_text_5 .v-container-padding-padding {\r\n                padding: 10px 20px 30px !important;\r\n            }\r\n\r\n            #u_content_text_5 .v-text-align {\r\n                text-align: center !important;\r\n            }\r\n        }\r\n    </style>\r\n\r\n\r\n\r\n    <!--[if !mso]><!-->\r\n    <link href=\"https://fonts.googleapis.com/css?family=Open+Sans:400,700&display=swap\" rel=\"stylesheet\" type=\"text/css\">\r\n    <link href=\"https://fonts.googleapis.com/css?family=Rubik:400,700&display=swap\" rel=\"stylesheet\" type=\"text/css\"><!--<![endif]-->\r\n\r\n</head>\r\n\r\n<body class=\"clean-body u_body\" style=\"margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #000000;color: #000000\">\r\n    <!--[if IE]><div class=\"ie-container\"><![endif]-->\r\n    <!--[if mso]><div class=\"mso-container\"><![endif]-->\r\n    <table id=\"u_body\" style=\"border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #000000;width:100%\" cellpadding=\"0\" cellspacing=\"0\">\r\n        <tbody>\r\n            <tr style=\"vertical-align: top\">\r\n                <td style=\"word-break: break-word;border-collapse: collapse !important;vertical-align: top\">\r\n                    <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td align=\"center\" style=\"background-color: #000000;\"><![endif]-->\r\n\r\n\r\n                    <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\r\n                        <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;\">\r\n                            <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;\">\r\n                                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-color: transparent;\"><![endif]-->\r\n                                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\" valign=\"top\"><![endif]-->\r\n                                <div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">\r\n                                    <div style=\"height: 100%;width: 100% !important;\">\r\n                                        <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;\">\r\n                                            <!--<![endif]-->\r\n\r\n                                            <table id=\"u_content_image_1\" style=\"font-family:'Open Sans',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                                                <tbody>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">\r\n                                                                <tr>\r\n                                                                    <td class=\"v-text-align\" style=\"padding-right: 0px;padding-left: 0px;\" align=\"center\">\r\n\r\n                                                                        <img align=\"center\" border=\"0\" src=\"[[qrcode]]\" alt=\"Logo\" title=\"Logo\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;max-width: 272.6px;\" width=\"272.6\" class=\"v-src-width v-src-max-width\" />\r\n\r\n                                                                    </td>\r\n                                                                </tr>\r\n                                                            </table>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                </tbody>\r\n                                            </table>\r\n\r\n                                            <!--[if (!mso)&(!IE)]><!-->\r\n                                        </div><!--<![endif]-->\r\n                                    </div>\r\n                                </div>\r\n                                <!--[if (mso)|(IE)]></td><![endif]-->\r\n                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n\r\n\r\n\r\n                    <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\r\n                        <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;\">\r\n                            <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-image: url('images/image-6.png');background-repeat: no-repeat;background-position: center top;background-color: transparent;\">\r\n                                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-image: url('images/image-6.png');background-repeat: no-repeat;background-position: center top;background-color: transparent;\"><![endif]-->\r\n                                <!--[if (mso)|(IE)]><td align=\"center\" width=\"600\" style=\"background-color: #ffffff;width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\" valign=\"top\"><![endif]-->\r\n                                <div class=\"u-col u-col-100\" style=\"max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;\">\r\n                                    <div style=\"background-color: #ffffff;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                        <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                            <!--<![endif]-->\r\n\r\n                                            <table style=\"font-family:'Open Sans',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                                                <tbody>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:60px 10px 10px;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <div class=\"v-text-align\" style=\"line-height: 170%; text-align: center; word-wrap: break-word;\">\r\n                                                                <p style=\"font-size: 14px; line-height: 170%;\"><span style=\"font-size: 20px; line-height: 34px;\"><strong><span style=\"line-height: 34px; font-size: 20px;\">Hello Dr. [[contactname]],</span></strong></span></p>\r\n                                                            </div>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                </tbody>\r\n                                            </table>\r\n\r\n                                            <table id=\"u_content_text_3\" style=\"font-family:'Open Sans',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                                                <tbody>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:10px 100px 20px;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <div class=\"v-text-align\" style=\"line-height: 170%; text-align: center; word-wrap: break-word;\">\r\n                                                                <p style=\"font-size: 14px; line-height: 170%;\"><span style=\"font-size: 16px; line-height: 27.2px;\">Your Registration code is </span><br><strong style=\"font-size: 16px; line-height: 27.2px;\">[[code]] </strong><br><span style=\"font-size: 16px; line-height: 27.2px;\">You will need this code to verify your registration at the conference.</span><br><span style=\"font-size: 16px; line-height: 27.2px;\">Or you can use this QR Code instead</span></p>\r\n                                                            </div>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:10px 100px 20px;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <div class=\"v-text-align\" style=\"line-height: 170%; text-align: center; word-wrap: break-word;\">\r\n                                                                <img align=\"center\" border=\"0\" src=\"https://buildingonsuccess.net/email/qr-code.png\" alt=\"Logo\" title=\"Logo\" style=\"outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;max-width: 272.6px;\" width=\"272.6\" class=\"v-src-width v-src-max-width\" />\r\n                                                            </div>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                </tbody>\r\n                                            </table>\r\n                                        </div>\r\n                                    </div>\r\n                                </div>\r\n\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n\r\n\r\n\r\n                  \r\n\r\n\r\n\r\n                    <div class=\"u-row-container\" style=\"padding: 0px;background-color: transparent\">\r\n                        <div class=\"u-row\" style=\"Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;\">\r\n                            <div style=\"border-collapse: collapse;display: table;width: 100%;height: 100%;background-image: url('images/image-5.png');background-repeat: no-repeat;background-position: center top;background-color: transparent;\">\r\n                                <!--[if (mso)|(IE)]><table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\"><tr><td style=\"padding: 0px;background-color: transparent;\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width:600px;\"><tr style=\"background-image: url('images/image-5.png');background-repeat: no-repeat;background-position: center top;background-color: transparent;\"><![endif]-->\r\n                                <!--[if (mso)|(IE)]><td align=\"center\" width=\"300\" style=\"background-color: #f1c40f;width: 300px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\" valign=\"top\"><![endif]-->\r\n                                <div class=\"u-col u-col-50\" style=\"max-width: 320px;min-width: 300px;display: table-cell;vertical-align: top;\">\r\n                                    <div style=\"background-color: #f1c40f;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                        <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                            <!--<![endif]-->\r\n\r\n                                            <table id=\"u_content_heading_2\" style=\"font-family:'Open Sans',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                                                <tbody>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:30px 10px 0px 50px;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <h1 class=\"v-text-align\" style=\"margin: 0px; line-height: 140%; text-align: left; word-wrap: break-word; font-weight: normal; font-family: 'Rubik',sans-serif; font-size: 22px;\">\r\n                                                                <div>\r\n                                                                    <div><strong>AME</strong> & Teoxane</div>\r\n                                                                </div>\r\n                                                            </h1>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                </tbody>\r\n                                            </table>\r\n\r\n                                      \r\n\r\n                                            <!--[if (!mso)&(!IE)]><!-->\r\n                                        </div><!--<![endif]-->\r\n                                    </div>\r\n                                </div>\r\n                                <!--[if (mso)|(IE)]></td><![endif]-->\r\n                                <!--[if (mso)|(IE)]><td align=\"center\" width=\"300\" style=\"background-color: #f1c40f;width: 300px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\" valign=\"top\"><![endif]-->\r\n                                <div class=\"u-col u-col-50\" style=\"max-width: 320px;min-width: 300px;display: table-cell;vertical-align: top;\">\r\n                                    <div style=\"background-color: #f1c40f;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                        <!--[if (!mso)&(!IE)]><!--><div style=\"height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;\">\r\n                                            <!--<![endif]-->\r\n\r\n                                            <table id=\"u_content_text_5\" style=\"font-family:'Open Sans',sans-serif;\" role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" border=\"0\">\r\n                                                <tbody>\r\n                                                    <tr>\r\n                                                        <td class=\"v-container-padding-padding\" style=\"overflow-wrap:break-word;word-break:break-word;padding:31px 50px 30px 10px;font-family:'Open Sans',sans-serif;\" align=\"left\">\r\n\r\n                                                            <div class=\"v-text-align\" style=\"line-height: 170%; text-align: right; word-wrap: break-word;\">\r\n                                                                <a href=\"https://goo.gl/maps/1qv5jDBhuK6L22dr9\" target=\"_blank\">location</a>\r\n                                                            </div>\r\n\r\n                                                        </td>\r\n                                                    </tr>\r\n                                                </tbody>\r\n                                            </table>\r\n\r\n                                            <!--[if (!mso)&(!IE)]><!-->\r\n                                        </div><!--<![endif]-->\r\n                                    </div>\r\n                                </div>\r\n                                <!--[if (mso)|(IE)]></td><![endif]-->\r\n                                <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->\r\n                            </div>\r\n                        </div>\r\n                    </div>\r\n\r\n\r\n\r\n                \r\n\r\n\r\n                    <!--[if (mso)|(IE)]></td></tr></table><![endif]-->\r\n                </td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n    <!--[if mso]></div><![endif]-->\r\n    <!--[if IE]></div><![endif]-->\r\n</body>\r\n\r\n</html>\r\n";
            string bodyy = body.Replace("[[contactname]]", "Mohamed Alsawy").Replace("[[code]]", "1001").Replace("[[qrcode]]", "https://{{URL}}.com/image/a.png");
            MailMessage m = new MailMessage();
            m.To.Add("ameksadevelopment@gmail.com");
            //m.To.Add("sawy7800@hotmail.com");
            m.Subject = "TED AME Registration Code";
            m.From = new MailAddress(EmailModel.EmailAddress);
            m.Sender = new MailAddress(EmailModel.EmailAddress);
            m.Body = bodyy;
            m.IsBodyHtml = true;
            m.Priority = MailPriority.High;
            SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
            smtp.EnableSsl = false;
            smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
            smtp.Send(m);
            return true;
        }

        public bool taha()
        {
            //499c4469-f53d-4bbc-8bd2-cca0deff03cf
            IEnumerable<UserAccount> del = db.userAccount.Where(a => a.extendidentityuserid == "499c4469-f53d-4bbc-8bd2-cca0deff03cf");
            foreach (var item in del)
            {
                db.userAccount.Remove(item);
            }
            db.SaveChanges();


            List<int> dis1 = db.account.Where(a => a.DistrictId == 5).Select(a => a.Id).ToList();

            foreach (var item in dis1)
            {
                UserAccount obj = new UserAccount();
                obj.AccountId = item;
                obj.extendidentityuserid = "499c4469-f53d-4bbc-8bd2-cca0deff03cf";
                db.userAccount.Add(obj);
            }
            db.SaveChanges();
            return true;
        }

        public List<YearlyWorkingDaysModel> WorkingDaysYearlyReport()
        {
            DateTime start = new DateTime(2022, 01, 01);
            DateTime end = new DateTime(2022, 08, 20);

           // List<ExtendIdentityUser> userlist = new List<ExtendIdentityUser>();

            List<ExtendIdentityUser> medicals = userManager.GetUsersInRoleAsync("Medical Representative").Result.ToList();

            //List<ExtendIdentityUser> sales = userManager.GetUsersInRoleAsync("Sales Representative").Result.ToList();

            //List<ExtendIdentityUser> Supportive = userManager.GetUsersInRoleAsync("Supportive").Result.ToList();


  

         

         

            List<YearlyWorkingDaysModel> res = new List<YearlyWorkingDaysModel>();
            
            foreach (var item in medicals)
            {
                YearlyWorkingDaysModel obj = new YearlyWorkingDaysModel();
                obj.RepName = item.FullName;
                obj.ManagerName = userManager.FindByIdAsync(item.extendidentityuserid).Result.FullName;
                

                res.Add(obj);
            }

            //foreach (var item in sales)
            //{
            //    YearlyWorkingDaysModel obj = new YearlyWorkingDaysModel();
            //    obj.RepName = item.FullName;
            //    obj.ManagerName = userManager.FindByIdAsync(item.extendidentityuserid).Result.FullName;
            //    int WorkingDays = db.accountSalesVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate.Date >= start && a.VisitDate.Date <= end).DistinctBy(a => a.VisitDate.Date).Select(a => a.Id).Count();
            //    obj.WorkingDays = WorkingDays;
            //    int TimeOff = db.userTimeOff.Where(a => a.DateTimeFrom >= start && a.DateTimeFrom <= end && a.ExtendIdentityUserId == item.Id && a.DateTimeTo - a.DateTimeFrom >= t && a.Accepted == true).DistinctBy(a => a.DateTimeFrom.Date).Select(a => a.Id).Count();
            //    obj.TimeOff = TimeOff;
            //    res.Add(obj);
            //}

            //foreach (var item in Supportive)
            //{
            //    YearlyWorkingDaysModel obj = new YearlyWorkingDaysModel();
            //    obj.RepName = item.FullName;
            //    obj.ManagerName = userManager.FindByIdAsync(item.extendidentityuserid).Result.FullName;
            //    int WorkingDays = db.AccountSupportiveVisit.Where(a => a.extendidentityuserid == item.Id && a.VisitDate.Date >= start && a.VisitDate.Date <= end).DistinctBy(a => a.VisitDate.Date).Select(a => a.Id).Count();
            //    obj.WorkingDays = WorkingDays;
            //    int TimeOff = db.userTimeOff.Where(a => a.DateTimeFrom >= start && a.DateTimeFrom <= end && a.ExtendIdentityUserId == item.Id && a.DateTimeTo - a.DateTimeFrom >= t && a.Accepted == true).DistinctBy(a => a.DateTimeFrom.Date).Select(a => a.Id).Count();
            //    obj.TimeOff = TimeOff;
            //    res.Add(obj);
            //}

            return res.OrderBy(a => a.RepName).OrderBy(a => a.ManagerName).ToList();
        }
    }
}

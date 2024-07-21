using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using AMEKSA.Repo;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class SystemAdminController : ControllerBase
    {
        private readonly DbContainer db;
        private readonly SignInManager<ExtendIdentityUser> signInManager;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly IUserRep userRep;
        private readonly ISystemAdminRep rep;
        private readonly ITimeRep ti;

        public SystemAdminController(DbContainer db, SignInManager<ExtendIdentityUser> signInManager,UserManager<ExtendIdentityUser> userManager, RoleManager<ExtendIdentityRole> roleManager,IUserRep userRep, ISystemAdminRep rep,ITimeRep ti)
        {
            this.db = db;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.userRep = userRep;
            this.rep = rep;
            this.ti = ti;
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetDoubledContacts()
        {
            List<Contact> c = db.contact.ToList();
            List<Contact> res = new List<Contact>();

            foreach (var item in c)
            {
                List<Contact> namecheck = c.Where(a => a.ContactName == item.ContactName).ToList();
                List<Contact> emailcheck = c.Where(a=>a.Email == item.Email && a.Email != null && a.Email != "").ToList();
                List<Contact> mobilecheck = c.Where(a=>a.MobileNumber == item.MobileNumber && a.MobileNumber != null && a.MobileNumber != "").ToList();
                List<Contact> landcheck = c.Where(a => a.LandLineNumber == item.LandLineNumber && a.LandLineNumber != null && a.LandLineNumber != "").ToList();
                if (namecheck.Count > 1 || emailcheck.Count > 1 || mobilecheck.Count > 1 || landcheck.Count > 1)
                {
                    if (namecheck.Count > 1)
                    {
                        res.AddRange(namecheck);
                    }
                    if (emailcheck.Count > 1)
                    {
                        res.AddRange(emailcheck);
                    }
                    if (mobilecheck.Count > 1)
                    {
                        res.AddRange(mobilecheck);
                    }
                    if (landcheck.Count > 1)
                    {
                        res.AddRange(landcheck);
                    }
                }
            }


            List<Account> acc = db.account.ToList();
            List<District> d = db.district.ToList();
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Duplicated Contacts");
            worksheet.Range("A1:F2000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A1:F2000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Columns("A:F").Width = 25;
            worksheet.Rows("1:2000").Height = 25;
            worksheet.Range("A1:F1").Style.Font.Bold = true;

            worksheet.Cell("A1").Value = "Contact";
            worksheet.Cell("B1").Value = "Account Aff.";
            worksheet.Cell("C1").Value = "District";
            worksheet.Cell("D1").Value = "Email";
            worksheet.Cell("E1").Value = "Mobile Number";
            worksheet.Cell("F1").Value = "Land Line Number";

            var row = 2;
            foreach (var item in res)
            {
                worksheet.Cell("A" + row).Value = item.ContactName;
                if (item.AccountId != null)
                {
                    worksheet.Cell("B" + row).Value = acc.Where(a=>a.Id == item.AccountId).FirstOrDefault().AccountName;
                }
                else
                {
                    worksheet.Cell("B" + row).Value = "-";
                }
                
                worksheet.Cell("C" + row).Value = d.Where(a=>a.Id == item.DistrictId).FirstOrDefault().DistrictName;
                worksheet.Cell("D" + row).Value = item.Email;
                worksheet.Cell("E" + row).Value = item.MobileNumber;
                worksheet.Cell("F" + row).Value = item.LandLineNumber;
                row++;
            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Doubled Contacts.xlsx");
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult WorkingDaysRemidner()
        {
            return Ok(rep.WorkingDaysRemidner());
        }

        [Route("[controller]/[Action]/{RepId}/{ManagerId}")]
        [HttpGet]
        public IActionResult LinkSubManager(string RepId, string ManagerId)
        {
            return Ok(rep.LinkSubManager(RepId, ManagerId));
        }

        [Route("[controller]/[Action]/{RepId}/{ManagerId}")]
        [HttpGet]
        public IActionResult UnLinkSubManager(string RepId, string ManagerId)
        {
            return Ok(rep.UnLinkSubManager(RepId, ManagerId));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetRepLinkedSubManagers(string id)
        {
            return Ok(rep.GetRepLinkedSubManagers(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetRepNotLinkedSubManagers(string id)
        {
            return Ok(rep.GetRepNotLinkedSubManagers(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetContactMedicalVisitByContactId(int id)
        {
            return Ok(rep.GetContactMedicalVisitByContactId(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetAccountMedicalVisitByAccountId(int id)
        {
            return Ok(rep.GetAccountMedicalVisitByAccountId(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetAccountSalesVisitByAccountId(int id)
        {
            return Ok(rep.GetAccountSalesVisitByAccountId(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetAccountSupportiveVisitByAccountId(int id)
        {
            return Ok(rep.GetAccountSupportiveVisitByAccountId(id));
        }


        [Route("[controller]/[Action]/{x}")]
        [HttpGet]
        public IActionResult GetRepsByCategory(int x)
        {
            IEnumerable<RepByCategory> res = rep.GetRepsByCategory(x);

            return Ok(res);
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetRepsByCategoryExcel()
        {
            DateTime now = ti.GetCurrentTime();
            DateTime n = new DateTime(now.Year, now.Month, now.Day);
            XLWorkbook workbook = new XLWorkbook();
            IEnumerable<RepByCategory> med =  rep.GetRepsByCategory(1);
            var medsheet = workbook.Worksheets.Add("Medical");
            medsheet.Rows(1, 100).Height = 20;
            medsheet.Columns(1, 10).Width = 15;
            medsheet.Range("A1:J100").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            medsheet.Range("A1:J100").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            medsheet.Range("A1:A2").Merge();
            medsheet.Range("B1:B2").Merge();
            medsheet.Range("C1:D1").Merge();
            medsheet.Range("E1:F1").Merge();
            medsheet.Range("G1:H1").Merge();
            medsheet.Range("I1:J1").Merge();
            medsheet.Range("A1:J2").Style.Font.Bold = true;
            medsheet.Range("A1:J2").Style.Font.FontSize = 16;
            medsheet.Range("A1:B2").Style.Fill.BackgroundColor = XLColor.FromArgb(191,191,191);
            medsheet.Column("A").Width = 30;
            medsheet.Column("B").Width = 20;
            medsheet.Range("A3:J100").Style.Font.FontSize = 14;
            medsheet.Range("C1:D100").Style.Fill.BackgroundColor = XLColor.FromArgb(146, 208, 80);
            medsheet.Range("E1:F100").Style.Fill.BackgroundColor = XLColor.FromArgb(149, 179, 215);
            medsheet.Range("G1:H100").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 192, 0);
            medsheet.Range("I1:J100").Style.Fill.BackgroundColor = XLColor.FromArgb(250, 191, 143);
            medsheet.Range("C3:C100").Style.Border.RightBorder = XLBorderStyleValues.None;
            medsheet.Range("E3:E100").Style.Border.RightBorder = XLBorderStyleValues.None;
            medsheet.Range("G3:G100").Style.Border.RightBorder = XLBorderStyleValues.None;
            medsheet.Range("I3:I100").Style.Border.RightBorder = XLBorderStyleValues.None;
            medsheet.Range("C1:J2").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            medsheet.Range("C1:J2").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            medsheet.Range("B1:B100").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            medsheet.Range("D1:D100").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            medsheet.Range("F1:F100").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            medsheet.Range("H1:H100").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            medsheet.Range("J1:J100").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            var medRow = 1;
            medsheet.Cell(medRow, 1).Value = "Represntative Name";
            medsheet.Cell(medRow, 2).Value = "Total Contacts";
            medsheet.Cell(medRow, 3).Value = "A+";
            medsheet.Cell(medRow, 5).Value = "A";
            medsheet.Cell(medRow, 7).Value = "B";
            medsheet.Cell(medRow, 9).Value = "C";
            medRow = 2;
            medsheet.Cell(medRow, 3).Value = "#";
            medsheet.Cell(medRow, 4).Value = "%";
            medsheet.Cell(medRow, 5).Value = "#";
            medsheet.Cell(medRow, 6).Value = "%";
            medsheet.Cell(medRow, 7).Value = "#";
            medsheet.Cell(medRow, 8).Value = "%";
            medsheet.Cell(medRow, 9).Value = "#";
            medsheet.Cell(medRow, 10).Value = "%";
            medRow = 3;
            foreach (var item in med)
            {
                medsheet.Cell(medRow, 1).Value = item.RepName;
                medsheet.Cell(medRow, 2).Value = item.TotalContacts;
                medsheet.Cell(medRow, 3).Value = item.APlusContacts;
                medsheet.Cell(medRow, 4).Value = (item.APlusContactsPercentage / 100).ToString("##.##" + "%");
                medsheet.Cell(medRow, 5).Value = item.AContacts;
                medsheet.Cell(medRow, 6).Value = (item.AContactsPercentage / 100).ToString("##.##" + "%");
                medsheet.Cell(medRow, 7).Value = item.BContacts;
                medsheet.Cell(medRow, 8).Value = (item.BContactsPercentage / 100).ToString("##.##"+"%");
                medsheet.Cell(medRow, 9).Value = item.CContacts;
                medsheet.Cell(medRow, 10).Value = (item.CContactsPercentage / 100).ToString("##.##" + "%");
                medsheet.Range("C" + medRow + ":J" + medRow).Style.Border.BottomBorder = XLBorderStyleValues.None;
                medRow++;
            }
            IEnumerable<RepByCategory> sal = rep.GetRepsByCategory(2);
            var salsheet = workbook.Worksheets.Add("Sales");
            salsheet.Rows(1, 100).Height = 20;
            salsheet.Columns(1, 10).Width = 15;
            salsheet.Range("A1:J100").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            salsheet.Range("A1:J100").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            salsheet.Range("A1:A2").Merge();
            salsheet.Range("B1:B2").Merge();
            salsheet.Range("C1:D1").Merge();
            salsheet.Range("E1:F1").Merge();
            salsheet.Range("G1:H1").Merge();
            salsheet.Range("I1:J1").Merge();
            salsheet.Range("A1:J2").Style.Font.Bold = true;
            salsheet.Range("A1:J2").Style.Font.FontSize = 16;
            salsheet.Range("A1:B2").Style.Fill.BackgroundColor = XLColor.FromArgb(191, 191, 191);
            salsheet.Column("A").Width = 30;
            salsheet.Column("B").Width = 20;
            salsheet.Range("A3:J100").Style.Font.FontSize = 14;
            salsheet.Range("C1:D100").Style.Fill.BackgroundColor = XLColor.FromArgb(146, 208, 80);
            salsheet.Range("E1:F100").Style.Fill.BackgroundColor = XLColor.FromArgb(149, 179, 215);
            salsheet.Range("G1:H100").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 192, 0);
            salsheet.Range("I1:J100").Style.Fill.BackgroundColor = XLColor.FromArgb(250, 191, 143);
            salsheet.Range("C3:C100").Style.Border.RightBorder = XLBorderStyleValues.None;
            salsheet.Range("E3:E100").Style.Border.RightBorder = XLBorderStyleValues.None;
            salsheet.Range("G3:G100").Style.Border.RightBorder = XLBorderStyleValues.None;
            salsheet.Range("I3:I100").Style.Border.RightBorder = XLBorderStyleValues.None;
            salsheet.Range("C1:J2").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            salsheet.Range("C1:J2").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            salsheet.Range("B1:B100").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            salsheet.Range("D1:D100").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            salsheet.Range("F1:F100").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            salsheet.Range("H1:H100").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            salsheet.Range("J1:J100").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            var salRow = 1;
            salsheet.Cell(salRow, 1).Value = "Represntative Name";
            salsheet.Cell(salRow, 2).Value = "Total Accounts";
            salsheet.Cell(salRow, 3).Value = "A+";
            salsheet.Cell(salRow, 5).Value = "A";
            salsheet.Cell(salRow, 7).Value = "B";
            salsheet.Cell(salRow, 9).Value = "C";
            salRow = 2;
            salsheet.Cell(salRow, 3).Value = "#";
            salsheet.Cell(salRow, 4).Value = "%";
            salsheet.Cell(salRow, 5).Value = "#";
            salsheet.Cell(salRow, 6).Value = "%";
            salsheet.Cell(salRow, 7).Value = "#";
            salsheet.Cell(salRow, 8).Value = "%";
            salsheet.Cell(salRow, 9).Value = "#";
            salsheet.Cell(salRow, 10).Value = "%";
            salRow = 3;
            foreach (var item in sal)
            {
                salsheet.Cell(salRow, 1).Value = item.RepName;
                salsheet.Cell(salRow, 2).Value = item.TotalContacts;
                salsheet.Cell(salRow, 3).Value = item.APlusContacts;
                salsheet.Cell(salRow, 4).Value = (item.APlusContactsPercentage / 100).ToString("##.##" + "%");
                salsheet.Cell(salRow, 5).Value = item.AContacts;
                salsheet.Cell(salRow, 6).Value = (item.AContactsPercentage / 100).ToString("##.##" + "%");
                salsheet.Cell(salRow, 7).Value = item.BContacts;
                salsheet.Cell(salRow, 8).Value = (item.BContactsPercentage / 100).ToString("##.##" + "%");
                salsheet.Cell(salRow, 9).Value = item.CContacts;
                salsheet.Cell(salRow, 10).Value = (item.CContactsPercentage / 100).ToString("##.##" + "%");
                salsheet.Range("C" + salRow + ":J" + salRow).Style.Border.BottomBorder = XLBorderStyleValues.None;
                salRow++;
            }

            IEnumerable<RepByCategory> sup = rep.GetRepsByCategory(3);
            var supsheet = workbook.Worksheets.Add("Supportive");
            supsheet.Rows(1, 100).Height = 20;
            supsheet.Columns(1, 10).Width = 15;
            supsheet.Range("A1:J100").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            supsheet.Range("A1:J100").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            supsheet.Range("A1:A2").Merge();
            supsheet.Range("B1:B2").Merge();
            supsheet.Range("C1:D1").Merge();
            supsheet.Range("E1:F1").Merge();
            supsheet.Range("G1:H1").Merge();
            supsheet.Range("I1:J1").Merge();
            supsheet.Range("A1:J2").Style.Font.Bold = true;
            supsheet.Range("A1:J2").Style.Font.FontSize = 16;
            supsheet.Range("A1:B2").Style.Fill.BackgroundColor = XLColor.FromArgb(191, 191, 191);
            supsheet.Column("A").Width = 30;
            supsheet.Column("B").Width = 20;
            supsheet.Range("A3:J100").Style.Font.FontSize = 14;
            supsheet.Range("C1:D100").Style.Fill.BackgroundColor = XLColor.FromArgb(146, 208, 80);
            supsheet.Range("E1:F100").Style.Fill.BackgroundColor = XLColor.FromArgb(149, 179, 215);
            supsheet.Range("G1:H100").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 192, 0);
            supsheet.Range("I1:J100").Style.Fill.BackgroundColor = XLColor.FromArgb(250, 191, 143);
            supsheet.Range("C3:C100").Style.Border.RightBorder = XLBorderStyleValues.None;
            supsheet.Range("E3:E100").Style.Border.RightBorder = XLBorderStyleValues.None;
            supsheet.Range("G3:G100").Style.Border.RightBorder = XLBorderStyleValues.None;
            supsheet.Range("I3:I100").Style.Border.RightBorder = XLBorderStyleValues.None;
            supsheet.Range("C1:J2").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            supsheet.Range("C1:J2").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            supsheet.Range("B1:B100").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            supsheet.Range("D1:D100").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            supsheet.Range("F1:F100").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            supsheet.Range("H1:H100").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            supsheet.Range("J1:J100").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            var supRow = 1;
            supsheet.Cell(supRow, 1).Value = "Represntative Name";
            supsheet.Cell(supRow, 2).Value = "Total Accounts";
            supsheet.Cell(supRow, 3).Value = "A+";
            supsheet.Cell(supRow, 5).Value = "A";
            supsheet.Cell(supRow, 7).Value = "B";
            supsheet.Cell(supRow, 9).Value = "C";
            supRow = 2;
            supsheet.Cell(supRow, 3).Value = "#";
            supsheet.Cell(supRow, 4).Value = "%";
            supsheet.Cell(supRow, 5).Value = "#";
            supsheet.Cell(supRow, 6).Value = "%";
            supsheet.Cell(supRow, 7).Value = "#";
            supsheet.Cell(supRow, 8).Value = "%";
            supsheet.Cell(supRow, 9).Value = "#";
            supsheet.Cell(supRow, 10).Value = "%";
            supRow = 3;
            foreach (var item in sup)
            {
                supsheet.Cell(supRow, 1).Value = item.RepName;
                supsheet.Cell(supRow, 2).Value = item.TotalContacts;
                supsheet.Cell(supRow, 3).Value = item.APlusContacts;
                supsheet.Cell(supRow, 4).Value = (item.APlusContactsPercentage / 100).ToString("##.##" + "%");
                supsheet.Cell(supRow, 5).Value = item.AContacts;
                supsheet.Cell(supRow, 6).Value = (item.AContactsPercentage / 100).ToString("##.##" + "%");
                supsheet.Cell(supRow, 7).Value = item.BContacts;
                supsheet.Cell(supRow, 8).Value = (item.BContactsPercentage / 100).ToString("##.##" + "%");
                supsheet.Cell(supRow, 9).Value = item.CContacts;
                supsheet.Cell(supRow, 10).Value = (item.CContactsPercentage / 100).ToString("##.##" + "%");
                supsheet.Range("C" + supRow + ":J" + supRow).Style.Border.BottomBorder = XLBorderStyleValues.None;
                supRow++;
            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Layout Report On " + n.Date + ".xlsx");
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult mail()
        {
            SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
            smtp.EnableSsl = false;
            
            smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
            string body = "A new account was created by ame reporting tool system admin with your email address \n\nClick Here to verify your account";
            smtp.Send(EmailModel.EmailAddress, "assassin.creed171@gmail.com", "AME Reporting Tool: Email Verification", body);
            
            return Ok();
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult SystemAdminRegister(SystemAdminRegisterModel obj)
        {


            ExtendIdentityUser u = new ExtendIdentityUser();
            u.Email = obj.Email;
            u.UserName = obj.Email;
            u.FullName = obj.FullName;
            u.PhoneNumber = obj.PhoneNumber;
            u.SecurityStamp = Guid.NewGuid().ToString();
            u.EmailConfirmed = true;
            var result = userManager.CreateAsync(u, obj.Password).Result;
            var addtorole = userManager.AddToRoleAsync(u, obj.RoleName).Result;

            if (result.Succeeded)
            {
                ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
                string token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                //465
                SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
                string body = "A new account was created by AME reporting tool system admin with your email address \n\nClick Here to verify your account\n https://{{DashboardURL}}/emailconfirmed.html?" + token + "&" + obj.Email;
                smtp.Send(EmailModel.EmailAddress, obj.Email, "AME Reporting Tool: Email Verification", body);
                return Ok(true);
            }
            else
            {
                return Ok(result.Errors.First().Description);
            }

        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult TopLineManagerRegister(TopLineManagerRegisterModel obj)
        {
            ExtendIdentityUser u = new ExtendIdentityUser();
            u.Email = obj.Email;
            u.UserName = obj.Email;
            u.FullName = obj.FullName;
            u.PhoneNumber = obj.PhoneNumber;
            u.SecurityStamp = Guid.NewGuid().ToString();
            u.EmailConfirmed = true;
            var result = userManager.CreateAsync(u, obj.Password).Result;
            var addtorole = userManager.AddToRoleAsync(u, obj.RoleName).Result;

            if (result.Succeeded)
            {
                ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
                string token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                //465
                SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
                string body = "A new account was created by AME reporting tool system admin with your email address \n\nClick Here to verify your account\n https://{{DashboardURL}}/emailconfirmed.html?" + token + "&" + obj.Email;
                smtp.Send(EmailModel.EmailAddress, obj.Email, "AME Reporting Tool: Email Verification", body);
                return Ok(true);
            }
            else
            {

                return Ok(result.Errors.First().Description);
            }

        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult FirstLineManagerRegister(FirstLineManagerRegisterModel obj)
        {
            ExtendIdentityUser u = new ExtendIdentityUser();
            u.Email = obj.Email;
            u.UserName = obj.Email;
            u.FullName = obj.FullName;
            u.PhoneNumber = obj.PhoneNumber;
            u.CityId = obj.CityId;
            u.SecurityStamp = Guid.NewGuid().ToString();
            u.EmailConfirmed = true;
            var result = userManager.CreateAsync(u, obj.Password).Result;
            var addtorole = userManager.AddToRoleAsync(u, obj.RoleName).Result;

            if (result.Succeeded)
            {
                ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
                string token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                //465
                SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
                string body = "A new account was created by AME reporting tool system admin with your email address \n\nClick Here to verify your account\n https://{{DashboardURL}}/emailconfirmed.html?" + token + "&" + obj.Email;
                smtp.Send(EmailModel.EmailAddress, obj.Email, "AME Reporting Tool: Email Verification", body);
                return Ok(true);
            }
            else
            {

                return Ok(result.Errors.First().Description);
            }

        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult MedicalRegister(MedicalRegisterModel obj)
        {


            ExtendIdentityUser u = new ExtendIdentityUser();
            u.Email = obj.Email;
            u.UserName = obj.Email;
            u.FullName = obj.FullName;
            u.PhoneNumber = obj.PhoneNumber;
            u.CityId = obj.CityId;
            u.extendidentityuserid = obj.ManagerId;
            u.SecurityStamp = Guid.NewGuid().ToString();
            u.EmailConfirmed = true;
            var result = userManager.CreateAsync(u, obj.Password).Result;
            var addtorole = userManager.AddToRoleAsync(u, obj.RoleName).Result;

            if (result.Succeeded)
            {
                ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
                string token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                //465
                SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
                string body = "A new account was created by ame reporting tool system admin with your email address \n\nClick Here to verify your account\n https://{{DashboardURL}}/emailconfirmed.html?" + token + "&" + obj.Email;
                smtp.Send(EmailModel.EmailAddress, obj.Email, "AME Reporting Tool: Email Verification", body);

                UserVisitsLimit v = new UserVisitsLimit();
                v.ExtendIdentityUserId = user.Id;
                v.Days = 3;
                db.usersVisitsLimit.Add(v);
                db.SaveChanges();
                

                return Ok(true);
            }
            else
            {
                return Ok(result.Errors.First().Description);
            }

        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult SalesRegister(SalesRegisterModel obj)
        {

            ExtendIdentityUser u = new ExtendIdentityUser();
            u.Email = obj.Email;
            u.UserName = obj.Email;
            u.FullName = obj.FullName;
            u.PhoneNumber = obj.PhoneNumber;
            u.CityId = obj.CityId;
            u.extendidentityuserid = obj.ManagerId;
            u.SecurityStamp = Guid.NewGuid().ToString();
            u.EmailConfirmed = true;
            var result = userManager.CreateAsync(u, obj.Password).Result;
            var addtorole = userManager.AddToRoleAsync(u, obj.RoleName).Result;

            if (result.Succeeded && addtorole.Succeeded)
            {
                ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
                string token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                //465
                SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
                string body = "A new account was created by ame reporting tool system admin with your email address \n\nClick Here to verify your account\n https://{{DashboardURL}}/emailconfirmed.html?" + token + "&" + obj.Email;
                smtp.Send(EmailModel.EmailAddress, obj.Email, "AME Reporting Tool: Email Verification", body);

                UserVisitsLimit v = new UserVisitsLimit();
                v.ExtendIdentityUserId = user.Id;
                v.Days = 3;
                db.usersVisitsLimit.Add(v);
                db.SaveChanges();

                return Ok(true);
            }
            else
            {
                return Ok(result.Errors.First().Description);
            }
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult SupportiveRegister(SupportiveRegisterModel obj)
        {
            
            ExtendIdentityUser u = new ExtendIdentityUser();
            u.Email = obj.Email;
            u.UserName = obj.Email;
            u.FullName = obj.FullName;
            u.PhoneNumber = obj.PhoneNumber;
            u.CityId = obj.CityId;
            u.extendidentityuserid = obj.ManagerId;
            u.SecurityStamp = Guid.NewGuid().ToString();
            u.EmailConfirmed = true;
            var result = userManager.CreateAsync(u, obj.Password).Result;
            var addtorole = userManager.AddToRoleAsync(u, obj.RoleName).Result;

            if (result.Succeeded && addtorole.Succeeded)
            {
                ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
                string token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                //465
                SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
                string body = "A new account was created by ame reporting tool system admin with your email address \n\nClick Here to verify your account\n https://{{DashboardURL}}/emailconfirmed.html?" + token + "&" + obj.Email;
                smtp.Send(EmailModel.EmailAddress, obj.Email, "AME Reporting Tool: Email Verification", body);

                UserVisitsLimit v = new UserVisitsLimit();
                v.ExtendIdentityUserId = user.Id;
                v.Days = 3;
                db.usersVisitsLimit.Add(v);
                db.SaveChanges();

                return Ok(true);
            }
            else
            {
                return Ok(result.Errors.First().Description);
            }
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult MaintenanceRegister(MaintenanceRegisterModel obj)
        {
            ExtendIdentityUser u = new ExtendIdentityUser();
            u.Email = obj.Email;
            u.UserName = obj.Email;
            u.FullName = obj.FullName;
            u.PhoneNumber = obj.PhoneNumber;
            u.SecurityStamp = Guid.NewGuid().ToString();
            u.EmailConfirmed = true;
            var result = userManager.CreateAsync(u, obj.Password).Result;
            var addtorole = userManager.AddToRoleAsync(u, obj.RoleName).Result;

            if (result.Succeeded)
            {
                ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
                string token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                //465
                SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
                string body = "A new account was created by ame reporting tool system admin with your email address \n\nClick Here to verify your account\n https://{{DashboardURL}}/emailconfirmed.html?" + token + "&" + obj.Email;
                smtp.Send(EmailModel.EmailAddress, obj.Email, "AME Reporting Tool: Email Verification", body);
                return Ok(true);
            }
            else
            {

                return Ok(result.Errors.First().Description);
            }

        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult HrRegister(HRRegisterationModel obj)
        {
            ExtendIdentityUser u = new ExtendIdentityUser();
            u.Email = obj.Email;
            u.UserName = obj.Email;
            u.FullName = obj.FullName;
            u.PhoneNumber = obj.PhoneNumber;
            u.SecurityStamp = Guid.NewGuid().ToString();
            u.EmailConfirmed = true;
            var result = userManager.CreateAsync(u, obj.Password).Result;
            var addtorole = userManager.AddToRoleAsync(u, obj.RoleName).Result;

            if (result.Succeeded)
            {
                ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
                string token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                //465
                SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
                string body = "A new account was created by ame reporting tool system admin with your email address \n\nClick Here to verify your account\n https://{{DashboardURL}}/emailconfirmed.html?" + token + "&" + obj.Email;
                smtp.Send(EmailModel.EmailAddress, obj.Email, "AME Reporting Tool: Email Verification", body);
                return Ok(true);
            }
            else
            {

                return Ok(result.Errors.First().Description);
            }

        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult SemiSystemAdminRegister(SemiSystemAdminRegisterModel obj)
        {
            ExtendIdentityUser u = new ExtendIdentityUser();
            u.Email = obj.Email;
            u.UserName = obj.Email;
            u.FullName = obj.FullName;
            u.PhoneNumber = obj.PhoneNumber;
            u.SecurityStamp = Guid.NewGuid().ToString();
            u.EmailConfirmed = true;
            var result = userManager.CreateAsync(u, obj.Password).Result;
            var addtorole = userManager.AddToRoleAsync(u, obj.RoleName).Result;

            if (result.Succeeded)
            {
                ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
                string token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                //465
                SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
                string body = "A new account was created by ame reporting tool system admin with your email address \n\nClick Here to verify your account\n https://{{DashboardURL}}/emailconfirmed.html?" + token + "&" + obj.Email;
                smtp.Send(EmailModel.EmailAddress, obj.Email, "AME Reporting Tool: Email Verification", body);
                return Ok(true);
            }
            else
            {

                return Ok(result.Errors.First().Description);
            }

        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AccountingManagerRegister(AccountingManagerRegisterModel obj)
        {
            ExtendIdentityUser u = new ExtendIdentityUser();
            u.Email = obj.Email;
            u.UserName = obj.Email;
            u.FullName = obj.FullName;
            u.PhoneNumber = obj.PhoneNumber;
            u.SecurityStamp = Guid.NewGuid().ToString();
            u.EmailConfirmed = true;
            var result = userManager.CreateAsync(u, obj.Password).Result;
            var addtorole = userManager.AddToRoleAsync(u, obj.RoleName).Result;

            if (result.Succeeded)
            {
                ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
                string token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                //465
                SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
                string body = "A new account was created by AME reporting tool system admin with your email address \n\nClick Here to verify your account\n https://{{DashboardURL}}/emailconfirmed.html?" + token + "&" + obj.Email;
                smtp.Send(EmailModel.EmailAddress, obj.Email, "AME Reporting Tool: Email Verification", body);
                return Ok(true);
            }
            else
            {

                return Ok(result.Errors.First().Description);
            }

        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AccountantRegister(AccountantRegisterModel obj)
        {
            ExtendIdentityUser u = new ExtendIdentityUser();
            u.Email = obj.Email;
            u.UserName = obj.Email;
            u.FullName = obj.FullName;
            u.PhoneNumber = obj.PhoneNumber;
            u.CityId = obj.CityId;
            u.SecurityStamp = Guid.NewGuid().ToString();
            u.EmailConfirmed = true;
            var result = userManager.CreateAsync(u, obj.Password).Result;
            var addtorole = userManager.AddToRoleAsync(u, obj.RoleName).Result;

            if (result.Succeeded)
            {
                ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
                string token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                //465
                SmtpClient smtp = new SmtpClient(EmailModel.SmtpServer, EmailModel.port);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential(EmailModel.EmailAddress, EmailModel.Password);
                string body = "A new account was created by AME reporting tool system admin with your email address \n\nClick Here to verify your account\n https://{{DashboardURL}}/emailconfirmed.html?" + token + "&" + obj.Email;
                smtp.Send(EmailModel.EmailAddress, obj.Email, "AME Reporting Tool: Email Verification", body);
                return Ok(true);
            }
            else
            {

                return Ok(result.Errors.First().Description);
            }

        }

        [Route("[controller]/[Action]/{userId?}")]
        [HttpDelete("{userId?}")]
        public IActionResult DeleteUser(string userId)
        {

            ExtendIdentityUser user = userManager.FindByIdAsync(userId).Result;
            string role = userManager.GetRolesAsync(user).Result.First();

            if (role == "first line manager")
            {
                IEnumerable<ExtendIdentityUser> representatives = userManager.Users.Where(a => a.extendidentityuserid == user.Id);

                foreach (var item in representatives)
                {
                    item.extendidentityuserid = null;
                }
            }

            IdentityResult result = userManager.DeleteAsync(user).Result;

            if (result.Succeeded)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }

        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult CountUsersByRole()
        {
            return Ok(userRep.usersRolesCount());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult CountUsersByCity()
        {
            return Ok(userRep.usersCitiesCount());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            IEnumerable<CustomUsers> res = userRep.GetAllUsers();
            return Ok(res);
        }

        [Route("[controller]/[Action]/{rolename}")]
        [HttpGet]
        public IActionResult GetUsersByRole(string roleName)
        {
            return Ok(userRep.GetUserByRole(roleName));
        }


        [Route("[controller]/[Action]/{userId?}")]
        [HttpGet("{userId?}")]
        public IActionResult GetUserLinkedAccounts(string userId)
        {
            return Ok(userRep.GetUserLinkedAccounts(userId));
        }

        [Route("[controller]/[Action]/{userId?}")]
        [HttpGet("{userId?}")]
        public IActionResult GetUserUnlinkedAccounts(string userId)
        {
            return Ok(userRep.GetUserUnlinkedAccounts(userId));
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult LinkUserWithAccount(UserAccountModel obj)
        {
            return Ok(userRep.LinkUserWithAccount(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult UnlinkUserWithAccount(UserAccountModel obj)
        {
            return Ok(userRep.UnlinkUserWithAccount(obj));
        }


        [Route("[controller]/[Action]/{userId?}")]
        [HttpGet("{userId?}")]
        public IActionResult GetUserLinkedContacts(string userId)
        {
            return Ok(userRep.GetUserLinkedContacts(userId));
        }

        [Route("[controller]/[Action]/{userId?}")]
        [HttpGet("{userId?}")]
        public IActionResult GetUserUnlinkedContacts(string userId)
        {
            return Ok(userRep.GetUserUnlinkedContacts(userId));
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult LinkUserWithContact(UserContactModel obj)
        {
            return Ok(userRep.LinkUserWithContact(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult UnlinkUserWithContact(UserContactModel obj)
        {
            return Ok(userRep.UnlinkUserWithContact(obj));
        }


        [Route("[controller]/[Action]/{userId?}")]
        [HttpGet("{userId?}")]
        public IActionResult GetUserLinkedBrands(string userId)
        {
            return Ok(userRep.GetUserLinkedBrands(userId));
        }

        [Route("[controller]/[Action]/{userId?}")]
        [HttpGet("{userId?}")]
        public IActionResult GetUserUnlinkedBrands(string userId)
        {
            return Ok(userRep.GetUserUnlinkedBrands(userId));
        }


        [Route("[controller]/[Action]/{userId?}/{brandId?}")]
        [HttpGet]
        public IActionResult LinkUserWithBrand(string userId, int brandId)
        {
            return Ok(userRep.LinkUserWithBrand(userId, brandId));
        }

        [Route("[controller]/[Action]/{userId?}/{brandId?}")]
        [HttpGet]
        public IActionResult UnlinkUserWithBrand(string userId, int brandId)
        {
            return Ok(userRep.UnlinkUserWithBrand(userId, brandId));
        }

        [Route("[controller]/[Action]/{userId}")]
        [HttpGet]
        public IActionResult DeactivateUser(string userId)
        {
            return Ok(userRep.DeactivateUser(userId));
        }

        [Route("[controller]/[Action]/{userId?}")]
        [HttpGet]
        public IActionResult ActivateUser(string userId)
        {
            return Ok(userRep.ActivateUser(userId));
        }

        [Route("[controller]/[Action]/{userId?}")]
        [HttpGet("{userId?}")]
        public IActionResult GetUserLinkedAccountsExcel(string userId)
        {
            string name = userManager.FindByIdAsync(userId).Result.FullName;
           
            IEnumerable<Account> accounts = userRep.GetUserLinkedAccounts(userId);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Accounts");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Represntative Name";
            worksheet.Cell(currentRow, 2).Value = "Account";
            currentRow = 2;
            foreach (var acc in accounts)
            {
                worksheet.Cell(currentRow, 1).Value = name;
                worksheet.Cell(currentRow, 2).Value = acc.AccountName;
                currentRow++;
            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + "'s Accounts.xlsx");

        }

        [Route("[controller]/[Action]/{userId?}")]
        [HttpGet("{userId?}")]
        public IActionResult GetUserLinkedContactsExcel(string userId)
        {
            string name = userManager.FindByIdAsync(userId).Result.FullName;

            IEnumerable<Contact> contacts = userRep.GetUserLinkedContacts(userId);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Contacts");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Represntative Name";
            worksheet.Cell(currentRow, 2).Value = "Contact";
            currentRow = 2;
            foreach (var con in contacts)
            {
                worksheet.Cell(currentRow, 1).Value = name;
                worksheet.Cell(currentRow, 2).Value = con.ContactName;
                currentRow++;
            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + "'s Contacts.xlsx");
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult AllSalesOrgExcel()
        {
            XLWorkbook workbook = new XLWorkbook();

            IEnumerable<AccountsPerUserModel> list = rep.GetAllSalesAccounts();

            foreach (var res in list)
            {
                string[] n = res.FullName.Split(' ');
                var worksheet = workbook.Worksheets.Add(n[0]+" "+n[1]+" Accounts");
                worksheet.Range("A1:j300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A1:j300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:j300").Style.Font.Bold = true;
                worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Columns(1, 50).Width = 15;
                worksheet.Rows(1, 500).Height = 25;
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Account Name";
                worksheet.Cell(currentRow, 2).Value = "Category";
                worksheet.Cell(currentRow, 3).Value = "Account Type";
                worksheet.Cell(currentRow, 4).Value = "District";
                worksheet.Cell(currentRow, 5).Value = "Address";
                worksheet.Cell(currentRow, 6).Value = "Phone Number";
                worksheet.Cell(currentRow, 7).Value = "Purchase Type";
                worksheet.Cell(currentRow, 8).Value = "Email";
                worksheet.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
                currentRow = 2;

                foreach (var item in res.Accounts)
                {
                    worksheet.Cell(currentRow, 1).Value = item.AccountName;
                    worksheet.Cell(currentRow, 2).Value = item.CategoryName;
                    worksheet.Cell(currentRow, 3).Value = item.AccountTypeName;
                    worksheet.Cell(currentRow, 4).Value = item.DistrictName;
                    worksheet.Cell(currentRow, 5).Value = item.Address;
                    worksheet.Cell(currentRow, 6).Value = item.PhoneNumber;
                    worksheet.Cell(currentRow, 7).Value = item.PurchaseTypeName;
                    worksheet.Cell(currentRow, 8).Value = item.Email;
                    currentRow++;
                }
            }
            workbook.Worksheets.OrderBy(a => a.Name);

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 "All Sales Representatives Accounts.xlsx");
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult AllSupportiveOrgExcel()
        {
            XLWorkbook workbook = new XLWorkbook();

            IEnumerable<AccountsPerUserModel> list = rep.GetAllSupportiveAccounts();

            foreach (var res in list)
            {
                string[] n = res.FullName.Split(' ');
                var worksheet = workbook.Worksheets.Add(n[0] + " " + n[1] + " Accounts");
                worksheet.Range("A1:j300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A1:j300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:j300").Style.Font.Bold = true;
                worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Columns(1, 50).Width = 15;
                worksheet.Rows(1, 500).Height = 25;
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Account Name";
                worksheet.Cell(currentRow, 2).Value = "Category";
                worksheet.Cell(currentRow, 3).Value = "Account Type";
                worksheet.Cell(currentRow, 4).Value = "District";
                worksheet.Cell(currentRow, 5).Value = "Address";
                worksheet.Cell(currentRow, 6).Value = "Phone Number";
                worksheet.Cell(currentRow, 7).Value = "Purchase Type";
                worksheet.Cell(currentRow, 8).Value = "Email";
                worksheet.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
                currentRow = 2;

                foreach (var item in res.Accounts)
                {
                    worksheet.Cell(currentRow, 1).Value = item.AccountName;
                    worksheet.Cell(currentRow, 2).Value = item.CategoryName;
                    worksheet.Cell(currentRow, 3).Value = item.AccountTypeName;
                    worksheet.Cell(currentRow, 4).Value = item.DistrictName;
                    worksheet.Cell(currentRow, 5).Value = item.Address;
                    worksheet.Cell(currentRow, 6).Value = item.PhoneNumber;
                    worksheet.Cell(currentRow, 7).Value = item.PurchaseTypeName;
                    worksheet.Cell(currentRow, 8).Value = item.Email;
                    currentRow++;
                }
            }
            workbook.Worksheets.OrderBy(a => a.Name);

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 "All Supportive Representatives Accounts.xlsx");
        }

        [Route("[controller]/[Action]/{ManagerId}")]
        [HttpGet]
        public IActionResult TeamSalesOrgExcel(string ManagerId)
        {
            string name = userManager.FindByIdAsync(ManagerId).Result.FullName;
            XLWorkbook workbook = new XLWorkbook();

            IEnumerable<AccountsPerUserModel> list = rep.GetTeamSalesAccounts(ManagerId);

            foreach (var res in list)
            {
                string[] n = res.FullName.Split(' ');
                var worksheet = workbook.Worksheets.Add(n[0] + " " + n[1] + " Accounts");
                worksheet.Range("A1:j300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A1:j300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:j300").Style.Font.Bold = true;
                worksheet.Columns(1, 50).Width = 15;
                worksheet.Rows(1, 500).Height = 25;
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Account Name";
                worksheet.Cell(currentRow, 2).Value = "Category";
                worksheet.Cell(currentRow, 3).Value = "Account Type";
                worksheet.Cell(currentRow, 4).Value = "District";
                worksheet.Cell(currentRow, 5).Value = "Address";
                worksheet.Cell(currentRow, 6).Value = "Phone Number";
                worksheet.Cell(currentRow, 7).Value = "Purchase Type";
                worksheet.Cell(currentRow, 8).Value = "Email";
                worksheet.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
                currentRow = 2;

                foreach (var item in res.Accounts)
                {
                    worksheet.Cell(currentRow, 1).Value = item.AccountName;
                    worksheet.Cell(currentRow, 2).Value = item.CategoryName;
                    worksheet.Cell(currentRow, 3).Value = item.AccountTypeName;
                    worksheet.Cell(currentRow, 4).Value = item.DistrictName;
                    worksheet.Cell(currentRow, 5).Value = item.Address;
                    worksheet.Cell(currentRow, 6).Value = item.PhoneNumber;
                    worksheet.Cell(currentRow, 7).Value = item.PurchaseTypeName;
                    worksheet.Cell(currentRow, 8).Value = item.Email;
                    currentRow++;
                }
            }

            workbook.Worksheets.OrderBy(a => a.Name);
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 name+"'s Sales Team Accounts.xlsx");
        }

        [Route("[controller]/[Action]/{ManagerId}")]
        [HttpGet]
        public IActionResult TeamSupportiveOrgExcel(string ManagerId)
        {
            string name = userManager.FindByIdAsync(ManagerId).Result.FullName;
            XLWorkbook workbook = new XLWorkbook();

            IEnumerable<AccountsPerUserModel> list = rep.GetTeamSupportiveAccounts(ManagerId);

            foreach (var res in list)
            {
                string[] n = res.FullName.Split(' ');
                var worksheet = workbook.Worksheets.Add(n[0] + " " + n[1] + " Accounts");
                worksheet.Range("A1:j300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A1:j300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:j300").Style.Font.Bold = true;
                worksheet.Columns(1, 50).Width = 15;
                worksheet.Rows(1, 500).Height = 25;
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Account Name";
                worksheet.Cell(currentRow, 2).Value = "Category";
                worksheet.Cell(currentRow, 3).Value = "Account Type";
                worksheet.Cell(currentRow, 4).Value = "District";
                worksheet.Cell(currentRow, 5).Value = "Address";
                worksheet.Cell(currentRow, 6).Value = "Phone Number";
                worksheet.Cell(currentRow, 7).Value = "Purchase Type";
                worksheet.Cell(currentRow, 8).Value = "Email";
                worksheet.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
                currentRow = 2;

                foreach (var item in res.Accounts)
                {
                    worksheet.Cell(currentRow, 1).Value = item.AccountName;
                    worksheet.Cell(currentRow, 2).Value = item.CategoryName;
                    worksheet.Cell(currentRow, 3).Value = item.AccountTypeName;
                    worksheet.Cell(currentRow, 4).Value = item.DistrictName;
                    worksheet.Cell(currentRow, 5).Value = item.Address;
                    worksheet.Cell(currentRow, 6).Value = item.PhoneNumber;
                    worksheet.Cell(currentRow, 7).Value = item.PurchaseTypeName;
                    worksheet.Cell(currentRow, 8).Value = item.Email;
                    currentRow++;
                }
            }

            workbook.Worksheets.OrderBy(a => a.Name);
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 name + "'s Sales Team Accounts.xlsx");
        }

        [Route("[controller]/[Action]/{UserId}")]
        [HttpGet]
        public IActionResult SalesOrgExcel(string UserId)
        {
            XLWorkbook workbook = new XLWorkbook();

            AccountsPerUserModel list = rep.GetRepAccounts(UserId);


                string[] n = list.FullName.Split(' ');
                var worksheet = workbook.Worksheets.Add(n[0] + " " + n[1] + " Accounts");
            worksheet.Range("A1:j300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A1:j300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:j300").Style.Font.Bold = true;
            worksheet.Columns(1, 50).Width = 15;
            worksheet.Rows(1, 500).Height = 25;
            var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Account Name";
                worksheet.Cell(currentRow, 2).Value = "Category";
                worksheet.Cell(currentRow, 3).Value = "Account Type";
                worksheet.Cell(currentRow, 4).Value = "District";
                worksheet.Cell(currentRow, 5).Value = "Address";
                worksheet.Cell(currentRow, 6).Value = "Phone Number";
                worksheet.Cell(currentRow, 7).Value = "Purchase Type";
                worksheet.Cell(currentRow, 8).Value = "Email";
            worksheet.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
            currentRow = 2;

                foreach (var item in list.Accounts)
                {
                    worksheet.Cell(currentRow, 1).Value = item.AccountName;
                    worksheet.Cell(currentRow, 2).Value = item.CategoryName;
                    worksheet.Cell(currentRow, 3).Value = item.AccountTypeName;
                    worksheet.Cell(currentRow, 4).Value = item.DistrictName;
                    worksheet.Cell(currentRow, 5).Value = item.Address;
                    worksheet.Cell(currentRow, 6).Value = item.PhoneNumber;
                    worksheet.Cell(currentRow, 7).Value = item.PurchaseTypeName;
                    worksheet.Cell(currentRow, 8).Value = item.Email;
                currentRow++;
            }
           
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 list.FullName+"'s Accounts.xlsx");
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult UnAssignedAccountandContactsExcel()
        {
            XLWorkbook workbook = new XLWorkbook();
            
            IEnumerable<CustomAccount> acc = rep.GetUnAssignedAccounts();
            var accsheet = workbook.Worksheets.Add("Accounts");
            accsheet.Range("A1:j300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            accsheet.Range("A1:j300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            accsheet.Range("A1:j300").Style.Font.Bold = true;
            accsheet.Columns(1, 50).Width = 15;
            accsheet.Rows(1, 500).Height = 25;
            var currentRow = 1;
            accsheet.Cell(currentRow, 1).Value = "Account Name";
            accsheet.Cell(currentRow, 2).Value = "Category";
            accsheet.Cell(currentRow, 3).Value = "Account Type";
            accsheet.Cell(currentRow, 4).Value = "District";
            accsheet.Cell(currentRow, 5).Value = "Address";
            accsheet.Cell(currentRow, 6).Value = "Phone Number";
            accsheet.Cell(currentRow, 7).Value = "Purchase Type";
            accsheet.Cell(currentRow, 8).Value = "Email";
            accsheet.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
            currentRow = 2;

            foreach (var a in acc)
            {
                accsheet.Cell(currentRow, 1).Value = a.AccountName;
                accsheet.Cell(currentRow, 2).Value = a.CategoryName;
                accsheet.Cell(currentRow, 3).Value = a.AccountTypeName;
                accsheet.Cell(currentRow, 4).Value = a.DistrictName;
                accsheet.Cell(currentRow, 5).Value = a.Address;
                accsheet.Cell(currentRow, 6).Value = a.PhoneNumber;
                accsheet.Cell(currentRow, 7).Value = a.PurchaseTypeName;
                accsheet.Cell(currentRow, 8).Value = a.Email;
                currentRow++;
            }


            IEnumerable<CustomContact> con = rep.GetUnAssignedContacts();
            var consheet = workbook.Worksheets.Add("Contacts");
            consheet.Range("A1:K300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            consheet.Range("A1:K300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            consheet.Range("A1:K300").Style.Font.Bold = true;
            consheet.Columns(1, 50).Width = 15;
            consheet.Rows(1, 500).Height = 25;
            var row = 1;
            consheet.Cell(row, 1).Value = "Contact Name";
            consheet.Cell(row, 2).Value = "Category";
            consheet.Cell(row, 3).Value = "Account Affiliation";
            consheet.Cell(row, 4).Value = "Contact Type";
            consheet.Cell(row, 5).Value = "Gender";
            consheet.Cell(row, 6).Value = "District";
            consheet.Cell(row, 7).Value = "Address";
            consheet.Cell(row, 8).Value = "Land Line Number";
            consheet.Cell(row, 9).Value = "Mobile Number";
            consheet.Cell(row, 10).Value = "Purchase Type";
            consheet.Cell(row, 11).Value = "Email";
            consheet.Range("A1:K1").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
            row = 2;
            foreach (var item in con)
            {
                consheet.Cell(row, 1).Value = item.ContactName;
                consheet.Cell(row, 2).Value = item.CategoryName;
                consheet.Cell(row, 3).Value = item.AccountName;
                consheet.Cell(row, 4).Value = item.ContactTypeName;
                string gender = "";
                if (item.Gender == false)
                {
                    gender = "Male";
                }
                else
                {
                    if (item.Gender == true)
                    {
                        gender = "Female";
                    }
                    else
                    {
                        gender = "Not Set";
                    }
                }


                consheet.Cell(row, 5).Value = gender;
                consheet.Cell(row, 6).Value = item.DistrictName;
                consheet.Cell(row, 7).Value = item.Address;
                consheet.Cell(row, 8).Value = item.LandLineNumber;
                consheet.Cell(row, 9).Value = item.MobileNumber;
                consheet.Cell(row, 10).Value = item.PurchaseTypeName;
                consheet.Cell(row, 11).Value = item.Email;
                row++;
            }
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 "Unassigned Accounts & Contacts.xlsx");

        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult AllMedicalOrgExcel()
        {
            XLWorkbook workbook = new XLWorkbook();

            IEnumerable<AccountsPerUserModel> list = rep.GetAllMedicalAccounts();

            foreach (var res in list)
            {
                string[] n = res.FullName.Split(' ');
                var worksheet = workbook.Worksheets.Add(n[0] + " " + n[1] + " Accounts");
                worksheet.Range("A1:j300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A1:j300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:j300").Style.Font.Bold = true;
                worksheet.Columns(1, 50).Width = 15;
                worksheet.Rows(1, 500).Height = 25;
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Account Name";
                worksheet.Cell(currentRow, 2).Value = "Category";
                worksheet.Cell(currentRow, 3).Value = "Account Type";
                worksheet.Cell(currentRow, 4).Value = "District";
                worksheet.Cell(currentRow, 5).Value = "Address";
                worksheet.Cell(currentRow, 6).Value = "Phone Number";
                worksheet.Cell(currentRow, 7).Value = "Purchase Type";
                worksheet.Cell(currentRow, 8).Value = "Email";
                worksheet.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
                currentRow = 2;

                foreach (var item in res.Accounts)
                {
                    worksheet.Cell(currentRow, 1).Value = item.AccountName;
                    worksheet.Cell(currentRow, 2).Value = item.CategoryName;
                    worksheet.Cell(currentRow, 3).Value = item.AccountTypeName;
                    worksheet.Cell(currentRow, 4).Value = item.DistrictName;
                    worksheet.Cell(currentRow, 5).Value = item.Address;
                    worksheet.Cell(currentRow, 6).Value = item.PhoneNumber;
                    worksheet.Cell(currentRow, 7).Value = item.PurchaseTypeName;
                    worksheet.Cell(currentRow, 8).Value = item.Email;
                    currentRow++;
                }
            }

            IEnumerable<ContactsPerUserModel> listt = rep.GetAllMedicalContacts();
            foreach (var res in listt)
            {
                string[] n = res.FullName.Split(' ');
                var worksheet = workbook.Worksheets.Add(n[0] + " " + n[1] + " Contacts");
                worksheet.Range("A1:K300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A1:K300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:K300").Style.Font.Bold = true;
                worksheet.Columns(1, 50).Width = 15;
                worksheet.Rows(1, 500).Height = 25;
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Contact Name";
                worksheet.Cell(currentRow, 2).Value = "Category";
                worksheet.Cell(currentRow, 3).Value = "Account Affiliation";
                worksheet.Cell(currentRow, 4).Value = "Contact Type";
                worksheet.Cell(currentRow, 5).Value = "Gender";
                worksheet.Cell(currentRow, 6).Value = "District";
                worksheet.Cell(currentRow, 7).Value = "Address";
                worksheet.Cell(currentRow, 8).Value = "Land Line Number";
                worksheet.Cell(currentRow, 9).Value = "Mobile Number";
                worksheet.Cell(currentRow, 10).Value = "Purchase Type";
                worksheet.Cell(currentRow, 11).Value = "Email";
                worksheet.Range("A1:K1").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
                currentRow = 2;
                foreach (var item in res.Contacts)
                {
                    worksheet.Cell(currentRow, 1).Value = item.ContactName;
                    worksheet.Cell(currentRow, 2).Value = item.CategoryName;
                    worksheet.Cell(currentRow, 3).Value = item.AccountName;
                    worksheet.Cell(currentRow, 4).Value = item.ContactTypeName;
                    string gender = "";
                    if (item.Gender == false)
                    {
                        gender = "Male";
                    }
                    else
                    {
                        if (item.Gender == true)
                        {
                            gender = "Female";
                        }
                        else
                        {
                            gender = "Not Set";
                        }
                    }
                  
                    
                    worksheet.Cell(currentRow, 5).Value = gender;
                    worksheet.Cell(currentRow, 6).Value = item.DistrictName;
                    worksheet.Cell(currentRow, 7).Value = item.Address;
                    worksheet.Cell(currentRow, 8).Value = item.LandLineNumber;
                    worksheet.Cell(currentRow, 9).Value = item.MobileNumber;
                    worksheet.Cell(currentRow, 10).Value = item.PurchaseTypeName;
                    worksheet.Cell(currentRow, 11).Value = item.Email;
                    currentRow++;
                }
            }
      

            workbook.Worksheets.OrderBy(a => a.Name);

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 "All Medical Representatives Accounts & Contacts.xlsx");
        }

        [Route("[controller]/[Action]/{ManagerId}")]
        [HttpGet]
        public IActionResult TeamMedicalOrgExcel(string ManagerId)
        {
            string name = userManager.FindByIdAsync(ManagerId).Result.FullName;

            XLWorkbook workbook = new XLWorkbook();

            IEnumerable<AccountsPerUserModel> list = rep.GetTeamAccounts(ManagerId);

            foreach (var res in list)
            {
                string[] n = res.FullName.Split(' ');
                var worksheet = workbook.Worksheets.Add(n[0] + " " + n[1] + " Accounts");
                worksheet.Range("A1:j300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A1:j300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:j300").Style.Font.Bold = true;
                worksheet.Columns(1, 50).Width = 15;
                worksheet.Rows(1, 500).Height = 25;
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Account Name";
                worksheet.Cell(currentRow, 2).Value = "Category";
                worksheet.Cell(currentRow, 3).Value = "Account Type";
                worksheet.Cell(currentRow, 4).Value = "District";
                worksheet.Cell(currentRow, 5).Value = "Address";
                worksheet.Cell(currentRow, 6).Value = "Phone Number";
                worksheet.Cell(currentRow, 7).Value = "Purchase Type";
                worksheet.Cell(currentRow, 8).Value = "Email";
                worksheet.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
                currentRow = 2;

                foreach (var item in res.Accounts)
                {
                    worksheet.Cell(currentRow, 1).Value = item.AccountName;
                    worksheet.Cell(currentRow, 2).Value = item.CategoryName;
                    worksheet.Cell(currentRow, 3).Value = item.AccountTypeName;
                    worksheet.Cell(currentRow, 4).Value = item.DistrictName;
                    worksheet.Cell(currentRow, 5).Value = item.Address;
                    worksheet.Cell(currentRow, 6).Value = item.PhoneNumber;
                    worksheet.Cell(currentRow, 7).Value = item.PurchaseTypeName;
                    worksheet.Cell(currentRow, 8).Value = item.Email;
                    currentRow++;
                }
            }

            IEnumerable<ContactsPerUserModel> listt = rep.GetTeamContacts(ManagerId);
            foreach (var res in listt)
            {
                string[] n = res.FullName.Split(' ');
                var worksheet = workbook.Worksheets.Add(n[0] + " " + n[1] + " Contacts");
                worksheet.Range("A1:K300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A1:K300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:K300").Style.Font.Bold = true;
                worksheet.Columns(1, 50).Width = 15;
                worksheet.Rows(1, 500).Height = 25;
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Contact Name";
                worksheet.Cell(currentRow, 2).Value = "Category";
                worksheet.Cell(currentRow, 3).Value = "Account Affiliation";
                worksheet.Cell(currentRow, 4).Value = "Contact Type";
                worksheet.Cell(currentRow, 5).Value = "Gender";
                worksheet.Cell(currentRow, 6).Value = "District";
                worksheet.Cell(currentRow, 7).Value = "Address";
                worksheet.Cell(currentRow, 8).Value = "Land Line Number";
                worksheet.Cell(currentRow, 9).Value = "Mobile Number";
                worksheet.Cell(currentRow, 10).Value = "Purchase Type";
                worksheet.Cell(currentRow, 11).Value = "Email";
                worksheet.Range("A1:K1").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
                currentRow = 2;
                foreach (var item in res.Contacts)
                {
                    worksheet.Cell(currentRow, 1).Value = item.ContactName;
                    worksheet.Cell(currentRow, 2).Value = item.CategoryName;
                    worksheet.Cell(currentRow, 3).Value = item.AccountName;
                    worksheet.Cell(currentRow, 4).Value = item.ContactTypeName;
                    string gender = "";
                    if (item.Gender == false)
                    {
                        gender = "Male";
                    }
                    else
                    {
                        if (item.Gender == true)
                        {
                            gender = "Female";
                        }
                        else
                        {
                            gender = "Not Set";
                        }
                    }


                    worksheet.Cell(currentRow, 5).Value = gender;
                    worksheet.Cell(currentRow, 6).Value = item.DistrictName;
                    worksheet.Cell(currentRow, 7).Value = item.Address;
                    worksheet.Cell(currentRow, 8).Value = item.LandLineNumber;
                    worksheet.Cell(currentRow, 9).Value = item.MobileNumber;
                    worksheet.Cell(currentRow, 10).Value = item.PurchaseTypeName;
                    worksheet.Cell(currentRow, 11).Value = item.Email;
                    currentRow++;
                }
            }


            workbook.Worksheets.OrderBy(a => a.Name);

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 name+"'s Medical Team Accounts & Contacts.xlsx");
        }

        [Route("[controller]/[Action]/{UserId}")]
        [HttpGet]
        public IActionResult MedicalOrgExcel(string UserId)
        {
            XLWorkbook workbook = new XLWorkbook();

            AccountsPerUserModel list = rep.GetRepAccounts(UserId);



            var worksheet = workbook.Worksheets.Add("Accounts");
            worksheet.Range("A1:j300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A1:j300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:j300").Style.Font.Bold = true;
            worksheet.Columns(1, 50).Width = 15;
            worksheet.Rows(1, 500).Height = 25;
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Account Name";
            worksheet.Cell(currentRow, 2).Value = "Category";
            worksheet.Cell(currentRow, 3).Value = "Account Type";
            worksheet.Cell(currentRow, 4).Value = "District";
            worksheet.Cell(currentRow, 5).Value = "Address";
            worksheet.Cell(currentRow, 6).Value = "Phone Number";
            worksheet.Cell(currentRow, 7).Value = "Purchase Type";
            worksheet.Cell(currentRow, 8).Value = "Email";
            worksheet.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
            currentRow = 2;

            foreach (var item in list.Accounts)
            {
                worksheet.Cell(currentRow, 1).Value = item.AccountName;
                worksheet.Cell(currentRow, 2).Value = item.CategoryName;
                worksheet.Cell(currentRow, 3).Value = item.AccountTypeName;
                worksheet.Cell(currentRow, 4).Value = item.DistrictName;
                worksheet.Cell(currentRow, 5).Value = item.Address;
                worksheet.Cell(currentRow, 6).Value = item.PhoneNumber;
                worksheet.Cell(currentRow, 7).Value = item.PurchaseTypeName;
                worksheet.Cell(currentRow, 8).Value = item.Email;
                currentRow++;
            }


            ContactsPerUserModel listt = rep.GetRepContacts(UserId);


            var worksheett = workbook.Worksheets.Add("Contacts");
            worksheett.Range("A1:K300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheett.Range("A1:K300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheett.Range("A1:K300").Style.Font.Bold = true;
            worksheett.Columns(1, 50).Width = 15;
            worksheett.Rows(1, 500).Height = 25;
            var currentRoww = 1;
            worksheett.Cell(currentRoww, 1).Value = "Contact Name";
            worksheett.Cell(currentRoww, 2).Value = "Category";
            worksheett.Cell(currentRoww, 3).Value = "Account Affiliation";
            worksheett.Cell(currentRoww, 4).Value = "Contact Type";
            worksheett.Cell(currentRoww, 5).Value = "Gender";
            worksheett.Cell(currentRoww, 6).Value = "District";
            worksheett.Cell(currentRoww, 7).Value = "Address";
            worksheett.Cell(currentRoww, 8).Value = "Land Line Number";
            worksheett.Cell(currentRoww, 9).Value = "Mobile Number";
            worksheett.Cell(currentRoww, 10).Value = "Purchase Type";
            worksheett.Cell(currentRoww, 11).Value = "Email";
            worksheett.Range("A1:K1").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
            currentRoww = 2;
            foreach (var item in listt.Contacts)
            {
                worksheett.Cell(currentRoww, 1).Value = item.ContactName;
                worksheett.Cell(currentRoww, 2).Value = item.CategoryName;
                worksheett.Cell(currentRoww, 3).Value = item.AccountName;
                worksheett.Cell(currentRoww, 4).Value = item.ContactTypeName;
                string gender = "";
                if (item.Gender == false)
                {
                    gender = "Male";
                }
                else
                {
                    if (item.Gender == true)
                    {
                        gender = "Female";
                    }
                    else
                    {
                        gender = "Not Set";
                    }
                }


                worksheett.Cell(currentRoww, 5).Value = gender;
                worksheett.Cell(currentRoww, 6).Value = item.DistrictName;
                worksheett.Cell(currentRoww, 7).Value = item.Address;
                worksheett.Cell(currentRoww, 8).Value = item.LandLineNumber;
                worksheett.Cell(currentRoww, 9).Value = item.MobileNumber;
                worksheett.Cell(currentRoww, 10).Value = item.PurchaseTypeName;
                worksheett.Cell(currentRoww, 11).Value = item.Email;
                currentRoww++;
            }



            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 list.FullName + "'s Accounts & Contacts.xlsx");
        }

    }
}

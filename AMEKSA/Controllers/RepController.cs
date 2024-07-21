using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.CustomEntities;
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
    public class RepController : ControllerBase
    {
        private readonly IRepRep rep;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public RepController(IRepRep rep,UserManager<ExtendIdentityUser> userManager)
        {
            this.rep = rep;
            this.userManager = userManager;
        }


        [Route("[controller]/[action]/{days}")]
        [HttpGet]
        public IActionResult SetAllRepsVisitLimit(int days)
        {
            return Ok(rep.SetAllRepsVisitLimit(days));
        }


        [Route("[controller]/[action]/{userId}")]
        [HttpGet]
        public IActionResult GetuserVisitLimit(string userId)
        {
            return Ok(rep.GetuserVisitLimit(userId));
        }

        [Route("[controller]/[action]/{userId}/{days}")]
        [HttpGet]
        public IActionResult EditVisitLimit(string userId, int days)
        {
            return Ok(rep.EditVisitLimit(userId, days));
        }

        [Route("[controller]/[action]/{year}/{month}/{userId}")]
        [HttpGet]
        public IActionResult VisitedAccountsByMonthSupportive(int year, int month, string userId)
        {
            return Ok(rep.VisitedAccountsByMonthSupportive(year,month,userId));
        }

        [Route("[controller]/[action]/{userId}")]
        [HttpGet]
        public IActionResult GetVisitsDateLimit(string userId)
        {
            return Ok(rep.GetVisitsDateLimit(userId));
        }

        [Route("[controller]/[action]/{year}/{month}/{userid}")]
        [HttpGet]
        public IActionResult VisitedContactsByMonth(int year, int month, string userId)
        {
            return Ok(rep.VisitedContactsByMonth(year,month,userId));
        }

        [Route("[controller]/[action]/{userid}")]
        [HttpGet]
        public IActionResult VisitedContactsThisMonthByCategory(string userid)
        {
            return Ok(rep.VisitedContactsThisMonthByCategory(userid));
        }

        [Route("[controller]/[Action]/{userId}/{dayfrom}/{monthfrom}/{yearfrom}/{dayto}/{monthto}/{yearto}")]
        [HttpGet]
        public IActionResult ContactMedicalVisitReportExcel(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {

            string name = userManager.FindByIdAsync(userId).Result.FullName;
            string startdate = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd-MM-yyyy");
            string enddate = new DateTime(yearto, monthto, dayto).ToString("dd-MM-yyyy");
            IEnumerable<RepContactVisitsReport> vis = rep.ContactMedicalVisitReport(userId, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visits Report");
            worksheet.Range("F1:I1").Merge();
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Contact";
            worksheet.Cell(currentRow, 2).Value = "Account Affiliation";
            worksheet.Cell(currentRow, 3).Value = "Category";
            worksheet.Cell(currentRow, 4).Value = "Visits Count";
            worksheet.Cell(currentRow, 5).Value = "Monthly Target";
            worksheet.Cell(currentRow, 6).Value = "Last Four Visits";
         
            currentRow = 2;
            foreach (var viss in vis)
            {

                worksheet.Cell(currentRow, 1).Value = viss.ContactName;
                worksheet.Cell(currentRow, 2).Value = viss.AccountName;
                worksheet.Cell(currentRow, 3).Value = viss.CategoryName;
                worksheet.Cell(currentRow, 4).Value = viss.VisitsCount;
                worksheet.Cell(currentRow, 5).Value = viss.MonthlyTarget;

                string LastVisit = viss.LastVisit.ToString("dd/MM/yyyy");
                string BeforeLastVisit = viss.BeforeLastVisit.ToString("dd/MM/yyyy");
                string BeforeBeforelastVisit = viss.BeforeBeforeLastVisit.ToString("dd/MM/yyyy");
                string BeforeBeforeBeforeLastVisit = viss.BeforeBeforeBeforeLastVisit.ToString("dd/MM/yyyy");
                if (LastVisit == "01/01/0001" || BeforeLastVisit == "01/01/0001" || BeforeBeforelastVisit == "01/01/0001" || BeforeBeforeBeforeLastVisit == "01/01/0001")
                {
                    if (LastVisit == "01/01/0001")
                    {
                        LastVisit = "No Visit";
                    }
                    else
                    {
                        LastVisit = viss.LastVisit.Date.ToString();
                    }
                    if (BeforeLastVisit == "01/01/0001")
                    {
                        BeforeLastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeLastVisit = viss.BeforeLastVisit.Date.ToString();
                    }
                    if (BeforeBeforelastVisit == "01/01/0001")
                    {
                        BeforeBeforelastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeBeforelastVisit = viss.BeforeBeforeLastVisit.Date.ToString();
                    }
                    if (BeforeBeforeBeforeLastVisit == "01/01/0001")
                    {
                        BeforeBeforeBeforeLastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeBeforeBeforeLastVisit = viss.BeforeBeforeBeforeLastVisit.Date.ToString();
                    }
                }

                worksheet.Cell(currentRow, 6).Value = LastVisit;
                worksheet.Cell(currentRow, 7).Value = BeforeLastVisit;
                worksheet.Cell(currentRow, 8).Value = BeforeBeforelastVisit;
                worksheet.Cell(currentRow, 9).Value = BeforeBeforeBeforeLastVisit;
                currentRow++;
            }

            IEnumerable<RepContactNotVisitsReport> notvis = rep.ContactMedicalNotVisitReport(userId, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            var secworksheet = workbook.Worksheets.Add("Not Visited");
            
            var row = 1;
            secworksheet.Cell(row, 1).Value = "Contact";
            secworksheet.Cell(row, 2).Value = "Account Affiliation";
            secworksheet.Cell(row, 3).Value = "Category";
            secworksheet.Cell(row, 4).Value = "Monthly Target";

            row = 2;

            foreach (var item in notvis)
            {
                secworksheet.Cell(row, 1).Value = item.ContactName;
                secworksheet.Cell(row, 2).Value = item.AccountName;
                secworksheet.Cell(row, 3).Value = item.CategoryName;
                secworksheet.Cell(row, 4).Value = item.MonthlyTarget;
                row++;
            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + " visits report to contacts from " + startdate + " to " + enddate + ".xlsx");

        }




        [Route("[controller]/[Action]/{userId}/{dayfrom}/{monthfrom}/{yearfrom}/{dayto}/{monthto}/{yearto}")]
        [HttpGet]
        public IActionResult AccountSupportiveVisitReportExcel(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {

            string name = userManager.FindByIdAsync(userId).Result.FullName;
            string startdate = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd-MM-yyyy");
            string enddate = new DateTime(yearto, monthto, dayto).ToString("dd-MM-yyyy");
            IEnumerable<RepAccountVisitsReport> vis = rep.AccountSupportiveVisitReport(userId, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visited");
            worksheet.Range("D1:G1").Merge();
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Account";
            worksheet.Cell(currentRow, 2).Value = "Category";
            worksheet.Cell(currentRow, 3).Value = "Visits Count";
            worksheet.Cell(currentRow, 4).Value = "Last Four Visits";



            currentRow = 2;
            foreach (var viss in vis)
            {

                worksheet.Cell(currentRow, 1).Value = viss.AccountName;
                worksheet.Cell(currentRow, 2).Value = viss.CategoryName;
                worksheet.Cell(currentRow, 3).Value = viss.VisitsCount;

                string LastVisit = viss.LastVisit.ToString("dd/MM/yyyy");
                string BeforeLastVisit = viss.BeforeLastVisit.ToString("dd/MM/yyyy");
                string BeforeBeforelastVisit = viss.BeforeBeforeLastVisit.ToString("dd/MM/yyyy");
                string BeforeBeforeBeforeLastVisit = viss.BeforeBeforeBeforeLastVisit.ToString("dd/MM/yyyy");
                if (LastVisit == "01/01/0001" || BeforeLastVisit == "01/01/0001" || BeforeBeforelastVisit == "01/01/0001" || BeforeBeforeBeforeLastVisit == "01/01/0001")
                {
                    if (LastVisit == "01/01/0001")
                    {
                        LastVisit = "No Visit";
                    }
                    else
                    {
                        LastVisit = viss.LastVisit.Date.ToString();
                    }
                    if (BeforeLastVisit == "01/01/0001")
                    {
                        BeforeLastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeLastVisit = viss.BeforeLastVisit.Date.ToString();
                    }
                    if (BeforeBeforelastVisit == "01/01/0001")
                    {
                        BeforeBeforelastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeBeforelastVisit = viss.BeforeBeforeLastVisit.Date.ToString();
                    }
                    if (BeforeBeforeBeforeLastVisit == "01/01/0001")
                    {
                        BeforeBeforeBeforeLastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeBeforeBeforeLastVisit = viss.BeforeBeforeBeforeLastVisit.Date.ToString();
                    }
                }
                worksheet.Cell(currentRow, 4).Value = LastVisit;
                worksheet.Cell(currentRow, 5).Value = BeforeLastVisit;
                worksheet.Cell(currentRow, 6).Value = BeforeBeforelastVisit;
                worksheet.Cell(currentRow, 7).Value = BeforeBeforeBeforeLastVisit;
                currentRow++;
            }


            var secworksheet = workbook.Worksheets.Add("Not Visited");
            IEnumerable<RepAccountNotVisitsReport> notvis = rep.AccountMedicalNotVisitReport(userId, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);
            var row = 1;
            secworksheet.Cell(row, 1).Value = "Account";
            secworksheet.Cell(row, 2).Value = "Category";

            row = 2;

            foreach (var item in notvis)
            {
                secworksheet.Cell(row, 1).Value = item.AccountName;
                secworksheet.Cell(row, 2).Value = item.CategoryName;
                row++;
            }


            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + " visits report to Accounts from " + startdate + " to " + enddate + ".xlsx");

        }


        [Route("[controller]/[Action]/{userId}/{dayfrom}/{monthfrom}/{yearfrom}/{dayto}/{monthto}/{yearto}")]
        [HttpGet]
        public IActionResult AccountMedicalVisitReportExcel(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {

            string name = userManager.FindByIdAsync(userId).Result.FullName;
            string startdate = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd-MM-yyyy");
            string enddate = new DateTime(yearto, monthto, dayto).ToString("dd-MM-yyyy");
            IEnumerable<RepAccountVisitsReport> vis = rep.AccountMedicalVisitReport(userId, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visited");
            worksheet.Range("D1:G1").Merge();
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Account";
            worksheet.Cell(currentRow, 2).Value = "Category";
            worksheet.Cell(currentRow, 3).Value = "Visits Count";
            worksheet.Cell(currentRow, 4).Value = "Last Four Visits";



            currentRow = 2;
            foreach (var viss in vis)
            {

                worksheet.Cell(currentRow, 1).Value = viss.AccountName;
                worksheet.Cell(currentRow, 2).Value = viss.CategoryName;
                worksheet.Cell(currentRow, 3).Value = viss.VisitsCount;

                string LastVisit = viss.LastVisit.ToString("dd/MM/yyyy");
                string BeforeLastVisit = viss.BeforeLastVisit.ToString("dd/MM/yyyy");
                string BeforeBeforelastVisit = viss.BeforeBeforeLastVisit.ToString("dd/MM/yyyy");
                string BeforeBeforeBeforeLastVisit = viss.BeforeBeforeBeforeLastVisit.ToString("dd/MM/yyyy");
                if (LastVisit == "01/01/0001" || BeforeLastVisit == "01/01/0001" || BeforeBeforelastVisit == "01/01/0001" || BeforeBeforeBeforeLastVisit == "01/01/0001")
                {
                    if (LastVisit == "01/01/0001")
                    {
                        LastVisit = "No Visit";
                    }
                    else
                    {
                        LastVisit = viss.LastVisit.Date.ToString();
                    }
                    if (BeforeLastVisit == "01/01/0001")
                    {
                        BeforeLastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeLastVisit = viss.BeforeLastVisit.Date.ToString();
                    }
                    if (BeforeBeforelastVisit == "01/01/0001")
                    {
                        BeforeBeforelastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeBeforelastVisit = viss.BeforeBeforeLastVisit.Date.ToString();
                    }
                    if (BeforeBeforeBeforeLastVisit == "01/01/0001")
                    {
                        BeforeBeforeBeforeLastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeBeforeBeforeLastVisit = viss.BeforeBeforeBeforeLastVisit.Date.ToString();
                    }
                }
                worksheet.Cell(currentRow, 4).Value = LastVisit;
                worksheet.Cell(currentRow, 5).Value = BeforeLastVisit;
                worksheet.Cell(currentRow, 6).Value = BeforeBeforelastVisit;
                worksheet.Cell(currentRow, 7).Value = BeforeBeforeBeforeLastVisit;
                currentRow++;
            }


            var secworksheet = workbook.Worksheets.Add("Not Visited");
            IEnumerable<RepAccountNotVisitsReport> notvis = rep.AccountMedicalNotVisitReport(userId, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);
            var row = 1;
            secworksheet.Cell(row, 1).Value = "Account";
            secworksheet.Cell(row, 2).Value = "Category";

            row = 2;

            foreach (var item in notvis)
            {
                secworksheet.Cell(row, 1).Value = item.AccountName;
                secworksheet.Cell(row, 2).Value = item.CategoryName;
                row++;
            }


            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + " visits report to Accounts from " + startdate + " to " + enddate + ".xlsx");

        }

        [Route("[controller]/[Action]/{userId}/{dayfrom}/{monthfrom}/{yearfrom}/{dayto}/{monthto}/{yearto}")]
        [HttpGet]
        public IActionResult AccountAndContactMedicalVisitReportExcel(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {

            string name = userManager.FindByIdAsync(userId).Result.FullName;
            string startdate = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd-MM-yyyy");
            string enddate = new DateTime(yearto, monthto, dayto).ToString("dd-MM-yyyy");
            IEnumerable<RepAccountVisitsReport> vis = rep.AccountMedicalVisitReport(userId, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visited Accounts");
            worksheet.Range("D1:G1").Merge();
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Account";
            worksheet.Cell(currentRow, 2).Value = "Category";
            worksheet.Cell(currentRow, 3).Value = "Visits Count";
            worksheet.Cell(currentRow, 4).Value = "Last Four Visits";



            currentRow = 2;
            foreach (var viss in vis)
            {

                worksheet.Cell(currentRow, 1).Value = viss.AccountName;
                worksheet.Cell(currentRow, 2).Value = viss.CategoryName;
                worksheet.Cell(currentRow, 3).Value = viss.VisitsCount;

                string LastVisit = viss.LastVisit.ToString("dd/MM/yyyy");
                string BeforeLastVisit = viss.BeforeLastVisit.ToString("dd/MM/yyyy");
                string BeforeBeforelastVisit = viss.BeforeBeforeLastVisit.ToString("dd/MM/yyyy");
                string BeforeBeforeBeforeLastVisit = viss.BeforeBeforeBeforeLastVisit.ToString("dd/MM/yyyy");
                if (LastVisit == "01/01/0001" || BeforeLastVisit == "01/01/0001" || BeforeBeforelastVisit == "01/01/0001" || BeforeBeforeBeforeLastVisit == "01/01/0001")
                {
                    if (LastVisit == "01/01/0001")
                    {
                        LastVisit = "No Visit";
                    }
                    else
                    {
                        LastVisit = viss.LastVisit.Date.ToString();
                    }
                    if (BeforeLastVisit == "01/01/0001")
                    {
                        BeforeLastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeLastVisit = viss.BeforeLastVisit.Date.ToString();
                    }
                    if (BeforeBeforelastVisit == "01/01/0001")
                    {
                        BeforeBeforelastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeBeforelastVisit = viss.BeforeBeforeLastVisit.Date.ToString();
                    }
                    if (BeforeBeforeBeforeLastVisit == "01/01/0001")
                    {
                        BeforeBeforeBeforeLastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeBeforeBeforeLastVisit = viss.BeforeBeforeBeforeLastVisit.Date.ToString();
                    }
                }
                worksheet.Cell(currentRow, 4).Value = LastVisit;
                worksheet.Cell(currentRow, 5).Value = BeforeLastVisit;
                worksheet.Cell(currentRow, 6).Value = BeforeBeforelastVisit;
                worksheet.Cell(currentRow, 7).Value = BeforeBeforeBeforeLastVisit;
                currentRow++;
            }


            var secworksheet = workbook.Worksheets.Add("Not Visited Accounts");
            IEnumerable<RepAccountNotVisitsReport> notvis = rep.AccountMedicalNotVisitReport(userId, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);
            var row = 1;
            secworksheet.Cell(row, 1).Value = "Account";
            secworksheet.Cell(row, 2).Value = "Category";

            row = 2;

            foreach (var item in notvis)
            {
                secworksheet.Cell(row, 1).Value = item.AccountName;
                secworksheet.Cell(row, 2).Value = item.CategoryName;
                row++;
            }


            IEnumerable<RepContactVisitsReport> visc = rep.ContactMedicalVisitReport(userId, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

        

            var worksheett = workbook.Worksheets.Add("Visited Contacts");
            worksheett.Range("F1:I1").Merge();
            var currentRoww = 1;
            worksheett.Cell(currentRoww, 1).Value = "Contact";
            worksheett.Cell(currentRoww, 2).Value = "Account Affiliation";
            worksheett.Cell(currentRoww, 3).Value = "Category";
            worksheett.Cell(currentRoww, 4).Value = "Visits Count";
            worksheett.Cell(currentRoww, 5).Value = "Monthly Target";
            worksheett.Cell(currentRoww, 6).Value = "Last three Visits";

            currentRoww = 2;
            foreach (var viss in visc)
            {

                worksheett.Cell(currentRoww, 1).Value = viss.ContactName;
                worksheett.Cell(currentRoww, 2).Value = viss.AccountName;
                worksheett.Cell(currentRoww, 3).Value = viss.CategoryName;
                worksheett.Cell(currentRoww, 4).Value = viss.VisitsCount;
                worksheett.Cell(currentRoww, 5).Value = viss.MonthlyTarget;

                string LastVisit = viss.LastVisit.ToString("dd/MM/yyyy");
                string BeforeLastVisit = viss.BeforeLastVisit.ToString("dd/MM/yyyy");
                string BeforeBeforelastVisit = viss.BeforeBeforeLastVisit.ToString("dd/MM/yyyy");
                string BeforeBeforeBeforeLastVisit = viss.BeforeBeforeBeforeLastVisit.ToString("dd/MM/yyyy");
                if (LastVisit == "01/01/0001" || BeforeLastVisit == "01/01/0001" || BeforeBeforelastVisit == "01/01/0001" || BeforeBeforeBeforeLastVisit == "01/01/0001")
                {
                    if (LastVisit == "01/01/0001")
                    {
                        LastVisit = "No Visit";
                    }
                    else
                    {
                        LastVisit = viss.LastVisit.Date.ToString();
                    }
                    if (BeforeLastVisit == "01/01/0001")
                    {
                        BeforeLastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeLastVisit = viss.BeforeLastVisit.Date.ToString();
                    }
                    if (BeforeBeforelastVisit == "01/01/0001")
                    {
                        BeforeBeforelastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeBeforelastVisit = viss.BeforeBeforeLastVisit.Date.ToString();
                    }
                    if (BeforeBeforeBeforeLastVisit == "01/01/0001")
                    {
                        BeforeBeforeBeforeLastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeBeforeBeforeLastVisit = viss.BeforeBeforeBeforeLastVisit.Date.ToString();
                    }
                }

                worksheett.Cell(currentRoww, 6).Value = LastVisit;
                worksheett.Cell(currentRoww, 7).Value = BeforeLastVisit;
                worksheett.Cell(currentRoww, 8).Value = BeforeBeforelastVisit;
                worksheett.Cell(currentRoww, 9).Value = BeforeBeforeBeforeLastVisit;
                currentRoww++;
            }

            IEnumerable<RepContactNotVisitsReport> notvisc = rep.ContactMedicalNotVisitReport(userId, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            var secworksheett = workbook.Worksheets.Add("Not Visited Contacts");

            var roww = 1;
            secworksheett.Cell(roww, 1).Value = "Contact";
            secworksheett.Cell(roww, 2).Value = "Account Affiliation";
            secworksheett.Cell(roww, 3).Value = "Category";
            secworksheett.Cell(roww, 4).Value = "Monthly Target";

            roww = 2;

            foreach (var item in notvisc)
            {
                secworksheett.Cell(roww, 1).Value = item.ContactName;
                secworksheett.Cell(roww, 2).Value = item.AccountName;
                secworksheett.Cell(roww, 3).Value = item.CategoryName;
                secworksheett.Cell(roww, 4).Value = item.MonthlyTarget;
                roww++;
            }



            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + " visits report from " + startdate + " to " + enddate + ".xlsx");

        }



        [Route("[controller]/[Action]/{userId}/{dayfrom}/{monthfrom}/{yearfrom}/{dayto}/{monthto}/{yearto}")]
        [HttpGet]
        public IActionResult AccountSalesVisitReportExcel(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {

            string name = userManager.FindByIdAsync(userId).Result.FullName;
            string startdate = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd-MM-yyyy");
            string enddate = new DateTime(yearto, monthto, dayto).ToString("dd-MM-yyyy");
            IEnumerable<RepAccountVisitsReport> vis = rep.AccountSalesVisitReport(userId, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visited");
            worksheet.Range("D1:G1").Merge();
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Account";
            worksheet.Cell(currentRow, 2).Value = "Category";
            worksheet.Cell(currentRow, 3).Value = "Visits Count";
            worksheet.Cell(currentRow, 4).Value = "Last three Visits";

            currentRow = 2;
            foreach (var viss in vis)
            {

                worksheet.Cell(currentRow, 1).Value = viss.AccountName;
                worksheet.Cell(currentRow, 2).Value = viss.CategoryName;
                worksheet.Cell(currentRow, 3).Value = viss.VisitsCount;

                string LastVisit = viss.LastVisit.ToString("dd/MM/yyyy");
                string BeforeLastVisit = viss.BeforeLastVisit.ToString("dd/MM/yyyy");
                string BeforeBeforelastVisit = viss.BeforeBeforeLastVisit.ToString("dd/MM/yyyy");
                string BeforeBeforeBeforeLastVisit = viss.BeforeBeforeBeforeLastVisit.ToString("dd/MM/yyyy");
                if (LastVisit == "01/01/0001" || BeforeLastVisit == "01/01/0001" || BeforeBeforelastVisit == "01/01/0001" || BeforeBeforeBeforeLastVisit == "01/01/0001")
                {
                    if (LastVisit == "01/01/0001")
                    {
                        LastVisit = "No Visit";
                    }
                    else
                    {
                        LastVisit = viss.LastVisit.Date.ToString();
                    }
                    if (BeforeLastVisit == "01/01/0001")
                    {
                        BeforeLastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeLastVisit = viss.BeforeLastVisit.Date.ToString();
                    }
                    if (BeforeBeforelastVisit == "01/01/0001")
                    {
                        BeforeBeforelastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeBeforelastVisit = viss.BeforeBeforeLastVisit.Date.ToString();
                    }
                    if (BeforeBeforeBeforeLastVisit == "01/01/0001")
                    {
                        BeforeBeforeBeforeLastVisit = "No Visit";
                    }
                    else
                    {
                        BeforeBeforeBeforeLastVisit = viss.BeforeBeforeBeforeLastVisit.Date.ToString();
                    }
                }
                worksheet.Cell(currentRow, 4).Value = LastVisit;
                worksheet.Cell(currentRow, 5).Value = BeforeLastVisit;
                worksheet.Cell(currentRow, 6).Value = BeforeBeforelastVisit;
                worksheet.Cell(currentRow, 7).Value = BeforeBeforeBeforeLastVisit;
                currentRow++;
            }

            

            IEnumerable<RepAccountNotVisitsReport> notvis = rep.AccountSalesNotVisitReport(userId, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);
            var secworksheet = workbook.Worksheets.Add("Not Visited");
         
            var row = 1;
            secworksheet.Cell(row, 1).Value = "Account";
            secworksheet.Cell(row, 2).Value = "Category";
            row = 2;

            foreach (var item in notvis)
            {
                secworksheet.Cell(row, 1).Value = item.AccountName;
                secworksheet.Cell(row, 2).Value = item.CategoryName;
                row++;
            }

            MemoryStream stream = new MemoryStream();

            workbook.SaveAs(stream);

            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + " visits report to Accounts from " + startdate + " to " + enddate + ".xlsx");

        }

       

        [Route("[controller]/[action]/{year}/{month}/{userid}")]
        [HttpGet]
        public IActionResult VisitedAccountsByMonthSales(int year, int month, string userid)
        {
            return Ok(rep.VisitedAccountsByMonthSales(year, month, userid));
        }

    }
}

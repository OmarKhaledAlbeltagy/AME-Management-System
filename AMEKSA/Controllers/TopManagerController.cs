using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Privilage;
using AMEKSA.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AMEKSA.CustomEntities;
using ClosedXML.Excel;
using System.IO;
using AMEKSA.Models;
using AMEKSA.MonthlyPlanModels;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class TopManagerController : ControllerBase
    {
        private readonly ITopManagerRep topManagerRep;
        private readonly IFirstManagerRep firstManagerRep;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly ITimeRep ti;

        public TopManagerController(ITopManagerRep topManagerRep,IFirstManagerRep firstManagerRep, UserManager<ExtendIdentityUser> userManager, ITimeRep ti)
        {
            this.topManagerRep = topManagerRep;
            this.firstManagerRep = firstManagerRep;
            this.userManager = userManager;
            this.ti = ti;
        }

        //GetAllFirstManagersAndTopManagers
        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllFirstManagersAndTopManagers()
        {
           return Ok( topManagerRep.GetAllFirstManagersAndTopManagers() );
        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}")]
        [HttpGet]
        public IActionResult PlansExcel(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto)
        {
            DateTime now = ti.GetCurrentTime();
            XLWorkbook workbook = new XLWorkbook();


            //start medical
            List<CustomContactPlanExcel> medicals = topManagerRep.MedicalPlansExcel(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto);

            var medicalsheet =  workbook.Worksheets.Add("Medical Reps");
            medicalsheet.Range("A1:H50000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            medicalsheet.Range("A1:H50000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            medicalsheet.Range("A1:H50000").Style.Font.Bold = true;
            
            medicalsheet.Column("A").Width = 35;
            medicalsheet.Column("B").Width = 35;
            medicalsheet.Column("C").Width = 35;

            medicalsheet.Rows("1:50000").Height = 25;
            var row = 1;
            foreach (var item in medicals)
            {


                if (item.planlist.ToArray().Length == 0)
                {
                    continue;
                }


                medicalsheet.Range("A"+row+":C"+row).Merge();
                medicalsheet.Cell("A" + row).Value = item.UserName;
                medicalsheet.Range("A" + row + ":C" + row).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                medicalsheet.Cell("C" + row).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                medicalsheet.Cell("A" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                row++;
                medicalsheet.Cell("A" + row).Value = "Contact";
                medicalsheet.Cell("B" + row).Value = "Account Affiliation";
                medicalsheet.Cell("C" + row).Value = "Planned Date";
                medicalsheet.Cell("C" + row).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                medicalsheet.Cell("A" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thick;


                foreach (var p in item.planlist)
                {
                    row++;
                    medicalsheet.Cell("A" + row).Value = p.OrgName;
                    medicalsheet.Cell("B" + row).Value = p.Aff;
                    medicalsheet.Cell("C" + row).Value = p.PlannedDate.Date;
                    medicalsheet.Cell("C" + row).DataType = XLDataType.DateTime;
                    medicalsheet.Cell("C" + row).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    medicalsheet.Cell("A" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    medicalsheet.Range("A" + row+":C"+row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    if (p.Status == true)
                    {
                        medicalsheet.Range("A"+row+":C"+row).Style.Fill.BackgroundColor = XLColor.FromArgb(198, 224, 180);
                    }
                    else
                    {
                        if (p.PlannedDate >= now)
                        {
                            medicalsheet.Range("A" + row + ":C" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 230, 153);
                        }
                        else
                        {
                            medicalsheet.Range("A" + row + ":C" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(236, 142, 132);
                        }
                    }

                    


                }

                medicalsheet.Range("A" + row + ":C" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                row++;
                row++;

            }

            //start sales

            List<CustomAccountPlanExcel> sales = topManagerRep.SalesPlansExcel(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto);

            var salessheet = workbook.Worksheets.Add("Sales Reps");

            salessheet.Range("A1:H50000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            salessheet.Range("A1:H50000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            salessheet.Range("A1:H50000").Style.Font.Bold = true;

            salessheet.Column("A").Width = 35;
            salessheet.Column("B").Width = 35;
            salessheet.Rows("1:50000").Height = 25;

            row = 1;

            foreach (var item in sales)
            {
                if (item.planlist.ToArray().Length == 0)
                {
                    continue;
                }

                salessheet.Range("A" + row + ":B" + row).Merge();
                salessheet.Cell("A" + row).Value = item.UserName;
                salessheet.Range("A" + row + ":B" + row).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                salessheet.Cell("B" + row).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                salessheet.Cell("A" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                row++;
                salessheet.Cell("A" + row).Value = "Contact";
                salessheet.Cell("B" + row).Value = "Planned Date";
                salessheet.Cell("B" + row).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                salessheet.Cell("A" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thick;


                foreach (var p in item.planlist)
                {
                    row++;
                    salessheet.Cell("A" + row).Value = p.OrgName;
                    salessheet.Cell("B" + row).Value = p.PlannedDate.Date;
                    salessheet.Cell("B" + row).DataType = XLDataType.DateTime;
                    salessheet.Cell("B" + row).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    salessheet.Cell("A" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    salessheet.Range("A" + row + ":B" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    if (p.Status == true)
                    {
                        salessheet.Range("A" + row + ":B" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(198, 224, 180);
                    }
                    else
                    {
                        if (p.PlannedDate >= now)
                        {
                            salessheet.Range("A" + row + ":B" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 230, 153);
                        }
                        else
                        {
                            salessheet.Range("A" + row + ":B" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(236, 142, 132);
                        }
                    }




                }

                salessheet.Range("A" + row + ":B" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                row++;
                row++;
            }


            //start supportive


            List<CustomAccountPlanExcel> supportive = topManagerRep.SupportivePlansExcel(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto);

            var supportivesheet = workbook.Worksheets.Add("Supportive Reps");

            supportivesheet.Range("A1:H50000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            supportivesheet.Range("A1:H50000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            supportivesheet.Range("A1:H50000").Style.Font.Bold = true;

            supportivesheet.Column("A").Width = 35;
            supportivesheet.Column("B").Width = 35;
            supportivesheet.Rows("1:50000").Height = 25;

            row = 1;

            foreach (var item in supportive)
            {
                if (item.planlist.ToArray().Length == 0)
                {
                    continue;
                }

                supportivesheet.Range("A" + row + ":B" + row).Merge();
                supportivesheet.Cell("A" + row).Value = item.UserName;
                supportivesheet.Range("A" + row + ":B" + row).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                supportivesheet.Cell("B" + row).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                supportivesheet.Cell("A" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                row++;
                supportivesheet.Cell("A" + row).Value = "Contact";
                supportivesheet.Cell("B" + row).Value = "Planned Date";
                supportivesheet.Cell("B" + row).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                supportivesheet.Cell("A" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thick;


                foreach (var p in item.planlist)
                {
                    row++;
                    supportivesheet.Cell("A" + row).Value = p.OrgName;
                    supportivesheet.Cell("B" + row).Value = p.PlannedDate.Date;
                    supportivesheet.Cell("B" + row).DataType = XLDataType.DateTime;
                    supportivesheet.Cell("B" + row).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    supportivesheet.Cell("A" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    supportivesheet.Range("A" + row + ":B" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    if (p.Status == true)
                    {
                        supportivesheet.Range("A" + row + ":B" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(198, 224, 180);
                    }
                    else
                    {
                        if (p.PlannedDate >= now)
                        {
                            supportivesheet.Range("A" + row + ":B" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 230, 153);
                        }
                        else
                        {
                            supportivesheet.Range("A" + row + ":B" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(236, 142, 132);
                        }
                    }




                }

                supportivesheet.Range("A" + row + ":B" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                row++;
                row++;
            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();
            GC.Collect();
            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               "Plans Quick Report.xlsx");
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult MorrisLine()
        {
            return Ok(topManagerRep.MorrisLine());
        }

        [Route("[controller]/[Action]/{year}/{month}")]
        [HttpGet]
        public IActionResult GetVisitsCountReyadByMonth(int year,int month)
        {

            return Ok(topManagerRep.GetVisitsCountReyadByMonth(year,month));
        }

        [Route("[controller]/[Action]/{year}/{month}")]
        [HttpGet]
        public IActionResult GetVisitsCountDamamByMonth(int year, int month)
        {

            return Ok(topManagerRep.GetVisitsCountDamamByMonth(year, month));
        }

        [Route("[controller]/[Action]/{year}/{month}")]
        [HttpGet]
        public IActionResult GetVisitsCountJeddahByMonth(int year, int month)
        {

            return Ok(topManagerRep.GetVisitsCountJeddahByMonth(year, month));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetVisitsCountThisMonthReyad()
        {

            return Ok(topManagerRep.GetVisitsCountThisMonthReyad());
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetTarget(SearchTargetModel obj)
        {

            return Ok(topManagerRep.GetTarget(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetVisitsCountThisMonthJeddah()
        {

            return Ok(topManagerRep.GetVisitsCountThisMonthJeddah());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetVisitsCountThisMonthDamam()
        {

            return Ok(topManagerRep.GetVisitsCountThisMonthDamam());
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult AccountThisMonthPercentage()
        {

            return Ok(topManagerRep.AccountThisMonthPercentage());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult ContactThisMonthPercentage()
        {
            return Ok(topManagerRep.ContactThisMonthPercentage());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult AccountpastMonthPercentage()
        {
            return Ok(topManagerRep.AccountpastMonthPercentage());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult ContactPastMonthPercentage()
        {
            return Ok(topManagerRep.ContactPastMonthPercentage());
        }
        
        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllFirstManagers()
        {
            return Ok(topManagerRep.GetAllFirstManagers());
        }

        [Route("[controller]/[Action]/{userid}/{dayfrom}/{monthfrom}/{yearfrom}/{dayto}/{monthto}/{yearto}")]
        [HttpGet]
        public IActionResult AccountMedicalVisitExcel(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            string name = "";
            if (userid == "0")
            {
                name = "AME";
            }
            else
            {
                name = userManager.FindByIdAsync(userid).Result.FullName;
            }
            string startdate = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd-MM-yyyy");
            string enddate = new DateTime(yearto, monthto, dayto).ToString("dd-MM-yyyy");
            IEnumerable<CustomAccountMedicalVisit> vis = topManagerRep.GetDetailedAMV(userid, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visits");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Representative Name";
            worksheet.Cell(currentRow, 2).Value = "Account";
            worksheet.Cell(currentRow, 3).Value = "Type";
            worksheet.Cell(currentRow, 4).Value = "Category";
            worksheet.Cell(currentRow, 5).Value = "Visit Date & Time";
            worksheet.Cell(currentRow, 6).Value = "Submitting Date & Time";
            worksheet.Cell(currentRow, 7).Value = "Visit Notes";
            worksheet.Cell(currentRow, 8).Value = "Additional Notes";
            worksheet.Cell(currentRow, 9).Value = "Products";
            worksheet.Cell(currentRow, 10).Value = "Persons Met";


            currentRow = 2;
            foreach (var viss in vis)
            {

                IEnumerable<CustomAccountMedicalVisitProducts> productss = viss.product.DistinctBy(a => a.ProductName);
                IEnumerable<CustomVisitPerson> personss = viss.person;
                worksheet.Cell(currentRow, 1).Value = viss.UserName;
                worksheet.Cell(currentRow, 2).Value = viss.AccountName;
                worksheet.Cell(currentRow, 3).Value = viss.AccountTypeName;
                worksheet.Cell(currentRow, 4).Value = viss.CategoryName;
                string date = viss.VisitDate.ToString("dd/MM/yyyy");
                TimeSpan time = viss.VisitTime.TimeOfDay;
                worksheet.Cell(currentRow, 5).Value = date + " " + time;

                DateTime sub = viss.SubmittingDate;
                worksheet.Cell(currentRow, 6).Value = sub.ToString("dd/MM/yyyy hh:mm tt");

                worksheet.Cell(currentRow, 7).Value = viss.VisitNotes;
                worksheet.Cell(currentRow, 8).Value = viss.AdditionalNotes;

                string productstring = "";

                foreach (var item in productss)
                {
                    productstring = productstring+ item.ProductName + " - ";
                }

                if (productstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 9).Value = productstring.Remove(productstring.Length - 3);
                }
                string personstring = "";
                foreach (var item in personss)
                {
                    personstring = personstring + "( " + item.PersonName + " - " + item.PersonPosition + " ) | ";
                }

                if (personstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 10).Value = personstring.Remove(personstring.Length - 3);
                }
                currentRow++;
            }



            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + "'s team visits to accounts from " + startdate + " to " + enddate + ".xlsx");




        }

        [Route("[controller]/[Action]/{userid}/{dayfrom}/{monthfrom}/{yearfrom}/{dayto}/{monthto}/{yearto}")]
        [HttpGet]
        public IActionResult AccountSalesVisitExcel(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            string name = "";
            if (userid == "0")
            {
                name = "AME";
            }
            else
            {
                name = userManager.FindByIdAsync(userid).Result.FullName;
            }
            string startdate = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd-MM-yyyy");
            string enddate = new DateTime(yearto, monthto, dayto).ToString("dd-MM-yyyy");
            IEnumerable<CustomAccountSalesVisit> vis = topManagerRep.GetDetailedASV(userid, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visits");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Representative Name";
            worksheet.Cell(currentRow, 2).Value = "Account";
            worksheet.Cell(currentRow, 3).Value = "Type";
            worksheet.Cell(currentRow, 4).Value = "Category";
            worksheet.Cell(currentRow, 5).Value = "Visit Date & Time";
            worksheet.Cell(currentRow, 6).Value = "Submitting Date & Time";
            worksheet.Cell(currentRow, 7).Value = "Visit Notes";
            worksheet.Cell(currentRow, 8).Value = "Payment Notes";
            worksheet.Cell(currentRow, 9).Value = "Brand";
            worksheet.Cell(currentRow, 10).Value = "Persons Met";


            currentRow = 2;
            foreach (var viss in vis)
            {

                IEnumerable<CustomVisitBrand> brandss = viss.brands.DistinctBy(a => a.BrandName);
                IEnumerable<CustomVisitPerson> personss = viss.persons;
                worksheet.Cell(currentRow, 1).Value = viss.UserName;
                worksheet.Cell(currentRow, 2).Value = viss.AccountName;
                worksheet.Cell(currentRow, 3).Value = viss.AccountTypeName;
                worksheet.Cell(currentRow, 4).Value = viss.CategoryName;
                string date = viss.VisitDate.ToString("dd/MM/yyyy");
                TimeSpan time = viss.VisitTime.TimeOfDay;
                worksheet.Cell(currentRow, 5).Value = date + " " + time;

                DateTime subdate = viss.SubmittingDate;




                worksheet.Cell(currentRow, 6).Value = subdate.ToString("dd/MM/yyyy hh:mm tt");


                worksheet.Cell(currentRow, 7).Value = viss.VisitNotes;
                worksheet.Cell(currentRow, 8).Value = viss.PaymentNotes;

                string brandstring = "";

                foreach (var item in brandss)
                {
                    brandstring = brandstring + item.BrandName + " - ";
                }

                if (brandstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 9).Value = brandstring.Remove(brandstring.Length - 3);
                }
                string personstring = "";
                foreach (var item in personss)
                {
                    personstring = personstring + "( " + item.PersonName + " - " + item.PersonPosition + " ) | ";
                }

                if (personstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 10).Value = personstring.Remove(personstring.Length - 3);
                }
                currentRow++;
            }


            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + "'s team visits to accounts from " + startdate + " to " + enddate + ".xlsx");




        }


        [Route("[controller]/[Action]/{userid}/{dayfrom}/{monthfrom}/{yearfrom}/{dayto}/{monthto}/{yearto}")]
        [HttpGet]
        public IActionResult AccountSupportiveVisitExcel(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            string name = "";
            if (userid == "0")
            {
                name = "AME";
            }
            else
            {
                name = userManager.FindByIdAsync(userid).Result.FullName;
            }
            string startdate = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd-MM-yyyy");
            string enddate = new DateTime(yearto, monthto, dayto).ToString("dd-MM-yyyy");
            IEnumerable<CustomAccountSupportiveVisit> vis = topManagerRep.GetDetailedASupportiveV(userid, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visits");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Representative Name";
            worksheet.Cell(currentRow, 2).Value = "Account";
            worksheet.Cell(currentRow, 3).Value = "Type";
            worksheet.Cell(currentRow, 4).Value = "Category";
            worksheet.Cell(currentRow, 5).Value = "Visit Date & Time";
            worksheet.Cell(currentRow, 6).Value = "Submitting Date & Time";
            worksheet.Cell(currentRow, 7).Value = "Visit Notes";
            worksheet.Cell(currentRow, 8).Value = "Additional Notes";
            worksheet.Cell(currentRow, 9).Value = "Brand";
            worksheet.Cell(currentRow, 10).Value = "Persons Met";


            currentRow = 2;
            foreach (var viss in vis)
            {

                IEnumerable<CustomVisitProduct> productss = viss.products.DistinctBy(a => a.ProductName);
                IEnumerable<CustomVisitPerson> personss = viss.persons;
                worksheet.Cell(currentRow, 1).Value = viss.UserName;
                worksheet.Cell(currentRow, 2).Value = viss.AccountName;
                worksheet.Cell(currentRow, 3).Value = viss.AccountTypeName;
                worksheet.Cell(currentRow, 4).Value = viss.CategoryName;
                string date = viss.VisitDate.ToString("dd/MM/yyyy");
                TimeSpan time = viss.VisitTime.TimeOfDay;
                worksheet.Cell(currentRow, 5).Value = date + " " + time;

                DateTime subdate = viss.SubmittingDate;




                worksheet.Cell(currentRow, 6).Value = subdate.ToString("dd/MM/yyyy hh:mm tt");


                worksheet.Cell(currentRow, 7).Value = viss.VisitNotes;
                worksheet.Cell(currentRow, 8).Value = viss.AdditionalNotes;

                string brandstring = "";

                foreach (var item in productss)
                {
                    brandstring = brandstring + item.ProductName + " - ";
                }

                if (brandstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 9).Value = brandstring.Remove(brandstring.Length - 3);
                }
                string personstring = "";
                foreach (var item in personss)
                {
                    personstring = personstring + "( " + item.PersonName + " - " + item.PersonPosition + " ) | ";
                }

                if (personstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 10).Value = personstring.Remove(personstring.Length - 3);
                }
                currentRow++;
            }


            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + "'s suuportive team visits to accounts from " + startdate + " to " + enddate + ".xlsx");




        }

        [Route("[controller]/[Action]/{userid}/{dayfrom}/{monthfrom}/{yearfrom}/{dayto}/{monthto}/{yearto}")]
        [HttpGet]
        public IActionResult ContactMedicalVisitExcel(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto)
        {
            string name = "";
            if (userid == "0")
            {
                name = "AME";
            }
            else
            {
                name = userManager.FindByIdAsync(userid).Result.FullName;
            }
            
            string startdate = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd-MM-yyyy");
            string enddate = new DateTime(yearto, monthto, dayto).ToString("dd-MM-yyyy");
            IEnumerable<CustomContactMedicalVisit> vis = topManagerRep.GetDetailedCMV(userid, dayfrom, monthfrom, yearfrom, dayto, monthto, yearto);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visits");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Representative Name";
            worksheet.Cell(currentRow, 2).Value = "Contact";
            worksheet.Cell(currentRow, 3).Value = "Type";
            worksheet.Cell(currentRow, 4).Value = "Category";
            worksheet.Cell(currentRow, 5).Value = "Visit Date & Time";
            worksheet.Cell(currentRow, 6).Value = "Submitting Date & Time";
            worksheet.Cell(currentRow, 7).Value = "Visit Notes";
            worksheet.Cell(currentRow, 8).Value = "Requests";
            worksheet.Cell(currentRow, 9).Value = "Products";
            worksheet.Cell(currentRow, 10).Value = "Sales Aids";


            currentRow = 2;
            foreach (var viss in vis)
            {
                IEnumerable<CustomContactMedicalVisitProducts> productss = viss.customcontactmedicalvisitproduct.ToList().DistinctBy(a => a.ProductName);
                IEnumerable<CustomContactSalesAid> salesaids = viss.customcontactsalesaid;
                worksheet.Cell(currentRow, 1).Value = viss.UserName;
                worksheet.Cell(currentRow, 2).Value = viss.ContactName;
                worksheet.Cell(currentRow, 3).Value = viss.ContactTypeName;
                worksheet.Cell(currentRow, 4).Value = viss.CategoryName;

                string date = viss.VisitDate.ToString("dd/MM/yyyy");
          
                TimeSpan time = viss.VisitTime.TimeOfDay;
                worksheet.Cell(currentRow, 5).Value = date + " " + time;
                
                DateTime sub = viss.SubmittingDate;
                worksheet.Cell(currentRow, 6).Value = sub.ToString("dd/MM/yyyy hh:mm tt");
               
                worksheet.Cell(currentRow, 7).Value = viss.VisitNotes;
                worksheet.Cell(currentRow, 8).Value = viss.Requests;

                string productstring = "";

                foreach (var item in productss)
                {
                    productstring = productstring + "( Product: " + item.ProductName + " - Product Share: " + item.ProductShare + " ) | ";
                }
                if (productstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 9).Value = productstring.Remove(productstring.Length - 3);
                }


                string salesaidsstring = "";
                foreach (var item in salesaids)
                {
                    salesaidsstring = salesaidsstring + "( " + item.SalesAidName + " ) | ";
                }

                if (salesaidsstring.Length > 3)
                {
                    worksheet.Cell(currentRow, 10).Value = salesaidsstring.Remove(salesaidsstring.Length - 3);
                }
                currentRow++;
            }



            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + "'s team visits to contacts from " + startdate + " to " + enddate + ".xlsx");




        }




        [Route("[controller]/[Action]/{ManagerId}/{CategoryId}/{Month}/{Year}")]
        [HttpGet]
        public IActionResult GetTargetExcel(string ManagerId,int CategoryId,int Month,int Year)
        {
            string name = userManager.FindByIdAsync(ManagerId).Result.FullName;
            string month = new DateTime(Year, Month, 1).ToString("MMMM");
            
            SearchTargetModel obj = new SearchTargetModel();
            obj.CategoryId = CategoryId;
            obj.ManagerId = ManagerId;
            obj.Month = Month;
            obj.year = Year;
            IEnumerable<CustomTarget> vis = topManagerRep.GetTarget(obj);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Target");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Representative Name";
            worksheet.Cell(currentRow, 2).Value = "Contact";
            worksheet.Cell(currentRow, 3).Value = "Category";
            worksheet.Cell(currentRow, 4).Value = "Current Visits";
            worksheet.Cell(currentRow, 5).Value = "Monthly Target";
        


            currentRow = 2;
            foreach (var viss in vis)
            {
          
                worksheet.Cell(currentRow, 1).Value = viss.FullName;
                worksheet.Cell(currentRow, 2).Value = viss.ContactName;
                worksheet.Cell(currentRow, 3).Value = viss.CategoryName;
                worksheet.Cell(currentRow, 4).Value = viss.CurrentVisits;
                if (viss.MonthlyTarget == null)
                {
                    viss.MonthlyTarget = 0;
                }
                worksheet.Cell(currentRow, 5).Value = viss.MonthlyTarget;

              

                if (viss.CurrentVisits == 0 || viss.CurrentVisits < viss.MonthlyTarget)
                {
                    worksheet.Cell(currentRow, 4).Style.Font.FontColor = XLColor.Red;
                }
                else
                {
                    if (viss.CurrentVisits >= viss.MonthlyTarget)
                    {
                        worksheet.Cell(currentRow, 4).Style.Font.FontColor = XLColor.Green;
                    }
                }

                currentRow++;
            }



            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + "'s team Target Reprot on " + month + ".xlsx");




        }


    }
}

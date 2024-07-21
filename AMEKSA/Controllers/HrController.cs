using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Models;
using AMEKSA.Privilage;
using AMEKSA.Repo;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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
    public class HrController : ControllerBase
    {
        private readonly IHrRep rep;
        private readonly ITimeRep ti;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public HrController(IHrRep rep, ITimeRep ti, UserManager<ExtendIdentityUser> userManager)
        {
            this.rep = rep;
            this.ti = ti;
            this.userManager = userManager;
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetTenClockReport(string id)
        {
            DateTime now = ti.GetCurrentTime();
            DateTime yes = new DateTime();
            switch (now.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    yes = now.AddDays(-3);
                    break;
                case DayOfWeek.Monday:
                    yes = now.AddDays(-1);
                    break;
                case DayOfWeek.Tuesday:
                    yes = now.AddDays(-1);
                    break;
                case DayOfWeek.Wednesday:
                    yes = now.AddDays(-1);
                    break;
                case DayOfWeek.Thursday:
                    yes = now.AddDays(-1);
                    break;
                case DayOfWeek.Friday:
                    yes = now.AddDays(-1);
                    break;
                case DayOfWeek.Saturday:
                    yes = now.AddDays(-2);
                    break;
            }
            TenClockReportModel obj = rep.GetTenClockReport();
            string name = userManager.FindByIdAsync(id).Result.FullName;
            
            XLWorkbook workbook = new XLWorkbook();
            workbook.Protect();
            
            var worksheetmedical = workbook.Worksheets.Add("Medical");
            worksheetmedical.Column("A").Width = 30;
            worksheetmedical.Column("B").Width = 30;
            
            worksheetmedical.Rows("1:150").Height = 25;

            worksheetmedical.Range("A1:B1").Merge();
            worksheetmedical.Cell("A2").Value = "Created Reports";
            worksheetmedical.Cell("B2").Value = "Didn't Create Reports";
            worksheetmedical.Cell("A1").Value = "Medical Representatives";
            worksheetmedical.Cell("A2").Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);
            worksheetmedical.Cell("B2").Style.Fill.BackgroundColor = XLColor.FromArgb(244, 176, 132);
            worksheetmedical.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromArgb(155, 194, 230);

            worksheetmedical.Column("E").Width = 40;
            worksheetmedical.Column("F").Width = 40;

            worksheetmedical.Range("A1:F150").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheetmedical.Range("A1:F150").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheetmedical.Range("A1:B2").Style.Font.FontSize = 16;
            worksheetmedical.Range("A1:B2").Style.Font.Bold = true;
            worksheetmedical.Range("E2:F4").Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheetmedical.Range("E2:F4").Style.Font.FontSize = 16;
            worksheetmedical.Range("E2:F4").Style.Font.Bold = true;
            worksheetmedical.Cell("E2").Value = "Report Date:";
            worksheetmedical.Cell("F2").Value = yes;
            worksheetmedical.Cell("E3").Value = "Exported By:";
            worksheetmedical.Cell("F3").Value = name;
            worksheetmedical.Cell("E4").Value = "Exported On:";
            worksheetmedical.Cell("F4").Value = now;

            var medicalrow = 3;
            foreach (var item in obj.MedicalReported)
            {
                worksheetmedical.Cell("A" + medicalrow).Value = item.FullName;
                medicalrow++;
            }

            medicalrow = 3;
            foreach (var item in obj.MedicalNotReported)
            {
                worksheetmedical.Cell("B" + medicalrow).Value = item.FullName;
                medicalrow++;
            }




            var worksheetsales = workbook.Worksheets.Add("Sales");
            worksheetsales.Column("A").Width = 30;
            worksheetsales.Column("B").Width = 30;

            worksheetsales.Rows("1:150").Height = 25;

            worksheetsales.Range("A1:B1").Merge();
            worksheetsales.Cell("A2").Value = "Created Reports";
            worksheetsales.Cell("B2").Value = "Didn't Create Reports";
            worksheetsales.Cell("A1").Value = "Sales Representatives";
            worksheetsales.Cell("A2").Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);
            worksheetsales.Cell("B2").Style.Fill.BackgroundColor = XLColor.FromArgb(244, 176, 132);
            worksheetsales.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromArgb(155, 194, 230);

            worksheetsales.Column("E").Width = 40;
            worksheetsales.Column("F").Width = 40;

            worksheetsales.Range("A1:F150").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheetsales.Range("A1:F150").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheetsales.Range("A1:B2").Style.Font.FontSize = 16;
            worksheetsales.Range("A1:B2").Style.Font.Bold = true;
            worksheetsales.Range("E2:F4").Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheetsales.Range("E2:F4").Style.Font.FontSize = 16;
            worksheetsales.Range("E2:F4").Style.Font.Bold = true;
            worksheetsales.Cell("E2").Value = "Report Date:";
            worksheetsales.Cell("F2").Value = yes;
            worksheetsales.Cell("E3").Value = "Exported By:";
            worksheetsales.Cell("F3").Value = name;
            worksheetsales.Cell("E4").Value = "Exported On:";
            worksheetsales.Cell("F4").Value = now;

            var salesrow = 3;
            foreach (var item in obj.SalesReported)
            {
                worksheetsales.Cell("A" + salesrow).Value = item.FullName;
                salesrow++;
            }

            salesrow = 3;
            foreach (var item in obj.SalesNotReported)
            {
                worksheetsales.Cell("B" + salesrow).Value = item.FullName;
                salesrow++;
            }


            worksheetmedical.Protect();
            worksheetsales.Protect();
            MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            


            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               "10 Clock Report On "+now+".xlsx");
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllMedicalRep()
        {
            return Ok(rep.GetAllMedicalRep());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllSalesRep()
        {
            return Ok(rep.GetAllSalesRep());
        }


        [Route("[controller]/[Action]/{userid}")]
        [HttpGet]
        public IActionResult GetActualWorkingDaysSales(string userid)
        {
            return Ok(rep.GetActualWorkingDaysSales(userid));
        }

        [Route("[controller]/[Action]/{userid}")]
        [HttpGet]
        public IActionResult GetActualWorkingDaysMedical(string userid)
        {
            return Ok(rep.GetActualWorkingDaysMedical(userid));
        }

     
        [Route("[controller]/[Action]/{userid}")]
        [HttpGet]
        public IActionResult GetTimeOffDays(string userid)
        {
            return Ok(rep.GetTimeOffDays(userid));
        }
    }
}

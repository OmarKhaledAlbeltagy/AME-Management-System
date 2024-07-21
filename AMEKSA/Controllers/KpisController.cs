using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    public class KpisController : ControllerBase
    {
        private readonly IKpisRep rep;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly ITimeRep ti;

        public KpisController(IKpisRep rep,UserManager<ExtendIdentityUser> userManager, ITimeRep ti)
        {
            this.rep = rep;
            this.userManager = userManager;
            this.ti = ti;
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllProperties()
        {
            return Ok(rep.GetAllProperties());
        }

        [Route("[controller]/[Action]/{id}/{value}")]
        [HttpGet]
        public IActionResult EditProperty(int id,int value)
        {
            return Ok(rep.EditProperty(id,value));
        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{yearto}/{monthto}/{managerId}/{ex}")]
        [HttpGet]
        public IActionResult GetTeamMedicalKpi(int yearfrom, int monthfrom, int yearto, int monthto,string managerId, string ex)
        {
            string name = userManager.FindByIdAsync(managerId).Result.FullName;
            string exporter = userManager.FindByIdAsync(ex).Result.FullName;
            IEnumerable<MedicalKpiModel> result = rep.GetTeamMedicalKpi(yearfrom, monthfrom, yearto, monthto, managerId);
            XLWorkbook workbook = new XLWorkbook();
            foreach (var res in result)
            {
                var worksheet = workbook.Worksheets.Add(res.FullName);
                worksheet.Range("D13:E14").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                worksheet.Cell("D13").Value = "Exported By";
                worksheet.Cell("D14").Value = "Exporting Date";
                worksheet.Cell("E13").Value = exporter;
                worksheet.Cell("E14").Value = ti.GetCurrentTime().ToString("dd/MM/yyyy");
                worksheet.Rows("1:25").Height = 20;
                worksheet.Cell("F2").Style.Font.FontSize = 75;
                worksheet.Range("B2:E2").Merge();
                worksheet.Range("C3:E3").Merge();
                worksheet.Range("C4:E4").Merge();
                worksheet.Range("C5:E5").Merge();
                worksheet.Range("C6:E6").Merge();
                worksheet.Range("F2:G7").Merge();
                worksheet.Range("B18:G18").Merge();
                worksheet.Cell("B8").Style.Font.Bold = true;
                worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Column("A").Width = 15;
                worksheet.Row(2).Height = 33;
                worksheet.Column("B").Width = 32;
                worksheet.Column("C").Width = 32;
                worksheet.Column("D").Width = 32;
                worksheet.Column("E").Width = 32;
                worksheet.Column("F").Width = 32;
                worksheet.Column("G").Width = 32;
                worksheet.Cell("B15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("C15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("D15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("E15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("F15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("G15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B16").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B17").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B19").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B20").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B21").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B22").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B23").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B2").Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                worksheet.Range("B2:G12").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B2:G12").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("F2:G7").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B8:B12").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B15:G23").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B15:G23").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("A1:G25").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:G25").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell("B2").Style.Font.Bold = true;
                worksheet.Cell("B2").Style.Font.FontSize = 26;
                worksheet.Range("B3:B6").Style.Font.Bold = true;
                worksheet.Cell("F2").Style.Font.Bold = true;
                worksheet.Cell("B23").Style.Font.Bold = true;
                worksheet.Cell("B23").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell("B23").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("C7:C12").Style.Font.Bold = true;
                worksheet.Range("E8:E12").Style.Font.Bold = true;
                worksheet.Range("B15:B23").Style.Font.Bold = true;
                worksheet.Range("C15:G15").Style.Font.Bold = true;

                worksheet.Cell("B2").Value = "Medical KPI";
                worksheet.Cell("B3").Value = "Name";
                worksheet.Cell("B4").Value = "Office";
                worksheet.Cell("B5").Value = "Team";
                worksheet.Cell("B6").Value = "Month";
                worksheet.Cell("C8").Value = "Actual Total Number Of Visits";
                worksheet.Cell("C9").Value = "Number Of A+ & A Contacts Visited";
                worksheet.Cell("C10").Value = "Number Of B Contacts Visited";
                worksheet.Cell("C11").Value = "Number Of C Contacts Visited";
                worksheet.Cell("C12").Value = "Selling Days In The Field";
                worksheet.Cell("E8").Value = "Targeted Number Of Visits";
                worksheet.Cell("E9").Value = "Number Of A+ & A Contacts Listed";
                worksheet.Cell("E10").Value = "Number Of B Contacts Listed";
                worksheet.Cell("E11").Value = "Number Of C Contacts Listed";
                worksheet.Cell("E12").Value = "Working Days";
                worksheet.Cell("B15").Value = "KPI";
                worksheet.Cell("C15").Value = "KPI Target";
                worksheet.Cell("D15").Value = "Actual";
                worksheet.Cell("E15").Value = "Rate";
                worksheet.Cell("F15").Value = "Weight";
                worksheet.Cell("G15").Value = "Score";
                worksheet.Cell("B16").Value = "Average Visits Per Day";
                worksheet.Cell("B17").Value = "Visits Target Acheivment";
                worksheet.Cell("B18").Value = "Coverage For Targeted Customers";
                worksheet.Cell("B19").Value = "A+ & A";
                worksheet.Cell("B20").Value = "B";
                worksheet.Cell("B21").Value = "C";
                worksheet.Cell("B22").Value = "Selling Days In The Field";
                worksheet.Cell("B23").Value = "Total Score";


                worksheet.Cell("C3").Value = res.FullName;
                worksheet.Cell("C4").Value = res.CityName;
                worksheet.Cell("C5").Value = res.RoleName;
                worksheet.Cell("C6").Value = res.Month;
                worksheet.Cell("D8").Value = res.ActualTotalNumberOfVisits;
                worksheet.Cell("D9").Value = res.APlusAndAVisited;
                worksheet.Cell("D10").Value = res.BVisited;
                worksheet.Cell("D11").Value = res.CVisited;
                worksheet.Cell("D12").Value = res.SellingDaysInTheField;
                worksheet.Cell("F8").FormulaA1 = "=SUM(F12*C16)";
                worksheet.Cell("F9").Value = res.AplusAndAListed;
                worksheet.Cell("F10").Value = res.BListed;
                worksheet.Cell("F11").Value = res.CListed;
                worksheet.Cell("F12").Value = res.WorkingDays;
                worksheet.Cell("C16").Value = res.AverageVisitsPerDayKpiTarget;
                worksheet.Cell("C7").Value = "Time Off Territory Days";
                worksheet.Cell("D7").Value = res.TimeOffDays;
                worksheet.Cell("C17").FormulaA1 = "=SUM(C16*C22)";
                worksheet.Cell("C19").FormulaA1 = "=SUM(F9)";
                worksheet.Cell("C20").FormulaA1 = "=SUM(F10)";
                worksheet.Cell("C21").FormulaA1 = "=SUM(F11)";
                worksheet.Cell("C22").FormulaA1 = "=SUM(F12)";
                worksheet.Cell("D16").FormulaA1 = "=SUM(D17/D22)";
                worksheet.Cell("D17").FormulaA1 = "=SUM(D8)";
                worksheet.Cell("D19").FormulaA1 = "SUM(D9)";
                worksheet.Cell("D20").FormulaA1 = "=SUM(D10)";
                worksheet.Cell("D21").FormulaA1 = "=SUM(D11)";
                worksheet.Cell("D22").FormulaA1 = "=SUM(D12)";
                worksheet.Cell("E16").FormulaA1 = "=SUM(D16/C16)*100";
                worksheet.Cell("E17").FormulaA1 = "=SUM(D17/C17)*100";
                worksheet.Cell("E19").FormulaA1 = "=SUM(D19/C19)*100";
                worksheet.Cell("E20").FormulaA1 = "=SUM(D20/C20)*100";
                worksheet.Cell("E21").FormulaA1 = "=SUM(D21/C21)*100";
                worksheet.Cell("E22").FormulaA1 = "=SUM(D22/C22)*100";
                worksheet.Cell("F16").Value = res.AverageVisitsPerDayWeight;
                worksheet.Cell("F17").Value = res.VisitsTargetAchievmentWeight;
                worksheet.Cell("F19").Value = res.APlusAndAWeight;
                worksheet.Cell("F20").Value = res.BWeight;
                worksheet.Cell("F21").Value = res.CWeight;
                worksheet.Cell("F22").Value = res.SellingDaysInTheFieldWeight;
                worksheet.Cell("G16").FormulaA1 = "=SUM(E16*F16)/100";
                worksheet.Cell("G17").FormulaA1 = "=SUM(E17*F17)/100";
                worksheet.Cell("G19").FormulaA1 = "=SUM(E19*F19)/100";
                worksheet.Cell("G20").FormulaA1 = "=SUM(E20*F20)/100";
                worksheet.Cell("G21").FormulaA1 = "=SUM(E21*F21)/100";
                worksheet.Cell("G22").FormulaA1 = "=SUM(E22*F22)/100";
                worksheet.Cell("G23").FormulaA1 = "=SUM(G16:G22)";
                worksheet.Cell("F2").Style.NumberFormat.Format = "0%";
                worksheet.Cell("F2").DataType = XLDataType.Number;

                worksheet.Cell("F2").FormulaA1 = "=SUM(ROUND((G23),0)/100)";
            }
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               name+"'s Medical Team KPIs.xlsx");

        }


  
        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{yearto}/{monthto}/{userId}/{ex}")]
        [HttpGet]
        public IActionResult GetMedicalKpi(int yearfrom, int monthfrom, int yearto, int monthto, string userId, string ex)
        {
            string exporter = userManager.FindByIdAsync(ex).Result.FullName;
            MedicalKpiModel res = rep.GetMedicalKpi(yearfrom, monthfrom, yearto, monthto, userId);
            XLWorkbook workbook = new XLWorkbook();
            
            var worksheet = workbook.Worksheets.Add(res.FullName);
            worksheet.Range("D13:E14").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
            worksheet.Cell("D13").Value = "Exported By";
            worksheet.Cell("D14").Value = "Exporting Date";
            worksheet.Cell("E13").Value = exporter;
            worksheet.Cell("E14").Value = ti.GetCurrentTime().ToString("dd/MM/yyyy");
            worksheet.Rows("1:25").Height = 20;
            worksheet.Cell("F2").Style.Font.FontSize = 75;
            worksheet.Range("B2:E2").Merge();
            worksheet.Range("C3:E3").Merge();
            worksheet.Range("C4:E4").Merge();
            worksheet.Range("C5:E5").Merge();
            worksheet.Range("C6:E6").Merge();
            worksheet.Range("F2:G7").Merge();
            worksheet.Range("B18:G18").Merge();
            worksheet.Cell("B8").Style.Font.Bold = true;
            worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Column("A").Width = 15;
            worksheet.Row(2).Height = 33;
            worksheet.Column("B").Width = 32;
            worksheet.Column("C").Width = 32;
            worksheet.Column("D").Width = 32;
            worksheet.Column("E").Width = 32;
            worksheet.Column("F").Width = 32;
            worksheet.Column("G").Width = 32;
            worksheet.Cell("B15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("C15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("D15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("E15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("F15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("G15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B16").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B17").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B19").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B20").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B21").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B22").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B23").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B2").Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Range("B2:G12").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("B2:G12").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("F2:G7").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("B8:B12").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("B15:G23").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("B15:G23").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A1:G25").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:G25").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell("B2").Style.Font.Bold = true;
            worksheet.Cell("B2").Style.Font.FontSize = 26;
            worksheet.Range("B3:B6").Style.Font.Bold = true;
            worksheet.Cell("F2").Style.Font.Bold = true;
            worksheet.Cell("B23").Style.Font.Bold = true;
            worksheet.Cell("B23").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Cell("B23").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("C7:C12").Style.Font.Bold = true;
            worksheet.Range("E8:E12").Style.Font.Bold = true;
            worksheet.Range("B15:B23").Style.Font.Bold = true;
            worksheet.Range("C15:G15").Style.Font.Bold = true;

            worksheet.Cell("B2").Value = "Medical KPI";
            worksheet.Cell("B3").Value = "Name";
            worksheet.Cell("B4").Value = "Office";
            worksheet.Cell("B5").Value = "Team";
            worksheet.Cell("B6").Value = "Month";
            worksheet.Cell("C8").Value = "Actual Total Number Of Visits";
            worksheet.Cell("C9").Value = "Number Of A+ & A Contacts Visited";
            worksheet.Cell("C10").Value = "Number Of B Contacts Visited";
            worksheet.Cell("C11").Value = "Number Of C Contacts Visited";
            worksheet.Cell("C12").Value = "Selling Days In The Field";
            worksheet.Cell("E8").Value = "Targeted Number Of Visits";
            worksheet.Cell("E9").Value = "Number Of A+ & A Contacts Listed";
            worksheet.Cell("E10").Value = "Number Of B Contacts Listed";
            worksheet.Cell("E11").Value = "Number Of C Contacts Listed";
            worksheet.Cell("E12").Value = "Working Days";
            worksheet.Cell("B15").Value = "KPI";
            worksheet.Cell("C15").Value = "KPI Target";
            worksheet.Cell("D15").Value = "Actual";
            worksheet.Cell("E15").Value = "Rate";
            worksheet.Cell("F15").Value = "Weight";
            worksheet.Cell("G15").Value = "Score";
            worksheet.Cell("B16").Value = "Average Visits Per Day";
            worksheet.Cell("B17").Value = "Visits Target Acheivment";
            worksheet.Cell("B18").Value = "Coverage For Targeted Customers";
            worksheet.Cell("B19").Value = "A+ & A";
            worksheet.Cell("B20").Value = "B";
            worksheet.Cell("B21").Value = "C";
            worksheet.Cell("B22").Value = "Selling Days In The Field";
            worksheet.Cell("B23").Value = "Total Score";


            worksheet.Cell("C3").Value = res.FullName;
            worksheet.Cell("C4").Value = res.CityName;
            worksheet.Cell("C5").Value = res.RoleName;
            worksheet.Cell("C6").Value = res.Month;
            worksheet.Cell("D8").Value = res.ActualTotalNumberOfVisits;
            worksheet.Cell("D9").Value = res.APlusAndAVisited;
            worksheet.Cell("D10").Value = res.BVisited;
            worksheet.Cell("D11").Value = res.CVisited;
            worksheet.Cell("D12").Value = res.SellingDaysInTheField;
            worksheet.Cell("F8").FormulaA1 = "=SUM(F12*C16)";
            worksheet.Cell("F9").Value = res.AplusAndAListed;
            worksheet.Cell("F10").Value = res.BListed;
            worksheet.Cell("F11").Value = res.CListed;
            worksheet.Cell("F12").Value = res.WorkingDays;
            worksheet.Cell("C16").Value = res.AverageVisitsPerDayKpiTarget;
            worksheet.Cell("C7").Value = "Time Off Territory Days";
            worksheet.Cell("D7").Value = res.TimeOffDays;
            worksheet.Cell("C17").FormulaA1 = "=SUM(C16*C22)";
            worksheet.Cell("C19").FormulaA1 = "=SUM(F9)";
            worksheet.Cell("C20").FormulaA1 = "=SUM(F10)";
            worksheet.Cell("C21").FormulaA1 = "=SUM(F11)";
            worksheet.Cell("C22").FormulaA1 = "=SUM(F12)";
            worksheet.Cell("D16").FormulaA1 = "=SUM(D17/D22)";
            worksheet.Cell("D17").FormulaA1 = "=SUM(D8)";
            worksheet.Cell("D19").FormulaA1 = "SUM(D9)";
            worksheet.Cell("D20").FormulaA1 = "=SUM(D10)";
            worksheet.Cell("D21").FormulaA1 = "=SUM(D11)";
            worksheet.Cell("D22").FormulaA1 = "=SUM(D12)";
            worksheet.Cell("E16").FormulaA1 = "=SUM(D16/C16)*100";
            worksheet.Cell("E17").FormulaA1 = "=SUM(D17/C17)*100";
            worksheet.Cell("E19").FormulaA1 = "=SUM(D19/C19)*100";
            worksheet.Cell("E20").FormulaA1 = "=SUM(D20/C20)*100";
            worksheet.Cell("E21").FormulaA1 = "=SUM(D21/C21)*100";
            worksheet.Cell("E22").FormulaA1 = "=SUM(D22/C22)*100";
            worksheet.Cell("F16").Value = res.AverageVisitsPerDayWeight;
            worksheet.Cell("F17").Value = res.VisitsTargetAchievmentWeight;
            worksheet.Cell("F19").Value = res.APlusAndAWeight;
            worksheet.Cell("F20").Value = res.BWeight;
            worksheet.Cell("F21").Value = res.CWeight;
            worksheet.Cell("F22").Value = res.SellingDaysInTheFieldWeight;
            worksheet.Cell("G16").FormulaA1 = "=SUM(E16*F16)/100";
            worksheet.Cell("G17").FormulaA1 = "=SUM(E17*F17)/100";
            worksheet.Cell("G19").FormulaA1 = "=SUM(E19*F19)/100";
            worksheet.Cell("G20").FormulaA1 = "=SUM(E20*F20)/100";
            worksheet.Cell("G21").FormulaA1 = "=SUM(E21*F21)/100";
            worksheet.Cell("G22").FormulaA1 = "=SUM(E22*F22)/100";
            worksheet.Cell("G23").FormulaA1 = "=SUM(G16:G22)";
            worksheet.Cell("F2").Style.NumberFormat.Format = "0%";
            worksheet.Cell("F2").DataType = XLDataType.Number;

            worksheet.Cell("F2").FormulaA1 = "=SUM(ROUND((G23),0)/100)";

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 res.FullName+"' KPI.xlsx");
        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{yearto}/{monthto}/{ex}")]
        [HttpGet]
        public IActionResult GetAllMedicalKpi(int yearfrom, int monthfrom, int yearto, int monthto, string ex)
        {
            string exporter = userManager.FindByIdAsync(ex).Result.FullName;
            IEnumerable<MedicalKpiModel> result = rep.GetAllMedicalKpi(yearfrom, monthfrom,yearto,monthto);
            XLWorkbook workbook = new XLWorkbook();
            foreach (var res in result)
            {
                var worksheet = workbook.Worksheets.Add(res.FullName);
                worksheet.Range("D13:E14").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                worksheet.Cell("D13").Value = "Exported By";
                worksheet.Cell("D14").Value = "Exporting Date";
                worksheet.Cell("E13").Value = exporter;
                worksheet.Cell("E14").Value = ti.GetCurrentTime().ToString("dd/MM/yyyy");
                worksheet.Rows("1:25").Height = 20;
                worksheet.Cell("F2").Style.Font.FontSize = 75;
                worksheet.Range("B2:E2").Merge();
                worksheet.Range("C3:E3").Merge();
                worksheet.Range("C4:E4").Merge();
                worksheet.Range("C5:E5").Merge();
                worksheet.Range("C6:E6").Merge();
                worksheet.Range("F2:G7").Merge();
                worksheet.Range("B18:G18").Merge();
                worksheet.Cell("B8").Style.Font.Bold = true;
                worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Column("A").Width = 15;
                worksheet.Row(2).Height = 33;
                worksheet.Column("B").Width = 32;
                worksheet.Column("C").Width = 32;
                worksheet.Column("D").Width = 32;
                worksheet.Column("E").Width = 32;
                worksheet.Column("F").Width = 32;
                worksheet.Column("G").Width = 32;
                worksheet.Cell("B15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("C15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("D15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("E15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("F15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("G15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B16").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B17").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B19").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B20").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B21").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B22").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B23").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B2").Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                worksheet.Range("B2:G12").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B2:G12").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("F2:G7").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B8:B12").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B15:G23").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B15:G23").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("A1:G25").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:G25").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell("B2").Style.Font.Bold = true;
                worksheet.Cell("B2").Style.Font.FontSize = 26;
                worksheet.Range("B3:B6").Style.Font.Bold = true;
                worksheet.Cell("F2").Style.Font.Bold = true;
                worksheet.Cell("B23").Style.Font.Bold = true;
                worksheet.Cell("B23").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell("B23").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("C7:C12").Style.Font.Bold = true;
                worksheet.Range("E8:E12").Style.Font.Bold = true;
                worksheet.Range("B15:B23").Style.Font.Bold = true;
                worksheet.Range("C15:G15").Style.Font.Bold = true;

                worksheet.Cell("B2").Value = "Medical KPI";
                worksheet.Cell("B3").Value = "Name";
                worksheet.Cell("B4").Value = "Office";
                worksheet.Cell("B5").Value = "Team";
                worksheet.Cell("B6").Value = "Month";
                worksheet.Cell("C8").Value = "Actual Total Number Of Visits";
                worksheet.Cell("C9").Value = "Number Of A+ & A Contacts Visited";
                worksheet.Cell("C10").Value = "Number Of B Contacts Visited";
                worksheet.Cell("C11").Value = "Number Of C Contacts Visited";
                worksheet.Cell("C12").Value = "Selling Days In The Field";
                worksheet.Cell("E8").Value = "Targeted Number Of Visits";
                worksheet.Cell("E9").Value = "Number Of A+ & A Contacts Listed";
                worksheet.Cell("E10").Value = "Number Of B Contacts Listed";
                worksheet.Cell("E11").Value = "Number Of C Contacts Listed";
                worksheet.Cell("E12").Value = "Working Days";
                worksheet.Cell("B15").Value = "KPI";
                worksheet.Cell("C15").Value = "KPI Target";
                worksheet.Cell("D15").Value = "Actual";
                worksheet.Cell("E15").Value = "Rate";
                worksheet.Cell("F15").Value = "Weight";
                worksheet.Cell("G15").Value = "Score";
                worksheet.Cell("B16").Value = "Average Visits Per Day";
                worksheet.Cell("B17").Value = "Visits Target Acheivment";
                worksheet.Cell("B18").Value = "Coverage For Targeted Customers";
                worksheet.Cell("B19").Value = "A+ & A";
                worksheet.Cell("B20").Value = "B";
                worksheet.Cell("B21").Value = "C";
                worksheet.Cell("B22").Value = "Selling Days In The Field";
                worksheet.Cell("B23").Value = "Total Score";


                worksheet.Cell("C3").Value = res.FullName;
                worksheet.Cell("C4").Value = res.CityName;
                worksheet.Cell("C5").Value = res.RoleName;
                worksheet.Cell("C6").Value = res.Month;
                worksheet.Cell("D8").Value = res.ActualTotalNumberOfVisits;
                worksheet.Cell("D9").Value = res.APlusAndAVisited;
                worksheet.Cell("D10").Value = res.BVisited;
                worksheet.Cell("D11").Value = res.CVisited;
                worksheet.Cell("D12").Value = res.SellingDaysInTheField;
                //Targeted Number Of Visits KPI Target
                worksheet.Cell("F8").FormulaA1 = "=SUM(F12*C16)";
                worksheet.Cell("F9").Value = res.AplusAndAListed;
                worksheet.Cell("F10").Value = res.BListed;
                worksheet.Cell("F11").Value = res.CListed;
                worksheet.Cell("F12").Value = res.WorkingDays;
                worksheet.Cell("C16").Value = res.AverageVisitsPerDayKpiTarget;
                worksheet.Cell("C7").Value = "Time Off Territory Days";
                worksheet.Cell("D7").Value = res.TimeOffDays;
                //Visits Target Acheivment - KPI Target
                worksheet.Cell("C17").FormulaA1 = "=SUM(C16*C22)";
                worksheet.Cell("C19").FormulaA1 = "=SUM(F9)";
                worksheet.Cell("C20").FormulaA1 = "=SUM(F10)";
                worksheet.Cell("C21").FormulaA1 = "=SUM(F11)";
                worksheet.Cell("C22").FormulaA1 = "=SUM(F12)";
                //Average Visits Per Day - Actual
                worksheet.Cell("D16").FormulaA1 = "=SUM(D17/D22)";
                worksheet.Cell("D17").FormulaA1 = "=SUM(D8)";
                worksheet.Cell("D19").FormulaA1 = "=SUM(D9)";
                worksheet.Cell("D20").FormulaA1 = "=SUM(D10)";
                worksheet.Cell("D21").FormulaA1 = "=SUM(D11)";
                worksheet.Cell("D22").FormulaA1 = "=SUM(D12)";
                //Average Visits Per Day - Rate
                worksheet.Cell("E16").FormulaA1 = "=SUM(D16/C16)*100";
                //Visits Target Acheivment - Rate
                worksheet.Cell("E17").FormulaA1 = "=SUM(D17/C17)*100";
                //A+ & A - Rate
                worksheet.Cell("E19").FormulaA1 = "=SUM(D19/C19)*100";
                //B - Rate
                worksheet.Cell("E20").FormulaA1 = "=SUM(D20/C20)*100";
                //C - Rate
                worksheet.Cell("E21").FormulaA1 = "=SUM(D21/C21)*100";
                //Selling Days in The Field - Rate
                worksheet.Cell("E22").FormulaA1 = "=SUM(D22/C22)*100";
                worksheet.Cell("F16").Value = res.AverageVisitsPerDayWeight;
                worksheet.Cell("F17").Value = res.VisitsTargetAchievmentWeight;
                worksheet.Cell("F19").Value = res.APlusAndAWeight;
                worksheet.Cell("F20").Value = res.BWeight;
                worksheet.Cell("F21").Value = res.CWeight;
                worksheet.Cell("F22").Value = res.SellingDaysInTheFieldWeight;
                //Average Visits Per Day -  Score
                worksheet.Cell("G16").FormulaA1 = "=SUM(E16*F16)/100";
                //Visits Target Acheivment - Score
                worksheet.Cell("G17").FormulaA1 = "=SUM(E17*F17)/100";
                //A+ & A Score
                worksheet.Cell("G19").FormulaA1 = "=SUM(E19*F19)/100";
                //B Score
                worksheet.Cell("G20").FormulaA1 = "=SUM(E20*F20)/100";
                //C Score
                worksheet.Cell("G21").FormulaA1 = "=SUM(E21*F21)/100";
                //Selling Days in The Filed Score
                worksheet.Cell("G22").FormulaA1 = "=SUM(E22*F22)/100";
                //Total Score
                worksheet.Cell("G23").FormulaA1 = "=SUM(G16:G22)";
                worksheet.Cell("F2").Style.NumberFormat.Format = "0%";
                worksheet.Cell("F2").DataType = XLDataType.Number;
                //Score %
                worksheet.Cell("F2").FormulaA1 = "=SUM(ROUND((G23),0)/100)";
            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               "All Medical Representatives KPIs.xlsx");
        }



        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{yearto}/{monthto}")]
        [HttpGet]
        public IActionResult GetAllMedicalKpiChart(int yearfrom, int monthfrom, int yearto, int monthto)
        {
            return Ok(rep.GetAllMedicalKpiChart(yearfrom, monthfrom, yearto, monthto));
        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{yearto}/{monthto}/{managerId}")]
        [HttpGet]
        public IActionResult GetTeamMedicalKpiChart(int yearfrom, int monthfrom, int yearto, int monthto, string managerId)
        {
            return Ok(rep.GetTeamMedicalKpiChart(yearfrom, monthfrom, yearto, monthto, managerId));
        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{yearto}/{monthto}/{UserId}")]
        [HttpGet]
        public IActionResult GetMedicalKpiChart(int yearfrom, int monthfrom, int yearto, int monthto, string UserId)
        {
            return Ok(rep.GetMedicalKpiChart(yearfrom, monthfrom, yearto, monthto, UserId));
        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{yearto}/{monthto}/{managerId}/{ex}")]
        [HttpGet]
        public IActionResult GetTeamSalesKpiExcel(int yearfrom, int monthfrom, int yearto, int monthto, string managerId, string ex)
        {
            string exporter = userManager.FindByIdAsync(ex).Result.FullName;
            string name = userManager.FindByIdAsync(managerId).Result.FullName;
            IEnumerable<SalesKpiModel> result = rep.GetTeamSalesKpi(yearfrom, monthfrom, yearto, monthto, managerId);
            XLWorkbook workbook = new XLWorkbook();
            string m = "";
            foreach (var res in result)
            {
                var worksheet = workbook.Worksheets.Add(res.FullName);
                worksheet.Range("D12:E13").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                worksheet.Cell("D12").Value = "Exported By";
                worksheet.Cell("D13").Value = "Exporting Date";
                worksheet.Cell("E12").Value = exporter;
                worksheet.Cell("E13").Value = ti.GetCurrentTime().ToString("dd/MM/yyyy");
                worksheet.Rows("1:21").Height = 20;
                worksheet.Cell("F2").Style.Font.FontSize = 75;
                worksheet.Range("B2:E2").Merge();
                worksheet.Range("C3:E3").Merge();
                worksheet.Range("C4:E4").Merge();
                worksheet.Range("C5:E5").Merge();
                worksheet.Range("C6:E6").Merge();
                worksheet.Range("F2:G7").Merge();
                worksheet.Range("B18:G18").Merge();
                worksheet.Cell("B8").Style.Font.Bold = true;
                worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Column("A").Width = 15;
                worksheet.Row(2).Height = 33;
                worksheet.Column("B").Width = 32;
                worksheet.Column("C").Width = 32;
                worksheet.Column("D").Width = 32;
                worksheet.Column("E").Width = 32;
                worksheet.Column("F").Width = 32;
                worksheet.Column("G").Width = 32;
                worksheet.Cell("B15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("C15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("D15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("E15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("F15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("G15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B16").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B17").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B19").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B20").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B21").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B2").Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                worksheet.Range("B2:G10").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B2:G10").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("F2:G7").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B8:B10").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B15:G21").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B15:G21").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;




                worksheet.Range("A1:G21").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:G21").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell("B2").Style.Font.Bold = true;
                worksheet.Cell("B2").Style.Font.FontSize = 26;
                worksheet.Range("B3:B6").Style.Font.Bold = true;
                worksheet.Cell("F2").Style.Font.Bold = true;
                worksheet.Range("C7:C10").Style.Font.Bold = true;
                worksheet.Range("E8:E10").Style.Font.Bold = true;
                worksheet.Range("B15:B21").Style.Font.Bold = true;
                worksheet.Range("C15:G15").Style.Font.Bold = true;
                worksheet.Cell("B2").Value = "Sales KPI";
                worksheet.Cell("B3").Value = "Name";
                worksheet.Cell("B4").Value = "Office";
                worksheet.Cell("B5").Value = "Team";
                worksheet.Cell("B6").Value = "Month";

                worksheet.Cell("C8").Value = "Actual Total Number Of Visits";
                worksheet.Cell("C9").Value = "Listed Accounts";
                worksheet.Cell("C10").Value = "Selling Days In The Field";
                worksheet.Cell("E8").Value = "Targeted Number Of Visits";
                worksheet.Cell("E9").Value = "Visited Accounts";
                worksheet.Cell("E10").Value = "Working Days";

                worksheet.Cell("B15").Value = "KPI";
                worksheet.Cell("C15").Value = "KPI Target";
                worksheet.Cell("D15").Value = "Actual";
                worksheet.Cell("E15").Value = "Rate %";
                worksheet.Cell("F15").Value = "Weight";
                worksheet.Cell("G15").Value = "Score";
                worksheet.Cell("B16").Value = "Average Visits Per Day";
                worksheet.Cell("B17").Value = "Visits Target Achievment";
                worksheet.Cell("B19").Value = "Coverage For Listed Accounts";
                worksheet.Cell("B20").Value = "Selling Days In The Field";
                worksheet.Cell("B21").Value = "Total Score";
                worksheet.Cell("C3").Value = res.FullName;
                worksheet.Cell("C4").Value = res.CityName;
                worksheet.Cell("C5").Value = res.RoleName;
                worksheet.Cell("C6").Value = res.Month;
                worksheet.Cell("D8").Value = res.ActualTotalNumberOfVisits;
                worksheet.Cell("D9").Value = res.ListedAccounts;
                worksheet.Cell("D10").Value = res.SellingDaysInTheField;
                //worksheet.Cell("F8").Value = res.TargetedNumberOfVisits;
                worksheet.Cell("F8").FormulaA1 = "=SUM(F10*C16)";
                worksheet.Cell("F9").Value = res.VisitedAccounts;
                worksheet.Cell("F10").Value = res.WorkingDays;
                worksheet.Cell("C16").Value = res.AverageVisitsPerDayKpiTarget;
                //worksheet.Cell("C17").Value = res.TargetedNumberOfVisits;
                worksheet.Cell("C17").FormulaA1 = "=SUM(F8)";
                worksheet.Cell("C19").Value = res.ListedAccounts;
                worksheet.Cell("C20").Value = res.WorkingDays;

                //worksheet.Cell("D16").Value = res.AverageVisitsPerDayActual;
                //worksheet.Cell("D17").Value = res.ActualTotalNumberOfVisits;
                //worksheet.Cell("D19").Value = res.VisitedAccounts;
                //worksheet.Cell("D20").Value = res.SellingDaysInTheField;
                //worksheet.Cell("E16").Value = res.AverageVisitsPerDayRate;
                //worksheet.Cell("E17").Value = res.VisitsTargetAchievmentRate;
                //worksheet.Cell("E19").Value = res.CoverageForListedAccountsRate;
                //worksheet.Cell("E20").Value = res.SellingDaysInTheFieldKpiRate;
                worksheet.Cell("D16").FormulaA1 = "=SUM(D17/D20)";
                worksheet.Cell("D17").FormulaA1 = "=SUM(D8)";
                worksheet.Cell("D19").FormulaA1 = "=SUM(F9)";
                worksheet.Cell("D20").FormulaA1 = "=SUM(D10)";
                worksheet.Cell("E16").FormulaA1 = "=SUM(D16/C16)*100";
                worksheet.Cell("E17").FormulaA1 = "=SUM(D17/C17)*100";
                worksheet.Cell("E19").FormulaA1 = "=SUM(D19/C19)*100";
                worksheet.Cell("E20").FormulaA1 = "=SUM(D20/C20)*100";

                worksheet.Cell("F16").Value = res.AverageVisitsPerDayWeight;
                worksheet.Cell("F17").Value = res.VisitsTargetAchievmentWeight;
                worksheet.Cell("F19").Value = res.CoverageForListedAccountsWeight;
                worksheet.Cell("F20").Value = res.SellingDaysInTheFieldKpiWeight;
                worksheet.Cell("C7").Value = "Time Off Territory Days";
                worksheet.Cell("D7").Value = res.TimeOffDays;

                //worksheet.Cell("G16").Value = res.AverageVisitsPerDayScore;
                //worksheet.Cell("G17").Value = res.VisitsTargetAchievmentScore;
                //worksheet.Cell("G19").Value = res.CoverageForListedAccountsScore;
                //worksheet.Cell("G20").Value = res.SellingDaysInTheFieldScore;
                //worksheet.Cell("G21").Value = res.TotalScore;
                //worksheet.Cell("F2").Value = Math.Round(res.TotalScore)+"%";

                worksheet.Cell("G16").FormulaA1 = "=SUM(E16*F16)/100";
                worksheet.Cell("G17").FormulaA1 = "=SUM(E17*F17)/100";
                worksheet.Cell("G19").FormulaA1 = "=SUM(E19*F19)/100";
                worksheet.Cell("G20").FormulaA1 = "=SUM(E20*F20)/100";

                worksheet.Cell("G21").FormulaA1 = "=SUM(G16:G20)";
                worksheet.Cell("F2").Style.NumberFormat.Format = "0%";
                worksheet.Cell("F2").DataType = XLDataType.Number;

                worksheet.Cell("F2").FormulaA1 = "=SUM(ROUND((G21),0)/100)";
                m = res.Month;
            }
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + "'s Sales Team KPIs.xlsx");

        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{yearto}/{monthto}/{managerId}/{ex}")]
        [HttpGet]
        public IActionResult GetTeamSupportiveKpiExcel(int yearfrom, int monthfrom, int yearto, int monthto, string managerId, string ex)
        {
            string exporter = userManager.FindByIdAsync(ex).Result.FullName;
            string name = userManager.FindByIdAsync(managerId).Result.FullName;
            IEnumerable<SalesKpiModel> result = rep.GetTeamSupportiveKpi(yearfrom, monthfrom, yearto, monthto, managerId);
            XLWorkbook workbook = new XLWorkbook();
            string m = "";
            foreach (var res in result)
            {
                var worksheet = workbook.Worksheets.Add(res.FullName);
                worksheet.Range("D12:E13").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                worksheet.Cell("D12").Value = "Exported By";
                worksheet.Cell("D13").Value = "Exporting Date";
                worksheet.Cell("E12").Value = exporter;
                worksheet.Cell("E13").Value = ti.GetCurrentTime().ToString("dd/MM/yyyy");
                worksheet.Rows("1:21").Height = 20;
                worksheet.Cell("F2").Style.Font.FontSize = 75;
                worksheet.Range("B2:E2").Merge();
                worksheet.Range("C3:E3").Merge();
                worksheet.Range("C4:E4").Merge();
                worksheet.Range("C5:E5").Merge();
                worksheet.Range("C6:E6").Merge();
                worksheet.Range("F2:G7").Merge();
                worksheet.Range("B18:G18").Merge();
                worksheet.Cell("B8").Style.Font.Bold = true;
                worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Column("A").Width = 15;
                worksheet.Row(2).Height = 33;
                worksheet.Column("B").Width = 32;
                worksheet.Column("C").Width = 32;
                worksheet.Column("D").Width = 32;
                worksheet.Column("E").Width = 32;
                worksheet.Column("F").Width = 32;
                worksheet.Column("G").Width = 32;
                worksheet.Cell("B15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("C15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("D15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("E15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("F15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("G15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B16").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B17").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B19").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B20").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B21").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B2").Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                worksheet.Range("B2:G10").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B2:G10").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("F2:G7").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B8:B10").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B15:G21").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B15:G21").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;




                worksheet.Range("A1:G21").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:G21").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell("B2").Style.Font.Bold = true;
                worksheet.Cell("B2").Style.Font.FontSize = 26;
                worksheet.Range("B3:B6").Style.Font.Bold = true;
                worksheet.Cell("F2").Style.Font.Bold = true;
                worksheet.Range("C7:C10").Style.Font.Bold = true;
                worksheet.Range("E8:E10").Style.Font.Bold = true;
                worksheet.Range("B15:B21").Style.Font.Bold = true;
                worksheet.Range("C15:G15").Style.Font.Bold = true;
                worksheet.Cell("B2").Value = "Supportive KPI";
                worksheet.Cell("B3").Value = "Name";
                worksheet.Cell("B4").Value = "Office";
                worksheet.Cell("B5").Value = "Team";
                worksheet.Cell("B6").Value = "Month";

                worksheet.Cell("C8").Value = "Actual Total Number Of Visits";
                worksheet.Cell("C9").Value = "Listed Accounts";
                worksheet.Cell("C10").Value = "Selling Days In The Field";
                worksheet.Cell("E8").Value = "Targeted Number Of Visits";
                worksheet.Cell("E9").Value = "Visited Accounts";
                worksheet.Cell("E10").Value = "Working Days";

                worksheet.Cell("B15").Value = "KPI";
                worksheet.Cell("C15").Value = "KPI Target";
                worksheet.Cell("D15").Value = "Actual";
                worksheet.Cell("E15").Value = "Rate %";
                worksheet.Cell("F15").Value = "Weight";
                worksheet.Cell("G15").Value = "Score";
                worksheet.Cell("B16").Value = "Average Visits Per Day";
                worksheet.Cell("B17").Value = "Visits Target Achievment";
                worksheet.Cell("B19").Value = "Coverage For Listed Accounts";
                worksheet.Cell("B20").Value = "Selling Days In The Field";
                worksheet.Cell("B21").Value = "Total Score";
                worksheet.Cell("C3").Value = res.FullName;
                worksheet.Cell("C4").Value = res.CityName;
                worksheet.Cell("C5").Value = res.RoleName;
                worksheet.Cell("C6").Value = res.Month;
                worksheet.Cell("D8").Value = res.ActualTotalNumberOfVisits;
                worksheet.Cell("D9").Value = res.ListedAccounts;
                worksheet.Cell("D10").Value = res.SellingDaysInTheField;
                //worksheet.Cell("F8").Value = res.TargetedNumberOfVisits;
                worksheet.Cell("F8").FormulaA1 = "=SUM(F10*C16)";
                worksheet.Cell("F9").Value = res.VisitedAccounts;
                worksheet.Cell("F10").Value = res.WorkingDays;
                worksheet.Cell("C16").Value = res.AverageVisitsPerDayKpiTarget;
                //worksheet.Cell("C17").Value = res.TargetedNumberOfVisits;
                worksheet.Cell("C17").FormulaA1 = "=SUM(F8)";
                worksheet.Cell("C19").Value = res.ListedAccounts;
                worksheet.Cell("C20").Value = res.WorkingDays;

                //worksheet.Cell("D16").Value = res.AverageVisitsPerDayActual;
                //worksheet.Cell("D17").Value = res.ActualTotalNumberOfVisits;
                //worksheet.Cell("D19").Value = res.VisitedAccounts;
                //worksheet.Cell("D20").Value = res.SellingDaysInTheField;
                //worksheet.Cell("E16").Value = res.AverageVisitsPerDayRate;
                //worksheet.Cell("E17").Value = res.VisitsTargetAchievmentRate;
                //worksheet.Cell("E19").Value = res.CoverageForListedAccountsRate;
                //worksheet.Cell("E20").Value = res.SellingDaysInTheFieldKpiRate;
                worksheet.Cell("D16").FormulaA1 = "=SUM(D17/D20)";
                worksheet.Cell("D17").FormulaA1 = "=SUM(D8)";
                worksheet.Cell("D19").FormulaA1 = "=SUM(F9)";
                worksheet.Cell("D20").FormulaA1 = "=SUM(D10)";
                worksheet.Cell("E16").FormulaA1 = "=SUM(D16/C16)*100";
                worksheet.Cell("E17").FormulaA1 = "=SUM(D17/C17)*100";
                worksheet.Cell("E19").FormulaA1 = "=SUM(D19/C19)*100";
                worksheet.Cell("E20").FormulaA1 = "=SUM(D20/C20)*100";

                worksheet.Cell("F16").Value = res.AverageVisitsPerDayWeight;
                worksheet.Cell("F17").Value = res.VisitsTargetAchievmentWeight;
                worksheet.Cell("F19").Value = res.CoverageForListedAccountsWeight;
                worksheet.Cell("F20").Value = res.SellingDaysInTheFieldKpiWeight;
                worksheet.Cell("C7").Value = "Time Off Territory Days";
                worksheet.Cell("D7").Value = res.TimeOffDays;

                //worksheet.Cell("G16").Value = res.AverageVisitsPerDayScore;
                //worksheet.Cell("G17").Value = res.VisitsTargetAchievmentScore;
                //worksheet.Cell("G19").Value = res.CoverageForListedAccountsScore;
                //worksheet.Cell("G20").Value = res.SellingDaysInTheFieldScore;
                //worksheet.Cell("G21").Value = res.TotalScore;
                //worksheet.Cell("F2").Value = Math.Round(res.TotalScore)+"%";

                worksheet.Cell("G16").FormulaA1 = "=SUM(E16*F16)/100";
                worksheet.Cell("G17").FormulaA1 = "=SUM(E17*F17)/100";
                worksheet.Cell("G19").FormulaA1 = "=SUM(E19*F19)/100";
                worksheet.Cell("G20").FormulaA1 = "=SUM(E20*F20)/100";

                worksheet.Cell("G21").FormulaA1 = "=SUM(G16:G20)";
                worksheet.Cell("F2").Style.NumberFormat.Format = "0%";
                worksheet.Cell("F2").DataType = XLDataType.Number;

                worksheet.Cell("F2").FormulaA1 = "=SUM(ROUND((G21),0)/100)";
                m = res.Month;
            }
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                name + "'s Supportive Team KPIs.xlsx");

        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{yearto}/{monthto}/{ex}")]
        [HttpGet]
        public IActionResult GetAllSalesKpiExcel(int yearfrom, int monthfrom, int yearto, int monthto, string ex)
        {
            string exporter = userManager.FindByIdAsync(ex).Result.FullName;
            IEnumerable<SalesKpiModel> result = rep.GetAllSalesKpi(yearfrom, monthfrom,yearto,monthto);
            XLWorkbook workbook = new XLWorkbook();
            string m = "";
            foreach (var res in result)
            {
                var worksheet = workbook.Worksheets.Add(res.FullName);
                worksheet.Range("D12:E13").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                worksheet.Cell("D12").Value = "Exported By";
                worksheet.Cell("D13").Value = "Exporting Date";
                worksheet.Cell("E12").Value = exporter;
                worksheet.Cell("E13").Value = ti.GetCurrentTime().ToString("dd/MM/yyyy");
                worksheet.Rows("1:21").Height = 20;
                worksheet.Cell("F2").Style.Font.FontSize = 75;
                worksheet.Range("B2:E2").Merge();
                worksheet.Range("C3:E3").Merge();
                worksheet.Range("C4:E4").Merge();
                worksheet.Range("C5:E5").Merge();
                worksheet.Range("C6:E6").Merge();
                worksheet.Range("F2:G7").Merge();
                worksheet.Range("B18:G18").Merge();
                worksheet.Cell("B8").Style.Font.Bold = true;
                worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Column("A").Width = 15;
                worksheet.Row(2).Height = 33;
                worksheet.Column("B").Width = 32;
                worksheet.Column("C").Width = 32;
                worksheet.Column("D").Width = 32;
                worksheet.Column("E").Width = 32;
                worksheet.Column("F").Width = 32;
                worksheet.Column("G").Width = 32;
                worksheet.Cell("B15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("C15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("D15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("E15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("F15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("G15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B16").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B17").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B19").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B20").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B21").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B2").Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                worksheet.Range("B2:G10").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B2:G10").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("F2:G7").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B8:B10").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B15:G21").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B15:G21").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;




                worksheet.Range("A1:G21").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:G21").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell("B2").Style.Font.Bold = true;
                worksheet.Cell("B2").Style.Font.FontSize = 26;
                worksheet.Range("B3:B6").Style.Font.Bold = true;
                worksheet.Cell("F2").Style.Font.Bold = true;
                worksheet.Range("C7:C10").Style.Font.Bold = true;
                worksheet.Range("E8:E10").Style.Font.Bold = true;
                worksheet.Range("B15:B21").Style.Font.Bold = true;
                worksheet.Range("C15:G15").Style.Font.Bold = true;
                worksheet.Cell("B2").Value = "Sales KPI";
                worksheet.Cell("B3").Value = "Name";
                worksheet.Cell("B4").Value = "Office";
                worksheet.Cell("B5").Value = "Team";
                worksheet.Cell("B6").Value = "Month";

                worksheet.Cell("C8").Value = "Actual Total Number Of Visits";
                worksheet.Cell("C9").Value = "Listed Accounts";
                worksheet.Cell("C10").Value = "Selling Days In The Field";
                worksheet.Cell("E8").Value = "Targeted Number Of Visits";
                worksheet.Cell("E9").Value = "Visited Accounts";
                worksheet.Cell("E10").Value = "Working Days";

                worksheet.Cell("B15").Value = "KPI";
                worksheet.Cell("C15").Value = "KPI Target";
                worksheet.Cell("D15").Value = "Actual";
                worksheet.Cell("E15").Value = "Rate %";
                worksheet.Cell("F15").Value = "Weight";
                worksheet.Cell("G15").Value = "Score";
                worksheet.Cell("B16").Value = "Average Visits Per Day";
                worksheet.Cell("B17").Value = "Visits Target Achievment";
                worksheet.Cell("B19").Value = "Coverage For Listed Accounts";
                worksheet.Cell("B20").Value = "Selling Days In The Field";
                worksheet.Cell("B21").Value = "Total Score";
                worksheet.Cell("C3").Value = res.FullName;
                worksheet.Cell("C4").Value = res.CityName;
                worksheet.Cell("C5").Value = res.RoleName;
                worksheet.Cell("C6").Value = res.Month;
                worksheet.Cell("D8").Value = res.ActualTotalNumberOfVisits;
                worksheet.Cell("D9").Value = res.ListedAccounts;
                worksheet.Cell("D10").Value = res.SellingDaysInTheField;
                //worksheet.Cell("F8").Value = res.TargetedNumberOfVisits;
                worksheet.Cell("F8").FormulaA1 = "=SUM(F10*C16)";
                worksheet.Cell("F9").Value = res.VisitedAccounts;
                worksheet.Cell("F10").Value = res.WorkingDays;
                worksheet.Cell("C16").Value = res.AverageVisitsPerDayKpiTarget;
                //worksheet.Cell("C17").Value = res.TargetedNumberOfVisits;
                worksheet.Cell("C17").FormulaA1 = "=SUM(F8)";
                worksheet.Cell("C19").Value = res.ListedAccounts;
                worksheet.Cell("C20").Value = res.WorkingDays;

                //worksheet.Cell("D16").Value = res.AverageVisitsPerDayActual;
                //worksheet.Cell("D17").Value = res.ActualTotalNumberOfVisits;
                //worksheet.Cell("D19").Value = res.VisitedAccounts;
                //worksheet.Cell("D20").Value = res.SellingDaysInTheField;
                //worksheet.Cell("E16").Value = res.AverageVisitsPerDayRate;
                //worksheet.Cell("E17").Value = res.VisitsTargetAchievmentRate;
                //worksheet.Cell("E19").Value = res.CoverageForListedAccountsRate;
                //worksheet.Cell("E20").Value = res.SellingDaysInTheFieldKpiRate;

                worksheet.Cell("D16").FormulaA1 = "=SUM(D17/D20)";
                worksheet.Cell("D17").FormulaA1 = "=SUM(D8)";
                worksheet.Cell("D19").FormulaA1 = "=SUM(F9)";
                worksheet.Cell("D20").FormulaA1 = "=SUM(D10)";
                worksheet.Cell("E16").FormulaA1 = "=SUM(D16/C16)*100";
                worksheet.Cell("E17").FormulaA1 = "=SUM(D17/C17)*100";
                worksheet.Cell("E19").FormulaA1 = "=SUM(D19/C19)*100";
                worksheet.Cell("E20").FormulaA1 = "=SUM(D20/C20)*100";

                worksheet.Cell("F16").Value = res.AverageVisitsPerDayWeight;
                worksheet.Cell("F17").Value = res.VisitsTargetAchievmentWeight;
                worksheet.Cell("F19").Value = res.CoverageForListedAccountsWeight;
                worksheet.Cell("F20").Value = res.SellingDaysInTheFieldKpiWeight;
                worksheet.Cell("C7").Value = "Time Off Territory Days";
                worksheet.Cell("D7").Value = res.TimeOffDays;
                //worksheet.Cell("G16").Value = res.AverageVisitsPerDayScore;
                //worksheet.Cell("G17").Value = res.VisitsTargetAchievmentScore;
                //worksheet.Cell("G19").Value = res.CoverageForListedAccountsScore;
                //worksheet.Cell("G20").Value = res.SellingDaysInTheFieldScore;
                //worksheet.Cell("G21").Value = res.TotalScore;
                //worksheet.Cell("F2").Value = Math.Round(res.TotalScore)+"%";

                worksheet.Cell("G16").FormulaA1 = "=SUM(E16*F16)/100";
                worksheet.Cell("G17").FormulaA1 = "=SUM(E17*F17)/100";
                worksheet.Cell("G19").FormulaA1 = "=SUM(E19*F19)/100";
                worksheet.Cell("G20").FormulaA1 = "=SUM(E20*F20)/100";

                worksheet.Cell("G21").FormulaA1 = "=SUM(G16:G20)";
                worksheet.Cell("F2").Style.NumberFormat.Format = "0%";
                worksheet.Cell("F2").DataType = XLDataType.Number;

                worksheet.Cell("F2").FormulaA1 = "=SUM(ROUND((G21),0)/100)";
                m = res.Month;
                
            }
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "All Sales KPIs.xlsx");

        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{yearto}/{monthto}/{ex}")]
        [HttpGet]
        public IActionResult GetAllSupportiveKpiExcel(int yearfrom, int monthfrom, int yearto, int monthto, string ex)
        {
            string exporter = userManager.FindByIdAsync(ex).Result.FullName;
            IEnumerable<SalesKpiModel> result = rep.GetAllSupportiveKpi(yearfrom, monthfrom,yearto,monthto);
            XLWorkbook workbook = new XLWorkbook();
            string m = "";
            foreach (var res in result)
            {
                var worksheet = workbook.Worksheets.Add(res.FullName);
                worksheet.Range("D12:E13").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                worksheet.Cell("D12").Value = "Exported By";
                worksheet.Cell("D13").Value = "Exporting Date";
                worksheet.Cell("E12").Value = exporter;
                worksheet.Cell("E13").Value = ti.GetCurrentTime().ToString("dd/MM/yyyy");
                worksheet.Rows("1:21").Height = 20;
                worksheet.Cell("F2").Style.Font.FontSize = 75;
                worksheet.Range("B2:E2").Merge();
                worksheet.Range("C3:E3").Merge();
                worksheet.Range("C4:E4").Merge();
                worksheet.Range("C5:E5").Merge();
                worksheet.Range("C6:E6").Merge();
                worksheet.Range("F2:G7").Merge();
                worksheet.Range("B18:G18").Merge();
                worksheet.Cell("B8").Style.Font.Bold = true;
                worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Column("A").Width = 15;
                worksheet.Row(2).Height = 33;
                worksheet.Column("B").Width = 32;
                worksheet.Column("C").Width = 32;
                worksheet.Column("D").Width = 32;
                worksheet.Column("E").Width = 32;
                worksheet.Column("F").Width = 32;
                worksheet.Column("G").Width = 32;
                worksheet.Cell("B15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("C15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("D15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("E15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("F15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("G15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B16").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B17").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B19").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B20").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B21").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
                worksheet.Cell("B2").Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                worksheet.Range("B2:G10").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B2:G10").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("F2:G7").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B8:B10").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B15:G21").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("B15:G21").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;




                worksheet.Range("A1:G21").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:G21").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell("B2").Style.Font.Bold = true;
                worksheet.Cell("B2").Style.Font.FontSize = 26;
                worksheet.Range("B3:B6").Style.Font.Bold = true;
                worksheet.Cell("F2").Style.Font.Bold = true;
                worksheet.Range("C7:C10").Style.Font.Bold = true;
                worksheet.Range("E8:E10").Style.Font.Bold = true;
                worksheet.Range("B15:B21").Style.Font.Bold = true;
                worksheet.Range("C15:G15").Style.Font.Bold = true;
                worksheet.Cell("B2").Value = "Supportive KPI";
                worksheet.Cell("B3").Value = "Name";
                worksheet.Cell("B4").Value = "Office";
                worksheet.Cell("B5").Value = "Team";
                worksheet.Cell("B6").Value = "Month";

                worksheet.Cell("C8").Value = "Actual Total Number Of Visits";
                worksheet.Cell("C9").Value = "Listed Accounts";
                worksheet.Cell("C10").Value = "Selling Days In The Field";
                worksheet.Cell("E8").Value = "Targeted Number Of Visits";
                worksheet.Cell("E9").Value = "Visited Accounts";
                worksheet.Cell("E10").Value = "Working Days";

                worksheet.Cell("B15").Value = "KPI";
                worksheet.Cell("C15").Value = "KPI Target";
                worksheet.Cell("D15").Value = "Actual";
                worksheet.Cell("E15").Value = "Rate %";
                worksheet.Cell("F15").Value = "Weight";
                worksheet.Cell("G15").Value = "Score";
                worksheet.Cell("B16").Value = "Average Visits Per Day";
                worksheet.Cell("B17").Value = "Visits Target Achievment";
                worksheet.Cell("B19").Value = "Coverage For Listed Accounts";
                worksheet.Cell("B20").Value = "Selling Days In The Field";
                worksheet.Cell("B21").Value = "Total Score";
                worksheet.Cell("C3").Value = res.FullName;
                worksheet.Cell("C4").Value = res.CityName;
                worksheet.Cell("C5").Value = res.RoleName;
                worksheet.Cell("C6").Value = res.Month;
                worksheet.Cell("D8").Value = res.ActualTotalNumberOfVisits;
                worksheet.Cell("D9").Value = res.ListedAccounts;
                worksheet.Cell("D10").Value = res.SellingDaysInTheField;
                //worksheet.Cell("F8").Value = res.TargetedNumberOfVisits;
                worksheet.Cell("F8").FormulaA1 = "=SUM(F10*C16)";
                worksheet.Cell("F9").Value = res.VisitedAccounts;
                worksheet.Cell("F10").Value = res.WorkingDays;
                worksheet.Cell("C16").Value = res.AverageVisitsPerDayKpiTarget;
                //worksheet.Cell("C17").Value = res.TargetedNumberOfVisits;
                worksheet.Cell("C17").FormulaA1 = "=SUM(F8)";
                worksheet.Cell("C19").Value = res.ListedAccounts;
                worksheet.Cell("C20").Value = res.WorkingDays;

                //worksheet.Cell("D16").Value = res.AverageVisitsPerDayActual;
                //worksheet.Cell("D17").Value = res.ActualTotalNumberOfVisits;
                //worksheet.Cell("D19").Value = res.VisitedAccounts;
                //worksheet.Cell("D20").Value = res.SellingDaysInTheField;
                //worksheet.Cell("E16").Value = res.AverageVisitsPerDayRate;
                //worksheet.Cell("E17").Value = res.VisitsTargetAchievmentRate;
                //worksheet.Cell("E19").Value = res.CoverageForListedAccountsRate;
                //worksheet.Cell("E20").Value = res.SellingDaysInTheFieldKpiRate;
                worksheet.Cell("D16").FormulaA1 = "=SUM(D17/D20)";
                worksheet.Cell("D17").FormulaA1 = "=SUM(D8)";
                worksheet.Cell("D19").FormulaA1 = "=SUM(F9)";
                worksheet.Cell("D20").FormulaA1 = "=SUM(D10)";
                worksheet.Cell("E16").FormulaA1 = "=SUM(D16/C16)*100";
                worksheet.Cell("E17").FormulaA1 = "=SUM(D17/C17)*100";
                worksheet.Cell("E19").FormulaA1 = "=SUM(D19/C19)*100";
                worksheet.Cell("E20").FormulaA1 = "=SUM(D20/C20)*100";

                worksheet.Cell("F16").Value = res.AverageVisitsPerDayWeight;
                worksheet.Cell("F17").Value = res.VisitsTargetAchievmentWeight;
                worksheet.Cell("F19").Value = res.CoverageForListedAccountsWeight;
                worksheet.Cell("F20").Value = res.SellingDaysInTheFieldKpiWeight;
                worksheet.Cell("C7").Value = "Time Off Territory Days";
                worksheet.Cell("D7").Value = res.TimeOffDays;
                //worksheet.Cell("G16").Value = res.AverageVisitsPerDayScore;
                //worksheet.Cell("G17").Value = res.VisitsTargetAchievmentScore;
                //worksheet.Cell("G19").Value = res.CoverageForListedAccountsScore;
                //worksheet.Cell("G20").Value = res.SellingDaysInTheFieldScore;
                //worksheet.Cell("G21").Value = res.TotalScore;
                //worksheet.Cell("F2").Value = Math.Round(res.TotalScore)+"%";

                worksheet.Cell("G16").FormulaA1 = "=SUM(E16*F16)/100";
                worksheet.Cell("G17").FormulaA1 = "=SUM(E17*F17)/100";
                worksheet.Cell("G19").FormulaA1 = "=SUM(E19*F19)/100";
                worksheet.Cell("G20").FormulaA1 = "=SUM(E20*F20)/100";

                worksheet.Cell("G21").FormulaA1 = "=SUM(G16:G20)";
                worksheet.Cell("F2").Style.NumberFormat.Format = "0%";
                worksheet.Cell("F2").DataType = XLDataType.Number;

                worksheet.Cell("F2").FormulaA1 = "=SUM(ROUND((G21),0)/100)";
                m = res.Month;
            }
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "All Supportive KPIs.xlsx");

        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{yearto}/{monthto}/{userId}/{ex}")]
        [HttpGet]
        public IActionResult GetSalesKpiExcel(int yearfrom, int monthfrom, int yearto, int monthto, string userId, string ex)
        {
            string exporter = userManager.FindByIdAsync(ex).Result.FullName;
            SalesKpiModel res = rep.GetSalesKpi(yearfrom, monthfrom, yearto, monthto,userId);
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(res.FullName);
            worksheet.Range("D12:E13").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
            worksheet.Cell("D12").Value = "Exported By";
            worksheet.Cell("D13").Value = "Exporting Date";
            worksheet.Cell("E12").Value = exporter;
            worksheet.Cell("E13").Value = ti.GetCurrentTime().ToString("dd/MM/yyyy");
            worksheet.Rows("1:21").Height = 20;
            worksheet.Cell("F2").Style.Font.FontSize = 75;
            worksheet.Range("B2:E2").Merge();
            worksheet.Range("C3:E3").Merge();
            worksheet.Range("C4:E4").Merge();
            worksheet.Range("C5:E5").Merge();
            worksheet.Range("C6:E6").Merge();
            worksheet.Range("F2:G7").Merge();
            worksheet.Range("B18:G18").Merge();
            worksheet.Cell("B8").Style.Font.Bold = true;
            worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Column("A").Width = 15;
            worksheet.Row(2).Height = 33;
            worksheet.Column("B").Width = 32;
            worksheet.Column("C").Width = 32;
            worksheet.Column("D").Width = 32; 
            worksheet.Column("E").Width = 32;
            worksheet.Column("F").Width = 32;
            worksheet.Column("G").Width = 32;
            worksheet.Cell("B15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("C15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("D15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("E15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("F15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("G15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B16").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B17").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B19").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B20").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B21").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B2").Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Range("B2:G10").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("B2:G10").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("F2:G7").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("B8:B10").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("B15:G21").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("B15:G21").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
       



            worksheet.Range("A1:G21").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:G21").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell("B2").Style.Font.Bold = true;
            worksheet.Cell("B2").Style.Font.FontSize = 26;
            worksheet.Range("B3:B6").Style.Font.Bold = true;
            worksheet.Cell("F2").Style.Font.Bold = true;
            worksheet.Range("C7:C10").Style.Font.Bold = true;
            worksheet.Range("E8:E10").Style.Font.Bold = true;
            worksheet.Range("B15:B21").Style.Font.Bold = true;
            worksheet.Range("C15:G15").Style.Font.Bold = true;
            worksheet.Cell("B2").Value = "Sales KPI";
            worksheet.Cell("B3").Value = "Name";
            worksheet.Cell("B4").Value = "Office";
            worksheet.Cell("B5").Value = "Team";
            worksheet.Cell("B6").Value = "Month";
          
            worksheet.Cell("C8").Value = "Actual Total Number Of Visits";
            worksheet.Cell("C9").Value = "Listed Accounts";
            worksheet.Cell("C10").Value = "Selling Days In The Field";
            worksheet.Cell("E8").Value = "Targeted Number Of Visits";
            worksheet.Cell("E9").Value = "Visited Accounts";
            worksheet.Cell("E10").Value = "Working Days";
     
            worksheet.Cell("B15").Value = "KPI";
            worksheet.Cell("C15").Value = "KPI Target";
            worksheet.Cell("D15").Value = "Actual";
            worksheet.Cell("E15").Value = "Rate %";
            worksheet.Cell("F15").Value = "Weight";
            worksheet.Cell("G15").Value = "Score";
            worksheet.Cell("B16").Value = "Average Visits Per Day";
            worksheet.Cell("B17").Value = "Visits Target Achievment";
            worksheet.Cell("B19").Value = "Coverage For Listed Accounts";
            worksheet.Cell("B20").Value = "Selling Days In The Field";
            worksheet.Cell("B21").Value = "Total Score";
            worksheet.Cell("C3").Value = res.FullName;
            worksheet.Cell("C4").Value = res.CityName;
            worksheet.Cell("C5").Value = res.RoleName;
            worksheet.Cell("C6").Value = res.Month;
            worksheet.Cell("D8").Value = res.ActualTotalNumberOfVisits;
            worksheet.Cell("D9").Value = res.ListedAccounts;
            worksheet.Cell("D10").Value = res.SellingDaysInTheField;
            //worksheet.Cell("F8").Value = res.TargetedNumberOfVisits;
            worksheet.Cell("F8").FormulaA1 = "=SUM(F10*C16)";
            worksheet.Cell("F9").Value = res.VisitedAccounts;
            worksheet.Cell("F10").Value = res.WorkingDays;
            worksheet.Cell("C16").Value = res.AverageVisitsPerDayKpiTarget;
            //worksheet.Cell("C17").Value = res.TargetedNumberOfVisits;
            worksheet.Cell("C17").FormulaA1 = "=SUM(F8)";
            worksheet.Cell("C19").Value = res.ListedAccounts;
            worksheet.Cell("C20").Value = res.WorkingDays;

            
            //worksheet.Cell("D16").Value = res.AverageVisitsPerDayActual;
            //worksheet.Cell("D17").Value = res.ActualTotalNumberOfVisits;
            //worksheet.Cell("D19").Value = res.VisitedAccounts;
            //worksheet.Cell("D20").Value = res.SellingDaysInTheField;
            //worksheet.Cell("E16").Value = res.AverageVisitsPerDayRate;
            //worksheet.Cell("E17").Value = res.VisitsTargetAchievmentRate;
            //worksheet.Cell("E19").Value = res.CoverageForListedAccountsRate;
            //worksheet.Cell("E20").Value = res.SellingDaysInTheFieldKpiRate;
            worksheet.Cell("D16").FormulaA1 = "=SUM(D17/D20)";
            worksheet.Cell("D17").FormulaA1 = "=SUM(D8)";
            worksheet.Cell("D19").FormulaA1 = "=SUM(F9)";
            worksheet.Cell("D20").FormulaA1 = "=SUM(D10)";
            worksheet.Cell("E16").FormulaA1 = "=SUM(D16/C16)*100";
            worksheet.Cell("E17").FormulaA1 = "=SUM(D17/C17)*100";
            worksheet.Cell("E19").FormulaA1 = "=SUM(D19/C19)*100";
            worksheet.Cell("E20").FormulaA1 = "=SUM(D20/C20)*100";

            worksheet.Cell("F16").Value = res.AverageVisitsPerDayWeight;
            worksheet.Cell("F17").Value = res.VisitsTargetAchievmentWeight;
            worksheet.Cell("F19").Value = res.CoverageForListedAccountsWeight;
            worksheet.Cell("F20").Value = res.SellingDaysInTheFieldKpiWeight;
            worksheet.Cell("C7").Value = "Time Off Territory Days";
            worksheet.Cell("D7").Value = res.TimeOffDays;
            //worksheet.Cell("G16").Value = res.AverageVisitsPerDayScore;
            //worksheet.Cell("G17").Value = res.VisitsTargetAchievmentScore;
            //worksheet.Cell("G19").Value = res.CoverageForListedAccountsScore;
            //worksheet.Cell("G20").Value = res.SellingDaysInTheFieldScore;
            //worksheet.Cell("G21").Value = res.TotalScore;
            //worksheet.Cell("F2").Value = Math.Round(res.TotalScore)+"%";

            worksheet.Cell("G16").FormulaA1 = "=SUM(E16*F16)/100";
            worksheet.Cell("G17").FormulaA1 = "=SUM(E17*F17)/100";
            worksheet.Cell("G19").FormulaA1 = "=SUM(E19*F19)/100";
            worksheet.Cell("G20").FormulaA1 = "=SUM(E20*F20)/100";

            worksheet.Cell("G21").FormulaA1 = "=SUM(G16:G20)";
            worksheet.Cell("F2").Style.NumberFormat.Format = "0%";
            worksheet.Cell("F2").DataType = XLDataType.Number;

            worksheet.Cell("F2").FormulaA1 = "=SUM(ROUND((G21),0)/100)";



            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                res.FullName+"' KPI.xlsx");
      
        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{yearto}/{monthto}/{userId}/{ex}")]
        [HttpGet]
        public IActionResult GetSupportiveKpiExcel(int yearfrom, int monthfrom, int yearto, int monthto, string userId, string ex)
        {
            string exporter = userManager.FindByIdAsync(ex).Result.FullName;
            SalesKpiModel res = rep.GetSupportiveKpi(yearfrom, monthfrom, yearto, monthto, userId);
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(res.FullName);
            worksheet.Range("D12:E13").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
            worksheet.Cell("D12").Value = "Exported By";
            worksheet.Cell("D13").Value = "Exporting Date";
            worksheet.Cell("E12").Value = exporter;
            worksheet.Cell("E13").Value = ti.GetCurrentTime().ToString("dd/MM/yyyy");
            worksheet.Rows("1:21").Height = 20;
            worksheet.Cell("F2").Style.Font.FontSize = 75;
            worksheet.Range("B2:E2").Merge();
            worksheet.Range("C3:E3").Merge();
            worksheet.Range("C4:E4").Merge();
            worksheet.Range("C5:E5").Merge();
            worksheet.Range("C6:E6").Merge();
            worksheet.Range("F2:G7").Merge();
            worksheet.Range("B18:G18").Merge();
            worksheet.Cell("B8").Style.Font.Bold = true;
            worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Column("A").Width = 15;
            worksheet.Row(2).Height = 33;
            worksheet.Column("B").Width = 32;
            worksheet.Column("C").Width = 32;
            worksheet.Column("D").Width = 32;
            worksheet.Column("E").Width = 32;
            worksheet.Column("F").Width = 32;
            worksheet.Column("G").Width = 32;
            worksheet.Cell("B15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("C15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("D15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("E15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("F15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("G15").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B16").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B17").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B18").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B19").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B20").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B21").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 192, 192);
            worksheet.Cell("B2").Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Range("B2:G10").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("B2:G10").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("F2:G7").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("B8:B10").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("B15:G21").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("B15:G21").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;




            worksheet.Range("A1:G21").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:G21").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell("B2").Style.Font.Bold = true;
            worksheet.Cell("B2").Style.Font.FontSize = 26;
            worksheet.Range("B3:B6").Style.Font.Bold = true;
            worksheet.Cell("F2").Style.Font.Bold = true;
            worksheet.Range("C7:C10").Style.Font.Bold = true;
            worksheet.Range("E8:E10").Style.Font.Bold = true;
            worksheet.Range("B15:B21").Style.Font.Bold = true;
            worksheet.Range("C15:G15").Style.Font.Bold = true;
            worksheet.Cell("B2").Value = "Supportive KPI";
            worksheet.Cell("B3").Value = "Name";
            worksheet.Cell("B4").Value = "Office";
            worksheet.Cell("B5").Value = "Team";
            worksheet.Cell("B6").Value = "Month";

            worksheet.Cell("C8").Value = "Actual Total Number Of Visits";
            worksheet.Cell("C9").Value = "Listed Accounts";
            worksheet.Cell("C10").Value = "Selling Days In The Field";
            worksheet.Cell("E8").Value = "Targeted Number Of Visits";
            worksheet.Cell("E9").Value = "Visited Accounts";
            worksheet.Cell("E10").Value = "Working Days";

            worksheet.Cell("B15").Value = "KPI";
            worksheet.Cell("C15").Value = "KPI Target";
            worksheet.Cell("D15").Value = "Actual";
            worksheet.Cell("E15").Value = "Rate %";
            worksheet.Cell("F15").Value = "Weight";
            worksheet.Cell("G15").Value = "Score";
            worksheet.Cell("B16").Value = "Average Visits Per Day";
            worksheet.Cell("B17").Value = "Visits Target Achievment";
            worksheet.Cell("B19").Value = "Coverage For Listed Accounts";
            worksheet.Cell("B20").Value = "Selling Days In The Field";
            worksheet.Cell("B21").Value = "Total Score";
            worksheet.Cell("C3").Value = res.FullName;
            worksheet.Cell("C4").Value = res.CityName;
            worksheet.Cell("C5").Value = res.RoleName;
            worksheet.Cell("C6").Value = res.Month;
            worksheet.Cell("D8").Value = res.ActualTotalNumberOfVisits;
            worksheet.Cell("D9").Value = res.ListedAccounts;
            worksheet.Cell("D10").Value = res.SellingDaysInTheField;
            //worksheet.Cell("F8").Value = res.TargetedNumberOfVisits;
            worksheet.Cell("F8").FormulaA1 = "=SUM(F10*C16)";
            worksheet.Cell("F9").Value = res.VisitedAccounts;
            worksheet.Cell("F10").Value = res.WorkingDays;
            worksheet.Cell("C16").Value = res.AverageVisitsPerDayKpiTarget;
            //worksheet.Cell("C17").Value = res.TargetedNumberOfVisits;
            worksheet.Cell("C17").FormulaA1 = "=SUM(F8)";
            worksheet.Cell("C19").Value = res.ListedAccounts;
            worksheet.Cell("C20").Value = res.WorkingDays;


            //worksheet.Cell("D16").Value = res.AverageVisitsPerDayActual;
            //worksheet.Cell("D17").Value = res.ActualTotalNumberOfVisits;
            //worksheet.Cell("D19").Value = res.VisitedAccounts;
            //worksheet.Cell("D20").Value = res.SellingDaysInTheField;
            //worksheet.Cell("E16").Value = res.AverageVisitsPerDayRate;
            //worksheet.Cell("E17").Value = res.VisitsTargetAchievmentRate;
            //worksheet.Cell("E19").Value = res.CoverageForListedAccountsRate;
            //worksheet.Cell("E20").Value = res.SellingDaysInTheFieldKpiRate;
            worksheet.Cell("D16").FormulaA1 = "=SUM(D17/D20)";
            worksheet.Cell("D17").FormulaA1 = "=SUM(D8)";
            worksheet.Cell("D19").FormulaA1 = "=SUM(F9)";
            worksheet.Cell("D20").FormulaA1 = "=SUM(D10)";
            worksheet.Cell("E16").FormulaA1 = "=SUM(D16/C16)*100";
            worksheet.Cell("E17").FormulaA1 = "=SUM(D17/C17)*100";
            worksheet.Cell("E19").FormulaA1 = "=SUM(D19/C19)*100";
            worksheet.Cell("E20").FormulaA1 = "=SUM(D20/C20)*100";

            worksheet.Cell("F16").Value = res.AverageVisitsPerDayWeight;
            worksheet.Cell("F17").Value = res.VisitsTargetAchievmentWeight;
            worksheet.Cell("F19").Value = res.CoverageForListedAccountsWeight;
            worksheet.Cell("F20").Value = res.SellingDaysInTheFieldKpiWeight;
            worksheet.Cell("C7").Value = "Time Off Territory Days";
            worksheet.Cell("D7").Value = res.TimeOffDays;
            //worksheet.Cell("G16").Value = res.AverageVisitsPerDayScore;
            //worksheet.Cell("G17").Value = res.VisitsTargetAchievmentScore;
            //worksheet.Cell("G19").Value = res.CoverageForListedAccountsScore;
            //worksheet.Cell("G20").Value = res.SellingDaysInTheFieldScore;
            //worksheet.Cell("G21").Value = res.TotalScore;
            //worksheet.Cell("F2").Value = Math.Round(res.TotalScore)+"%";

            worksheet.Cell("G16").FormulaA1 = "=SUM(E16*F16)/100";
            worksheet.Cell("G17").FormulaA1 = "=SUM(E17*F17)/100";
            worksheet.Cell("G19").FormulaA1 = "=SUM(E19*F19)/100";
            worksheet.Cell("G20").FormulaA1 = "=SUM(E20*F20)/100";

            worksheet.Cell("G21").FormulaA1 = "=SUM(G16:G20)";
            worksheet.Cell("F2").Style.NumberFormat.Format = "0%";
            worksheet.Cell("F2").DataType = XLDataType.Number;

            worksheet.Cell("F2").FormulaA1 = "=SUM(ROUND((G21),0)/100)";



            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                res.FullName + "' KPI.xlsx");

        }


    }
}

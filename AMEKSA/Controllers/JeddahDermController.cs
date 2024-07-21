using AMEKSA.Models;
using AMEKSA.Repo;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class JeddahDermController : ControllerBase
    {
        private readonly IJeddahDermRep rep;
        private readonly ITimeRep ti;

        public JeddahDermController(IJeddahDermRep rep, ITimeRep ti)
        {
            this.rep = rep;
            this.ti = ti;
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetBothData()
        {
            List<JeddaDermBothExcelModel> res = rep.GetBothData();

            string now = ti.GetCurrentTime().ToString("dd MMMM yyyy hh:mm tt");

            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("JeddaDerm Both Data");
            worksheet.Range("A1:G1").Merge();

            worksheet.Range("A2:G2").Merge();

            worksheet.Range("A1:G1000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A1:G1000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:G1000").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A1:G1000").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A1:G3").Style.Font.FontSize = 18;
            worksheet.Range("A4:G1000").Style.Font.FontSize = 14;
            worksheet.Columns("A:G").Width = 25;

            worksheet.Rows("1:2").Height = 35;

            worksheet.Rows("3:1000").Height = 25;

        

            worksheet.Range("A1:G2").Style.Fill.BackgroundColor = XLColor.FromArgb(142, 169, 219);
            worksheet.Range("A3:G3").Style.Fill.BackgroundColor = XLColor.FromArgb(250, 191, 143);
            worksheet.Cell("A1").Value = "Exporting Date & Time";
            worksheet.Cell("A2").Value = now;

            worksheet.Cell("A3").Value = "Full Name";
            worksheet.Cell("B3").Value = "Mobile Number";
            worksheet.Cell("C3").Value = "Clinic";
            worksheet.Cell("D3").Value = "City";
            worksheet.Cell("E3").Value = "Query";
            worksheet.Cell("F3").Value = "Representative";
            worksheet.Cell("G3").Value = "Date & Time";
            var row = 4;
            foreach (var item in res)
            {
                worksheet.Cell("A" + row).Value = item.FullName;
                worksheet.Cell("B" + row).Value = item.MobileNumber;
                worksheet.Cell("C" + row).Value = item.Clinic;
                worksheet.Cell("D" + row).Value = item.City;
                worksheet.Cell("E" + row).Value = item.AboutWhatQuery;
                worksheet.Cell("F" + row).Value = item.UserName;
                worksheet.Cell("G" + row).Value = item.datetime;
                row++;
            }
            MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(
            content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "JeddaDerm Both Data.xlsx");

        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddJeddaDermBoth(JeddaDermBothModel obj)
        {
            return Ok(rep.AddJeddaDermBoth(obj));
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult SaamSheet()
        {
            List<SaamAnonRegisterModel> res = rep.GetAll();

            DateTime now = ti.GetCurrentTime();

            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("JeddaDerm");
            worksheet.Range("A1:E1").Merge();
            worksheet.Range("A2:E2").Merge();
            worksheet.Range("A1:E1").Style.Border.BottomBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A3:E3").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A3:E3").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A1:E2").Style.Fill.BackgroundColor = XLColor.FromArgb(189, 215, 238);
            worksheet.Range("A3:E3").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 217, 102);
            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Cell("A2").Style.Font.Bold = true;
            worksheet.Range("A1:E500").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:E500").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Columns("A:E").Width = 40;
            worksheet.Rows("1:500").Height = 25;
            worksheet.Range("A3:E3").Style.Font.Bold = true;
            worksheet.Cell("A1").Value = "Saudi Derm";
            worksheet.Cell("A2").Value = now.Date;
            worksheet.Range("A1:E2").Style.Font.FontSize = 18;
            worksheet.Range("A3:E3").Style.Font.FontSize = 14;
            worksheet.Range("A4:E500").Style.Font.FontSize = 14;
            worksheet.Cell("A3").Value = "Name";
            worksheet.Cell("B3").Value = "Phone";
            worksheet.Cell("C3").Value = "Email";
            worksheet.Cell("D3").Value = "Health Specialities Authority Number";
            worksheet.Cell("E3").Value = "Workshop";
            var row = 4;

            foreach (var item in res)
            {
                worksheet.Cell("A" + row).Value = item.FullName;
                worksheet.Cell("B" + row).Value = item.PhoneNumber;
                worksheet.Cell("C" + row).Value = item.Email;
                worksheet.Cell("D" + row).Value = item.HSAN;
                var w = "";
                switch(item.Workshop)
                {
                    case 0:
                        w = "Only Main Conference";
                        break;
                    case 1:
                        w = "Only Workshop on 16 February";
                        break;
                    case 2:
                        w = "Only Workshop on 17 February";
                        break;
                    case 3:
                        w = "Main Conference + Workshop on 16 February";
                        break;
                    case 4:
                        w = "Main Conference + Workshop on 17 February";
                        break;
                    default:
                        w = "";
                        break;
                    
                }
                worksheet.Cell("E" + row).Value = w;
                row++;
            }

            MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(
            content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Saudi Derm on " + now.Date + ".xlsx");
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditRegister(SaamAnonRegisterModel s)
        {
            return Ok(rep.EditRegister(s));
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AnonRegister(SaamAnonRegisterModel s)
        {
            return Ok(rep.AnonRegister(s));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetInvitationData(int id)
        {
            return Ok(rep.GetInvitationData(id));
        }

        [Route("[controller]/[Action]/{gu}")]
        [HttpGet]
        public IActionResult Start(string gu)
        {
            return Ok(rep.Start(gu));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult Accept(SaamRegisterModel m)
        {
            return Ok(rep.Accept(m));
        }
    }
}

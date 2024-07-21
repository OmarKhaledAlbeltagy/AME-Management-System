using AMEKSA.Models;
using AMEKSA.Repo;
using ClosedXML.Excel;
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
    public class DermController : ControllerBase
    {
        private readonly IDermRep rep;
        private readonly ITimeRep ti;

        public DermController(IDermRep rep, ITimeRep ti)
        {
            this.rep = rep;
            this.ti = ti;
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult SaamSheet()
        {
            List<SaamAnonRegisterModel> res = rep.GetAll();

            DateTime now = ti.GetCurrentTime();

            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Saudi Derm");
            worksheet.Range("A1:D1").Merge();
            worksheet.Range("A2:D2").Merge();
            worksheet.Range("A1:D1").Style.Border.BottomBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A3:D3").Style.Border.InsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A3:D3").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A1:D2").Style.Fill.BackgroundColor = XLColor.FromArgb(189, 215, 238);
            worksheet.Range("A3:D3").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 217, 102);
            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Cell("A2").Style.Font.Bold = true;
            worksheet.Range("A1:D500").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:D500").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Columns("A:D").Width = 40;
            worksheet.Rows("1:500").Height = 25;
            worksheet.Range("A3:D3").Style.Font.Bold = true;
            worksheet.Cell("A1").Value = "Saudi Derm";
            worksheet.Cell("A2").Value = now.Date;
            worksheet.Range("A1:D2").Style.Font.FontSize = 18;
            worksheet.Range("A3:D3").Style.Font.FontSize = 14;
            worksheet.Range("A4:D500").Style.Font.FontSize = 14;
            worksheet.Cell("A3").Value = "Name";
            worksheet.Cell("B3").Value = "Phone";
            worksheet.Cell("C3").Value = "Email";
            worksheet.Cell("D3").Value = "Health Specialities Authority Number";

            var row = 4;

            foreach (var item in res)
            {
                worksheet.Cell("A" + row).Value = item.FullName;
                worksheet.Cell("B" + row).Value = item.PhoneNumber;
                worksheet.Cell("C" + row).Value = item.Email;
                worksheet.Cell("D" + row).Value = item.HSAN;
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

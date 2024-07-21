using AMEKSA.Entities;
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
using System.Linq;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class TedController : ControllerBase
    {
        private readonly ITedRep rep;
        private readonly ITimeRep ti;
        private readonly IImcasCommentRep imc;

        public TedController(ITedRep rep,ITimeRep ti,IImcasCommentRep imc)
        {
            this.rep = rep;
            this.ti = ti;
            this.imc = imc;
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddComment(AddImcasComment obj)
        {
            return Ok(imc.AddComment(obj));
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AnonRegister(SaamAnonRegisterModel m)
        {
            return Ok(rep.AnonRegister(m));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetTedReportExcel()
        {
            string time = ti.GetCurrentTime().ToString("hh:mm tt");
            List<TedAttendanceTable> list = rep.GetAttendanceReport();
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet1");
            worksheet.Columns("A:F").Width = 25;
            worksheet.Column("A").Width = 35;
            worksheet.Column("B").Width = 20;
            worksheet.Column("D").Width = 35;
            worksheet.Column("E").Width = 20;
            worksheet.Rows("1:1000").Height = 25;
            worksheet.Range("A1:E1").Style.Font.FontSize = 18;
            worksheet.Range("A2:E2").Style.Font.FontSize = 18;
            worksheet.Range("A1:B1").Style.Fill.BackgroundColor = XLColor.FromArgb(17, 194, 109);
            worksheet.Range("E1:D1").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 76, 82);
            worksheet.Range("A1:B1").Merge();
            worksheet.Range("D1:E1").Merge();

            worksheet.Cell("A1").Value = "Waiting List";
            worksheet.Cell("D1").Value = "Attended";

            worksheet.Cell("A2").Value = "Name";
            worksheet.Cell("B2").Value = "Code";

            worksheet.Cell("D2").Value = "Name";
            worksheet.Cell("E2").Value = "Code";

            worksheet.Range("A2:B2").Style.Fill.BackgroundColor = XLColor.FromArgb(17, 194, 109);
            worksheet.Range("D2:E2").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 76, 82);

            worksheet.Range("A1:F500").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:F500").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A3:E500").Style.Font.FontSize = 14;
            worksheet.Range("A1:E2").Style.Font.Bold = true;
            var row = 3;
            foreach (var item in list.Where(a=>a.att == false))
            {
                worksheet.Cell("A" + row).Value = item.Name;
                worksheet.Cell("B" + row).Value = item.Code;
                row++;
            }

            row = 3;
            foreach (var item in list.Where(a => a.att == true))
            {
                worksheet.Cell("D" + row).Value = item.Name;
                worksheet.Cell("E" + row).Value = item.Code;
                row++;
            }

                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(
         content,
         "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        time+".xlsx");
        }
   


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetTedReport()
        {
            return Ok(rep.GetTedReport());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAttendanceReport()
        {
            return Ok(rep.GetAttendanceReport());
        }


        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult Attendance(int id)
        {
            return Ok(rep.Attendance(id));
        }

        [Route("[controller]/[Action]/{g}")]
        [HttpGet]
        public IActionResult GetDataByGuid(string g)
        {
            return Ok(rep.GetDataByGuid(g));
        }

        [Route("[controller]/[Action]/{c}")]
        [HttpGet]
        public IActionResult GetDataByCode(int c)
        {
            return Ok(rep.GetDataByCode(c));
        }

        
        [Route("[controller]/[Action]/{g}")]
        [HttpGet]
        public IActionResult GetDoctorName(string g)
        {
            return Ok(rep.GetDoctorName(g));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult SubmitChangeInfo(TedRegisterModel m)
        {
            return Ok(rep.SubmitChangeInfo(m));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult ChangeInfo(int id)
        {
            return Ok(rep.ChangeInfo(id));
        }


        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult ChangeMind(int id)
        {
            return Ok(rep.ChangeMind(id));
        }
        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult Accept(TedRegisterModel r)
        {
            return Ok(rep.Accept(r));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult Reject(int id)
        {
            return Ok(rep.Reject(id));
        }

        [Route("[controller]/[Action]/{g}")]
        [HttpGet]
        public IActionResult Start(string g)
        {
            return Ok(rep.Start(g));
        }
    }
}

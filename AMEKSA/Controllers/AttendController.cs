using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Repo;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class AttendController : ControllerBase
    {
        private readonly IAttendRep rep;
        private readonly DbContainer db;

        public AttendController(IAttendRep rep, DbContainer db)
        {
            this.rep = rep;
            this.db = db;
        }

        [Route("[controller]/[Action]/{attend}/{cityid}")]
        [HttpGet]
        public IActionResult ConfirmAttend(byte attend, int cityid)
        {
            return Ok(rep.ConfirmAttend(attend,cityid));
        }

        [Route("[controller]/[Action]/{id}/{first}/{last}/{phone}")]
        [HttpGet]
        public IActionResult ConfirmForm(int id, string first, string last, string phone)
        {
            return Ok(rep.ConfirmForm(id,first,last,phone));
        }

        [Route("[controller]/[Action]/{id}/{why}")]
        [HttpGet]
        public IActionResult RejectForm(int id, string why)
        {
            return Ok(rep.RejectForm(id, why));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult ReConfirm(int id)
        {
            return Ok(rep.ReConfirmAttend(id));
        }

        //GetAttendCount
        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAttendCount()
        {
            return Ok(rep.GetAttendCount());
        }

        [Route("[controller]/[Action]/{ManagerId}")]
        [HttpGet]
        public IActionResult GetAttendCountCity(string ManagerId)
        {
            return Ok(rep.GetAttendCountCity(ManagerId));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult Came(int id)
        {
            return Ok(rep.Came(id));
        }

        
        [Route("[controller]/[Action]/{first}/{last}/{phone}/{CityId}")]
        [HttpGet]
        public IActionResult ConfirmFormInEvent(string first, string last, string phone, int CityId)
        {
            return Ok(rep.ConfirmFormInEvent(first,last,phone,CityId));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllData()
        {
            XLWorkbook workbook = new XLWorkbook();

            AttendCountModel Counts = rep.GetAttendCount();

            var countsheet = workbook.Worksheets.Add("General Report");

            countsheet.Rows("1:5").Height = 20;
            countsheet.Columns("A:C").Width = 20;
            countsheet.Range("A1:C1").Style.Font.FontSize = 16;
            countsheet.Range("A1:C1").Style.Font.Bold = true;
            countsheet.Range("A2:C5").Style.Font.FontSize = 14;
            countsheet.Range("A1:C5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            countsheet.Range("A1:C5").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            countsheet.Cell("A1").Value = "City";
            countsheet.Cell("B1").Value = "Coming";
            countsheet.Cell("C1").Value = "Not Coming";
            countsheet.Cell("A2").Value = "Riyadh";
            countsheet.Cell("B2").Value = Counts.RiyadhComing;
            countsheet.Cell("C2").Value = Counts.RiyadhNotComing;
            countsheet.Cell("A3").Value = "Jeddah";
            countsheet.Cell("B3").Value = Counts.JeddahComing;
            countsheet.Cell("C3").Value = Counts.JeddahNotComing;
            countsheet.Cell("A4").Value = "Dammam";
            countsheet.Cell("B4").Value = Counts.DammamComing;
            countsheet.Cell("C4").Value = Counts.DammamNotComing;
            countsheet.Cell("A5").Value = "Total";
            countsheet.Cell("B5").Value = Counts.TotalComing;
            countsheet.Cell("C5").Value = Counts.TotalNotComing;

            var RiyadhSheet = workbook.Worksheets.Add("Riyadh");

            IEnumerable<Attend> RiyadhComing = rep.GetAllComingData(1);
            IEnumerable<Attend> RiyadhNotComing = rep.GetAllNotComingData(1);
            RiyadhSheet.Rows("1:500").Height = 20;
            RiyadhSheet.Columns("A:Z").Width = 20;
            RiyadhSheet.Range("A1:F500").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            RiyadhSheet.Range("A1:F500").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            RiyadhSheet.Range("A1:F2").Style.Font.FontSize = 16;
            RiyadhSheet.Range("A1:F2").Style.Font.Bold = true;
            RiyadhSheet.Range("A3:F500").Style.Font.FontSize = 14;
            RiyadhSheet.Range("D1:D500").Style.Border.RightBorder = XLBorderStyleValues.Double;
            RiyadhSheet.Range("A1:D1").Merge();
            RiyadhSheet.Range("E1:F1").Merge();

            RiyadhSheet.Cell("A1").Value = "Coming";
            RiyadhSheet.Cell("E1").Value = "Not Coming";

            RiyadhSheet.Cell("A2").Value = "ID";
            RiyadhSheet.Cell("B2").Value = "First Name";
            RiyadhSheet.Cell("C2").Value = "Last Name";
            RiyadhSheet.Cell("D2").Value = "Mobile Number";
            RiyadhSheet.Cell("E2").Value = "ID";
            RiyadhSheet.Cell("F2").Value = "Reason";
            var riyadhrow = 3;
            foreach (var item in RiyadhComing)
            {
                RiyadhSheet.Cell(riyadhrow, 1).Value = item.Id;
                RiyadhSheet.Cell(riyadhrow, 2).Value = item.FirstName;
                RiyadhSheet.Cell(riyadhrow, 3).Value = item.LastName;
                RiyadhSheet.Cell(riyadhrow, 4).Value = item.PhoneNumber;
           
                    if (item.Came == true)
                    {
                        RiyadhSheet.Range("A"+riyadhrow+":D"+riyadhrow).Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);
                    }
           
                
                riyadhrow++;
            }
            riyadhrow = 3;
            foreach (var item in RiyadhNotComing)
            {
                RiyadhSheet.Cell(riyadhrow, 5).Value = item.Id;
                RiyadhSheet.Cell(riyadhrow, 6).Value = item.why;

                if (item.Came == true)
                {
                    RiyadhSheet.Range("E" + riyadhrow + ":F" + riyadhrow).Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);
                }

                riyadhrow++;
            }


            //////////////


            var DammamSheet = workbook.Worksheets.Add("Dammam");

            IEnumerable<Attend> DammamComing = rep.GetAllComingData(2);
            IEnumerable<Attend> DammamNotComing = rep.GetAllNotComingData(2);
            DammamSheet.Rows("1:500").Height = 20;
            DammamSheet.Columns("A:Z").Width = 20;
            DammamSheet.Range("A1:F500").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            DammamSheet.Range("A1:F500").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            DammamSheet.Range("A1:F2").Style.Font.FontSize = 16;
            DammamSheet.Range("A1:F2").Style.Font.Bold = true;
            DammamSheet.Range("A3:F500").Style.Font.FontSize = 14;
            DammamSheet.Range("D1:D500").Style.Border.RightBorder = XLBorderStyleValues.Double;
            DammamSheet.Range("A1:D1").Merge();
            DammamSheet.Range("E1:F1").Merge();

            DammamSheet.Cell("A1").Value = "Coming";
            DammamSheet.Cell("E1").Value = "Not Coming";

            DammamSheet.Cell("A2").Value = "ID";
            DammamSheet.Cell("B2").Value = "First Name";
            DammamSheet.Cell("C2").Value = "Last Name";
            DammamSheet.Cell("D2").Value = "Mobile Number";
            DammamSheet.Cell("E2").Value = "ID";
            DammamSheet.Cell("F2").Value = "Reason";
            var dammamrow = 3;
            foreach (var item in DammamComing)
            {
                DammamSheet.Cell(dammamrow, 1).Value = item.Id;
                DammamSheet.Cell(dammamrow, 2).Value = item.FirstName;
                DammamSheet.Cell(dammamrow, 3).Value = item.LastName;
                DammamSheet.Cell(dammamrow, 4).Value = item.PhoneNumber;
                if (item.Came == true)
                {
                    DammamSheet.Range("A" + dammamrow + ":D" + dammamrow).Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);
                }
                dammamrow++;
            }

            dammamrow = 3;
            foreach (var item in DammamNotComing)
            {
                DammamSheet.Cell(dammamrow, 5).Value = item.Id;
                DammamSheet.Cell(dammamrow, 6).Value = item.why;
                if (item.Came == true)
                {
                    DammamSheet.Range("E" + dammamrow + ":F" + dammamrow).Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);
                }
                dammamrow++;
            }

            //////////////


            var JeddahSheet = workbook.Worksheets.Add("Jeddah");

            IEnumerable<Attend> JeddahComing = rep.GetAllComingData(3);
            IEnumerable<Attend> JeddahNotComing = rep.GetAllNotComingData(3);
            JeddahSheet.Rows("1:500").Height = 20;
            JeddahSheet.Columns("A:Z").Width = 20;
            JeddahSheet.Range("A1:F500").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            JeddahSheet.Range("A1:F500").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            JeddahSheet.Range("A1:F2").Style.Font.FontSize = 16;
            JeddahSheet.Range("A1:F2").Style.Font.Bold = true;
            JeddahSheet.Range("A3:F500").Style.Font.FontSize = 14;
            JeddahSheet.Range("D1:D500").Style.Border.RightBorder = XLBorderStyleValues.Double;
            JeddahSheet.Range("A1:D1").Merge();
            JeddahSheet.Range("E1:F1").Merge();

            JeddahSheet.Cell("A1").Value = "Coming";
            JeddahSheet.Cell("E1").Value = "Not Coming";

            JeddahSheet.Cell("A2").Value = "ID";
            JeddahSheet.Cell("B2").Value = "First Name";
            JeddahSheet.Cell("C2").Value = "Last Name";
            JeddahSheet.Cell("D2").Value = "Mobile Number";
            JeddahSheet.Cell("E2").Value = "ID";
            JeddahSheet.Cell("F2").Value = "Reason";
            var jeddahrow = 3;
            foreach (var item in JeddahComing)
            {
                JeddahSheet.Cell(jeddahrow, 1).Value = item.Id;
                JeddahSheet.Cell(jeddahrow, 2).Value = item.FirstName;
                JeddahSheet.Cell(jeddahrow, 3).Value = item.LastName;
                JeddahSheet.Cell(jeddahrow, 4).Value = item.PhoneNumber;
                if (item.Came == true)
                {
                    JeddahSheet.Range("A" + jeddahrow + ":D" + jeddahrow).Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);
                }
                jeddahrow++;
            }

            jeddahrow = 3;
            foreach (var item in JeddahNotComing)
            {
                JeddahSheet.Cell(jeddahrow, 5).Value = item.Id;
                JeddahSheet.Cell(jeddahrow, 6).Value = item.why;
                if (item.Came == true)
                {
                    JeddahSheet.Range("E" + jeddahrow + ":F" + jeddahrow).Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);
                }
                jeddahrow++;
            }









          
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Back To Success Event Attendance Report.xlsx");
        }



        [Route("[controller]/[Action]/{ManagerId}")]
        [HttpGet]
        public IActionResult GetAllDataCity(string ManagerId)
        {
            int CityId = (int)db.Users.Find(ManagerId).CityId;
            string CityName = db.city.Find(CityId).CityName;

            XLWorkbook workbook = new XLWorkbook();

            var RiyadhSheet = workbook.Worksheets.Add(CityName);

            IEnumerable<Attend> RiyadhComing = rep.GetAllComingData(CityId);
            IEnumerable<Attend> RiyadhNotComing = rep.GetAllNotComingData(CityId);
            RiyadhSheet.Rows("1:500").Height = 20;
            RiyadhSheet.Columns("A:Z").Width = 20;
            RiyadhSheet.Range("A1:F500").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            RiyadhSheet.Range("A1:F500").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            RiyadhSheet.Range("A1:F2").Style.Font.FontSize = 16;
            RiyadhSheet.Range("A1:F2").Style.Font.Bold = true;
            RiyadhSheet.Range("A3:F500").Style.Font.FontSize = 14;
            RiyadhSheet.Range("D1:D500").Style.Border.RightBorder = XLBorderStyleValues.Double;
            RiyadhSheet.Range("A1:D1").Merge();
            RiyadhSheet.Range("E1:F1").Merge();

            RiyadhSheet.Cell("A1").Value = "Coming";
            RiyadhSheet.Cell("E1").Value = "Not Coming";

            RiyadhSheet.Cell("A2").Value = "ID";
            RiyadhSheet.Cell("B2").Value = "First Name";
            RiyadhSheet.Cell("C2").Value = "Last Name";
            RiyadhSheet.Cell("D2").Value = "Mobile Number";
            RiyadhSheet.Cell("E2").Value = "ID";
            RiyadhSheet.Cell("F2").Value = "Reason";
            var riyadhrow = 3;
            foreach (var item in RiyadhComing)
            {
                RiyadhSheet.Cell(riyadhrow, 1).Value = item.Id;
                RiyadhSheet.Cell(riyadhrow, 2).Value = item.FirstName;
                RiyadhSheet.Cell(riyadhrow, 3).Value = item.LastName;
                RiyadhSheet.Cell(riyadhrow, 4).Value = item.PhoneNumber;
                riyadhrow++;
            }
            riyadhrow = 3;
            foreach (var item in RiyadhNotComing)
            {
                RiyadhSheet.Cell(riyadhrow, 5).Value = item.Id;
                RiyadhSheet.Cell(riyadhrow, 6).Value = item.why;
                riyadhrow++;
            }

            MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Back To Success Event Attendance Report.xlsx");
        }
    }
}

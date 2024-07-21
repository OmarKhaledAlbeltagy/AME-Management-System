using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Repo;
using ClosedXML;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class EventAccountingController : ControllerBase
    {
        private readonly IEventAccountingRep rep;
        private readonly DbContainer db;
        private readonly IEventRep eventRep;

        public EventAccountingController(IEventAccountingRep rep, DbContainer db, IEventRep eventRep)
        {
            this.rep = rep;
            this.db = db;
            this.eventRep = eventRep;
        }

        [Route("[controller]/[Action]/{From}/{To}/{UserId}")]
        [HttpGet]
        public IActionResult GetEventsTotalExpenses(DateTime From, DateTime To, string UserId)
        {
            List<EventExcelModel> ress = rep.GetEventsTotalExpenses(From, To, UserId);
            if (ress.Count == 0 || ress == null)
            {
                return Ok("There are no events in this time frame");
            }
            else
            {
                XLWorkbook workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Events Totals");
                worksheet.Cell("A1").Value = "Event";
                worksheet.Cell("B1").Value = "Total";
                worksheet.Cell("C1").Value = "Type";
                worksheet.Cell("D1").Value = "From";
                worksheet.Cell("E1").Value = "To";
                worksheet.Cell("F1").Value = "Attend.";
                worksheet.Cell("G1").Value = "Speakers";
                worksheet.Cell("H1").Value = "Registration";
                worksheet.Cell("I1").Value = "Tickets";
                worksheet.Cell("J1").Value = "Hotels";
                worksheet.Cell("K1").Value = "Transportation";


                var row = 2;
                var longestcol = 0;
                var longestrow = 0;
                foreach (var res in ress)
                {
                    worksheet.Cell("A" + row).Value = res.EventName;
                    worksheet.Cell("B" + row).Value = res.Total;
                    worksheet.Cell("C" + row).Value = res.EventType;
                    worksheet.Cell("D" + row).Value = res.StartDate;
                    worksheet.Cell("E" + row).Value = res.EndDate;
                    worksheet.Cell("F" + row).Value = res.Attendees;

                    

                    if (res.EventType == "Workshop")
                    {


                        if (res.speakers.Count != 0)
                        {
                            string speakerss = "| ";
                            foreach (var item in res.speakers)
                            {
                                speakerss = speakerss + item.SpeakerName + " | ";
                            }
                            int len = speakerss.Length;
                            int startindx = len - 3;
                            speakerss.Remove(startindx, 3);
                            worksheet.Cell("G" + row).Value = speakerss;
                        }
                        
                    }

                    EventTotalExpensesModel regcheck = res.totals.Where(a => a.Title == "Registration").FirstOrDefault();
                    if (regcheck != null)
                    {
                        worksheet.Cell("H" + row).Value = regcheck.Value;
                    }

                    EventTotalExpensesModel ticcheck = res.totals.Where(a => a.Title == "Tickets").FirstOrDefault();
                    if (ticcheck != null)
                    {
                        worksheet.Cell("I" + row).Value = ticcheck.Value;
                    }


                    EventTotalExpensesModel hotcheck = res.totals.Where(a => a.Title == "Hotels").FirstOrDefault();
                    if (hotcheck != null)
                    {
                        worksheet.Cell("J" + row).Value = hotcheck.Value;
                    }


                    EventTotalExpensesModel transcheck = res.totals.Where(a => a.Title == "Transportation").FirstOrDefault();
                    if (transcheck != null)
                    {
                        worksheet.Cell("K" + row).Value = transcheck.Value;
                    }

                    var col = 11;
                    var others = res.totals.Where(a => a.Title != "Registration" && a.Title != "Tickets" && a.Title != "Hotels" && a.Title != "Transportation");
                    foreach (var item in others)
                    {
                        worksheet.Cell(1, col).Value = "Other";
                        worksheet.Cell(row,col).Value = item.Value;
                        col++;
                    }
                    row++;

                    col--;
                    if (col > longestcol)
                    {
                        longestcol = col;
                    }
                    if (row > longestrow)
                    {
                        longestrow = row;
                    }

                }
                longestrow--;
                worksheet.Rows("1:500").Height = 25;

                worksheet.Column(1).Width = 60;
                worksheet.Column(2).Width = 12;
                worksheet.Column(3).Width = 13;
                worksheet.Column(4).Width = 14;
                worksheet.Column(5).Width = 14;
                worksheet.Column(6).Width = 12;
                worksheet.Column(7).Width = 20;
                worksheet.Columns(8,50).Width = 16;
               

                worksheet.Range(1, 1, 500, longestcol).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range(1, 1, 500, longestcol).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(2, 1, 500, longestcol).Style.Font.FontSize = 12;
                worksheet.Range(1, 1, 1, longestcol).Style.Font.FontSize = 16;
                worksheet.Range(1, 1, 1, longestcol).Style.Font.Bold = true;
                worksheet.Range(1, 1, 1, longestcol).Style.Fill.BackgroundColor = XLColor.FromArgb(184,204,228);

                worksheet.Range("G1:G"+longestrow).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                worksheet.Range("G1:G" + longestrow).Style.Border.RightBorderColor = XLColor.FromArgb(184, 204, 228);

                worksheet.Range(1,1,longestrow,longestcol).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                worksheet.Range(1, 1, longestrow, longestcol).Style.Border.OutsideBorderColor = XLColor.FromArgb(184, 204, 228);
                worksheet.Range("B2:B"+longestrow).Style.Font.FontColor = XLColor.FromArgb(48, 82, 124);
                worksheet.Range("B2:B" + longestrow).Style.Font.Bold = true;

                longestrow = longestrow + 2;
                worksheet.Range("B" + longestrow + ":G" + longestrow).Merge();
                worksheet.Cell("B" + longestrow).Value = "Exported By " + ress.FirstOrDefault().UserNameExported + " on " + ress.FirstOrDefault().ExportingDateTime;
                worksheet.Cell("B" + longestrow).Style.Fill.BackgroundColor = XLColor.FromArgb(184, 204, 228);
                worksheet.Cell("B"+longestrow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell("B" + longestrow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell("B" + longestrow).Style.Font.Underline = XLFontUnderlineValues.Single;
                worksheet.Cell("B" + longestrow).Style.Font.Bold = true;
                worksheet.Cell("B" + longestrow).Style.Font.FontSize = 16;
                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Events Totals.xlsx");

            }

        }

        [Route("[controller]/[Action]/{EventId}/{UserId}")]
        [HttpGet]
        public IActionResult GetNotMyEventAccountingRequests(int EventId, string UserId)
        {
            return Ok(rep.GetNotMyEventAccountingRequests(EventId, UserId));
        }

        [Route("[controller]/[Action]/{EventId}/{UserId}")]
        [HttpGet]
        public IActionResult GetAccountingManagerAccountingRequests(int EventId, string UserId)
        {
            return Ok(rep.GetAccountingManagerAccountingRequests(EventId, UserId));
        }


        [Route("[controller]/[Action]/{EventId}/{UserId}")]
        [HttpGet]
        public IActionResult GetEventExpenses(int EventId, string UserId)
        {
            return Ok(rep.GetEventExpenses(EventId, UserId));
        }

        [Route("[controller]/[Action]/{EventId}/{UserId}")]
        [HttpGet]
        public IActionResult MyExpensesExcel(int EventId, string UserId)
        {
            EventExcelModel res = rep.GetMyExpensesForExcel(EventId, UserId);
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Event Expenses");
            worksheet.Cell("A1").Value = "Event Name:";
            worksheet.Cell("A2").Value = "Event Type:";
            worksheet.Cell("A3").Value = "Date:";
            worksheet.Cell("A4").Value = "No. of Attendance:";

            worksheet.Range("B1:F1").Merge();
            worksheet.Range("B2:F2").Merge();
            worksheet.Range("B3:F3").Merge();
            worksheet.Range("B4:F4").Merge();

            worksheet.Cell("B1").Value = res.EventName;
            worksheet.Cell("B2").Value = res.EventType;

            worksheet.Cell("B4").Value = res.Attendees;
            if (res.StartDate == res.EndDate)
            {
                worksheet.Cell("B3").Value = res.StartDate;
            }
            else
            {
                worksheet.Cell("B3").Value = "From: " + res.StartDate + " - To: " + res.EndDate;
            }


            if (res.EventType == "Workshop")
            {
                worksheet.Range("B5:F5").Merge();
                worksheet.Cell("A5").Value = "Speakers:";

                if (res.speakers.Count != 0)
                {

                    string speakerss = "| ";
                    foreach (var item in res.speakers)
                    {
                        speakerss = speakerss + item.SpeakerName + " | ";
                    }
                    int len = speakerss.Length;
                    int startindx = len - 3;
                    speakerss.Remove(startindx, 3);
                    worksheet.Cell("B5").Value = speakerss;
                }

                worksheet.Range("A1:F5").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            }
            else
            {
                worksheet.Range("A1:F4").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            }




            worksheet.Cell("A7").Value = "Title";
            worksheet.Cell("B7").Value = "Value";
            worksheet.Cell("C7").Value = "Note";
            worksheet.Cell("D7").Value = "Date & Time";
            worksheet.Cell("E7").Value = "Reference";
            worksheet.Cell("F7").Value = "Rejection Reason";





            //View
            worksheet.Columns("A:F").Width = 20;
            worksheet.Rows("1:2000").Height = 25;
            worksheet.Columns("A").Width = 25;
            worksheet.Range("A1:F2000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A7:F2000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A7:F7").Style.Font.FontSize = 16;
            worksheet.Range("A7:F7").Style.Font.Bold = true;
            worksheet.Range("A8:F2000").Style.Font.FontSize = 12;
            worksheet.Range("A7:F7").Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);


            worksheet.Range("B1:B5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A1:B5").Style.Font.FontSize = 16;
            worksheet.Range("A1:A5").Style.Font.Bold = true;



            int row = 8;
            foreach (var item in res.Expenses)
            {
                worksheet.Cell("A" + row).Value = item.Title;
                worksheet.Cell("B" + row).Value = item.Value;
                worksheet.Cell("C" + row).Value = item.Note;
                worksheet.Cell("D" + row).Value = item.DateTime;
                worksheet.Cell("E" + row).Value = "Download Reference";
                worksheet.Cell("E" + row).Hyperlink.ExternalAddress = new System.Uri("https://{{URL}}.com/EventAccounting/DownloadReferenceRequest/" + item.Id);

                switch (item.Confirmed)
                {
                    case true:
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);
                        break;
                    case false:
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 0, 0);
                        if (item.RejectionReason != "" && item.RejectionReason != null)
                        {
                            worksheet.Cell("F" + row).Value = item.RejectionReason;
                        }
                        break;
                    default:
                        break;
                }

                row++;

            }
            row++;

            worksheet.Range("A" + row + ":F" + row).Merge();
            worksheet.Cell("A" + row).Value = "Exported By " + res.UserNameExported + " on " + res.ExportingDateTime;
            worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Range("A" + row + ":F" + row).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Cell("A" + row).Style.Font.Bold = true;
            worksheet.Cell("A" + row).Style.Font.FontSize = 14;



            if (res.EventType == "Workshop")
            {

                List<EventAttendanceModel> att = eventRep.GetEventAttenance(EventId);
                row++;
                row++;

                var stt = row;
                worksheet.Range("A" + row + ":E" + row).Style.Font.Bold = true;

                worksheet.Range("A" + row + ":E500").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A" + row + ":E500").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A" + row + ":E500").Style.Font.FontSize = 12;
                worksheet.Range("A" + row + ":E" + row).Style.Font.FontSize = 16;
                worksheet.Range("A" + row + ":E" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                worksheet.Cell("A" + row).Value = "Serial";
                worksheet.Cell("B" + row).Value = "Contact";
                worksheet.Cell("C" + row).Value = "Email";
                worksheet.Cell("D" + row).Value = "Account";
                worksheet.Cell("E" + row).Value = "Representative";
                row++;
                List<string> staff = new List<string>();
                var ser = 1;
                foreach (var item in att)
                {
                    worksheet.Cell("A" + row).Value = ser;
                    worksheet.Cell("B" + row).Value = item.ContactName;
                    worksheet.Cell("C" + row).Value = item.Email;
                    worksheet.Cell("D" + row).Value = item.AccountName;
                    worksheet.Cell("E" + row).Value = item.UserName;
                    staff.Add(item.UserName);
                    row++;
                    ser++;
                }
                int r = row - 1;
                worksheet.Range("A" + stt + ":E" + r).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;


                staff = DistinctByExtension.DistinctBy(staff, a => a).ToList();

                row++;
                int st = row;

                worksheet.Cell("B" + row).Value = "AME Staff";
                worksheet.Cell("B" + row).Style.Font.Bold = true;
                worksheet.Cell("B" + row).Style.Font.FontSize = 16;
                worksheet.Cell("B" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                row++;
                foreach (var item in staff)
                {
                    worksheet.Cell("B" + row).Value = item;
                    row++;
                }
                row--;
                worksheet.Range("B" + st + ":B" + row).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;


                row = st;
                worksheet.Cell("D" + row).Value = "Speakers";
                worksheet.Cell("D" + row).Style.Font.Bold = true;
                worksheet.Cell("D" + row).Style.Font.FontSize = 16;
                worksheet.Cell("D" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                row++;
                foreach (var item in res.speakers)
                {
                    worksheet.Cell("D" + row).Value = item.SpeakerName;
                    row++;
                }
                row--;
                worksheet.Range("D" + st + ":D" + row).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            }


            MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            try
            {

                return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", res.EventName + ".xlsx");
            }
            catch (Exception)
            {

                return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", res.EventName + "Event Expenses.xlsx");
            }


        }


        [Route("[controller]/[Action]/{EventId}/{UserId}")]
        [HttpGet]
        public IActionResult AllExpensesExcel(int EventId, string UserId)
        {
            EventExcelModel res = rep.GetAllExpensesForExcel(EventId, UserId);
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Event Expenses");
            worksheet.Cell("A1").Value = "Event Name:";
            worksheet.Cell("A2").Value = "Event Type:";
            worksheet.Cell("A3").Value = "Date:";
            worksheet.Cell("A4").Value = "No. of Attendance:";

            worksheet.Range("B1:F1").Merge();
            worksheet.Range("B2:F2").Merge();
            worksheet.Range("B3:F3").Merge();
            worksheet.Range("B4:F4").Merge();

            worksheet.Cell("B1").Value = res.EventName;
            worksheet.Cell("B2").Value = res.EventType;

            worksheet.Cell("B4").Value = res.Attendees;
            if (res.StartDate == res.EndDate)
            {
                worksheet.Cell("B3").Value = res.StartDate;
            }
            else
            {
                worksheet.Cell("B3").Value = "From: " + res.StartDate + " - To: " + res.EndDate;
            }


            if (res.EventType == "Workshop")
            {
                worksheet.Range("B5:F5").Merge();
                worksheet.Cell("A5").Value = "Speakers:";

                if (res.speakers.Count != 0)
                {
                    string speakerss = "| ";
                    foreach (var item in res.speakers)
                    {
                        speakerss = speakerss + item.SpeakerName + " | ";
                    }
                    int len = speakerss.Length;
                    int startindx = len - 3;
                    speakerss.Remove(startindx, 3);
                    worksheet.Cell("B5").Value = speakerss;
                }


                worksheet.Range("A1:F5").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            }
            else
            {
                worksheet.Range("A1:F4").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            }




            worksheet.Cell("A7").Value = "Accountant";
            worksheet.Cell("B7").Value = "Title";
            worksheet.Cell("C7").Value = "Value";
            worksheet.Cell("D7").Value = "Note";
            worksheet.Cell("E7").Value = "Date & Time";
            worksheet.Cell("F7").Value = "Reference";
            worksheet.Cell("G7").Value = "Rejection Reason";





            //View
            worksheet.Columns("A:G").Width = 20;
            worksheet.Rows("1:2000").Height = 25;
            worksheet.Columns("A").Width = 25;
            worksheet.Range("A1:G2000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A7:G2000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A7:G7").Style.Font.FontSize = 16;
            worksheet.Range("A7:G7").Style.Font.Bold = true;
            worksheet.Range("A8:G2000").Style.Font.FontSize = 12;
            worksheet.Range("A7:G7").Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);


            worksheet.Range("B1:B5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A1:B5").Style.Font.FontSize = 16;
            worksheet.Range("A1:A5").Style.Font.Bold = true;



            int row = 8;
            foreach (var item in res.Expenses)
            {
                worksheet.Cell("A" + row).Value = item.UserName;
                worksheet.Cell("B" + row).Value = item.Title;
                worksheet.Cell("C" + row).Value = item.Value;
                worksheet.Cell("D" + row).Value = item.Note;
                worksheet.Cell("E" + row).Value = item.DateTime;
                worksheet.Cell("F" + row).Value = "Download Reference";
                worksheet.Cell("F" + row).Hyperlink.ExternalAddress = new System.Uri("{{URL}}/EventAccounting/DownloadReferenceRequest/" + item.Id);

                switch (item.Confirmed)
                {
                    case true:
                        worksheet.Range("A" + row + ":B" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);
                        break;
                    case false:
                        worksheet.Range("A" + row + ":B" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 0, 0);
                        if (item.RejectionReason != "" && item.RejectionReason != null)
                        {
                            worksheet.Cell("G" + row).Value = item.RejectionReason;
                        }
                        break;
                    default:
                        break;
                }

                row++;

            }
            row++;

            worksheet.Range("A" + row + ":G" + row).Merge();
            worksheet.Cell("A" + row).Value = "Exported By " + res.UserNameExported + " on " + res.ExportingDateTime;
            worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheet.Range("A" + row + ":G" + row).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Cell("A" + row).Style.Font.Bold = true;
            worksheet.Cell("A" + row).Style.Font.FontSize = 14;


            if (res.EventType == "Workshop")
            {



               List<EventAttendanceModel> att = eventRep.GetEventAttenance(EventId);
                row++;
                row++;

                var stt = row;
                worksheet.Range("A" + row+":E"+row).Style.Font.Bold = true;

                worksheet.Range("A" + row + ":E500").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A" + row + ":E500").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A" + row + ":E500").Style.Font.FontSize = 12;
                worksheet.Range("A" + row + ":E" + row).Style.Font.FontSize = 16;
                worksheet.Range("A" + row + ":E" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                worksheet.Cell("A" + row).Value = "Serial";
                worksheet.Cell("B" + row).Value = "Contact";
                worksheet.Cell("C" + row).Value = "Email";
                worksheet.Cell("D" + row).Value = "Account";
                worksheet.Cell("E" + row).Value = "Representative";
                row++;
                List<string> staff = new List<string>();
                var ser = 1;
                foreach (var item in att)
                {
                    worksheet.Cell("A" + row).Value = ser;
                    worksheet.Cell("B" + row).Value = item.ContactName;
                    worksheet.Cell("C" + row).Value = item.Email;
                    worksheet.Cell("D" + row).Value = item.AccountName;
                    worksheet.Cell("E" + row).Value = item.UserName;
                    staff.Add(item.UserName);
                    row++;
                    ser++;
                }
                int r = row - 1;
                worksheet.Range("A" + stt + ":E" + r).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;


                staff = DistinctByExtension.DistinctBy(staff,a=>a).ToList();

                row++;
                int st = row;

                worksheet.Cell("B" + row).Value = "AME Staff";
                worksheet.Cell("B" + row).Style.Font.Bold = true;
                worksheet.Cell("B" + row).Style.Font.FontSize = 16;
                worksheet.Cell("B" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                row++;
                foreach (var item in staff)
                {
                    worksheet.Cell("B" + row).Value = item;
                    row++;
                }
                row--;
                worksheet.Range("B" + st + ":B" + row).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;


                row = st;
                worksheet.Cell("D" + row).Value = "Speakers";
                worksheet.Cell("D" + row).Style.Font.Bold = true;
                worksheet.Cell("D" + row).Style.Font.FontSize = 16;
                worksheet.Cell("D" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                row++;
                foreach (var item in res.speakers)
                {
                    worksheet.Cell("D" + row).Value = item.SpeakerName;
                    row++;
                }
                row--;
                worksheet.Range("D" + st + ":D" + row).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            }








            MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
            content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Event Expenses.xlsx");


        }


        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult ConfirmRequest(int id)
        {
            return Ok(rep.ConfirmRequest(id));
        }

        [Route("[controller]/[Action]/{From}/{To}")]
        [HttpGet]
        public IActionResult GetMorrisChart(DateTime From, DateTime To)
        {
            return Ok(rep.GetMorrisChart(From, To));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult RejectRequest(RejectExpenseRequestModel obj)
        {
            return Ok(rep.RejectRequest(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult RejectRequestAfterConfirm(RejectExpenseRequestModel obj)
        {
            return Ok(rep.RejectRequestAfterConfirm(obj));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult HoldRequest(int id)
        {
            return Ok(rep.HoldRequest(id));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetPendingRequests()
        {
            return Ok(rep.GetPendingRequests());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetPreviousRequests()
        {
            return Ok(rep.GetPreviousRequests());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetPendingRequestsCount()
        {
            int res = db.eventFeesRequest.Where(a=>a.Confirmed == null).Count();
            return Ok(res);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditExpenseWithFile([FromForm] EditExpenseWithFileModel obj)
        {
            return Ok(rep.EditExpenseWithFile(obj));
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditExpenseWithoutFile(EditExpenseWithoutFileModel obj)
        {
            return Ok(rep.EditExpenseWithoutFile(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditExpenseRequestWithFile([FromForm] EditExpenseWithFileModel obj)
        {
            return Ok(rep.EditExpenseRequestWithFile(obj));
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditExpenseRequestWithoutFile(EditExpenseWithoutFileModel obj)
        {
            return Ok(rep.EditExpenseRequestWithoutFile(obj));
        }


        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeleteExpense(int id)
        {
            return Ok(rep.DeleteExpense(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeleteExpenseRequest(int id)
        {
            return Ok(rep.DeleteExpenseRequest(id));
        }


        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DownloadReference(int id)
        {
            EventFees ef = db.eventFees.Find(id);
            string EventName = db.Event.Find(ef.EventtId).EventName;
            string UserName = db.Users.Find(ef.ExtendIdentityUserId).FullName;
            return File(ef.file, ef.ContentType, ef.Title+" Fees - "+EventName+" - "+ UserName + "."+ef.Extension);
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DownloadReferenceRequest(int id)
        {
            EventFeesRequest efr = db.eventFeesRequest.Find(id);
            string EventName = db.Event.Find(efr.EventtId).EventName;
            string UserName = db.Users.Find(efr.ExtendIdentityUserId).FullName;
            return File(efr.file, efr.ContentType, efr.Title + " Fees Request - " + EventName + " - " + UserName + "." + efr.Extension);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddAccountingItem([FromForm] AddAccountingItemModel obj)
        {
            return Ok(rep.AddAccountingItem(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddAccountingItemRequest([FromForm] AddAccountingItemModel obj)
        {
            return Ok(rep.AddAccountingItemRequest(obj));
        }

        [Route("[controller]/[Action]/{UserId}")]
        [HttpGet]
        public IActionResult GetMyEventsTotalFees(string UserId)
        {
            return Ok(rep.GetMyEventsTotalFees(UserId));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetEventsTotalFees()
        {
            return Ok(rep.GetEventsTotalFees());
        }


        [Route("[controller]/[Action]/{EventId}/{UserId}")]
        [HttpGet]
        public IActionResult GetMyEventAccounting(int EventId, string UserId)
        {
            return Ok(rep.GetMyEventAccounting(EventId, UserId));
        }

        [Route("[controller]/[Action]/{EventId}/{UserId}")]
        [HttpGet]
        public IActionResult GetMyEventAccountingRequests(int EventId, string UserId)
        {
            return Ok(rep.GetMyEventAccountingRequests(EventId, UserId));
        }
    }
}

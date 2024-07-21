using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
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
using Microsoft.Net.Http.Headers;
using Org.BouncyCastle.Utilities.Encoders;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class EventController : ControllerBase
    {
        private readonly IEventRep rep;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly ITimeRep ti;
        private readonly DbContainer db;

        public EventController(IEventRep rep, UserManager<ExtendIdentityUser> userManager,ITimeRep ti, DbContainer db)
        {
            this.rep = rep;
            this.userManager = userManager;
            this.ti = ti;
            this.db = db;
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetSixMonthEvents()
        {
            return Ok(rep.GetSixMonthEvents());
        }



        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetWorkshopById(int id)
        {
            return Ok(rep.GetWorkshopById(id));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditWorkshop(EditWorkshopModel obj)
        {
            return Ok(rep.EditWorkshop(obj));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public async Task<IActionResult> DownloadTicketsByEventId(int id)
        {
            Event e = db.Event.Find(id);
            string foldername = e.EventName.Split('/')[0] + " Tickets";
            List<EventTravelRequest> list = db.EventTravelRequest.Where(a => a.EventId == id && a.IsTicket == true).ToList();
            if (list.Count == 0)
            {
                return Ok("There is no Passports uploaded for this event");
            }
            string sourcePath = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/Ticket");
            string targetPath = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/AllTicket", foldername);

            Directory.CreateDirectory(targetPath);
            foreach (var item in list)
            {
                Contact c = db.contact.Find(item.ContactId);
                string ext = item.TicketFileContentType.Split('/')[1];
                string sourceFile = System.IO.Path.Combine(sourcePath, item.TicketFileName);
                string destFile = System.IO.Path.Combine(targetPath, c.ContactName + "." + ext);
                System.IO.File.Copy(sourceFile, destFile, true);
            }

            string startPath = targetPath;
            string zipPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/AllTicket", foldername + " Tickets.zip");
            ZipFile.CreateFromDirectory(startPath, zipPath);

            var memory = new MemoryStream();
            using (var stream = new FileStream(zipPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }



            memory.Position = 0;


            System.IO.DirectoryInfo di = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/AllTicket"));

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            return File(memory, "application/zip", e.EventName + " Tickets.zip");

        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public async Task<IActionResult> DownloadPassportsByEventId(int id)
        {
            Event e = db.Event.Find(id);
            string foldername = e.EventName.Split('/')[0] + " Passports";
            List<EventTravelRequest> list = db.EventTravelRequest.Where(a => a.EventId == id && a.IsPassport == true).ToList();
            if (list.Count == 0)
            {
                return Ok("There is no Passports uploaded for this event");
            }
            string sourcePath = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/Passport");
            string targetPath = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/AllPassport", foldername);

            Directory.CreateDirectory(targetPath);
            foreach (var item in list)
            {
                Contact c = db.contact.Find(item.ContactId);
                string ext = item.PassportFileContentType.Split('/')[1];
                string sourceFile = System.IO.Path.Combine(sourcePath, item.PassportFileName);
                string destFile = System.IO.Path.Combine(targetPath, c.ContactName + "." + ext);
                System.IO.File.Copy(sourceFile, destFile, true);
            }

            string startPath = targetPath;
            string zipPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/AllPassport", foldername + " Passports.zip");
            ZipFile.CreateFromDirectory(startPath, zipPath);

            var memory = new MemoryStream();
            using (var stream = new FileStream(zipPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }



            memory.Position = 0;


            System.IO.DirectoryInfo di = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/AllPassport"));

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            return File(memory, "application/zip", e.EventName + " Passports.zip");

        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllPastRequests()
        {
            return Ok(rep.GetAllPastRequests());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetEventsForSystemAdmin()
        {
            return Ok(rep.GetEventsForSystemAdmin());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult EventContacts()
        {
            List<Contact> c = new List<Contact>();

            List<EventTravelRequest> t = db.EventTravelRequest.ToList();

            //List<EventTravelRequest> list = DistinctByExtension.DistinctBy(t, a => a.ContactId).OrderBy(a => a.ContactId).OrderBy(a=>a.EventId).ToList();

            var x = t.Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new
            {
                Representative = b.FullName,
                EventId = a.EventId,
                ContactId = a.ContactId
            }).Join(db.Event, a => a.EventId, b => b.Id, (a, b) => new
            {
                Representative = a.Representative,
                Event = b.EventName,
                Date = b.From,
                ContactId = a.ContactId
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Representative = a.Representative,
                Event = a.Event,
                Date = a.Date,
                Contact = b.ContactName
            }).ToList();

            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet("Event Contacts");

            worksheet.Cell("A1").Value = "Name";
            worksheet.Cell("B1").Value = "Event";
            worksheet.Cell("C1").Value = "Date";
            worksheet.Cell("D1").Value = "Representative";
            worksheet.Column("A").Width = 35;
            worksheet.Column("B").Width = 50;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 30;
            worksheet.Range("A1:D2000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A1:D2000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:D1").Style.Font.FontSize = 18;
            worksheet.Range("A1:D1").Style.Font.Bold = true;
            worksheet.Range("A2:D2000").Style.Font.FontSize = 14;



            var row = 2;


            foreach (var item in x)
            {
                // Event e = db.Event.Find(item.EventId);
                worksheet.Cell("A" + row).Value = item.Contact;
                worksheet.Cell("B" + row).Value = item.Event;
                worksheet.Cell("C" + row).Value = item.Date;
                worksheet.Cell("D" + row).Value = item.Representative;

                row++;
            }


            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Events Attended Contacts.xlsx");
        }



        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DownloadEventPassports(int id)
        {
            List<EventTravelRequest> t = db.EventTravelRequest.Where(a => a.EventId == id && a.IsPassport == true).ToList();
            string eventname = db.Event.Find(id).EventName;
            System.IO.File.Create(Directory.GetCurrentDirectory() + "/Passport/" + eventname);
            foreach (var item in t)
            {
                System.IO.File.Copy(Directory.GetCurrentDirectory() + "/wwwroot/Passport/" + item.PassportFileName, Directory.GetCurrentDirectory() + "/wwwroot/Passport/" + eventname + "/" + item.PassportFileName, true);
                //string startPath = Directory.GetCurrentDirectory() + "/image/inv";
                //string zipPath = Directory.GetCurrentDirectory() + "/image/inv.zip";
                //ZipFile.CreateFromDirectory(startPath, zipPath);



               
            }
            return Ok(true);
            //MemoryStream memory = new MemoryStream();
            //var stream = new FileStream(zipPath, FileMode.Open);

            //stream.CopyTo(memory);
            //var content = memory.ToArray();

            //return File(
            //    content,
            //    "application/zip",
            //    eventname + " Passports.zip");
        }

            [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult TedSheet()
        {
            DateTime now = ti.GetCurrentTime();
            List<CustomEventRequest> res = rep.GetAllRequestsByEventId(9).OrderBy(a=>a.ContactName).ToList();

            XLWorkbook workbook = new XLWorkbook();


            var worksheet = workbook.Worksheets.Add("Riyadh");
            var worksheetout = workbook.Worksheets.Add("Out Of Riyadh");
            worksheet.Cell("E2").Style.Fill.BackgroundColor = XLColor.FromArgb(172, 185, 202);
            worksheet.Range("E2:I2").Merge();
            worksheet.Range("A6:A7").Merge();
            worksheet.Range("B6:B7").Merge();
            worksheet.Range("C6:C7").Merge();
            worksheet.Range("D6:D7").Merge();
            worksheet.Range("E6:E7").Merge();
            worksheet.Range("F5:H5").Merge();
            worksheet.Range("F6:F7").Merge();
            worksheet.Range("G6:G7").Merge();
            worksheet.Range("H6:H7").Merge();
            worksheet.Range("U6:U7").Merge();
            worksheet.Range("V6:V7").Merge();
            worksheet.Range("W6:W7").Merge();
            worksheet.Range("I6:N6").Merge();
            worksheet.Range("O6:T6").Merge();
            worksheet.Range("U6:U7").Merge();
            worksheet.Range("V6:V7").Merge();
            worksheet.Range("W6:W7").Merge();
            worksheet.Range("X6:X7").Merge();
            worksheet.Range("Y6:Y7").Merge();
            worksheet.Range("Z6:Z7").Merge();

            worksheet.Cell("F5").Value = res.FirstOrDefault().EventName;
            worksheet.Cell("A6").Value = "Serial";
            worksheet.Cell("B6").Value = "";
            worksheet.Cell("C6").Value = "Contact Name";
            worksheet.Cell("D6").Value = "Phone Number";
            worksheet.Cell("E6").Value = "Email";
            worksheet.Cell("F6").Value = "Account Affiliation";
            worksheet.Cell("G6").Value = "Passport Number";
            worksheet.Cell("H6").Value = "Passport Expiry Date";

            worksheet.Cell("I6").Value = "WAY IN";
            worksheet.Cell("I7").Value = "Date";
            worksheet.Cell("J7").Value = "Origin";
            worksheet.Cell("K7").Value = "Departure";
            worksheet.Cell("L7").Value = "Flight/Train";
            worksheet.Cell("M7").Value = "Destination";
            worksheet.Cell("N7").Value = "Arrival";

            worksheet.Cell("O6").Value = "WAY OUT";
            worksheet.Cell("O7").Value = "Date";
            worksheet.Cell("P7").Value = "Origin";
            worksheet.Cell("Q7").Value = "Departure";
            worksheet.Cell("R7").Value = "Flight/Train";
            worksheet.Cell("S7").Value = "Destination";
            worksheet.Cell("T7").Value = "Arrival";

            worksheet.Cell("U6").Value = "Hotel Name";
            worksheet.Cell("V6").Value = "Room Type";
            worksheet.Cell("W6").Value = "Accumpained";

            worksheet.Cell("X6").Value = "Representative";
            worksheet.Cell("Y6").Value = "Download";
            worksheet.Cell("Z6").Value = "Rep City";


            worksheet.Range("A6:H6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 217, 102);
            worksheet.Cell("F5").Style.Fill.BackgroundColor = XLColor.FromArgb(226, 107, 10);
            worksheet.Range("I6:N7").Style.Fill.BackgroundColor = XLColor.FromArgb(191, 191, 191);
            worksheet.Range("O6:T7").Style.Fill.BackgroundColor = XLColor.FromArgb(132, 151, 176);
            worksheet.Range("U6:W7").Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);

            worksheet.Range("A2:Z300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A2:Z300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Rows("2:500").Height = 20;
            worksheet.Row(5).Height = 25;
            worksheet.Column("A").Width = 8.5;
            worksheet.Column("B").Width = 8.5;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 20;
            worksheet.Column("E").Width = 20;
            worksheet.Column("F").Width = 25;
            worksheet.Column("G").Width = 20;
            worksheet.Column("H").Width = 25;

            worksheet.Column("I").Width = 18;
            worksheet.Column("J").Width = 15;
            worksheet.Column("K").Width = 25;
            worksheet.Column("L").Width = 18;
            worksheet.Column("M").Width = 15;
            worksheet.Column("N").Width = 25;

            worksheet.Column("O").Width = 18;
            worksheet.Column("P").Width = 15;
            worksheet.Column("Q").Width = 25;
            worksheet.Column("R").Width = 18;
            worksheet.Column("S").Width = 15;
            worksheet.Column("T").Width = 25;

            worksheet.Column("U").Width = 20;
            worksheet.Column("V").Width = 20;
            worksheet.Column("W").Width = 20;
            worksheet.Column("X").Width = 30;
            worksheet.Column("Y").Width = 25;
            worksheet.Column("Z").Width = 25;

            worksheet.Cell("E2").Style.Font.FontSize = 18;
            worksheet.Cell("F5").Style.Font.FontSize = 18;
            worksheet.Range("A6:Z7").Style.Font.FontSize = 14;
            worksheet.Range("A8:Z300").Style.Font.FontSize = 14;
            worksheet.Cell("E2").Style.Font.Bold = true;
            worksheet.Range("A6:Z7").Style.Font.Bold = true;
            worksheet.Cell("E2").Style.Font.Underline = XLFontUnderlineValues.Single;

            worksheet.Range("E2:I2").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            worksheet.Range("A6:Z300").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A6:Z300").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell("F4").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
            worksheet.Cell("G4").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);


            worksheet.Cell("F4").Value = "Accepted";
            worksheet.Cell("G4").Value = "Rejected";
            worksheet.Cell("H4").Value = "Pending";

            worksheet.Range("F4:H4").Style.Font.FontSize = 14;
            worksheet.Range("F4:H4").Style.Font.Bold = true;

            string dir = "https://{{URL}}.com/Event/DownloadPassport/";
            int row = 8;
            int s = 1;
            foreach (var item in res.Where(a=>a.RepCity == "الرياض"))
            {
                if (item.RepName == "MOKHTAR ELWAKEEL")
                {
                    continue;
                }
                worksheet.Cell("A" + row).Value = s;
                if (item.TopAction == true)
                {
                    if (item.TopConfirmed == true)
                    {
                        worksheet.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }
                    if (item.TopRejected == true)
                    {
                        worksheet.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }
                }
                else
                {
                    if (item.Confirmed == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }

                    if (item.Rejected == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }

                    if (item.Confirmed == false && item.Rejected == false)
                    {

                    }
                }

                switch (item.Gender)
                {
                    case false:
                        worksheet.Cell("B" + row).Value = "Mr";
                        break;
                    case true:
                        worksheet.Cell("B" + row).Value = "Mrs";
                        break;
                    default:
                        worksheet.Cell("B" + row).Value = "";
                        break;

                }

                worksheet.Cell("C" + row).Value = item.ContactName;
                worksheet.Cell("D" + row).Value = item.ContactPhone;
                worksheet.Cell("E" + row).Value = item.ContactMail;
                worksheet.Cell("F" + row).Value = item.AccountAffiliation;
                worksheet.Cell("G" + row).Value = item.PassportNumber;
                worksheet.Cell("H" + row).Value = item.PassportExpiryDate;

                if (item.WayInDeparture != null)
                {
                    DateTime wid = (DateTime)item.WayInDeparture;
                    worksheet.Cell("I" + row).Value = wid.ToString("dd MMMM yyyy");
                    worksheet.Cell("K" + row).Value = wid.TimeOfDay;
                }
                if (item.WayInArrival != null)
                {
                    DateTime wia = (DateTime)item.WayInArrival;
                    worksheet.Cell("N" + row).Value = wia.TimeOfDay;
                }
                if (item.WayOutDeparture != null)
                {
                    DateTime wod = (DateTime)item.WayOutDeparture;
                    worksheet.Cell("O" + row).Value = wod.ToString("dd MMMM yyyy");
                    worksheet.Cell("Q" + row).Value = wod.TimeOfDay;
                }
                if (item.WayOutArrival != null)
                {
                    DateTime woa = (DateTime)item.WayOutArrival;
                    worksheet.Cell("T" + row).Value = woa.TimeOfDay;
                }




                worksheet.Cell("I" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("J" + row).Value = item.WayInCityName;
                worksheet.Cell("L" + row).Value = item.WayInFlightNumber;
                worksheet.Cell("M" + row).Value = item.WayInDestinationName;



                worksheet.Cell("O" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("P" + row).Value = item.WayOutCityName;

                worksheet.Cell("R" + row).Value = item.WayOutFlightNumber;
                worksheet.Cell("S" + row).Value = item.WayOutDestinationName;




                worksheet.Cell("U" + row).Value = item.HotelName;
                worksheet.Cell("V" + row).Value = item.RoomType;
                worksheet.Cell("W" + row).Value = item.Accumpained;
                worksheet.Cell("X" + row).Value = item.RepName;
                if (item.IsPassport == true)
                {
                    worksheet.Cell("Y" + row).Value = "Download";
                    Uri x = new Uri(dir + item.Id);
                    worksheet.Cell("Y" + row).Hyperlink.ExternalAddress = x;
                }
                worksheet.Cell("Z" + row).Value = item.RepCity;
                row++;
                s++;
            }




            worksheetout.Cell("E2").Style.Fill.BackgroundColor = XLColor.FromArgb(172, 185, 202);
            worksheetout.Range("E2:I2").Merge();
            worksheetout.Range("A6:A7").Merge();
            worksheetout.Range("B6:B7").Merge();
            worksheetout.Range("C6:C7").Merge();
            worksheetout.Range("D6:D7").Merge();
            worksheetout.Range("E6:E7").Merge();
            worksheetout.Range("F5:H5").Merge();
            worksheetout.Range("F6:F7").Merge();
            worksheetout.Range("G6:G7").Merge();
            worksheetout.Range("H6:H7").Merge();
            worksheetout.Range("U6:U7").Merge();
            worksheetout.Range("V6:V7").Merge();
            worksheetout.Range("W6:W7").Merge();
            worksheetout.Range("I6:N6").Merge();
            worksheetout.Range("O6:T6").Merge();
            worksheetout.Range("U6:U7").Merge();
            worksheetout.Range("V6:V7").Merge();
            worksheetout.Range("W6:W7").Merge();
            worksheetout.Range("X6:X7").Merge();
            worksheetout.Range("Y6:Y7").Merge();
            worksheetout.Range("Z6:Z7").Merge();

            worksheetout.Cell("F5").Value = res.FirstOrDefault().EventName;
            worksheetout.Cell("A6").Value = "Serial";
            worksheetout.Cell("B6").Value = "";
            worksheetout.Cell("C6").Value = "Contact Name";
            worksheetout.Cell("D6").Value = "Phone Number";
            worksheetout.Cell("E6").Value = "Email";
            worksheetout.Cell("F6").Value = "Account Affiliation";
            worksheetout.Cell("G6").Value = "Passport Number";
            worksheetout.Cell("H6").Value = "Passport Expiry Date";

            worksheetout.Cell("I6").Value = "WAY IN";
            worksheetout.Cell("I7").Value = "Date";
            worksheetout.Cell("J7").Value = "Origin";
            worksheetout.Cell("K7").Value = "Departure";
            worksheetout.Cell("L7").Value = "Flight/Train";
            worksheetout.Cell("M7").Value = "Destination";
            worksheetout.Cell("N7").Value = "Arrival";

            worksheetout.Cell("O6").Value = "WAY OUT";
            worksheetout.Cell("O7").Value = "Date";
            worksheetout.Cell("P7").Value = "Origin";
            worksheetout.Cell("Q7").Value = "Departure";
            worksheetout.Cell("R7").Value = "Flight/Train";
            worksheetout.Cell("S7").Value = "Destination";
            worksheetout.Cell("T7").Value = "Arrival";

            worksheetout.Cell("U6").Value = "Hotel Name";
            worksheetout.Cell("V6").Value = "Room Type";
            worksheetout.Cell("W6").Value = "Accumpained";

            worksheetout.Cell("X6").Value = "Representative";
            worksheetout.Cell("Y6").Value = "Download";
            worksheetout.Cell("Z6").Value = "Rep City";


            worksheetout.Range("A6:H6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 217, 102);
            worksheetout.Cell("F5").Style.Fill.BackgroundColor = XLColor.FromArgb(226, 107, 10);
            worksheetout.Range("I6:N7").Style.Fill.BackgroundColor = XLColor.FromArgb(191, 191, 191);
            worksheetout.Range("O6:T7").Style.Fill.BackgroundColor = XLColor.FromArgb(132, 151, 176);
            worksheetout.Range("U6:W7").Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);

            worksheetout.Range("A2:Z300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheetout.Range("A2:Z300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheetout.Rows("2:500").Height = 20;
            worksheetout.Row(5).Height = 25;
            worksheetout.Column("A").Width = 8.5;
            worksheetout.Column("B").Width = 8.5;
            worksheetout.Column("C").Width = 20;
            worksheetout.Column("D").Width = 20;
            worksheetout.Column("E").Width = 20;
            worksheetout.Column("F").Width = 25;
            worksheetout.Column("G").Width = 20;
            worksheetout.Column("H").Width = 25;

            worksheetout.Column("I").Width = 18;
            worksheetout.Column("J").Width = 15;
            worksheetout.Column("K").Width = 25;
            worksheetout.Column("L").Width = 18;
            worksheetout.Column("M").Width = 15;
            worksheetout.Column("N").Width = 25;

            worksheetout.Column("O").Width = 18;
            worksheetout.Column("P").Width = 15;
            worksheetout.Column("Q").Width = 25;
            worksheetout.Column("R").Width = 18;
            worksheetout.Column("S").Width = 15;
            worksheetout.Column("T").Width = 25;

            worksheetout.Column("U").Width = 20;
            worksheetout.Column("V").Width = 20;
            worksheetout.Column("W").Width = 20;
            worksheetout.Column("X").Width = 30;
            worksheetout.Column("Y").Width = 25;
            worksheetout.Column("Z").Width = 25;

            worksheetout.Cell("E2").Style.Font.FontSize = 18;
            worksheetout.Cell("F5").Style.Font.FontSize = 18;
            worksheetout.Range("A6:Z7").Style.Font.FontSize = 14;
            worksheetout.Range("A8:Z300").Style.Font.FontSize = 14;
            worksheetout.Cell("E2").Style.Font.Bold = true;
            worksheetout.Range("A6:Z7").Style.Font.Bold = true;
            worksheetout.Cell("E2").Style.Font.Underline = XLFontUnderlineValues.Single;

            worksheetout.Range("E2:I2").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            worksheetout.Range("A6:Z300").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheetout.Range("A6:Z300").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheetout.Cell("F4").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
            worksheetout.Cell("G4").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);


            worksheetout.Cell("F4").Value = "Accepted";
            worksheetout.Cell("G4").Value = "Rejected";
            worksheetout.Cell("H4").Value = "Pending";

            worksheetout.Range("F4:H4").Style.Font.FontSize = 14;
            worksheetout.Range("F4:H4").Style.Font.Bold = true;




            row = 8;
             s = 1;
            foreach (var item in res.Where(a => a.RepCity != "الرياض"))
            {
                worksheetout.Cell("A" + row).Value = s;
                if (item.TopAction == true)
                {
                    if (item.TopConfirmed == true)
                    {
                        worksheetout.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }
                    if (item.TopRejected == true)
                    {
                        worksheetout.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }
                }
                else
                {
                    if (item.Confirmed == true)
                    {
                        worksheetout.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }

                    if (item.Rejected == true)
                    {
                        worksheetout.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }

                    if (item.Confirmed == false && item.Rejected == false)
                    {

                    }
                }

                switch (item.Gender)
                {
                    case false:
                        worksheetout.Cell("B" + row).Value = "Mr";
                        break;
                    case true:
                        worksheetout.Cell("B" + row).Value = "Mrs";
                        break;
                    default:
                        worksheetout.Cell("B" + row).Value = "";
                        break;

                }

                worksheetout.Cell("C" + row).Value = item.ContactName;
                worksheetout.Cell("D" + row).Value = item.ContactPhone;
                worksheetout.Cell("E" + row).Value = item.ContactMail;
                worksheetout.Cell("F" + row).Value = item.AccountAffiliation;
                worksheetout.Cell("G" + row).Value = item.PassportNumber;
                worksheetout.Cell("H" + row).Value = item.PassportExpiryDate;

                if (item.WayInDeparture != null)
                {
                    DateTime wid = (DateTime)item.WayInDeparture;
                    worksheetout.Cell("I" + row).Value = wid.ToString("dd MMMM yyyy");
                    worksheetout.Cell("K" + row).Value = wid.TimeOfDay;
                }
                if (item.WayInArrival != null)
                {
                    DateTime wia = (DateTime)item.WayInArrival;
                    worksheetout.Cell("N" + row).Value = wia.TimeOfDay;
                }
                if (item.WayOutDeparture != null)
                {
                    DateTime wod = (DateTime)item.WayOutDeparture;
                    worksheetout.Cell("O" + row).Value = wod.ToString("dd MMMM yyyy");
                    worksheetout.Cell("Q" + row).Value = wod.TimeOfDay;
                }
                if (item.WayOutArrival != null)
                {
                    DateTime woa = (DateTime)item.WayOutArrival;
                    worksheetout.Cell("T" + row).Value = woa.TimeOfDay;
                }




                worksheetout.Cell("I" + row).DataType = XLDataType.DateTime;
                worksheetout.Cell("J" + row).Value = item.WayInCityName;
                worksheetout.Cell("L" + row).Value = item.WayInFlightNumber;
                worksheetout.Cell("M" + row).Value = item.WayInDestinationName;



                worksheetout.Cell("O" + row).DataType = XLDataType.DateTime;
                worksheetout.Cell("P" + row).Value = item.WayOutCityName;

                worksheetout.Cell("R" + row).Value = item.WayOutFlightNumber;
                worksheetout.Cell("S" + row).Value = item.WayOutDestinationName;




                worksheetout.Cell("U" + row).Value = item.HotelName;
                worksheetout.Cell("V" + row).Value = item.RoomType;
                worksheetout.Cell("W" + row).Value = item.Accumpained;
                worksheetout.Cell("X" + row).Value = item.RepName;
                if (item.IsPassport == true)
                {
                    worksheetout.Cell("Y" + row).Value = "Download";
                    Uri x = new Uri(dir + item.Id);
                    worksheetout.Cell("Y" + row).Hyperlink.ExternalAddress = x;
                }
                worksheetout.Cell("Z" + row).Value = item.RepCity;
                row++;
                s++;
            }
            IEnumerable<CustomEventRequest> r = res.Where(a => a.RepName == "MOKHTAR ELWAKEEL");

            foreach (var item in r)
            {
                worksheetout.Cell("A" + row).Value = s;
                if (item.TopAction == true)
                {
                    if (item.TopConfirmed == true)
                    {
                        worksheetout.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }
                    if (item.TopRejected == true)
                    {
                        worksheetout.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }
                }
                else
                {
                    if (item.Confirmed == true)
                    {
                        worksheetout.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }

                    if (item.Rejected == true)
                    {
                        worksheetout.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }

                    if (item.Confirmed == false && item.Rejected == false)
                    {

                    }
                }

                switch (item.Gender)
                {
                    case false:
                        worksheetout.Cell("B" + row).Value = "Mr";
                        break;
                    case true:
                        worksheetout.Cell("B" + row).Value = "Mrs";
                        break;
                    default:
                        worksheetout.Cell("B" + row).Value = "";
                        break;

                }

                worksheetout.Cell("C" + row).Value = item.ContactName;
                worksheetout.Cell("D" + row).Value = item.ContactPhone;
                worksheetout.Cell("E" + row).Value = item.ContactMail;
                worksheetout.Cell("F" + row).Value = item.AccountAffiliation;
                worksheetout.Cell("G" + row).Value = item.PassportNumber;
                worksheetout.Cell("H" + row).Value = item.PassportExpiryDate;

                if (item.WayInDeparture != null)
                {
                    DateTime wid = (DateTime)item.WayInDeparture;
                    worksheetout.Cell("I" + row).Value = wid.ToString("dd MMMM yyyy");
                    worksheetout.Cell("K" + row).Value = wid.TimeOfDay;
                }
                if (item.WayInArrival != null)
                {
                    DateTime wia = (DateTime)item.WayInArrival;
                    worksheetout.Cell("N" + row).Value = wia.TimeOfDay;
                }
                if (item.WayOutDeparture != null)
                {
                    DateTime wod = (DateTime)item.WayOutDeparture;
                    worksheetout.Cell("O" + row).Value = wod.ToString("dd MMMM yyyy");
                    worksheetout.Cell("Q" + row).Value = wod.TimeOfDay;
                }
                if (item.WayOutArrival != null)
                {
                    DateTime woa = (DateTime)item.WayOutArrival;
                    worksheetout.Cell("T" + row).Value = woa.TimeOfDay;
                }




                worksheetout.Cell("I" + row).DataType = XLDataType.DateTime;
                worksheetout.Cell("J" + row).Value = item.WayInCityName;
                worksheetout.Cell("L" + row).Value = item.WayInFlightNumber;
                worksheetout.Cell("M" + row).Value = item.WayInDestinationName;



                worksheetout.Cell("O" + row).DataType = XLDataType.DateTime;
                worksheetout.Cell("P" + row).Value = item.WayOutCityName;

                worksheetout.Cell("R" + row).Value = item.WayOutFlightNumber;
                worksheetout.Cell("S" + row).Value = item.WayOutDestinationName;




                worksheetout.Cell("U" + row).Value = item.HotelName;
                worksheetout.Cell("V" + row).Value = item.RoomType;
                worksheetout.Cell("W" + row).Value = item.Accumpained;
                worksheetout.Cell("X" + row).Value = item.RepName;
                if (item.IsPassport == true)
                {
                    worksheetout.Cell("Y" + row).Value = "Download";
                    Uri x = new Uri(dir + item.Id);
                    worksheetout.Cell("Y" + row).Hyperlink.ExternalAddress = x;
                }
                worksheetout.Cell("Z" + row).Value = item.RepCity;
                row++;
                s++;
            }

            MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(
     content,
     "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    "Workshop Report.xlsx");
        }


        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetEventSpeakersAndLocation(int id)
        {
            Event ev = (rep.GetEventSpeakersAndLocation(id));
            return Ok(ev);


        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult FirstRequestsBulkAction(ProposalBulkModel m)
        {
            switch (m.Action)
            {
                case 1:
                    foreach (var item in m.Ids)
                    {
                        rep.ConfirmRequest(item);
                    }
                    break;
                case 2:
                    foreach (var item in m.Ids)
                    {
                        rep.RejectRequest(item);
                    }
                    break;
                case 3:
                    foreach (var item in m.Ids)
                    {
                        rep.HoldRequest(item);
                    }
                    break;
                case 4:
                    foreach (var item in m.Ids)
                    {
                        rep.DeleteRequest(item);
                    }
                    break;
            }
            return Ok(true);
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult FirstProposalBulkAction(ProposalBulkModel m)
        {
            switch (m.Action)
            {
                case 1:
                    foreach (var item in m.Ids)
                    {
                        rep.ConfirmProposal(item);
                    }
                    break;
                case 2:
                    foreach (var item in m.Ids)
                    {
                        rep.RejectProposal(item);
                    }
                    break;
                case 3:
                    foreach (var item in m.Ids)
                    {
                        rep.HoldProposal(item);
                    }
                    break;
                case 4:
                    foreach (var item in m.Ids)
                    {
                        rep.DeleteProposal(item);
                    }
                    break;
            }
            return Ok(true);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult TopRequestsBulkAction(ProposalBulkModel m)
        {
            switch (m.Action)
            {
                case 1:
                    foreach (var item in m.Ids)
                    {
                        rep.TopConfirmRequest(item, m.TopId);
                    }
                    break;
                case 2:
                    foreach (var item in m.Ids)
                    {
                        rep.TopRejectRequest(item, m.TopId);
                    }
                    break;
                case 3:
                    foreach (var item in m.Ids)
                    {
                        rep.TopHoldRequest(item);
                    }
                    break;
                case 4:
                    foreach (var item in m.Ids)
                    {
                        rep.DeleteRequest(item);
                    }
                    break;
            }
            return Ok(true);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult TopProposalBulkAction(ProposalBulkModel m)
        {
            switch (m.Action)
            {
                case 1:
                    foreach (var item in m.Ids)
                    {
                        rep.TopConfirmProposal(item, m.TopId);
                    }
                    break;
                case 2:
                    foreach (var item in m.Ids)
                    {
                        rep.TopRejectProposal(item, m.TopId);
                    }
                    break;
                case 3:
                    foreach (var item in m.Ids)
                    {
                        rep.TopHoldProposal(item);
                    }
                    break;
                case 4:
                    foreach (var item in m.Ids)
                    {
                        rep.DeleteProposal(item);
                    }
                    break;
            }
            return Ok(true);
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetWorkshopInfo(int id)
        {
            return Ok(rep.GetWorkshopInfo(id));
        }


        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeletePassport(int id)
        {
            return Ok(rep.DeletePassport(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeleteTicket(int id)
        {
            return Ok(rep.DeleteTicket(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetEventById(int id)
        {
            return Ok(rep.GetEventById(id));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditEvent(Event ev)
        {
            return Ok(rep.EditEvent(ev));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeleteProposal(int id)
        {
            return Ok(rep.DeleteProposal(id));
        }

        [Route("[controller]/[Action]/{id}/{TopId}")]
        [HttpGet]
        public IActionResult TopConfirmProposal(int id, string TopId)
        {
            return Ok(rep.TopConfirmProposal(id, TopId));
        }

        [Route("[controller]/[Action]/{id}/{TopId}")]
        [HttpGet]
        public IActionResult TopRejectProposal(int id, string TopId)
        {
            return Ok(rep.TopRejectProposal(id, TopId));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult TopHoldProposal(int id)
        {
            return Ok(rep.TopHoldProposal(id));
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllProposalRequests()
        {
            return Ok(rep.GetAllProposalRequests());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllPendingProposalRequests()
        {
            return Ok(rep.GetAllPendingProposalRequests());
        }

        [Route("[controller]/[Action]/{Id}")]
        [HttpGet]
        public IActionResult ConfirmProposal(int Id)
        {
            return Ok(rep.ConfirmProposal(Id));
        }

        [Route("[controller]/[Action]/{Id}")]
        [HttpGet]
        public IActionResult RejectProposal(int Id)
        {
            return Ok(rep.RejectProposal(Id));
        }

        [Route("[controller]/[Action]/{Id}")]
        [HttpGet]
        public IActionResult HoldProposal(int Id)
        {
            return Ok(rep.HoldProposal(Id));
        }

        [Route("[controller]/[Action]/{ManagerId}")]
        [HttpGet]
        public IActionResult GetMyTeamProposalRequests(string ManagerId)
        {
            return Ok(rep.GetMyTeamProposalRequests(ManagerId));
        }

        [Route("[controller]/[Action]/{ManagerId}")]
        [HttpGet]
        public IActionResult GetMyTeamPendingProposalRequests(string ManagerId)
        {
            return Ok(rep.GetMyTeamPendingProposalRequests(ManagerId));
        }

        [Route("[controller]/[Action]/{UserId}/{EventId}")]
        [HttpGet]
        public IActionResult GetApprovedEventContacts(string UserId, int EventId)
        {
            return Ok(rep.GetApprovedEventContacts(UserId,EventId));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetMyProposalRequests(string id)
        {
            return Ok(rep.GetMyProposalRequests(id));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult MakeEventProposal(List<EventProposalRequest> evp)
        {
            return Ok(rep.MakeEventProposal(evp));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllRequests()
        {
            IEnumerable<CustomEventRequest> res = rep.GetAllRequests();
            return Ok(res);
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult TopHoldRequest(int id)
        {
            bool res = rep.TopHoldRequest(id);
            return Ok(res);
        }

        [Route("[controller]/[Action]/{id}/{TopId}")]
        [HttpGet]
        public IActionResult TopConfirmRequest(int id, string TopId)
        {
            bool res = rep.TopConfirmRequest(id, TopId);
            return Ok(res);
        }

        [Route("[controller]/[Action]/{id}/{TopId}")]
        [HttpGet]
        public IActionResult TopRejectRequest(int id, string TopId)
        {
            bool res = rep.TopRejectRequest(id, TopId);
            return Ok(res);
        }

        [Route("[controller]/[Action]/{id}/{ManagerId}")]
        [HttpGet]
        public IActionResult GetMyTeamRequestsByEventId(int Id, string ManagerId)
        {
            IEnumerable<CustomEventRequest> res = rep.GetMyTeamRequestsByEventId(Id, ManagerId);

            DateTime now = ti.GetCurrentTime();
            string ManagerName = userManager.FindByIdAsync(ManagerId).Result.FullName;
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Events Requests Report");
            worksheet.Cell("E2").Style.Fill.BackgroundColor = XLColor.FromArgb(172, 185, 202);
            worksheet.Range("E2:I2").Merge();
            worksheet.Cell("E2").Value = "Exported By: Dr. " + ManagerName + " On " + now.ToString("dd MMMM yyyy - hh:mm tt");
            worksheet.Range("A6:A7").Merge();
            worksheet.Range("B6:B7").Merge();
            worksheet.Range("C6:C7").Merge();
            worksheet.Range("D6:D7").Merge();
            worksheet.Range("E6:E7").Merge();
            worksheet.Range("F6:F7").Merge();
            worksheet.Range("G6:G7").Merge();
            worksheet.Range("H6:H7").Merge();
            worksheet.Range("U6:U7").Merge();
            worksheet.Range("V6:V7").Merge();
            worksheet.Range("W6:W7").Merge();
            worksheet.Range("I6:N6").Merge();
            worksheet.Range("O6:T6").Merge();
            worksheet.Range("U6:U7").Merge();
            worksheet.Range("V6:V7").Merge();
            worksheet.Range("W6:W7").Merge();
            worksheet.Range("X6:X7").Merge();
            worksheet.Range("Y6:Y7").Merge();



            worksheet.Cell("A6").Value = "Serial";
            worksheet.Cell("B6").Value = "";
            worksheet.Cell("C6").Value = "Contact Name";
            worksheet.Cell("D6").Value = "Phone Number";
            worksheet.Cell("E6").Value = "Email";
            worksheet.Cell("F6").Value = "Account Affiliation";
            worksheet.Cell("G6").Value = "Passport Number";
            worksheet.Cell("H6").Value = "Passport Expiry Date";

            worksheet.Cell("I6").Value = "WAY IN";
            worksheet.Cell("I7").Value = "Date";
            worksheet.Cell("J7").Value = "Origin";
            worksheet.Cell("K7").Value = "Departure";
            worksheet.Cell("L7").Value = "Flight/Train";
            worksheet.Cell("M7").Value = "Destination";
            worksheet.Cell("N7").Value = "Arrival";

            worksheet.Cell("O6").Value = "WAY OUT";
            worksheet.Cell("O7").Value = "Date";
            worksheet.Cell("P7").Value = "Origin";
            worksheet.Cell("Q7").Value = "Departure";
            worksheet.Cell("R7").Value = "Flight/Train";
            worksheet.Cell("S7").Value = "Destination";
            worksheet.Cell("T7").Value = "Arrival";

            worksheet.Cell("U6").Value = "Hotel Name";
            worksheet.Cell("V6").Value = "Room Type";
            worksheet.Cell("W6").Value = "Accumpained";

            worksheet.Cell("X6").Value = "Representative";
            worksheet.Cell("Y6").Value = "Passport";


            worksheet.Range("A6:H6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 217, 102);
            worksheet.Range("I6:N7").Style.Fill.BackgroundColor = XLColor.FromArgb(191, 191, 191);
            worksheet.Range("O6:T7").Style.Fill.BackgroundColor = XLColor.FromArgb(132, 151, 176);
            worksheet.Range("U6:W7").Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);

            worksheet.Range("A2:X300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A2:X300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Rows("2:300").Height = 20;
            worksheet.Column("A").Width = 8.5;
            worksheet.Column("B").Width = 8.5;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 20;
            worksheet.Column("E").Width = 20;
            worksheet.Column("F").Width = 25;
            worksheet.Column("G").Width = 20;
            worksheet.Column("H").Width = 25;

            worksheet.Column("I").Width = 18;
            worksheet.Column("J").Width = 15;
            worksheet.Column("K").Width = 25;
            worksheet.Column("L").Width = 18;
            worksheet.Column("M").Width = 15;
            worksheet.Column("N").Width = 25;

            worksheet.Column("O").Width = 18;
            worksheet.Column("P").Width = 15;
            worksheet.Column("Q").Width = 25;
            worksheet.Column("R").Width = 18;
            worksheet.Column("S").Width = 15;
            worksheet.Column("T").Width = 25;

            worksheet.Column("U").Width = 20;
            worksheet.Column("V").Width = 20;
            worksheet.Column("W").Width = 20;
            worksheet.Column("X").Width = 30;

            worksheet.Cell("E2").Style.Font.FontSize = 18;
            worksheet.Range("A6:X7").Style.Font.FontSize = 14;
            worksheet.Range("A8:x300").Style.Font.FontSize = 14;
            worksheet.Cell("E2").Style.Font.Bold = true;
            worksheet.Range("A6:X7").Style.Font.Bold = true;
            worksheet.Cell("E2").Style.Font.Underline = XLFontUnderlineValues.Single;

            worksheet.Range("E2:I2").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            worksheet.Range("A6:X300").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A6:X300").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell("F4").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
            worksheet.Cell("G4").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
            

            worksheet.Cell("F4").Value = "Accepted";
            worksheet.Cell("G4").Value = "Rejected";
            worksheet.Cell("H4").Value = "Pending";

            worksheet.Range("F4:H4").Style.Font.FontSize = 14;
            worksheet.Range("F4:H4").Style.Font.Bold = true;

            string dir = "https://{{URL}}.com/Event/DownloadPassport/";
            int row = 8;
            int s = 1;
            foreach (var item in res)
            {
                worksheet.Cell("A" + row).Value = s;
                if (item.Confirmed == true)
                {
                    worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                }

                if (item.Rejected == true)
                {
                    worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                }

                if (item.Confirmed == false && item.Rejected == false)
                {
                    
                }

                switch (item.Gender)
                {
                    case false:
                        worksheet.Cell("B" + row).Value = "Mr";
                        break;
                    case true:
                        worksheet.Cell("B" + row).Value = "Mrs";
                        break;
                    default:
                        worksheet.Cell("B" + row).Value = "";
                        break;

                }

                worksheet.Cell("C" + row).Value = item.ContactName;
                worksheet.Cell("D" + row).Value = item.ContactPhone;
                worksheet.Cell("E" + row).Value = item.ContactMail;
                worksheet.Cell("F" + row).Value = item.AccountAffiliation;
                worksheet.Cell("G" + row).Value = item.PassportNumber;
                worksheet.Cell("H" + row).Value = item.PassportExpiryDate;
                if (item.WayInDeparture != null)
                {
                    DateTime wid = (DateTime)item.WayInDeparture;
                    worksheet.Cell("I" + row).Value = wid.ToString("dd MMMM yyyy");
                    worksheet.Cell("K" + row).Value = wid.TimeOfDay;
                }
                if (item.WayInArrival != null)
                {
                    DateTime wia = (DateTime)item.WayInArrival;
                    worksheet.Cell("N" + row).Value = wia.TimeOfDay;
                }
                if (item.WayOutDeparture != null)
                {
                    DateTime wod = (DateTime)item.WayOutDeparture;
                    worksheet.Cell("O" + row).Value = wod.ToString("dd MMMM yyyy");
                    worksheet.Cell("Q" + row).Value = wod.TimeOfDay;
                }
                if (item.WayOutArrival != null)
                {
                    DateTime woa = (DateTime)item.WayOutArrival;
                    worksheet.Cell("T" + row).Value = woa.TimeOfDay;
                }
                worksheet.Cell("I" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("J" + row).Value = item.WayInCityName;
                
                
                worksheet.Cell("L" + row).Value = item.WayInFlightNumber;
                worksheet.Cell("M" + row).Value = item.WayInDestinationName;
                
                
                worksheet.Cell("O" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("P" + row).Value = item.WayOutCityName;
                
                worksheet.Cell("R" + row).Value = item.WayOutFlightNumber;
                worksheet.Cell("S" + row).Value = item.WayOutDestinationName;
                
                



                worksheet.Cell("U" + row).Value = item.HotelName;
                worksheet.Cell("V" + row).Value = item.RoomType;
                worksheet.Cell("W" + row).Value = item.Accumpained;
                worksheet.Cell("X" + row).Value = item.RepName;

                if (item.IsPassport == true)
                {
                    worksheet.Cell("Y" + row).Value = "Download";
                    Uri x =  new Uri(dir + item.Id);
                    worksheet.Cell("Y" + row).Hyperlink.ExternalAddress = x;
                }

                row++;
                s++;
            }
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               ManagerName + "'s Team Events Travel Requests " + now.ToString("dd MMMM yyyy") + ".xlsx");

        }

        [Route("[controller]/[Action]/{id}/{ManagerId}")]
        [HttpGet]
        public IActionResult GetAllRequestsByEventId(int Id, string ManagerId)
        {
            DateTime now = ti.GetCurrentTime();
            IEnumerable<CustomEventRequest> res = rep.GetAllRequestsByEventId(Id);
            int EventTypeId = db.Event.Find(Id).EventTypeId;
            string EventName = db.Event.Find(Id).EventName;
            string ManagerName = userManager.FindByIdAsync(ManagerId).Result.FullName;
            
            XLWorkbook workbook = new XLWorkbook();
         
            if (EventTypeId == 2)
            {
                WorkshopInfoModel info =  rep.GetWorkshopInfo(Id);
                var worksheet = workbook.Worksheets.Add("Workshop");
                worksheet.Cell("E2").Style.Fill.BackgroundColor = XLColor.FromArgb(172, 185, 202);
                worksheet.Range("E2:I2").Merge();
                worksheet.Cell("E2").Value = "Exported By: Dr. " + ManagerName + " On " + now.ToString("dd MMMM yyyy - hh:mm tt");
                worksheet.Range("F5:H5").Merge();
                worksheet.Range("F6:H6").Merge();
                worksheet.Cell("F5").Value = res.FirstOrDefault().EventName;
                worksheet.Cell("F5").Style.Fill.BackgroundColor = XLColor.FromArgb(226, 107, 10);
                worksheet.Cell("F6").Style.Fill.BackgroundColor = XLColor.FromArgb(226, 107, 10);
                worksheet.Cell("F4").Value = "Accepted";
                worksheet.Cell("G4").Value = "Rejected";
                worksheet.Cell("H4").Value = "Pending";
                worksheet.Cell("F4").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                worksheet.Cell("G4").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                worksheet.Range("A2:I300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A2:I300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Rows("2:300").Height = 20;
                worksheet.Row(5).Height = 25;
                worksheet.Cell("F6").Value = info.location;
                worksheet.Cell("F7").Value = "Speakers";
                worksheet.Cell("G7").Value = "Location";
                worksheet.Cell("H7").Value = "Time";

                worksheet.Cell("E2").Style.Font.FontSize = 18;
                worksheet.Cell("F5").Style.Font.FontSize = 18;
                worksheet.Cell("F6").Style.Font.FontSize = 18;
                
             
                worksheet.Cell("E2").Style.Font.Bold = true;
                
                worksheet.Cell("E2").Style.Font.Underline = XLFontUnderlineValues.Single;

                worksheet.Range("E2:I2").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;


                worksheet.Cell("F7").Style.Font.Bold = true;
                worksheet.Cell("F7").Style.Font.FontSize = 14;

                worksheet.Cell("G7").Style.Font.Bold = true;
                worksheet.Cell("G7").Style.Font.FontSize = 14;

                worksheet.Cell("H7").Style.Font.Bold = true;
                worksheet.Cell("H7").Style.Font.FontSize = 14;

                worksheet.Cell("G8").Value = info.location;
                worksheet.Cell("H8").Value = info.From + " - " + info.To;

                worksheet.AddPicture(Directory.GetCurrentDirectory() + "/wwwroot/logo/logotrans.png").MoveTo(1,1).Scale(.5);
                var row = 8;
                foreach (var item in info.speakers)
                {
                    worksheet.Cell("F" + row).Value = item;
                    row++;
                }



                int srow = row + 1;
              




                worksheet.Range("A" + row + ":A" + srow).Merge();
                worksheet.Range("B" + row + ":B" + srow).Merge();
                worksheet.Range("C" + row + ":C" + srow).Merge();
                worksheet.Range("D" + row + ":D" + srow).Merge();
                worksheet.Range("E" + row + ":E" + srow).Merge();
                worksheet.Range("F" + row + ":F" + srow).Merge();
                worksheet.Range("A"+row+":F300").Style.Font.FontSize = 14;
                worksheet.Range("A"+row+":F300").Style.Font.Bold = true;
                worksheet.Range("A"+row+":F"+row).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 217, 102);
                worksheet.Range("A"+row+":F300").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("A"+row+":F300").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Column("A").Width = 8.5;
                worksheet.Column("B").Width = 25;
                worksheet.Column("C").Width = 25;
                worksheet.Column("D").Width = 25;
                worksheet.Column("E").Width = 25;
                worksheet.Column("F").Width = 25;
                worksheet.Column("G").Width = 25;
                worksheet.Column("H").Width = 25;
                worksheet.Column("I").Width = 25;
                worksheet.Cell("A" + row).Value = "Serial";
                worksheet.Cell("B" + row).Value = "Contact";
                worksheet.Cell("C" + row).Value = "Phone Number";
                worksheet.Cell("D" + row).Value = "Email";
                worksheet.Cell("E" + row).Value = "Account Affiliation";
                worksheet.Cell("F" + row).Value = "Representative";
                row = row +2;
                int s = 1;
                foreach (var item in res)
                {
                    if (item.TopAction == true)
                    {
                        if (item.TopConfirmed == true)
                        {
                            worksheet.Range("A" + row + ":F" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                        }
                        if (item.TopRejected == true)
                        {
                            worksheet.Range("A" + row + ":F" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                        }
                    }
                    else
                    {
                        if (item.Confirmed == true)
                        {
                            worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                        }

                        if (item.Rejected == true)
                        {
                            worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                        }

                        if (item.Confirmed == false && item.Rejected == false)
                        {

                        }
                    }

                    worksheet.Cell("A" + row).Value = s;
                    worksheet.Cell("B" + row).Value = item.ContactName;
                    worksheet.Cell("C" + row).Value = item.ContactPhone;
                    worksheet.Cell("D" + row).Value = item.ContactMail;
                    worksheet.Cell("E" + row).Value = item.AccountAffiliation;
                    worksheet.Cell("F" + row).Value = item.RepName;
                    row++;
                    s++;
                }




                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(
        content,
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
       EventName+" Detailed Data.xlsx");



            }

            else
            {

                
                
                
                var worksheet = workbook.Worksheets.Add("Events Requests Report");
                worksheet.Cell("E2").Style.Fill.BackgroundColor = XLColor.FromArgb(172, 185, 202);
                worksheet.Range("E2:I2").Merge();
                worksheet.Cell("E2").Value = "Exported By: Dr. " + ManagerName + " On " + now.ToString("dd MMMM yyyy - hh:mm tt");
                worksheet.Range("A6:A7").Merge();
                worksheet.Range("B6:B7").Merge();
                worksheet.Range("C6:C7").Merge();
                worksheet.Range("D6:D7").Merge();
                worksheet.Range("E6:E7").Merge();
                worksheet.Range("F5:H5").Merge();
                worksheet.Range("F6:F7").Merge();
                worksheet.Range("G6:G7").Merge();
                worksheet.Range("H6:H7").Merge();
                worksheet.Range("U6:U7").Merge();
                worksheet.Range("V6:V7").Merge();
                worksheet.Range("W6:W7").Merge();
                worksheet.Range("I6:N6").Merge();
                worksheet.Range("O6:T6").Merge();
                worksheet.Range("U6:U7").Merge();
                worksheet.Range("V6:V7").Merge();
                worksheet.Range("W6:W7").Merge();
                worksheet.Range("X6:X7").Merge();
                worksheet.Range("Y6:Y7").Merge();
                worksheet.Range("Z6:Z7").Merge();
                worksheet.Range("AA6:AA7").Merge();
                worksheet.Cell("F5").Value = res.FirstOrDefault().EventName;
                worksheet.Cell("A6").Value = "Serial";
                worksheet.Cell("B6").Value = "";
                worksheet.Cell("C6").Value = "Contact Name";
                worksheet.Cell("D6").Value = "Phone Number";
                worksheet.Cell("E6").Value = "Email";
                worksheet.Cell("F6").Value = "Account Affiliation";
                worksheet.Cell("G6").Value = "Passport Number";
                worksheet.Cell("H6").Value = "Passport Expiry Date";

                worksheet.Cell("I6").Value = "WAY IN";
                worksheet.Cell("I7").Value = "Date";
                worksheet.Cell("J7").Value = "Origin";
                worksheet.Cell("K7").Value = "Departure";
                worksheet.Cell("L7").Value = "Flight/Train";
                worksheet.Cell("M7").Value = "Destination";
                worksheet.Cell("N7").Value = "Arrival";

                worksheet.Cell("O6").Value = "WAY OUT";
                worksheet.Cell("O7").Value = "Date";
                worksheet.Cell("P7").Value = "Origin";
                worksheet.Cell("Q7").Value = "Departure";
                worksheet.Cell("R7").Value = "Flight/Train";
                worksheet.Cell("S7").Value = "Destination";
                worksheet.Cell("T7").Value = "Arrival";

                worksheet.Cell("U6").Value = "Hotel Name";
                worksheet.Cell("V6").Value = "Room Type";
                worksheet.Cell("W6").Value = "Accumpained";

                worksheet.Cell("X6").Value = "Representative";
                worksheet.Cell("Y6").Value = "Download";
                worksheet.Cell("Z6").Value = "Rep City";
                worksheet.Cell("AA6").Value = "H.S.A.N";

                worksheet.Range("A6:H6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 217, 102);
                worksheet.Cell("F5").Style.Fill.BackgroundColor = XLColor.FromArgb(226, 107, 10);
                worksheet.Range("I6:N7").Style.Fill.BackgroundColor = XLColor.FromArgb(191, 191, 191);
                worksheet.Range("O6:T7").Style.Fill.BackgroundColor = XLColor.FromArgb(132, 151, 176);
                worksheet.Range("U6:W7").Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);

                worksheet.Range("A2:AA300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A2:AA300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Rows("2:500").Height = 20;
                worksheet.Row(5).Height = 25;
                worksheet.Column("A").Width = 8.5;
                worksheet.Column("B").Width = 8.5;
                worksheet.Column("C").Width = 20;
                worksheet.Column("D").Width = 20;
                worksheet.Column("E").Width = 20;
                worksheet.Column("F").Width = 25;
                worksheet.Column("G").Width = 20;
                worksheet.Column("H").Width = 25;

                worksheet.Column("I").Width = 18;
                worksheet.Column("J").Width = 15;
                worksheet.Column("K").Width = 25;
                worksheet.Column("L").Width = 18;
                worksheet.Column("M").Width = 15;
                worksheet.Column("N").Width = 25;

                worksheet.Column("O").Width = 18;
                worksheet.Column("P").Width = 15;
                worksheet.Column("Q").Width = 25;
                worksheet.Column("R").Width = 18;
                worksheet.Column("S").Width = 15;
                worksheet.Column("T").Width = 25;

                worksheet.Column("U").Width = 20;
                worksheet.Column("V").Width = 20;
                worksheet.Column("W").Width = 20;
                worksheet.Column("X").Width = 30;
                worksheet.Column("Y").Width = 25;
                worksheet.Column("Z").Width = 25;
                worksheet.Column("AA").Width = 25;
                worksheet.Cell("E2").Style.Font.FontSize = 18;
                worksheet.Cell("F5").Style.Font.FontSize = 18;
                worksheet.Range("A6:AA7").Style.Font.FontSize = 14;
                worksheet.Range("A8:AA300").Style.Font.FontSize = 14;
                worksheet.Cell("E2").Style.Font.Bold = true;
                worksheet.Range("A6:AA7").Style.Font.Bold = true;
                worksheet.Cell("E2").Style.Font.Underline = XLFontUnderlineValues.Single;

                worksheet.Range("E2:I2").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                worksheet.Range("A6:AA300").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                worksheet.Range("A6:AA300").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                worksheet.Cell("F4").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                worksheet.Cell("G4").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                

                worksheet.Cell("F4").Value = "Accepted";
                worksheet.Cell("G4").Value = "Rejected";
                worksheet.Cell("H4").Value = "Pending";

                worksheet.Range("F4:H4").Style.Font.FontSize = 14;
                worksheet.Range("F4:H4").Style.Font.Bold = true;

                worksheet.AddPicture(Directory.GetCurrentDirectory() + "/wwwroot/logo/logotrans.png").MoveTo(1, 1).Scale(.5);

                string dir = "https://{{URL}}.com/Event/DownloadPassport/";
                int row = 8;
                int s = 1;
                foreach (var item in res)
                {
                    worksheet.Cell("A" + row).Value = s;
                    if (item.TopAction == true)
                    {
                        if (item.TopConfirmed == true)
                        {
                            worksheet.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                        }
                        if (item.TopRejected == true)
                        {
                            worksheet.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                        }
                    }
                    else
                    {
                        if (item.Confirmed == true)
                        {
                            worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                        }

                        if (item.Rejected == true)
                        {
                            worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                        }

                        if (item.Confirmed == false && item.Rejected == false)
                        {
                            
                        }
                    }

                    switch (item.Gender)
                    {
                        case false:
                            worksheet.Cell("B" + row).Value = "Mr";
                            break;
                        case true:
                            worksheet.Cell("B" + row).Value = "Mrs";
                            break;
                        default:
                            worksheet.Cell("B" + row).Value = "";
                            break;

                    }

                    worksheet.Cell("C" + row).Value = item.ContactName;
                    worksheet.Cell("D" + row).Value = item.ContactPhone;
                    worksheet.Cell("E" + row).Value = item.ContactMail;
                    worksheet.Cell("F" + row).Value = item.AccountAffiliation;
                    worksheet.Cell("G" + row).Value = item.PassportNumber;
                    worksheet.Cell("H" + row).Value = item.PassportExpiryDate;

                    if (item.WayInDeparture != null)
                    {
                        DateTime wid = (DateTime)item.WayInDeparture;
                        worksheet.Cell("I" + row).Value = wid.ToString("dd MMMM yyyy");
                        worksheet.Cell("K" + row).Value = wid.TimeOfDay;
                    }
                    if (item.WayInArrival != null)
                    {
                        DateTime wia = (DateTime)item.WayInArrival;
                        worksheet.Cell("N" + row).Value = wia.TimeOfDay;
                    }
                    if (item.WayOutDeparture != null)
                    {
                        DateTime wod = (DateTime)item.WayOutDeparture;
                        worksheet.Cell("O" + row).Value = wod.ToString("dd MMMM yyyy");
                        worksheet.Cell("Q" + row).Value = wod.TimeOfDay;
                    }
                    if (item.WayOutArrival != null)
                    {
                        DateTime woa = (DateTime)item.WayOutArrival;
                        worksheet.Cell("T" + row).Value = woa.TimeOfDay;
                    }




                    worksheet.Cell("I" + row).DataType = XLDataType.DateTime;
                    worksheet.Cell("J" + row).Value = item.WayInCityName;
                    worksheet.Cell("L" + row).Value = item.WayInFlightNumber;
                    worksheet.Cell("M" + row).Value = item.WayInDestinationName;



                    worksheet.Cell("O" + row).DataType = XLDataType.DateTime;
                    worksheet.Cell("P" + row).Value = item.WayOutCityName;

                    worksheet.Cell("R" + row).Value = item.WayOutFlightNumber;
                    worksheet.Cell("S" + row).Value = item.WayOutDestinationName;




                    worksheet.Cell("U" + row).Value = item.HotelName;
                    worksheet.Cell("V" + row).Value = item.RoomType;
                    worksheet.Cell("W" + row).Value = item.Accumpained;
                    worksheet.Cell("X" + row).Value = item.RepName;
                    if (item.IsPassport == true)
                    {
                        worksheet.Cell("Y" + row).Value = "Download";
                        Uri x = new Uri(dir + item.Id);
                        worksheet.Cell("Y" + row).Hyperlink.ExternalAddress = x;
                    }
                    worksheet.Cell("Z" + row).Value = item.RepCity;
                    if (item.Hsan == "" || item.Hsan == null)
                    {
                        item.Hsan = "-";
                    }
                    worksheet.Cell("AA" + row).Value = item.Hsan;
                    row++;
                    s++;
                }



                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(
         content,
         "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        EventName + " Detailed Data.xlsx");
            }








        }


        [Route("[controller]/[Action]/{ManagerId}")]
        [HttpGet]
        public IActionResult ExportMyTeamRequests(string ManagerId)
        {
            IEnumerable<CustomEventRequest> res = rep.GetMyTeamRequests(ManagerId);
            DateTime now = ti.GetCurrentTime();
            string ManagerName = userManager.FindByIdAsync(ManagerId).Result.FullName;
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Events Requests Report");
            worksheet.Cell("E2").Style.Fill.BackgroundColor = XLColor.FromArgb(172, 185, 202);
            worksheet.Range("E2:I2").Merge();
            worksheet.Cell("E2").Value = "Exported By: Dr. " + ManagerName+" On " + now.ToString("dd MMMM yyyy - hh:mm tt");
            worksheet.Range("A6:A7").Merge();
            worksheet.Range("B6:B7").Merge();
            worksheet.Range("C6:C7").Merge();
            worksheet.Range("D6:D7").Merge();
            worksheet.Range("E6:E7").Merge();
            worksheet.Range("F6:F7").Merge();
            worksheet.Range("G6:G7").Merge();
            worksheet.Range("H6:H7").Merge();
            worksheet.Range("U6:U7").Merge();
            worksheet.Range("V6:V7").Merge();
            worksheet.Range("W6:W7").Merge();
            worksheet.Range("I6:N6").Merge();
            worksheet.Range("O6:T6").Merge();
            worksheet.Range("U6:U7").Merge();
            worksheet.Range("V6:V7").Merge();
            worksheet.Range("W6:W7").Merge();
            worksheet.Range("X6:X7").Merge();
            worksheet.Range("Y6:Y7").Merge();


            worksheet.Cell("A6").Value = "Serial";
            worksheet.Cell("B6").Value = "Event Name";
            worksheet.Cell("C6").Value = "Contact Name";
            worksheet.Cell("D6").Value = "Phone Number";
            worksheet.Cell("E6").Value = "Email";
            worksheet.Cell("F6").Value = "Account Affiliation";
            worksheet.Cell("G6").Value = "Passport Number";
            worksheet.Cell("H6").Value = "Passport Expiry Date";
            
            worksheet.Cell("I6").Value = "WAY IN";
            worksheet.Cell("I7").Value = "Date";
            worksheet.Cell("J7").Value = "Origin";
            worksheet.Cell("K7").Value = "Departure";
            worksheet.Cell("L7").Value = "Flight/Train";
            worksheet.Cell("M7").Value = "Destination";
            worksheet.Cell("N7").Value = "Arrival";
            
            worksheet.Cell("O6").Value = "WAY OUT";
            worksheet.Cell("O7").Value = "Date";
            worksheet.Cell("P7").Value = "Origin";
            worksheet.Cell("Q7").Value = "Departure";
            worksheet.Cell("R7").Value = "Flight/Train";
            worksheet.Cell("S7").Value = "Destination";
            worksheet.Cell("T7").Value = "Arrival";
            
            worksheet.Cell("U6").Value = "Hotel Name";
            worksheet.Cell("V6").Value = "Room Type";
            worksheet.Cell("W6").Value = "Accumpained";

            worksheet.Cell("X6").Value = "Representative";
            worksheet.Cell("Y6").Value = "Download";

            worksheet.Range("A6:H6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 217, 102);
            worksheet.Range("I6:N7").Style.Fill.BackgroundColor = XLColor.FromArgb(191, 191, 191);
            worksheet.Range("O6:T7").Style.Fill.BackgroundColor = XLColor.FromArgb(132, 151, 176);
            worksheet.Range("U6:W7").Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);

            worksheet.Range("A2:Y300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A2:Y300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Rows("2:300").Height = 20;
            worksheet.Column("A").Width = 8.5;
            worksheet.Column("B").Width = 25;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 20;
            worksheet.Column("E").Width = 20;
            worksheet.Column("F").Width = 25;
            worksheet.Column("G").Width = 20;
            worksheet.Column("H").Width = 25;

            worksheet.Column("I").Width = 18;
            worksheet.Column("J").Width = 15;
            worksheet.Column("K").Width = 25;
            worksheet.Column("L").Width = 18;
            worksheet.Column("M").Width = 15;
            worksheet.Column("N").Width = 25;

            worksheet.Column("O").Width = 18;
            worksheet.Column("P").Width = 15;
            worksheet.Column("Q").Width = 25;
            worksheet.Column("R").Width = 18;
            worksheet.Column("S").Width = 15;
            worksheet.Column("T").Width = 25;

            worksheet.Column("U").Width = 20;
            worksheet.Column("V").Width = 20;
            worksheet.Column("W").Width = 20;
            worksheet.Column("X").Width = 30;
            worksheet.Column("Y").Width = 25;

            worksheet.Cell("E2").Style.Font.FontSize = 18;
            worksheet.Range("A6:Y7").Style.Font.FontSize = 14;
            worksheet.Range("A8:Y300").Style.Font.FontSize = 14;
            worksheet.Cell("E2").Style.Font.Bold = true;
            worksheet.Range("A6:Y7").Style.Font.Bold = true;
            worksheet.Cell("E2").Style.Font.Underline = XLFontUnderlineValues.Single;

            worksheet.Range("E2:I2").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            worksheet.Range("A6:Y300").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A6:Y300").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell("F4").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
            worksheet.Cell("G4").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
            

            worksheet.Cell("F4").Value = "Accepted";
            worksheet.Cell("G4").Value = "Rejected";
            worksheet.Cell("H4").Value = "Pending";

            worksheet.Range("F4:H4").Style.Font.FontSize = 14;
            worksheet.Range("F4:H4").Style.Font.Bold = true;

            string dir = "https://{{URL}}.com/Event/DownloadPassport/";
            int row = 8;
            int s = 1;
            foreach (var item in res)
            {
                worksheet.Cell("A" + row).Value = s;

                if (item.TopAction == true)
                {
                    if(item.TopConfirmed == true)
                    {
                        worksheet.Range("A" +row+":Y"+row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }
                    if (item.TopRejected == true)
                    {
                        worksheet.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }
                }
                else 
                {
                    if (item.Confirmed == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }

                    if (item.Rejected == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }

                    if (item.Confirmed == false && item.Rejected == false)
                    {
                        
                    }
                }



                worksheet.Cell("B" + row).Value = item.EventName;
                worksheet.Cell("C" + row).Value = item.ContactName;
                worksheet.Cell("D" + row).Value = item.ContactPhone;
                worksheet.Cell("E" + row).Value = item.ContactMail;
                worksheet.Cell("F" + row).Value = item.AccountAffiliation;
                worksheet.Cell("G" + row).Value = item.PassportNumber;
                worksheet.Cell("H" + row).Value = item.PassportExpiryDate;

                if (item.WayInDeparture != null)
                {
                    DateTime wid = (DateTime)item.WayInDeparture;
                    worksheet.Cell("I" + row).Value = wid.ToString("dd MMMM yyyy");
                    worksheet.Cell("K" + row).Value = wid.TimeOfDay;
                }
                if (item.WayInArrival != null)
                {
                    DateTime wia = (DateTime)item.WayInArrival;
                    worksheet.Cell("N" + row).Value = wia.TimeOfDay;
                }
                if (item.WayOutDeparture != null)
                {
                    DateTime wod = (DateTime)item.WayOutDeparture;
                    worksheet.Cell("O" + row).Value = wod.ToString("dd MMMM yyyy");
                    worksheet.Cell("Q" + row).Value = wod.TimeOfDay;
                }
                if (item.WayOutArrival != null)
                {
                    DateTime woa = (DateTime)item.WayOutArrival;
                    worksheet.Cell("T" + row).Value = woa.TimeOfDay;
                }

                
                worksheet.Cell("I" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("J" + row).Value = item.WayInCityName;
                
                worksheet.Cell("L" + row).Value = item.WayInFlightNumber;
                worksheet.Cell("M" + row).Value = item.WayInDestinationName;
                
   
               
                worksheet.Cell("O" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("P" + row).Value = item.WayOutCityName;
                
                worksheet.Cell("R" + row).Value = item.WayOutFlightNumber;
                worksheet.Cell("S" + row).Value = item.WayOutDestinationName;
                
             


                worksheet.Cell("U" + row).Value = item.HotelName;
                worksheet.Cell("V" + row).Value = item.RoomType;
                worksheet.Cell("W" + row).Value = item.Accumpained;
                worksheet.Cell("X" + row).Value = item.RepName;
                if (item.IsPassport == true)
                {
                    worksheet.Cell("Y" + row).Value = "Download";
                    Uri x = new Uri(dir + item.Id);
                    worksheet.Cell("Y" + row).Hyperlink.ExternalAddress = x;
                }
                row++;
                s++;
            }
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               ManagerName + "'s Team Events Travel Requests "+now.ToString("dd MMMM yyyy")+".xlsx");
        
        }

        [Route("[controller]/[Action]/{ManagerId}")]
        [HttpGet]
        public IActionResult ExportMyequests(string ManagerId)
        {
            IEnumerable<CustomEventRequest> res = rep.GetMyRequests(ManagerId);
            DateTime now = ti.GetCurrentTime();
            string ManagerName = userManager.FindByIdAsync(ManagerId).Result.FullName;
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Events Requests Report");
            worksheet.Cell("E2").Style.Fill.BackgroundColor = XLColor.FromArgb(172, 185, 202);
            worksheet.Range("E2:I2").Merge();
            worksheet.Cell("E2").Value = "Exported By: Dr. " + ManagerName + " On " + now.ToString("dd MMMM yyyy - hh:mm tt");
            worksheet.Range("A6:A7").Merge();
            worksheet.Range("B6:B7").Merge();
            worksheet.Range("C6:C7").Merge();
            worksheet.Range("D6:D7").Merge();
            worksheet.Range("E6:E7").Merge();
            worksheet.Range("F6:F7").Merge();
            worksheet.Range("G6:G7").Merge();
            worksheet.Range("H6:H7").Merge();
            worksheet.Range("U6:U7").Merge();
            worksheet.Range("V6:V7").Merge();
            worksheet.Range("W6:W7").Merge();
            worksheet.Range("I6:N6").Merge();
            worksheet.Range("O6:T6").Merge();
            worksheet.Range("U6:U7").Merge();
            worksheet.Range("V6:V7").Merge();
            worksheet.Range("W6:W7").Merge();
            worksheet.Range("X6:X7").Merge();
            worksheet.Range("Y6:Y7").Merge();


            worksheet.Cell("A6").Value = "Serial";
            worksheet.Cell("B6").Value = "Event Name";
            worksheet.Cell("C6").Value = "Contact Name";
            worksheet.Cell("D6").Value = "Phone Number";
            worksheet.Cell("E6").Value = "Email";
            worksheet.Cell("F6").Value = "Account Affiliation";
            worksheet.Cell("G6").Value = "Passport Number";
            worksheet.Cell("H6").Value = "Passport Expiry Date";

            worksheet.Cell("I6").Value = "WAY IN";
            worksheet.Cell("I7").Value = "Date";
            worksheet.Cell("J7").Value = "Origin";
            worksheet.Cell("K7").Value = "Departure";
            worksheet.Cell("L7").Value = "Flight/Train";
            worksheet.Cell("M7").Value = "Destination";
            worksheet.Cell("N7").Value = "Arrival";

            worksheet.Cell("O6").Value = "WAY OUT";
            worksheet.Cell("O7").Value = "Date";
            worksheet.Cell("P7").Value = "Origin";
            worksheet.Cell("Q7").Value = "Departure";
            worksheet.Cell("R7").Value = "Flight/Train";
            worksheet.Cell("S7").Value = "Destination";
            worksheet.Cell("T7").Value = "Arrival";

            worksheet.Cell("U6").Value = "Hotel Name";
            worksheet.Cell("V6").Value = "Room Type";
            worksheet.Cell("W6").Value = "Accumpained";

            worksheet.Cell("X6").Value = "Representative";
            worksheet.Cell("Y6").Value = "Download";

            worksheet.Range("A6:H6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 217, 102);
            worksheet.Range("I6:N7").Style.Fill.BackgroundColor = XLColor.FromArgb(191, 191, 191);
            worksheet.Range("O6:T7").Style.Fill.BackgroundColor = XLColor.FromArgb(132, 151, 176);
            worksheet.Range("U6:W7").Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);

            worksheet.Range("A2:Y300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A2:Y300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Rows("2:300").Height = 20;
            worksheet.Column("A").Width = 8.5;
            worksheet.Column("B").Width = 25;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 20;
            worksheet.Column("E").Width = 20;
            worksheet.Column("F").Width = 25;
            worksheet.Column("G").Width = 20;
            worksheet.Column("H").Width = 25;

            worksheet.Column("I").Width = 18;
            worksheet.Column("J").Width = 15;
            worksheet.Column("K").Width = 25;
            worksheet.Column("L").Width = 18;
            worksheet.Column("M").Width = 15;
            worksheet.Column("N").Width = 25;

            worksheet.Column("O").Width = 18;
            worksheet.Column("P").Width = 15;
            worksheet.Column("Q").Width = 25;
            worksheet.Column("R").Width = 18;
            worksheet.Column("S").Width = 15;
            worksheet.Column("T").Width = 25;

            worksheet.Column("U").Width = 20;
            worksheet.Column("V").Width = 20;
            worksheet.Column("W").Width = 20;
            worksheet.Column("X").Width = 30;
            worksheet.Column("Y").Width = 30;

            worksheet.Cell("E2").Style.Font.FontSize = 18;
            worksheet.Range("A6:Y7").Style.Font.FontSize = 14;
            worksheet.Range("A8:Y300").Style.Font.FontSize = 14;
            worksheet.Cell("E2").Style.Font.Bold = true;
            worksheet.Range("A6:Y7").Style.Font.Bold = true;
            worksheet.Cell("E2").Style.Font.Underline = XLFontUnderlineValues.Single;

            worksheet.Range("E2:I2").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            worksheet.Range("A6:Y300").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A6:Y300").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell("F4").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
            worksheet.Cell("G4").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
            

            worksheet.Cell("F4").Value = "Accepted";
            worksheet.Cell("G4").Value = "Rejected";
            worksheet.Cell("H4").Value = "Pending";

            worksheet.Range("F4:H4").Style.Font.FontSize = 14;
            worksheet.Range("F4:H4").Style.Font.Bold = true;

            string dir = "https://{{URL}}.com/Event/DownloadPassport/";
            int row = 8;
            int s = 1;
            foreach (var item in res)
            {
                worksheet.Cell("A" + row).Value = s;


                if (item.TopAction == true)
                {
                    if (item.TopConfirmed == true)
                    {
                        worksheet.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }
                    if (item.TopRejected == true)
                    {
                        worksheet.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }
                }
                else
                {
                    if (item.Confirmed == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }

                    if (item.Rejected == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }

                    if (item.Confirmed == false && item.Rejected == false)
                    {
                        
                    }
                }



                worksheet.Cell("B" + row).Value = item.EventName;
                worksheet.Cell("C" + row).Value = item.ContactName;
                worksheet.Cell("D" + row).Value = item.ContactPhone;
                worksheet.Cell("E" + row).Value = item.ContactMail;
                worksheet.Cell("F" + row).Value = item.AccountAffiliation;
                worksheet.Cell("G" + row).Value = item.PassportNumber;
                worksheet.Cell("H" + row).Value = item.PassportExpiryDate;
                if (item.WayInDeparture != null)
                {
                    DateTime wid = (DateTime)item.WayInDeparture;
                    worksheet.Cell("I" + row).Value = wid.ToString("dd MMMM yyyy");
                    worksheet.Cell("K" + row).Value = wid.TimeOfDay;
                }
                if (item.WayInArrival != null)
                {
                    DateTime wia = (DateTime)item.WayInArrival;
                    worksheet.Cell("N" + row).Value = wia.TimeOfDay;
                }
                if (item.WayOutDeparture != null)
                {
                    DateTime wod = (DateTime)item.WayOutDeparture;
                    worksheet.Cell("O" + row).Value = wod.ToString("dd MMMM yyyy");
                    worksheet.Cell("Q" + row).Value = wod.TimeOfDay;
                }
                if (item.WayOutArrival != null)
                {
                    DateTime woa = (DateTime)item.WayOutArrival;
                    worksheet.Cell("T" + row).Value = woa.TimeOfDay;
                }
               
                worksheet.Cell("I" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("J" + row).Value = item.WayInCityName;
                
                worksheet.Cell("L" + row).Value = item.WayInFlightNumber;
                worksheet.Cell("M" + row).Value = item.WayInDestinationName;
                

                
                worksheet.Cell("O" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("P" + row).Value = item.WayOutCityName;
                
                worksheet.Cell("R" + row).Value = item.WayOutFlightNumber;
                worksheet.Cell("S" + row).Value = item.WayOutDestinationName;
                



                worksheet.Cell("U" + row).Value = item.HotelName;
                worksheet.Cell("V" + row).Value = item.RoomType;
                worksheet.Cell("W" + row).Value = item.Accumpained;
                worksheet.Cell("X" + row).Value = item.RepName;
                if (item.IsPassport == true)
                {
                    worksheet.Cell("Y" + row).Value = "Download";
                    Uri x = new Uri(dir + item.Id);
                    worksheet.Cell("Y" + row).Hyperlink.ExternalAddress = x;
                }
                row++;
                s++;
            }
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               ManagerName + "'s Team Events Travel Requests " + now.ToString("dd MMMM yyyy") + ".xlsx");

        }

        [Route("[controller]/[Action]/{Id}")]
        [HttpGet]
        public IActionResult ExportMyProposalRequests(string Id)
        {
            IEnumerable<CustomProposalRequest> res = rep.GetMyProposalRequests(Id);
            DateTime now = ti.GetCurrentTime();
            string MyName = userManager.FindByIdAsync(Id).Result.FullName;
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Proposals Requests Report");
            worksheet.Cell("A2").Style.Fill.BackgroundColor = XLColor.FromArgb(172, 185, 202);
            worksheet.Range("A2:G2").Merge();
            worksheet.Cell("A2").Value = "Exported By: Dr. " + MyName + " On " + now.ToString("dd MMMM yyyy - hh:mm tt");
  



            worksheet.Cell("A6").Value = "Serial";
            worksheet.Cell("B6").Value = "";
            worksheet.Cell("C6").Value = "Contact Name";
            worksheet.Cell("D6").Value = "Phone Number";
            worksheet.Cell("E6").Value = "Email";
            worksheet.Cell("F6").Value = "Account Affiliation";
            worksheet.Cell("G6").Value = "Representative";


            worksheet.Range("A2:G300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A2:G300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Rows("2:300").Height = 20;
            worksheet.Column("A").Width = 8.5;
            worksheet.Column("B").Width = 8.5;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 20;
            worksheet.Column("E").Width = 20;
            worksheet.Column("F").Width = 25;
            worksheet.Column("G").Width = 50;

            worksheet.Row(2).Height = 30;
            worksheet.Row(6).Height = 30;

            worksheet.Cell("A2").Style.Font.FontSize = 18;
            worksheet.Range("A6:G6").Style.Font.FontSize = 14;
            worksheet.Range("A7:G300").Style.Font.FontSize = 14;
            worksheet.Cell("A2").Style.Font.Bold = true;
            worksheet.Range("A6:G6").Style.Font.Bold = true;
            worksheet.Cell("A2").Style.Font.Underline = XLFontUnderlineValues.Single;

            worksheet.Range("A2:G2").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            worksheet.Range("A6:G300").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A6:G300").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell("D4").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
            worksheet.Cell("E4").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
            
            worksheet.Range("A6:G6").Style.Fill.BackgroundColor = XLColor.FromArgb(226, 107, 10);

            worksheet.Cell("D4").Value = "Accepted";
            worksheet.Cell("E4").Value = "Rejected";
            worksheet.Cell("F4").Value = "Pending";

            worksheet.Range("D4:F4").Style.Font.FontSize = 14;
            worksheet.Range("D4:F4").Style.Font.Bold = true;


            int row = 7;
            int s = 1;
            foreach (var item in res)
            {

                worksheet.Cell("A" + row).Value = s;


                if (item.TopAction == true)
                {
                    if (item.TopConfirmed == true)
                    {
                        worksheet.Range("A" + row + ":G" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }
                    if (item.TopRejected == true)
                    {
                        worksheet.Range("A" + row + ":G" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }
                }
                else
                {
                    if (item.Confirmed == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }

                    if (item.Rejected == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }

                    if (item.Confirmed == false && item.Rejected == false)
                    {
                        
                    }
                }


                switch (item.Gender)
                {
                    case false:
                        worksheet.Cell("B" + row).Value = "Mr";
                        break;
                    case true:
                        worksheet.Cell("B" + row).Value = "Mrs";
                        break;
                    default:
                        worksheet.Cell("B" + row).Value = "";
                        break;

                }

                worksheet.Cell("C" + row).Value = item.ContactName;
                worksheet.Cell("D" + row).Value = item.ContactPhone;
                worksheet.Cell("E" + row).Value = item.ContactMail;
                worksheet.Cell("F" + row).Value = item.AccountAffiliation;
                worksheet.Cell("G" + row).Value = item.RepName;

                row++;
                s++;
            }
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               MyName+"'s Proposals" + now.ToString("dd MMMM yyyy") + ".xlsx");

        }

        [Route("[controller]/[Action]/{Id}")]
        [HttpGet]
        public IActionResult ExportAllProposalRequests(string Id)
        {
            IEnumerable<CustomProposalRequest> res = rep.GetAllProposalRequests();
            DateTime now = ti.GetCurrentTime();
            string MyName = userManager.FindByIdAsync(Id).Result.FullName;
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Proposals Requests Report");
            worksheet.Cell("A2").Style.Fill.BackgroundColor = XLColor.FromArgb(172, 185, 202);
            worksheet.Range("A2:G2").Merge();
            worksheet.Cell("A2").Value = "Exported By: Dr. " + MyName + " On " + now.ToString("dd MMMM yyyy - hh:mm tt");

            worksheet.Cell("A6").Value = "Serial";
            worksheet.Cell("B6").Value = "Event Name";
            worksheet.Cell("C6").Value = "Contact Name";
            worksheet.Cell("D6").Value = "Phone Number";
            worksheet.Cell("E6").Value = "Email";
            worksheet.Cell("F6").Value = "Account Affiliation";
            worksheet.Cell("G6").Value = "Representative";
            worksheet.Cell("H6").Value = "Rep City";


            worksheet.Range("A2:H300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A2:H300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Rows("2:500").Height = 20;
            worksheet.Column("A").Width = 8.5;
            worksheet.Column("B").Width = 25;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 20;
            worksheet.Column("E").Width = 20;
            worksheet.Column("F").Width = 25;
            worksheet.Column("G").Width = 50;
            worksheet.Column("H").Width = 25;
            worksheet.Row(2).Height = 30;
            worksheet.Row(6).Height = 30;

            worksheet.Cell("A2").Style.Font.FontSize = 18;
            worksheet.Range("A6:G6").Style.Font.FontSize = 14;
            worksheet.Range("A7:G300").Style.Font.FontSize = 14;
            worksheet.Cell("A2").Style.Font.Bold = true;
            worksheet.Range("A6:H6").Style.Font.Bold = true;
            worksheet.Cell("A2").Style.Font.Underline = XLFontUnderlineValues.Single;

            worksheet.Range("A2:H2").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            worksheet.Range("A6:H500").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A6:H500").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell("D4").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
            worksheet.Cell("E4").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
            
            worksheet.Range("A6:H6").Style.Fill.BackgroundColor = XLColor.FromArgb(226, 107, 10);

            worksheet.Cell("D4").Value = "Accepted";
            worksheet.Cell("E4").Value = "Rejected";
            worksheet.Cell("F4").Value = "Pending";

            worksheet.Range("D4:F4").Style.Font.FontSize = 14;
            worksheet.Range("D4:F4").Style.Font.Bold = true;


            int row = 7;
            int s = 1;
            foreach (var item in res)
            {

                worksheet.Cell("A" + row).Value = s;


                if (item.TopAction == true)
                {
                    if (item.TopConfirmed == true)
                    {
                        worksheet.Range("A" + row + ":H" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }
                    if (item.TopRejected == true)
                    {
                        worksheet.Range("A" + row + ":H" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }
                }
                else
                {
                    if (item.Confirmed == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }

                    if (item.Rejected == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }

                    if (item.Confirmed == false && item.Rejected == false)
                    {
                        
                    }
                }


                worksheet.Cell("B" + row).Value = item.EventName;
                worksheet.Cell("C" + row).Value = item.ContactName;
                worksheet.Cell("D" + row).Value = item.ContactPhone;
                worksheet.Cell("E" + row).Value = item.ContactMail;
                worksheet.Cell("F" + row).Value = item.AccountAffiliation;
                worksheet.Cell("G" + row).Value = item.RepName;
                worksheet.Cell("H" + row).Value = item.RepCity;
                row++;
                s++;
            }
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               "All Proposals on " + now.ToString("dd MMMM yyyy") + ".xlsx");

        }

        [Route("[controller]/[Action]/{UserId}/{EventId}")]
        [HttpGet]
        public IActionResult ExportEventProposalRequests(string UserId, int EventId)
        {
            IEnumerable<CustomProposalRequest> res = rep.GetEventProposalRequests(EventId);
            DateTime now = ti.GetCurrentTime();
            string MyName = userManager.FindByIdAsync(UserId).Result.FullName;
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Proposals Requests Report");
            worksheet.Cell("A2").Style.Fill.BackgroundColor = XLColor.FromArgb(172, 185, 202);
            worksheet.Range("A2:G2").Merge();
            worksheet.Cell("A2").Value = "Exported By: Dr. " + MyName + " On " + now.ToString("dd MMMM yyyy - hh:mm tt");

            worksheet.Cell("A6").Value = "Serial";
            worksheet.Cell("B6").Value = "Event Name";
            worksheet.Cell("C6").Value = "Contact Name";
            worksheet.Cell("D6").Value = "Phone Number";
            worksheet.Cell("E6").Value = "Email";
            worksheet.Cell("F6").Value = "Account Affiliation";
            worksheet.Cell("G6").Value = "Representative";
            worksheet.Cell("H6").Value = "Rep City";


            worksheet.Range("A2:H300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A2:H300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Rows("2:500").Height = 20;
            worksheet.Column("A").Width = 8.5;
            worksheet.Column("B").Width = 25;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 20;
            worksheet.Column("E").Width = 20;
            worksheet.Column("F").Width = 25;
            worksheet.Column("G").Width = 50;
            worksheet.Column("H").Width = 25;
            worksheet.Row(2).Height = 30;
            worksheet.Row(6).Height = 30;

            worksheet.Cell("A2").Style.Font.FontSize = 18;
            worksheet.Range("A6:G6").Style.Font.FontSize = 14;
            worksheet.Range("A7:G300").Style.Font.FontSize = 14;
            worksheet.Cell("A2").Style.Font.Bold = true;
            worksheet.Range("A6:H6").Style.Font.Bold = true;
            worksheet.Cell("A2").Style.Font.Underline = XLFontUnderlineValues.Single;

            worksheet.Range("A2:H2").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            worksheet.Range("A6:H500").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A6:H500").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell("D4").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
            worksheet.Cell("E4").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);

            worksheet.Range("A6:H6").Style.Fill.BackgroundColor = XLColor.FromArgb(226, 107, 10);

            worksheet.Cell("D4").Value = "Accepted";
            worksheet.Cell("E4").Value = "Rejected";
            worksheet.Cell("F4").Value = "Pending";

            worksheet.Range("D4:F4").Style.Font.FontSize = 14;
            worksheet.Range("D4:F4").Style.Font.Bold = true;


            int row = 7;
            int s = 1;
            foreach (var item in res)
            {

                worksheet.Cell("A" + row).Value = s;


                if (item.TopAction == true)
                {
                    if (item.TopConfirmed == true)
                    {
                        worksheet.Range("A" + row + ":H" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }
                    if (item.TopRejected == true)
                    {
                        worksheet.Range("A" + row + ":H" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }
                }
                else
                {
                    if (item.Confirmed == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }

                    if (item.Rejected == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }

                    if (item.Confirmed == false && item.Rejected == false)
                    {

                    }
                }


                worksheet.Cell("B" + row).Value = item.EventName;
                worksheet.Cell("C" + row).Value = item.ContactName;
                worksheet.Cell("D" + row).Value = item.ContactPhone;
                worksheet.Cell("E" + row).Value = item.ContactMail;
                worksheet.Cell("F" + row).Value = item.AccountAffiliation;
                worksheet.Cell("G" + row).Value = item.RepName;
                worksheet.Cell("H" + row).Value = item.RepCity;
                row++;
                s++;
            }
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               "Event Proposals on " + now.ToString("dd MMMM yyyy") + ".xlsx");

        }

        [Route("[controller]/[Action]/{Id}")]
        [HttpGet]
        public IActionResult ExportMyTeamProposalRequests(string Id)
        {
            IEnumerable<CustomProposalRequest> res = rep.GetMyTeamProposalRequests(Id);
            DateTime now = ti.GetCurrentTime();
            string MyName = userManager.FindByIdAsync(Id).Result.FullName;
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Proposals Requests Report");
            worksheet.Cell("A2").Style.Fill.BackgroundColor = XLColor.FromArgb(172, 185, 202);
            worksheet.Range("A2:G2").Merge();
            worksheet.Cell("A2").Value = "Exported By: Dr. " + MyName + " On " + now.ToString("dd MMMM yyyy - hh:mm tt");




            worksheet.Cell("A6").Value = "Serial";
            worksheet.Cell("B6").Value = "Event Name";
            worksheet.Cell("C6").Value = "Contact Name";
            worksheet.Cell("D6").Value = "Phone Number";
            worksheet.Cell("E6").Value = "Email";
            worksheet.Cell("F6").Value = "Account Affiliation";
            worksheet.Cell("G6").Value = "Representative";


            worksheet.Range("A2:G300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A2:G300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Rows("2:300").Height = 20;
            worksheet.Column("A").Width = 8.5;
            worksheet.Column("B").Width = 25;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 20;
            worksheet.Column("E").Width = 20;
            worksheet.Column("F").Width = 25;
            worksheet.Column("G").Width = 50;

            worksheet.Row(2).Height = 30;
            worksheet.Row(6).Height = 30;

            worksheet.Cell("A2").Style.Font.FontSize = 18;
            worksheet.Range("A6:G6").Style.Font.FontSize = 14;
            worksheet.Range("A7:G300").Style.Font.FontSize = 14;
            worksheet.Cell("A2").Style.Font.Bold = true;
            worksheet.Range("A6:G6").Style.Font.Bold = true;
            worksheet.Cell("A2").Style.Font.Underline = XLFontUnderlineValues.Single;

            worksheet.Range("A2:G2").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            worksheet.Range("A6:G300").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A6:G300").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell("D4").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
            worksheet.Cell("E4").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
            
            worksheet.Range("A6:G6").Style.Fill.BackgroundColor = XLColor.FromArgb(226, 107, 10);

            worksheet.Cell("D4").Value = "Accepted";
            worksheet.Cell("E4").Value = "Rejected";
            worksheet.Cell("F4").Value = "Pending";

            worksheet.Range("D4:F4").Style.Font.FontSize = 14;
            worksheet.Range("D4:F4").Style.Font.Bold = true;


            int row = 7;
            int s = 1;
            foreach (var item in res)
            {

                worksheet.Cell("A" + row).Value = s;


                if (item.TopAction == true)
                {
                    if (item.TopConfirmed == true)
                    {
                        worksheet.Range("A" + row + ":G" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }
                    if (item.TopRejected == true)
                    {
                        worksheet.Range("A" + row + ":G" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }
                }
                else
                {
                    if (item.Confirmed == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }

                    if (item.Rejected == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }

                    if (item.Confirmed == false && item.Rejected == false)
                    {
                        
                    }
                }



                worksheet.Cell("B" + row).Value = item.EventName;
                worksheet.Cell("C" + row).Value = item.ContactName;
                worksheet.Cell("D" + row).Value = item.ContactPhone;
                worksheet.Cell("E" + row).Value = item.ContactMail;
                worksheet.Cell("F" + row).Value = item.AccountAffiliation;
                worksheet.Cell("G" + row).Value = item.RepName;

                row++;
                s++;
            }
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               MyName + "'s Proposals" + now.ToString("dd MMMM yyyy") + ".xlsx");

        }

        [Route("[controller]/[Action]/{ManagerId}")]
        [HttpGet]
        public IActionResult ExportAll(string ManagerId)
        {
            IEnumerable<CustomEventRequest> ress = rep.GetMyRequests(ManagerId);
            IEnumerable<CustomEventRequest> res = rep.GetMyTeamRequests(ManagerId);
            DateTime now = ti.GetCurrentTime();
            string ManagerName = userManager.FindByIdAsync(ManagerId).Result.FullName;
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Events Requests Report");
            worksheet.Cell("E2").Style.Fill.BackgroundColor = XLColor.FromArgb(172, 185, 202);
            worksheet.Range("E2:I2").Merge();
            worksheet.Cell("E2").Value = "Exported By: Dr. " + ManagerName + " On " + now.ToString("dd MMMM yyyy - hh:mm tt");
            worksheet.Range("A6:A7").Merge();
            worksheet.Range("B6:B7").Merge();
            worksheet.Range("C6:C7").Merge();
            worksheet.Range("D6:D7").Merge();
            worksheet.Range("E6:E7").Merge();
            worksheet.Range("F6:F7").Merge();
            worksheet.Range("G6:G7").Merge();
            worksheet.Range("H6:H7").Merge();
            worksheet.Range("U6:U7").Merge();
            worksheet.Range("V6:V7").Merge();
            worksheet.Range("W6:W7").Merge();
            worksheet.Range("I6:N6").Merge();
            worksheet.Range("O6:T6").Merge();
            worksheet.Range("U6:U7").Merge();
            worksheet.Range("V6:V7").Merge();
            worksheet.Range("W6:W7").Merge();
            worksheet.Range("X6:X7").Merge();
            worksheet.Range("Y6:Y7").Merge();


            worksheet.Cell("A6").Value = "Serial";
            worksheet.Cell("B6").Value = "Event Name";
            worksheet.Cell("C6").Value = "Contact Name";
            worksheet.Cell("D6").Value = "Phone Number";
            worksheet.Cell("E6").Value = "Email";
            worksheet.Cell("F6").Value = "Account Affiliation";
            worksheet.Cell("G6").Value = "Passport Number";
            worksheet.Cell("H6").Value = "Passport Expiry Date";

            worksheet.Cell("I6").Value = "WAY IN";
            worksheet.Cell("I7").Value = "Date";
            worksheet.Cell("J7").Value = "Origin";
            worksheet.Cell("K7").Value = "Departure";
            worksheet.Cell("L7").Value = "Flight/Train";
            worksheet.Cell("M7").Value = "Destination";
            worksheet.Cell("N7").Value = "Arrival";

            worksheet.Cell("O6").Value = "WAY OUT";
            worksheet.Cell("O7").Value = "Date";
            worksheet.Cell("P7").Value = "Origin";
            worksheet.Cell("Q7").Value = "Departure";
            worksheet.Cell("R7").Value = "Flight/Train";
            worksheet.Cell("S7").Value = "Destination";
            worksheet.Cell("T7").Value = "Arrival";

            worksheet.Cell("U6").Value = "Hotel Name";
            worksheet.Cell("V6").Value = "Room Type";
            worksheet.Cell("W6").Value = "Accumpained";

            worksheet.Cell("X6").Value = "Representative";
            worksheet.Cell("Y6").Value = "Download";

            worksheet.Range("A6:H6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 217, 102);
            worksheet.Range("I6:N7").Style.Fill.BackgroundColor = XLColor.FromArgb(191, 191, 191);
            worksheet.Range("O6:T7").Style.Fill.BackgroundColor = XLColor.FromArgb(132, 151, 176);
            worksheet.Range("U6:W7").Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);

            worksheet.Range("A2:Y300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A2:Y300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Rows("2:300").Height = 20;
            worksheet.Column("A").Width = 8.5;
            worksheet.Column("B").Width = 25;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 20;
            worksheet.Column("E").Width = 20;
            worksheet.Column("F").Width = 25;
            worksheet.Column("G").Width = 20;
            worksheet.Column("H").Width = 25;

            worksheet.Column("I").Width = 18;
            worksheet.Column("J").Width = 15;
            worksheet.Column("K").Width = 25;
            worksheet.Column("L").Width = 18;
            worksheet.Column("M").Width = 15;
            worksheet.Column("N").Width = 25;

            worksheet.Column("O").Width = 18;
            worksheet.Column("P").Width = 15;
            worksheet.Column("Q").Width = 25;
            worksheet.Column("R").Width = 18;
            worksheet.Column("S").Width = 15;
            worksheet.Column("T").Width = 25;

            worksheet.Column("U").Width = 20;
            worksheet.Column("V").Width = 20;
            worksheet.Column("W").Width = 20;
            worksheet.Column("X").Width = 30;

            worksheet.Cell("E2").Style.Font.FontSize = 18;
            worksheet.Range("A6:Y7").Style.Font.FontSize = 14;
            worksheet.Range("A8:Y300").Style.Font.FontSize = 14;
            worksheet.Cell("E2").Style.Font.Bold = true;
            worksheet.Range("A6:Y7").Style.Font.Bold = true;
            worksheet.Cell("E2").Style.Font.Underline = XLFontUnderlineValues.Single;

            worksheet.Range("E2:I2").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            worksheet.Range("A6:Y300").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A6:Y300").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell("F4").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
            worksheet.Cell("G4").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
            

            worksheet.Cell("F4").Value = "Accepted";
            worksheet.Cell("G4").Value = "Rejected";
            worksheet.Cell("H4").Value = "Pending";

            worksheet.Range("F4:H4").Style.Font.FontSize = 14;
            worksheet.Range("F4:H4").Style.Font.Bold = true;

            string dir = "https://{{URL}}.com/Event/DownloadPassport/";
            int row = 8;
            int s = 1;
            foreach (var item in res)
            {
                worksheet.Cell("A" + row).Value = s;


                if (item.TopAction == true)
                {
                    if (item.TopConfirmed == true)
                    {
                        worksheet.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }
                    if (item.TopRejected == true)
                    {
                        worksheet.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }
                }
                else
                {
                    if (item.Confirmed == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }

                    if (item.Rejected == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }

                    if (item.Confirmed == false && item.Rejected == false)
                    {
                        
                    }
                }


                worksheet.Cell("B" + row).Value = item.EventName;
                worksheet.Cell("C" + row).Value = item.ContactName;
                worksheet.Cell("D" + row).Value = item.ContactPhone;
                worksheet.Cell("E" + row).Value = item.ContactMail;
                worksheet.Cell("F" + row).Value = item.AccountAffiliation;
                worksheet.Cell("G" + row).Value = item.PassportNumber;
                worksheet.Cell("H" + row).Value = item.PassportExpiryDate;
                if (item.WayInDeparture != null)
                {
                    DateTime wid = (DateTime)item.WayInDeparture;
                    worksheet.Cell("I" + row).Value = wid.ToString("dd MMMM yyyy");
                    worksheet.Cell("K" + row).Value = wid.TimeOfDay;
                }
                if (item.WayInArrival != null)
                {
                    DateTime wia = (DateTime)item.WayInArrival;
                    worksheet.Cell("N" + row).Value = wia.TimeOfDay;
                }
                if (item.WayOutDeparture != null)
                {
                    DateTime wod = (DateTime)item.WayOutDeparture;
                    worksheet.Cell("O" + row).Value = wod.ToString("dd MMMM yyyy");
                    worksheet.Cell("Q" + row).Value = wod.TimeOfDay;
                }
                if (item.WayOutArrival != null)
                {
                    DateTime woa = (DateTime)item.WayOutArrival;
                    worksheet.Cell("T" + row).Value = woa.TimeOfDay;
                }
               
                worksheet.Cell("I" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("J" + row).Value = item.WayInCityName;
                
                worksheet.Cell("L" + row).Value = item.WayInFlightNumber;
                worksheet.Cell("M" + row).Value = item.WayInDestinationName;
                

               
                worksheet.Cell("O" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("P" + row).Value = item.WayOutCityName;
                
                worksheet.Cell("R" + row).Value = item.WayOutFlightNumber;
                worksheet.Cell("S" + row).Value = item.WayOutDestinationName;
                



                worksheet.Cell("U" + row).Value = item.HotelName;
                worksheet.Cell("V" + row).Value = item.RoomType;
                worksheet.Cell("W" + row).Value = item.Accumpained;
                worksheet.Cell("X" + row).Value = item.RepName;
                if (item.IsPassport == true)
                {
                    worksheet.Cell("Y" + row).Value = "Download";
                    Uri x = new Uri(dir + item.Id);
                    worksheet.Cell("Y" + row).Hyperlink.ExternalAddress = x;
                }
                row++;
                s++;
            }

            foreach (var item in ress)
            {
                worksheet.Cell("A" + row).Value = s;
                if (item.TopAction == true)
                {
                    if (item.TopConfirmed == true)
                    {
                        worksheet.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }
                    if (item.TopRejected == true)
                    {
                        worksheet.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }
                }
                else
                {
                    if (item.Confirmed == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }

                    if (item.Rejected == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }

                    if (item.Confirmed == false && item.Rejected == false)
                    {
                        
                    }
                }


                worksheet.Cell("B" + row).Value = item.EventName;
                worksheet.Cell("C" + row).Value = item.ContactName;
                worksheet.Cell("D" + row).Value = item.ContactPhone;
                worksheet.Cell("E" + row).Value = item.ContactMail;
                worksheet.Cell("F" + row).Value = item.AccountAffiliation;
                worksheet.Cell("G" + row).Value = item.PassportNumber;
                worksheet.Cell("H" + row).Value = item.PassportExpiryDate;
                if (item.WayInDeparture != null)
                {
                    DateTime wid = (DateTime)item.WayInDeparture;
                    worksheet.Cell("I" + row).Value = wid.ToString("dd MMMM yyyy");
                    worksheet.Cell("K" + row).Value = wid.TimeOfDay;
                }
                if (item.WayInArrival != null)
                {
                    DateTime wia = (DateTime)item.WayInArrival;
                    worksheet.Cell("N" + row).Value = wia.TimeOfDay;
                }
                if (item.WayOutDeparture != null)
                {
                    DateTime wod = (DateTime)item.WayOutDeparture;
                    worksheet.Cell("O" + row).Value = wod.ToString("dd MMMM yyyy");
                    worksheet.Cell("Q" + row).Value = wod.TimeOfDay;
                }
                if (item.WayOutArrival != null)
                {
                    DateTime woa = (DateTime)item.WayOutArrival;
                    worksheet.Cell("T" + row).Value = woa.TimeOfDay;
                }
                
                worksheet.Cell("I" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("J" + row).Value = item.WayInCityName;
                
                worksheet.Cell("L" + row).Value = item.WayInFlightNumber;
                worksheet.Cell("M" + row).Value = item.WayInDestinationName;
                

                
                worksheet.Cell("O" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("P" + row).Value = item.WayOutCityName;
                
                worksheet.Cell("R" + row).Value = item.WayOutFlightNumber;
                worksheet.Cell("S" + row).Value = item.WayOutDestinationName;
                



                worksheet.Cell("U" + row).Value = item.HotelName;
                worksheet.Cell("V" + row).Value = item.RoomType;
                worksheet.Cell("W" + row).Value = item.Accumpained;
                worksheet.Cell("X" + row).Value = item.RepName;
                if (item.IsPassport == true)
                {
                    worksheet.Cell("Y" + row).Value = "Download";
                    Uri x = new Uri(dir + item.Id);
                    worksheet.Cell("Y" + row).Hyperlink.ExternalAddress = x;
                }
                row++;
                s++;
            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               ManagerName + "'s Team Events Travel Requests " + now.ToString("dd MMMM yyyy") + ".xlsx");

        }


        [Route("[controller]/[Action]/{ManagerId}")]
        [HttpGet]
        public IActionResult ExportAllTopManager(string ManagerId)
        {
            IEnumerable<CustomEventRequest> res = rep.GetAllRequests();

            DateTime now = ti.GetCurrentTime();
            XLWorkbook workbook = new XLWorkbook();
            string ManagerName = userManager.FindByIdAsync(ManagerId).Result.FullName;
            var worksheet = workbook.Worksheets.Add("Events Requests Report");
            worksheet.Cell("E2").Style.Fill.BackgroundColor = XLColor.FromArgb(172, 185, 202);
            worksheet.Range("E2:I2").Merge();
            worksheet.Cell("E2").Value = "Exported By: Dr. " + ManagerName + " On " + now.ToString("dd MMMM yyyy - hh:mm tt");
            worksheet.Range("A6:A7").Merge();
            worksheet.Range("B6:B7").Merge();
            worksheet.Range("C6:C7").Merge();
            worksheet.Range("D6:D7").Merge();
            worksheet.Range("E6:E7").Merge();
            worksheet.Range("F6:F7").Merge();
            worksheet.Range("G6:G7").Merge();
            worksheet.Range("H6:H7").Merge();
            worksheet.Range("U6:U7").Merge();
            worksheet.Range("V6:V7").Merge();
            worksheet.Range("W6:W7").Merge();
            worksheet.Range("I6:N6").Merge();
            worksheet.Range("O6:T6").Merge();
            worksheet.Range("U6:U7").Merge();
            worksheet.Range("V6:V7").Merge();
            worksheet.Range("W6:W7").Merge();
            worksheet.Range("X6:X7").Merge();
            worksheet.Range("Y6:Y7").Merge();


            worksheet.Cell("A6").Value = "Serial";
            worksheet.Cell("B6").Value = "Event Name";
            worksheet.Cell("C6").Value = "Contact Name";
            worksheet.Cell("D6").Value = "Phone Number";
            worksheet.Cell("E6").Value = "Email";
            worksheet.Cell("F6").Value = "Account Affiliation";
            worksheet.Cell("G6").Value = "Passport Number";
            worksheet.Cell("H6").Value = "Passport Expiry Date";

            worksheet.Cell("I6").Value = "WAY IN";
            worksheet.Cell("I7").Value = "Date";
            worksheet.Cell("J7").Value = "Origin";
            worksheet.Cell("K7").Value = "Departure";
            worksheet.Cell("L7").Value = "Flight/Train";
            worksheet.Cell("M7").Value = "Destination";
            worksheet.Cell("N7").Value = "Arrival";

            worksheet.Cell("O6").Value = "WAY OUT";
            worksheet.Cell("O7").Value = "Date";
            worksheet.Cell("P7").Value = "Origin";
            worksheet.Cell("Q7").Value = "Departure";
            worksheet.Cell("R7").Value = "Flight/Train";
            worksheet.Cell("S7").Value = "Destination";
            worksheet.Cell("T7").Value = "Arrival";

            worksheet.Cell("U6").Value = "Hotel Name";
            worksheet.Cell("V6").Value = "Room Type";
            worksheet.Cell("W6").Value = "Accumpained";

            worksheet.Cell("X6").Value = "Representative";
            worksheet.Cell("Y6").Value = "Download";

            worksheet.Range("A6:H6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 217, 102);
            worksheet.Range("I6:N7").Style.Fill.BackgroundColor = XLColor.FromArgb(191, 191, 191);
            worksheet.Range("O6:T7").Style.Fill.BackgroundColor = XLColor.FromArgb(132, 151, 176);
            worksheet.Range("U6:W7").Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);

            worksheet.Range("A2:X300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A2:X300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("Y1:Y300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("Y1:Y300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Rows("2:300").Height = 20;
            worksheet.Column("A").Width = 8.5;
            worksheet.Column("B").Width = 20;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 20;
            worksheet.Column("E").Width = 20;
            worksheet.Column("F").Width = 25;
            worksheet.Column("G").Width = 20;
            worksheet.Column("H").Width = 25;

            worksheet.Column("I").Width = 18;
            worksheet.Column("J").Width = 15;
            worksheet.Column("K").Width = 25;
            worksheet.Column("L").Width = 18;
            worksheet.Column("M").Width = 15;
            worksheet.Column("N").Width = 25;

            worksheet.Column("O").Width = 18;
            worksheet.Column("P").Width = 15;
            worksheet.Column("Q").Width = 25;
            worksheet.Column("R").Width = 18;
            worksheet.Column("S").Width = 15;
            worksheet.Column("T").Width = 25;

            worksheet.Column("U").Width = 20;
            worksheet.Column("V").Width = 20;
            worksheet.Column("W").Width = 20;
            worksheet.Column("X").Width = 30;
            worksheet.Column("Y").Width = 25;

            worksheet.Cell("E2").Style.Font.FontSize = 18;
            worksheet.Range("A6:Y7").Style.Font.FontSize = 14;
            worksheet.Range("A8:Y300").Style.Font.FontSize = 14;
            worksheet.Cell("E2").Style.Font.Bold = true;
            worksheet.Range("A6:Y7").Style.Font.Bold = true;
            worksheet.Cell("E2").Style.Font.Underline = XLFontUnderlineValues.Single;

            worksheet.Range("E2:I2").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            worksheet.Range("A6:Y300").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A6:Y300").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell("F4").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
            worksheet.Cell("G4").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
            

            worksheet.Cell("F4").Value = "Accepted";
            worksheet.Cell("G4").Value = "Rejected";
            worksheet.Cell("H4").Value = "Pending";

            worksheet.Range("F4:H4").Style.Font.FontSize = 14;
            worksheet.Range("F4:H4").Style.Font.Bold = true;

            string dir = "https://{{URL}}.com/Event/DownloadPassport/";
            int row = 8;
            int s = 1;

            foreach (var item in res)
            {
                worksheet.Cell("A" + row).Value = s;


                if (item.TopAction == true)
                {
                    if (item.TopConfirmed == true)
                    {
                        worksheet.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }
                    if (item.TopRejected == true)
                    {
                        worksheet.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }
                }
                else
                {
                    if (item.Confirmed == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }

                    if (item.Rejected == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }

                    if (item.Confirmed == false && item.Rejected == false)
                    {
                        
                    }
                }


              
                worksheet.Cell("B" + row).Value = item.EventName;
                worksheet.Cell("C" + row).Value = item.ContactName;
                worksheet.Cell("D" + row).Value = item.ContactPhone;
                worksheet.Cell("E" + row).Value = item.ContactMail;
                worksheet.Cell("F" + row).Value = item.AccountAffiliation;
                worksheet.Cell("G" + row).Value = item.PassportNumber;
                worksheet.Cell("H" + row).Value = item.PassportExpiryDate;
                if (item.WayInDeparture != null)
                {
                    DateTime wid = (DateTime)item.WayInDeparture;
                    worksheet.Cell("I" + row).Value = wid.ToString("dd MMMM yyyy");
                    worksheet.Cell("K" + row).Value = wid.TimeOfDay;
                }
                if (item.WayInArrival != null)
                {
                    DateTime wia = (DateTime)item.WayInArrival;
                    worksheet.Cell("N" + row).Value = wia.TimeOfDay;
                }
                if (item.WayOutDeparture != null)
                {
                    DateTime wod = (DateTime)item.WayOutDeparture;
                    worksheet.Cell("O" + row).Value = wod.ToString("dd MMMM yyyy");
                    worksheet.Cell("Q" + row).Value = wod.TimeOfDay;
                }
                if (item.WayOutArrival != null)
                {
                    DateTime woa = (DateTime)item.WayOutArrival;
                    worksheet.Cell("T" + row).Value = woa.TimeOfDay;
                }
                
                worksheet.Cell("I" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("J" + row).Value = item.WayInCityName;
                
                worksheet.Cell("L" + row).Value = item.WayInFlightNumber;
                worksheet.Cell("M" + row).Value = item.WayInDestinationName;
                

                
                worksheet.Cell("O" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("P" + row).Value = item.WayOutCityName;
                
                worksheet.Cell("R" + row).Value = item.WayOutFlightNumber;
                worksheet.Cell("S" + row).Value = item.WayOutDestinationName;
                



                worksheet.Cell("U" + row).Value = item.HotelName;
                worksheet.Cell("V" + row).Value = item.RoomType;
                worksheet.Cell("W" + row).Value = item.Accumpained;
                worksheet.Cell("X" + row).Value = item.RepName;
                if (item.IsPassport == true)
                {
                    worksheet.Cell("Y" + row).Value = "Download";
                    Uri x = new Uri(dir + item.Id);
                    worksheet.Cell("Y" + row).Hyperlink.ExternalAddress = x;
                }
                row++;
                s++;
            }

          
            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               "All Teams Events Travel Requests " + now.ToString("dd MMMM yyyy") + ".xlsx");

        }


    


        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeleteEventAndRequests(int id)
        {
            bool res = rep.DeleteEventAndRequests(id);
            return Ok(res);
        }


        [Route("[controller]/[Action]/{userid}")]
        [HttpGet]
        public IActionResult GetAllEvents(string userid)
        {
            IEnumerable<CustomEvent> res = rep.GetAllEvents(userid);
            return Ok(res);
        }


        [Route("[controller]/[Action]/{userid}")]
        [HttpGet]
        public IActionResult GetAllEventsExcel(string userid)
        {
            IEnumerable<CustomEvent> res = rep.GetAllEvents(userid);
            DateTime now = ti.GetCurrentTime();
            XLWorkbook workbook = new XLWorkbook();
            var worksheetu = workbook.Worksheets.Add("Upcoming Events");

            worksheetu.Range("A1:K1").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 217, 102);
            worksheetu.Range("A1:k1000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheetu.Range("A1:k1000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheetu.Rows("1:1000").Height = 25;
            worksheetu.Columns("A:K").Width = 25;
            worksheetu.Range("A1:K1").Style.Font.FontSize = 16;
            worksheetu.Range("A1:K1").Style.Font.Bold = true;
            worksheetu.Range("A2:K1000").Style.Font.FontSize = 13;

            worksheetu.Range("F1:F1000").Style.Font.FontColor = XLColor.FromHtml("#28a745");
            worksheetu.Range("G1:G1000").Style.Font.FontColor = XLColor.FromHtml("#dc3545");
            worksheetu.Range("H1:H1000").Style.Font.FontColor = XLColor.FromHtml("#007bff");

            worksheetu.Cell("A1").Value = "Event";
            worksheetu.Cell("B1").Value = "Event Type";
            worksheetu.Cell("C1").Value = "City";
            worksheetu.Cell("D1").Value = "From";
            worksheetu.Cell("E1").Value = "To";
            worksheetu.Cell("F1").Value = "Accepted Requests";
            worksheetu.Cell("G1").Value = "Rejected Requests";
            worksheetu.Cell("H1").Value = "Pending Requests";
            worksheetu.Cell("I1").Value = "Total Requests";
            worksheetu.Cell("J1").Value = "Download Passports";
            worksheetu.Cell("K1").Value = "Download Tickets";


            var row = 2;

            foreach (var item in res.Where(a=>a.IsUpcoming == true))
            {
                worksheetu.Cell("A" + row).Value = item.EventName;
                worksheetu.Cell("B" + row).Value = item.EventTypeName;
                worksheetu.Cell("C" + row).Value = item.CityName;
                worksheetu.Cell("D" + row).Value = item.From;
                worksheetu.Cell("E" + row).Value = item.To;
                worksheetu.Cell("F" + row).Value = item.AcceptedRequests;
                worksheetu.Cell("G" + row).Value = item.RejectedRequests;
                worksheetu.Cell("H" + row).Value = item.PendingRequests;
                worksheetu.Cell("I" + row).Value = item.TotalRequests;
                worksheetu.Cell("J" + row).Value = "Download Passports";
                worksheetu.Cell("K" + row).Value = "Download Tickets";


                Uri p = new Uri("https://{{URL}}.com/Event/DownloadPassportsByEventId/" + item.Id);
                Uri t = new Uri("https://{{URL}}.com/Event/DownloadTicketsByEventId/" + item.Id);
                worksheetu.Cell("J" + row).Hyperlink.ExternalAddress = p;
                worksheetu.Cell("K" + row).Hyperlink.ExternalAddress = t;
                row++;
            }

            var worksheetp = workbook.Worksheets.Add("Past Events");

            worksheetp.Range("A1:K1").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 217, 102);
            worksheetp.Range("A1:k1000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheetp.Range("A1:k1000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheetp.Rows("1:1000").Height = 25;
            worksheetp.Columns("A:K").Width = 25;
            worksheetp.Range("A1:K1").Style.Font.FontSize = 16;
            worksheetp.Range("A1:K1").Style.Font.Bold = true;
            worksheetp.Range("A2:K1000").Style.Font.FontSize = 13;

            worksheetp.Range("F1:F1000").Style.Font.FontColor = XLColor.FromHtml("#28a745");
            worksheetp.Range("G1:G1000").Style.Font.FontColor = XLColor.FromHtml("#dc3545");
            worksheetp.Range("H1:H1000").Style.Font.FontColor = XLColor.FromHtml("#007bff");

            worksheetp.Cell("A1").Value = "Event";
            worksheetp.Cell("B1").Value = "Event Type";
            worksheetp.Cell("C1").Value = "City";
            worksheetp.Cell("D1").Value = "From";
            worksheetp.Cell("E1").Value = "To";
            worksheetp.Cell("F1").Value = "Accepted Requests";
            worksheetp.Cell("G1").Value = "Rejected Requests";
            worksheetp.Cell("H1").Value = "Pending Requests";
            worksheetp.Cell("I1").Value = "Total Requests";
            worksheetp.Cell("J1").Value = "Download Passports";
            worksheetp.Cell("K1").Value = "Download Tickets";

             row = 2;

            foreach (var item in res.Where(a => a.IsUpcoming == false))
            {
                worksheetp.Cell("A" + row).Value = item.EventName;
                worksheetp.Cell("B" + row).Value = item.EventTypeName;
                worksheetp.Cell("C" + row).Value = item.CityName;
                worksheetp.Cell("D" + row).Value = item.From;
                worksheetp.Cell("E" + row).Value = item.To;
                worksheetp.Cell("F" + row).Value = item.AcceptedRequests;
                worksheetp.Cell("G" + row).Value = item.RejectedRequests;
                worksheetp.Cell("H" + row).Value = item.PendingRequests;
                worksheetp.Cell("I" + row).Value = item.TotalRequests;
                worksheetp.Cell("J" + row).Value = "Download Passports";
                worksheetp.Cell("K" + row).Value = "Download Tickets";


                Uri p = new Uri("https://{{URL}}.com/Event/DownloadPassportsByEventId/" + item.Id);
                Uri t = new Uri("https://{{URL}}.com/Event/DownloadTicketsByEventId/" + item.Id);
                worksheetp.Cell("J" + row).Hyperlink.ExternalAddress = p;
                worksheetp.Cell("K" + row).Hyperlink.ExternalAddress = t;
                row++;
            }


            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               "All Events "+now.Date+".xlsx");











        }


        [Route("[controller]/[Action]/{ManagerId}")]
        [HttpGet]
        public IActionResult ExportAllPastTopManager(string ManagerId)
        {
            IEnumerable<CustomEventRequest> res = rep.GetAllPastRequests();

            DateTime now = ti.GetCurrentTime();
            XLWorkbook workbook = new XLWorkbook();
            string ManagerName = userManager.FindByIdAsync(ManagerId).Result.FullName;
            var worksheet = workbook.Worksheets.Add("Events Requests Report");
            worksheet.Cell("E2").Style.Fill.BackgroundColor = XLColor.FromArgb(172, 185, 202);
            worksheet.Range("E2:I2").Merge();
            worksheet.Cell("E2").Value = "Exported By: Dr. " + ManagerName + " On " + now.ToString("dd MMMM yyyy - hh:mm tt");
            worksheet.Range("A6:A7").Merge();
            worksheet.Range("B6:B7").Merge();
            worksheet.Range("C6:C7").Merge();
            worksheet.Range("D6:D7").Merge();
            worksheet.Range("E6:E7").Merge();
            worksheet.Range("F6:F7").Merge();
            worksheet.Range("G6:G7").Merge();
            worksheet.Range("H6:H7").Merge();
            worksheet.Range("U6:U7").Merge();
            worksheet.Range("V6:V7").Merge();
            worksheet.Range("W6:W7").Merge();
            worksheet.Range("I6:N6").Merge();
            worksheet.Range("O6:T6").Merge();
            worksheet.Range("U6:U7").Merge();
            worksheet.Range("V6:V7").Merge();
            worksheet.Range("W6:W7").Merge();
            worksheet.Range("X6:X7").Merge();
            worksheet.Range("Y6:Y7").Merge();


            worksheet.Cell("A6").Value = "Serial";
            worksheet.Cell("B6").Value = "Event Name";
            worksheet.Cell("C6").Value = "Contact Name";
            worksheet.Cell("D6").Value = "Phone Number";
            worksheet.Cell("E6").Value = "Email";
            worksheet.Cell("F6").Value = "Account Affiliation";
            worksheet.Cell("G6").Value = "Passport Number";
            worksheet.Cell("H6").Value = "Passport Expiry Date";

            worksheet.Cell("I6").Value = "WAY IN";
            worksheet.Cell("I7").Value = "Date";
            worksheet.Cell("J7").Value = "Origin";
            worksheet.Cell("K7").Value = "Departure";
            worksheet.Cell("L7").Value = "Flight/Train";
            worksheet.Cell("M7").Value = "Destination";
            worksheet.Cell("N7").Value = "Arrival";

            worksheet.Cell("O6").Value = "WAY OUT";
            worksheet.Cell("O7").Value = "Date";
            worksheet.Cell("P7").Value = "Origin";
            worksheet.Cell("Q7").Value = "Departure";
            worksheet.Cell("R7").Value = "Flight/Train";
            worksheet.Cell("S7").Value = "Destination";
            worksheet.Cell("T7").Value = "Arrival";

            worksheet.Cell("U6").Value = "Hotel Name";
            worksheet.Cell("V6").Value = "Room Type";
            worksheet.Cell("W6").Value = "Accumpained";

            worksheet.Cell("X6").Value = "Representative";
            worksheet.Cell("Y6").Value = "Download";

            worksheet.Range("A6:H6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 217, 102);
            worksheet.Range("I6:N7").Style.Fill.BackgroundColor = XLColor.FromArgb(191, 191, 191);
            worksheet.Range("O6:T7").Style.Fill.BackgroundColor = XLColor.FromArgb(132, 151, 176);
            worksheet.Range("U6:W7").Style.Fill.BackgroundColor = XLColor.FromArgb(169, 208, 142);

            worksheet.Range("A2:X3000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A2:X3000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("Y1:Y3000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("Y1:Y3000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Rows("2:3000").Height = 20;
            worksheet.Column("A").Width = 8.5;
            worksheet.Column("B").Width = 20;
            worksheet.Column("C").Width = 20;
            worksheet.Column("D").Width = 20;
            worksheet.Column("E").Width = 20;
            worksheet.Column("F").Width = 25;
            worksheet.Column("G").Width = 20;
            worksheet.Column("H").Width = 25;

            worksheet.Column("I").Width = 18;
            worksheet.Column("J").Width = 15;
            worksheet.Column("K").Width = 25;
            worksheet.Column("L").Width = 18;
            worksheet.Column("M").Width = 15;
            worksheet.Column("N").Width = 25;

            worksheet.Column("O").Width = 18;
            worksheet.Column("P").Width = 15;
            worksheet.Column("Q").Width = 25;
            worksheet.Column("R").Width = 18;
            worksheet.Column("S").Width = 15;
            worksheet.Column("T").Width = 25;

            worksheet.Column("U").Width = 20;
            worksheet.Column("V").Width = 20;
            worksheet.Column("W").Width = 20;
            worksheet.Column("X").Width = 30;
            worksheet.Column("Y").Width = 25;

            worksheet.Cell("E2").Style.Font.FontSize = 18;
            worksheet.Range("A6:Y7").Style.Font.FontSize = 14;
            worksheet.Range("A8:Y3000").Style.Font.FontSize = 14;
            worksheet.Cell("E2").Style.Font.Bold = true;
            worksheet.Range("A6:Y7").Style.Font.Bold = true;
            worksheet.Cell("E2").Style.Font.Underline = XLFontUnderlineValues.Single;

            worksheet.Range("E2:I2").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            worksheet.Range("A6:Y3000").Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A6:Y3000").Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell("F4").Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
            worksheet.Cell("G4").Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);


            worksheet.Cell("F4").Value = "Accepted";
            worksheet.Cell("G4").Value = "Rejected";
            worksheet.Cell("H4").Value = "Pending";

            worksheet.Range("F4:H4").Style.Font.FontSize = 14;
            worksheet.Range("F4:H4").Style.Font.Bold = true;

            string dir = "https://{{URL}}.com/Event/DownloadPassport/";
            int row = 8;
            int s = 1;

            foreach (var item in res)
            {
                worksheet.Cell("A" + row).Value = s;


                if (item.TopAction == true)
                {
                    if (item.TopConfirmed == true)
                    {
                        worksheet.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }
                    if (item.TopRejected == true)
                    {
                        worksheet.Range("A" + row + ":Y" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }
                }
                else
                {
                    if (item.Confirmed == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                    }

                    if (item.Rejected == true)
                    {
                        worksheet.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(192, 0, 0);
                    }

                    if (item.Confirmed == false && item.Rejected == false)
                    {

                    }
                }



                worksheet.Cell("B" + row).Value = item.EventName;
                worksheet.Cell("C" + row).Value = item.ContactName;
                worksheet.Cell("D" + row).Value = item.ContactPhone;
                worksheet.Cell("E" + row).Value = item.ContactMail;
                worksheet.Cell("F" + row).Value = item.AccountAffiliation;
                worksheet.Cell("G" + row).Value = item.PassportNumber;
                worksheet.Cell("H" + row).Value = item.PassportExpiryDate;
                if (item.WayInDeparture != null)
                {
                    DateTime wid = (DateTime)item.WayInDeparture;
                    worksheet.Cell("I" + row).Value = wid.ToString("dd MMMM yyyy");
                    worksheet.Cell("K" + row).Value = wid.TimeOfDay;
                }
                if (item.WayInArrival != null)
                {
                    DateTime wia = (DateTime)item.WayInArrival;
                    worksheet.Cell("N" + row).Value = wia.TimeOfDay;
                }
                if (item.WayOutDeparture != null)
                {
                    DateTime wod = (DateTime)item.WayOutDeparture;
                    worksheet.Cell("O" + row).Value = wod.ToString("dd MMMM yyyy");
                    worksheet.Cell("Q" + row).Value = wod.TimeOfDay;
                }
                if (item.WayOutArrival != null)
                {
                    DateTime woa = (DateTime)item.WayOutArrival;
                    worksheet.Cell("T" + row).Value = woa.TimeOfDay;
                }

                worksheet.Cell("I" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("J" + row).Value = item.WayInCityName;

                worksheet.Cell("L" + row).Value = item.WayInFlightNumber;
                worksheet.Cell("M" + row).Value = item.WayInDestinationName;



                worksheet.Cell("O" + row).DataType = XLDataType.DateTime;
                worksheet.Cell("P" + row).Value = item.WayOutCityName;

                worksheet.Cell("R" + row).Value = item.WayOutFlightNumber;
                worksheet.Cell("S" + row).Value = item.WayOutDestinationName;




                worksheet.Cell("U" + row).Value = item.HotelName;
                worksheet.Cell("V" + row).Value = item.RoomType;
                worksheet.Cell("W" + row).Value = item.Accumpained;
                worksheet.Cell("X" + row).Value = item.RepName;
                if (item.IsPassport == true)
                {
                    worksheet.Cell("Y" + row).Value = "Download";
                    Uri x = new Uri(dir + item.Id);
                    worksheet.Cell("Y" + row).Hyperlink.ExternalAddress = x;
                }
                row++;
                s++;
            }


            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               "All Teams Events Travel Requests " + now.ToString("dd MMMM yyyy") + ".xlsx");

        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult HoldRequest(int id)
        {
            bool res = rep.HoldRequest(id);
            return Ok(res);
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult ConfirmRequest(int id)
        {
            bool res = rep.ConfirmRequest(id);
            return Ok(res);
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult RejectRequest(int id)
        {
            bool res = rep.RejectRequest(id);
            return Ok(res);
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetEventTravelRequest(int id)
        {
            EventTravelRequest res = rep.GetEventTravelRequest(id);
            return Ok(res);
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllCities()
        {
            IEnumerable<TravelCities> res = rep.GetAllCities();
            return Ok(res);
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeleteRequest(int id)
        {
            bool res = rep.DeleteRequest(id);
            return Ok(res);
        }

        [Route("[controller]/[Action]/{UserId}")]
        [HttpPost]
        public IActionResult EditRequest([FromBody] EventTravelRequest ev, string UserId)
        {
            DateTime def = new DateTime(0001, 01, 01);
            if (ev.WayInDeparture == def)
            {
                ev.WayInDeparture = null;
            }
            if (ev.WayOutDeparture == def)
            {
                ev.WayOutDeparture = null;
            }
            if (ev.WayInArrival == def)
            {
                ev.WayInArrival = null;
            }
            if (ev.WayOutArrival == def)
            {
                ev.WayOutArrival = null;
            }
            bool res = rep.EditRequest(ev, UserId);
            return Ok(res);
        }

        [Route("[controller]/[Action]/{ManagerId}")]
        [HttpGet]
        public IActionResult GetMyTeamRequests(string ManagerId)
        {
            IEnumerable<CustomEventRequest> res = rep.GetMyTeamRequests(ManagerId);
            return Ok(res);
        }

        [Route("[controller]/[Action]/{Id}")]
        [HttpGet]
        public IActionResult GetMyRequests(string Id)
        {
            IEnumerable<CustomEventRequest> res = rep.GetMyRequests(Id);
            return Ok(res);
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetEventTravellingById(int id)
        {
            Event res = rep.GetEventTravellingById(id);
            return Ok(res);
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult MakeEventRequest([FromForm] AddEventTravelRequestModel ev)
        {
            DateTime def = new DateTime(0001, 01, 01);
            if (ev.WayInDeparture == def)
            {
                ev.WayInDeparture = null;
            }
            if (ev.WayOutDeparture == def)
            {
                ev.WayOutDeparture = null;
            }
            if (ev.WayInArrival == def)
            {
                ev.WayInArrival = null;
            }
            if (ev.WayOutArrival == def)
            {
                ev.WayOutArrival = null;
            }
            bool res = rep.MakeEventRequestAsync(ev).Result;
            return Ok(res);
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpPost]
        public async Task<IActionResult> UploadTicket(IFormFile file, int id)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            int indx = file.FileName.Split('.').Length - 1;
            string ext = file.FileName.Split('.')[indx];
            string filename = id + "t." + ext;
            string contenttype = file.ContentType;
            var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot", "Ticket",
            filename);





            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            EventTravelRequest obj = db.EventTravelRequest.Find(id);
            obj.IsTicket = true;
            obj.TicketFileName = filename;
            obj.TicketFileContentType = contenttype;
            db.SaveChanges();
            return Ok(true);
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpPost]
        public async Task<IActionResult> UploadPassport(IFormFile file, int id)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");



            int indx = file.FileName.Split('.').Length - 1;
            string ext = file.FileName.Split('.')[indx];
            string filename = id + "p." + ext;
            string contenttype = file.ContentType;
            var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot", "Passport",
            filename);





            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            EventTravelRequest obj = db.EventTravelRequest.Find(id);
            obj.IsPassport = true;
            obj.PassportFileName = filename;
            obj.PassportFileContentType = contenttype;
            db.SaveChanges();
            return Ok(true);
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public async Task<IActionResult> DownloadTicket(int id)
        {
            EventTravelRequest ev = db.EventTravelRequest.Find(id);
            string filename = ev.TicketFileName;
            string contenttype = ev.TicketFileContentType;
            string ext = contenttype.Split('/')[1];
            string contactname = db.contact.Find(ev.ContactId).ContactName;
            var path = Path.Combine(
               Directory.GetCurrentDirectory(),
               "wwwroot", "Ticket",
               filename);



            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);

            }



            memory.Position = 0;




            return File(memory, contenttype, contactname + "." + ext);

        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public async Task<IActionResult> DownloadPassport(int id)
        {
            EventTravelRequest ev = db.EventTravelRequest.Find(id);
            string filename = ev.PassportFileName;
            string contenttype = ev.PassportFileContentType;
            string ext = contenttype.Split('/')[1];
            string contactname = db.contact.Find(ev.ContactId).ContactName;
            var path = Path.Combine(
               Directory.GetCurrentDirectory(),
               "wwwroot", "Passport",
               filename);
          
            

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
                   
            }

            

            memory.Position = 0;

            


            return File(memory, contenttype, contactname+"."+ext);
            
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetUpComingEvents()
        {
            IEnumerable<Event> res = rep.GetUpComingEvents();
            return Ok(res);
        }
        
        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetSixMonthsBackEvents()
        {
            IEnumerable<Event> res = rep.GetSixMonthsBackEvents();
            return Ok(res);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddNewEvent(Event ev)
        {
            bool res = rep.AddNewEvent(ev);
            return Ok(res);
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetEventTypes()
        {
            IEnumerable<EventType> res = rep.GetEventTypes();
            return Ok(res);
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetCountries()
        {
            IEnumerable<TravelCountriesModel> res = rep.GetCountries();
            return Ok(res);
        }

        [Route("[controller]/[Action]/{CountryName}")]
        [HttpGet]
        public IActionResult GetCitiesByCountryName(string CountryName)
        {
            IEnumerable<TravelCities> res = rep.GetCitiesByCountryName(CountryName);
            return Ok(res);
        }
    }
}

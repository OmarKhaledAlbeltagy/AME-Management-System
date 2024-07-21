using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AMEKSA.Repo;
using AMEKSA.DevModels;
using System.Drawing;
using System.Drawing.Imaging;
using QRCoder;
using IronBarCode;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using ClosedXML.Excel;
using AMEKSA.Models;
using MoreLinq.Extensions;
using System.Threading.Tasks;
using System.Text;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class DevController : ControllerBase
    {
        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly RoleManager<ExtendIdentityRole> roleManager;
        private readonly IAccountDeviceRep rep;

        public DevController(DbContainer db, UserManager<ExtendIdentityUser> userManager, RoleManager<ExtendIdentityRole> roleManager, IAccountDeviceRep rep)
        {
            this.db = db;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.rep = rep;
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult Alaa()
        {
            ExtendIdentityUser Alaa = db.Users.Find("323e97ab-deac-4b71-8dfb-325279577341");
            ExtendIdentityUser Ahmed = db.Users.Find("1b2d6a95-175c-4a77-8f8a-8cddbe3dc4ae");
            List<UserContact> AlaaContact = db.userContact.Where(a => a.extendidentityuserid == "323e97ab-deac-4b71-8dfb-325279577341").ToList();
            List<UserAccount> AlaaAccount = db.userAccount.Where(a => a.extendidentityuserid == "323e97ab-deac-4b71-8dfb-325279577341").ToList();

            foreach (var item in AlaaContact)
            {
                UserContact uc = new UserContact();
                uc.extendidentityuserid = "1b2d6a95-175c-4a77-8f8a-8cddbe3dc4ae";
                uc.MonthlyTarget = item.MonthlyTarget;
                uc.ContactId = item.ContactId;
                uc.CategoryId = item.CategoryId;
                db.userContact.Add(uc);
            }

            foreach (var item in AlaaAccount)
            {

                UserAccount uc = new UserAccount();
                uc.extendidentityuserid = "1b2d6a95-175c-4a77-8f8a-8cddbe3dc4ae";
                uc.AccountId = item.AccountId;
                db.userAccount.Add(uc);
            }
            db.SaveChanges();
            return Ok(true);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddContactsToYasser(List<DeleteAddContactsModel> list)
        {
            foreach (var item in list)
            {
                Contact c = db.contact.Where(a => a.ContactName == item.Contact).First();
                UserContact uc = new UserContact();
                uc.ContactId = c.Id;
                uc.extendidentityuserid = "b0050f2b-7df4-4fd5-954c-f469ae828448";
                uc.MonthlyTarget = 0;
                uc.CategoryId = item.Category;
                db.userContact.Add(uc);
            }
            db.SaveChanges();
            return Ok(true);
        }



        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult ChangePassword(string id)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(id).Result;
            string token = userManager.GeneratePasswordResetTokenAsync(user).Result;
           var res = userManager.ResetPasswordAsync(user, token, "m.shehawy@123").Result;

            return Ok(res);
        }



        [Route("[controller]/[Action]/{n}")]
        [HttpGet]
        public IActionResult number(string n)
        {
            NumberStringfy res = new NumberStringfy(n);
            string xz = res.GetNumberAr();
           
            return Ok(xz);
        }


        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetDeviceQr(int id)
        {
            byte[] f = System.IO.File.ReadAllBytes(Directory.GetCurrentDirectory() + "/wwwroot/DevicesQR/" + id + ".png");
            return File(f, "image/png");
        }




            [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult CreateRiyadhDevice()
        {
            AccountDevices a = new AccountDevices();
            a.ProductId = 29;
            a.SerialNumber = "R";
            a.AccountId = 922;
            a.IsEmpty = true;
            db.accountDevices.Add(a);
            db.SaveChanges();
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode("https://maintenance.{{DashboardURL}}?" + a.guid, QRCodeGenerator.ECCLevel.H);
            var qrCodeBitmap = new QRCode(qrCodeData).GetGraphic(60);

            var logoImage = Image.FromFile(Directory.GetCurrentDirectory() + "/wwwroot/logo/logotrans.png");
            var logoWidth = qrCodeBitmap.Width / 4;
            var logoHeight = qrCodeBitmap.Height / 8;
            var logoResized = new Bitmap(logoImage, logoWidth, logoHeight);

            var logoX = (qrCodeBitmap.Width - logoWidth) / 2;
            var logoY = (qrCodeBitmap.Height - logoHeight) / 2;

            using var graphics = Graphics.FromImage(qrCodeBitmap);
            graphics.DrawImage(logoResized, logoX, logoY, logoWidth, logoHeight);

            qrCodeBitmap.Save(Directory.GetCurrentDirectory() + "/wwwroot/DevicesQR/" + a.Id + ".png", ImageFormat.Png);

            return Ok("A device was created Successfully");
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult CreateJeddahDevice()
        {
            AccountDevices a = new AccountDevices();
            a.ProductId = 29;
            a.SerialNumber = "J";
            a.AccountId = 1206;
            a.IsEmpty = true;
            db.accountDevices.Add(a);
            db.SaveChanges();
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode("https://maintenance.{{DashboardURL}}?" + a.guid, QRCodeGenerator.ECCLevel.H);
            var qrCodeBitmap = new QRCode(qrCodeData).GetGraphic(60);

            var logoImage = Image.FromFile(Directory.GetCurrentDirectory() + "/wwwroot/logo/logotrans.png");
            var logoWidth = qrCodeBitmap.Width / 4;
            var logoHeight = qrCodeBitmap.Height / 8;
            var logoResized = new Bitmap(logoImage, logoWidth, logoHeight);

            var logoX = (qrCodeBitmap.Width - logoWidth) / 2;
            var logoY = (qrCodeBitmap.Height - logoHeight) / 2;

            using var graphics = Graphics.FromImage(qrCodeBitmap);
            graphics.DrawImage(logoResized, logoX, logoY, logoWidth, logoHeight);

            qrCodeBitmap.Save(Directory.GetCurrentDirectory() + "/wwwroot/DevicesQR/" + a.Id + ".png", ImageFormat.Png);

            return Ok("A device was created Successfully");
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult CreateDammamDevice()
        {
            AccountDevices a = new AccountDevices();
            a.ProductId = 29;
            a.SerialNumber = "D";
            a.AccountId = 1205;
            a.IsEmpty = true;
            db.accountDevices.Add(a);
            db.SaveChanges();
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode("https://maintenance.{{DashboardURL}}?" + a.guid, QRCodeGenerator.ECCLevel.H);
            var qrCodeBitmap = new QRCode(qrCodeData).GetGraphic(60);

            var logoImage = Image.FromFile(Directory.GetCurrentDirectory() + "/wwwroot/logo/logotrans.png");
            var logoWidth = qrCodeBitmap.Width / 4;
            var logoHeight = qrCodeBitmap.Height / 8;
            var logoResized = new Bitmap(logoImage, logoWidth, logoHeight);

            var logoX = (qrCodeBitmap.Width - logoWidth) / 2;
            var logoY = (qrCodeBitmap.Height - logoHeight) / 2;

            using var graphics = Graphics.FromImage(qrCodeBitmap);
            graphics.DrawImage(logoResized, logoX, logoY, logoWidth, logoHeight);

            qrCodeBitmap.Save(Directory.GetCurrentDirectory() + "/wwwroot/DevicesQR/" + a.Id + ".png", ImageFormat.Png);

            return Ok("A device was created Successfully");
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult CreateDevices()
        {
            for (int i = 30; i <= 50; i++)
            {
                string num = "";
                if (i < 10)
                {
                    num = "D0" + i.ToString();
                }
                else
                {
                    num = "D" + i.ToString();
                }
                AccountDevices a = new AccountDevices();
                a.ProductId = 29;
                a.SerialNumber = num;
                a.AccountId = 1205;
                db.accountDevices.Add(a);

                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode("https://maintenance.{{DashboardURL}}?" + a.guid, QRCodeGenerator.ECCLevel.H);
                var qrCodeBitmap = new QRCode(qrCodeData).GetGraphic(60);

                var logoImage = Image.FromFile(Directory.GetCurrentDirectory() + "/wwwroot/logo/logotrans.png");
                var logoWidth = qrCodeBitmap.Width / 4;
                var logoHeight = qrCodeBitmap.Height / 8;
                var logoResized = new Bitmap(logoImage, logoWidth, logoHeight);

                var logoX = (qrCodeBitmap.Width - logoWidth) / 2;
                var logoY = (qrCodeBitmap.Height - logoHeight) / 2;

                using var graphics = Graphics.FromImage(qrCodeBitmap);
                graphics.DrawImage(logoResized, logoX, logoY, logoWidth, logoHeight);

                qrCodeBitmap.Save(Directory.GetCurrentDirectory() + "/wwwroot/DevicesQR/" + a.Id + ".png", ImageFormat.Png);
            }

            for (int i = 1; i <= 50; i++)
            {
                string num = "";
                if (i < 10)
                {
                    num = "J0" + i.ToString();
                }
                else
                {
                    num = "J" + i.ToString();
                }
                AccountDevices a = new AccountDevices();
                a.ProductId = 29;
                a.SerialNumber = num;
                a.AccountId = 1206;
                db.accountDevices.Add(a);

                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode("https://maintenance.{{DashboardURL}}?" + a.guid, QRCodeGenerator.ECCLevel.H);
                var qrCodeBitmap = new QRCode(qrCodeData).GetGraphic(60);

                var logoImage = Image.FromFile(Directory.GetCurrentDirectory() + "/wwwroot/logo/logotrans.png");
                var logoWidth = qrCodeBitmap.Width / 4;
                var logoHeight = qrCodeBitmap.Height / 8;
                var logoResized = new Bitmap(logoImage, logoWidth, logoHeight);

                var logoX = (qrCodeBitmap.Width - logoWidth) / 2;
                var logoY = (qrCodeBitmap.Height - logoHeight) / 2;

                using var graphics = Graphics.FromImage(qrCodeBitmap);
                graphics.DrawImage(logoResized, logoX, logoY, logoWidth, logoHeight);

                qrCodeBitmap.Save(Directory.GetCurrentDirectory() + "/wwwroot/DevicesQR/" + a.Id + ".png", ImageFormat.Png);
            }
            db.SaveChanges();

            return Ok(true);



        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public async Task<IActionResult> Whats()
        {

            string BASE_URL = "https://zjkn6w.api.infobip.com";
            string API_KEY = "94c69012e5ce0335c5b8bea92c363581-000ac815-5e3d-4562-85b5-089120eb0373";
            string SENDER = "966549496932";
            //string RECIPIENT = "966567555020";
            string RECIPIENT = "201113447856";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://zjkn6w.api.infobip.com");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("App", API_KEY);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string message = $@"
            {{
                ""messages"": [
                {{
                    ""from"": ""{SENDER}"",
                    ""to"": ""{RECIPIENT}"",
                    ""content"": {{
                    ""templateName"": ""maintenance"",
                    ""templateData"": {{
                        ""body"": {{
                        ""placeholders"": [
                            ""sender"",
                            ""message"",
                            ""delivered""
                        ]
                        }}
                        
                    }},
                    ""language"": ""ar""
                }}
                }}
            ]
            }}";
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/whatsapp/1/message/template");
            httpRequest.Content = new StringContent(message, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();


            return Ok(responseContent);
        }



        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult Fix()
        { 
            List<ExtendIdentityUser> users = new List<ExtendIdentityUser>();

            List<ExtendIdentityUser> medical = userManager.GetUsersInRoleAsync("Medical Representative").Result.ToList();
            List<ExtendIdentityUser> sales = userManager.GetUsersInRoleAsync("Sales Representative").Result.ToList();
            List<ExtendIdentityUser> supportive = userManager.GetUsersInRoleAsync("Supportive").Result.ToList();

            foreach (var item in medical)
            {
                if (item.extendidentityuserid == null)
                {
                    users.Add(item);
                }
            }

            foreach (var item in sales)
            {
                if (item.extendidentityuserid == null)
                {
                    users.Add(item);
                }
            }

            foreach (var item in supportive)
            {
                if (item.extendidentityuserid == null)
                {
                    users.Add(item);
                }
            }

       
            foreach (var item in users)
            {
                ExtendIdentityUser u = db.Users.Find(item.Id);
                Passwords Manager = db.passwords.Where(a => a.UserId == item.Id).FirstOrDefault();
                if (Manager != null)
                {
                   string? ManagerName = Manager.Manager;
                    if (ManagerName != null)
                    {
                        string manid = db.Users.Where(a => a.FullName == ManagerName).FirstOrDefault().Id;
                        u.extendidentityuserid = manid;
                        db.SaveChanges();
                    }
                }
                
           
            }
            
          
       


            return Ok("Done");
        }



        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult ChangeAccountName([FromBody] List<AccountsForPlan> list)
        {
            foreach (var item in list)
            {
                Account a = db.account.Find(item.Id);
                a.AccountName = item.AccountName.ToUpper();
                
            }
            db.SaveChanges();
            return Ok(true);
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditWarranty([FromBody] List<DeleteSetWarranty> list)
        {
            foreach (var item in list)
            {
                AccountDevices x = db.accountDevices.Find(item.DeviceId);
                if (item.Warranty != null)
                {
                    DateTime w = (DateTime)item.Warranty;
                    x.Warranty = new DateTime(w.Year, w.Month, w.Day, 11, 59, 59);
                }
            }
            db.SaveChanges();
            return Ok(true);
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddBulkDevices([FromBody] List<AccountDevices> list)
        {
            foreach (var item in list)
            {
                rep.AddNewDevice(item);
            }

            return Ok(true);
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public async Task<IActionResult> qr()
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode("https://ame.med.sa/", QRCodeGenerator.ECCLevel.H);
            var qrCodeBitmap = new QRCode(qrCodeData).GetGraphic(60);

            var logoImage = Image.FromFile(Directory.GetCurrentDirectory()+"/wwwroot/logo/logotrans.png");
            var logoWidth = qrCodeBitmap.Width / 4;
            var logoHeight = qrCodeBitmap.Height / 8;
            var logoResized = new Bitmap(logoImage, logoWidth, logoHeight);

            var logoX = (qrCodeBitmap.Width - logoWidth) / 2;
            var logoY = (qrCodeBitmap.Height - logoHeight) / 2;

            using var graphics = Graphics.FromImage(qrCodeBitmap);
            graphics.DrawImage(logoResized, logoX, logoY, logoWidth, logoHeight);

            qrCodeBitmap.Save(Directory.GetCurrentDirectory() + "/wwwroot/DevicesQR/qr.png", ImageFormat.Png);
            return Ok(true);
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult abd()
        {
            string a = "e8a5b986-2ef4-4523-8eb2-af87f6202e29";

            List<Account> acc = db.account.Where(a => a.DistrictId == 9 || a.DistrictId == 10).ToList();

            foreach (var item in acc)
            {
                UserAccount x = new UserAccount();
                x.extendidentityuserid = a;
                x.AccountId = item.Id;
                db.userAccount.Add(x);
            }
            db.SaveChanges();

            return Ok(true);
        }


        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public async Task<IActionResult> DownloadPassportsByEventId(int id)
        {
            Event e = db.Event.Find(id);
            string foldername = e.EventName.Split('/')[0]+" Passports";
            List<EventTravelRequest> list = db.EventTravelRequest.Where(a => a.EventId == id && a.IsPassport == true).ToList();
            if (list.Count == 0)
            {
                return Ok("There is no Passports uploaded for this event");
            }
            string sourcePath = Path.Combine(Directory.GetCurrentDirectory()+ "/wwwroot/Passport");
            string targetPath = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/AllPassport", foldername);

            Directory.CreateDirectory(targetPath);
            foreach (var item in list)
            {
                Contact c = db.contact.Find(item.ContactId);
                string ext = item.PassportFileContentType.Split('/')[1];
                string sourceFile = System.IO.Path.Combine(sourcePath, item.PassportFileName);
                string destFile = System.IO.Path.Combine(targetPath, c.ContactName+"."+ext);
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

            return File(memory, "application/zip", e.EventName+" Passports.zip");

        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult EventContacts()
        {
            List<Contact> c = new List<Contact>();

            IEnumerable<EventTravelRequest> t = db.EventTravelRequest;

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



        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult TedComapred()
        {
            List<TedCompare> tclist = new List<TedCompare>();
            List<TED> t = db.ted.ToList();
            foreach (var item in t)
            {
                if (item.PhoneNumber == null && item.Email == null)
                {
                    continue;
                }
                Contact co = db.contact.Find(item.ContactId);
                TedCompare tc = new TedCompare();
                tc.Name = co.ContactName;
                tc.Email = co.Email;
                tc.Phone = co.MobileNumber;
                tc.TedPhone = item.PhoneNumber;
                tc.TedEmail = item.Email;
                tclist.Add(tc);
            }
            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet("Ted");

            worksheet.Range("A1:E500").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A1:E500").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:E1").Style.Font.FontSize = 18;
            worksheet.Range("A1:E1").Style.Font.Bold = true;
            worksheet.Range("A2:E500").Style.Font.FontSize = 14;

            worksheet.Cell("A1").Value = "Name";
            worksheet.Cell("B1").Value = "Current Phone";
            worksheet.Cell("C1").Value = "Current Email";
            worksheet.Cell("D1").Value = "Ted Phone";
            worksheet.Cell("E1").Value = "Ted Email";

            var row = 2;
            foreach (var item in tclist)
            {
                worksheet.Cell("A" + row).Value =item.Name;
                worksheet.Cell("B" + row).Value = item.Phone;
                worksheet.Cell("C" + row).Value = item.Email;
                worksheet.Cell("D" + row).Value = item.TedPhone;
                worksheet.Cell("E" + row).Value = item.TedEmail;

                if (item.Phone == item.TedPhone && item.Email == item.TedEmail)
                {
                    worksheet.Range("A"+row+":E"+row).Style.Fill.BackgroundColor = XLColor.FromArgb(0, 176, 80);
                }
                else
                {
                    worksheet.Range("B" + row + ":C" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(155, 194, 230);
                    worksheet.Range("D" + row + ":E" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 217, 102);
                }

                row++;
            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Ted Compared Data.xlsx");
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult not()
        {
            List<EventTravelRequest> e = db.EventTravelRequest.Where(a => a.EventId == 9).ToList();
            List<string> res = new List<string>();
            foreach (var item in e)
            {
                Invited i = db.invited.Where(a => a.ContactId == item.ContactId).FirstOrDefault();
                if (i == null)
                {
                    string cname = db.contact.Find(item.ContactId).ContactName;
                    res.Add(cname);
                }
              
            }
           
            return Ok(res);
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult pro()
        {
            IEnumerable<EventProposalRequest> r = db.EventProposalRequest;
            foreach (var item in r)
            {
                DateTime eventdate = db.Event.Find(item.EventId).From;
                item.RequestDate = eventdate;
            }
            db.SaveChanges();
            return Ok(true);
        }

        [Route("[controller]/[Action]/{g}")]
        [HttpGet]
        public IActionResult DownloadQR(int g)
        {
            byte[] f = System.IO.File.ReadAllBytes(Directory.GetCurrentDirectory()+"/image/"+g+".png");

            return File(f, "application/png", "Attendance QR Code.png");
        }



        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult readfile()
        {
            string f = System.IO.File.ReadAllText(Directory.GetCurrentDirectory()+"/Templates/EmailTemp.html");
            return Ok(true);
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult repcon()
        {
            List<ExtendIdentityUser> med = userManager.GetUsersInRoleAsync("Medical Representative").Result.ToList();
            List<UserContact> uc = new List<UserContact>();
            foreach (var item in med)
            {
                Contact c = new Contact();
                c.ContactName = item.FullName;
                c.Gender = false;
                c.DistrictId = db.district.Where(a => a.CityId == item.CityId).FirstOrDefault().CityId;
                c.MobileNumber = item.PhoneNumber;
                c.Email = item.Email;
                c.ContactTypeId = 9;
                c.AccountId = 922;
                c.Guidd = Guid.NewGuid();
                db.contact.Add(c);
                db.SaveChanges();
                UserContact u = new UserContact();
                u.extendidentityuserid = item.Id;
                u.ContactId = c.Id;
                u.CategoryId = 7;
                u.MonthlyTarget = 0;
                uc.Add(u);
            }

            foreach (var item in uc)
            {
                db.userContact.Add(item);
            }

            db.SaveChanges();

            return Ok(true);
        }
    

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult test()
        {
            return Ok("Working Fine");
        }

      

  


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult DeleteDistinct(ListOf m)
        {
            foreach (var item in m.Id)
            {
                EventTravelRequest r = db.EventTravelRequest.Find(item);
                db.EventTravelRequest.Remove(r);
            }
            db.SaveChanges();
            return Ok(true);
        }



        [Route("[controller]/[Action]/{name}")]
        [HttpGet("{name}")]
        public IActionResult AddRole(string name)
        {
            ExtendIdentityRole r = new ExtendIdentityRole();
            r.Name = name;
            var res = roleManager.CreateAsync(r).Result;
            return Ok(true);
        }
    }
}

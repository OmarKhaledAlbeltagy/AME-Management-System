using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Repo;
using ClosedXML;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class AccountDeviceController : ControllerBase
    {
        private readonly IAccountDeviceRep rep;
        private readonly DbContainer db;

        public AccountDeviceController(IAccountDeviceRep rep, DbContainer db)
        {
            this.rep = rep;
            this.db = db;
        }


        [Route("[controller]/[Action]/{Id}")]
        [HttpGet]
        public IActionResult GetDevicesByAccountId(int Id)
        {
            return Ok(rep.GetDevicesByAccountId(Id));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AssignTrainingEngineer(RequestEngineer obj)
        {
            return Ok(rep.AssignTrainingEngineer(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AssignMaintenanceEngineer(RequestEngineer obj)
        {
            return Ok(rep.AssignMaintenanceEngineer(obj));
        }
        
        [Route("[controller]/[Action]")]
        [HttpGet]
        public async Task<IActionResult> DownloadBulkQrs()
        {

            System.IO.DirectoryInfo dii = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/BulkQR"));

            foreach (FileInfo file in dii.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in dii.GetDirectories())
            {
                dir.Delete(true);
            }

            List<AccountDevices> list = db.accountDevices.Where(a => a.bulk == true).ToList();

            string sourcePath = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/DevicesQR");
            string targetPath = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/BulkQR/pngs");
            Directory.CreateDirectory(targetPath);

            foreach (var item in list)
            {
                string accountname = db.account.Find(item.AccountId).AccountName;
                string sourceFile = System.IO.Path.Combine(sourcePath, item.Id.ToString() + ".png");
                string destFile = System.IO.Path.Combine(targetPath, item.SerialNumber + " -- " + accountname.Replace('_', ' ') + " "+ item.Id +".png");
                System.IO.File.Copy(sourceFile, destFile, true);
            }



            string startPath = targetPath;
            string zipPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/BulkQR", " QRCodes.zip");
            ZipFile.CreateFromDirectory(startPath, zipPath);

            var memory = new MemoryStream();
            using (var stream = new FileStream(zipPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }



            memory.Position = 0;


            System.IO.DirectoryInfo di = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/BulkQR"));

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            foreach (var item in db.accountDevices)
            {
                item.bulk = false;
            }
            db.SaveChanges();

            return File(memory, "application/zip", "QR Codes.zip");
        }


        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public async Task<IActionResult> DownloadBulkQrsCity(int id)
        {

            System.IO.DirectoryInfo dii = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/BulkQR"));

            foreach (FileInfo file in dii.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in dii.GetDirectories())
            {
                dir.Delete(true);
            }


            var list = db.accountDevices.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                SerialNumber = a.SerialNumber,
                DistrictId = b.DistrictId
            }).Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = a.AccountId,
                SerialNumber = a.SerialNumber,
                CityId = b.CityId
            }).Where(a => a.CityId == id).OrderBy(a=>a.Id).ToList();

            string CityName = db.city.Find(id).CityName;
            string sourcePath = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/DevicesQR");
            string targetPath = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/BulkQR/pngs");
            Directory.CreateDirectory(targetPath);
  
            foreach (var item in list)
            {
                string accountname = db.account.Find(item.AccountId).AccountName;
                string sourceFile = System.IO.Path.Combine(sourcePath, item.Id.ToString() + ".png");
                string destFile = System.IO.Path.Combine(targetPath, item.SerialNumber + " " + accountname.Replace('_',' ') +" "+ item.Id +".png");
                System.IO.File.Copy(sourceFile, destFile, true);
            }



            string startPath = targetPath;
            string zipPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/BulkQR", " QRCodes.zip");
            ZipFile.CreateFromDirectory(startPath, zipPath);

            var memory = new MemoryStream();
            using (var stream = new FileStream(zipPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }



            memory.Position = 0;


            System.IO.DirectoryInfo di = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/BulkQR"));

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            return File(memory, "application/zip", CityName+" QR Codes.zip");
        }


        [Route("[controller]/[Action]/{m}")]
        [HttpGet]
        public IActionResult ServiceReportExcel(string m)
        {
            
            int year = int.Parse(m.Substring(0, 4));
            int month = int.Parse(m.Substring(5, 2));

            List<ServiceExcelModel> maint = rep.MaintenanceReportByMonth(month,year);
            List<ServiceExcelModel> train = rep.TrainingReportByMonth(month, year);


            if (maint.Count == 0 && train.Count == 0)
            {
                return Ok("There is no requests on choosed month");
            }



            string monthname = new DateTime(year, month, 1).ToString("MMMM");


            XLWorkbook workbook = new XLWorkbook();

            var worksheetm = workbook.Worksheets.Add("Maintenance Requests");

            worksheetm.Columns("A:H").Width = 25;
            worksheetm.Row(1).Height = 50;
            worksheetm.Row(2).Height = 35;
            
            worksheetm.Rows("3:1000").Height = 25;

            worksheetm.Range("A1:H1").Merge();
            worksheetm.Range("A1:H1000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheetm.Range("A1:H1000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheetm.Range("A1:H1").Style.Font.FontSize = 18;
            worksheetm.Range("A2:H2").Style.Font.FontSize = 15;
            worksheetm.Range("A3:H1000").Style.Font.FontSize = 12;
            worksheetm.Cell("A1").Style.Font.SetUnderline();
            worksheetm.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheetm.Range("A2:H2").Style.Fill.BackgroundColor = XLColor.FromArgb(255,230,153);

            worksheetm.Cell("A1").Value = "Maintenance Requests on " + monthname;

            worksheetm.Cell("A2").Value = "Account";
            worksheetm.Cell("B2").Value = "Device";
            worksheetm.Cell("C2").Value = "Name";
            worksheetm.Cell("D2").Value = "Mobile Number";
            worksheetm.Cell("E2").Value = "Brief";
            worksheetm.Cell("F2").Value = "Request Date & Time";
            worksheetm.Cell("G2").Value = "Visit Date";
            worksheetm.Cell("H2").Value = "Visit Report";
         
            var row = 3;

            foreach (var item in maint)
            {
                worksheetm.Cell("A" + row).Value = item.AccountName;
                worksheetm.Cell("B" + row).Value = item.ProductName;
                worksheetm.Cell("C" + row).Value = item.FullName;
                worksheetm.Cell("D" + row).Value = item.MobileNumber;
                worksheetm.Cell("D" + row).DataType = XLDataType.Text;
                worksheetm.Cell("E" + row).Value = item.brief;
                worksheetm.Cell("F" + row).Value = item.RequestDateTime;
                worksheetm.Cell("G" + row).Value = item.VisitDate;
                worksheetm.Cell("H" + row).Value = item.VisitReport;

                if (item.Done == true)
                {
                    worksheetm.Range("A"+row+":H"+row).Style.Fill.BackgroundColor = XLColor.FromArgb(198, 224, 180);
                }
                row++;
            }








            var worksheett = workbook.Worksheets.Add("Training Requests");

            worksheett.Columns("A:H").Width = 25;
            worksheett.Row(1).Height = 50;
            worksheett.Row(2).Height = 35;

            worksheett.Rows("3:1000").Height = 25;

            worksheett.Range("A1:H1").Merge();
            worksheett.Range("A1:H1000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheett.Range("A1:H1000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheett.Range("A1:H1").Style.Font.FontSize = 18;
            worksheett.Range("A2:H2").Style.Font.FontSize = 15;
            worksheett.Range("A3:H1000").Style.Font.FontSize = 12;
            worksheett.Cell("A1").Style.Font.SetUnderline();
            worksheett.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
            worksheett.Range("A2:H2").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 230, 153);

            worksheett.Cell("A1").Value = "Maintenance Requests on " + monthname;

            worksheett.Cell("A2").Value = "Account";
            worksheett.Cell("B2").Value = "Device";
            worksheett.Cell("C2").Value = "Name";
            worksheett.Cell("D2").Value = "Mobile Number";
            worksheett.Cell("E2").Value = "Requested Date for Training";
            worksheett.Cell("F2").Value = "Request Date & Time";
            worksheett.Cell("G2").Value = "Visit Date";
            worksheett.Cell("H2").Value = "Visit Report";

            row = 3;

            foreach (var item in train)
            {
                worksheett.Cell("A" + row).Value = item.AccountName;
                worksheett.Cell("B" + row).Value = item.ProductName;
                worksheett.Cell("C" + row).Value = item.FullName;
                worksheett.Cell("D" + row).Value = item.MobileNumber;
                worksheett.Cell("D" + row).DataType = XLDataType.Text;
                worksheett.Cell("E" + row).Value = item.RequestedDate;
                worksheett.Cell("F" + row).Value = item.RequestDateTime;
                worksheett.Cell("G" + row).Value = item.VisitDate;
                worksheett.Cell("H" + row).Value = item.VisitReport;

                if (item.Done == true)
                {
                    worksheetm.Range("A" + row + ":H" + row).Style.Fill.BackgroundColor = XLColor.FromArgb(198, 224, 180);
                }
                row++;
            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
              "Service Requests on "+ monthname + ".xlsx");
        }


        [Route("[controller]/[Action]/{g}")]
        [HttpGet]
        public IActionResult GetMaintenanceVisitDetailsRep(string g)
        {
            return Ok(rep.GetMaintenanceVisitDetailsRep(g));
        }

        [Route("[controller]/[Action]/{g}")]
        [HttpGet]
        public IActionResult GetTrainingVisitDetailsRep(string g)
        {
            return Ok(rep.GetTrainingVisitDetailsRep(g));
        }



        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult SubmitMaintenanceVisit(SubmitTrainingVisitModel obj)
        {
            return Ok(rep.SubmitMaintenanceVisit(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult SubmitTrainingVisit(SubmitTrainingVisitModel obj)
        {
            return Ok(rep.SubmitTrainingVisit(obj));
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetUpcomingMaintenanceRequests()
        {
            return Ok(rep.GetUpcomingMaintenanceRequests());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetUpcomingTrainingRequests()
        {
            return Ok(rep.GetUpcomingTrainingRequests());
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult TrainingRequest(TrainingRequest obj)
        {
            return Ok(rep.TrainingRequest(obj).Result);
        }



        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult MantenanceRequest(MaintenanceRequest obj)
        {
            return Ok(rep.MantenanceRequest(obj).Result);
        }

        [Route("[controller]/[Action]/{g}")]
        [HttpGet]
        public IActionResult ScanQrCode(string g)
        {
            return Ok(rep.ScanQrCode(g));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult PrepareBulkQR(ListOfIdsModel obj)
        {
            return Ok(rep.PrepareBulkQR(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditDevice(AccountDevices obj)
        {
            return Ok(rep.EditDevice(obj));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetAccountDeviceById(int id)
        {
            return Ok(rep.GetAccountDeviceById(id));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddNewDevice(AccountDevices obj)
        {
            return Ok(rep.AddNewDevice(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllDevices()
        {
            return Ok(rep.GetAllDevices());
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DownloadQr(int id)
        {

            AccountDevices d = db.accountDevices.Find(id);
            string serial = d.SerialNumber;
            string accountname = db.account.Find(d.AccountId).AccountName;
            byte[] f = System.IO.File.ReadAllBytes(Directory.GetCurrentDirectory() + "/wwwroot/DevicesQR/" + id + ".png");

            return File(f, "application/png", serial+" -- "+accountname+".png");

        }

       


    }
}

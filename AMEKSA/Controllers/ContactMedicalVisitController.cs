using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.ContactMedicalVisitModels;
using AMEKSA.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AMEKSA.Models;
using AMEKSA.CustomEntities;
using ClosedXML.Excel;
using System.IO;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class ContactMedicalVisitController : ControllerBase
    {
        private readonly IContactMedicalVisitRep rep;

        public ContactMedicalVisitController(IContactMedicalVisitRep rep)
        {
            this.rep = rep;
        }

        [Route("[controller]/[Action]/{userId}")]
        [HttpGet]
        public IActionResult GetContactsToVisitNow(string userId)
        {
            return Ok(rep.GetContactsToVisitNow(userId));
        }

        [Route("[controller]/[Action]/{userId}")]
        [HttpGet]
        public IActionResult GetMyTotalTarget(string userId)
        {
            return Ok(rep.GetMyTotalTarget(userId));
        }

        [Route("[controller]/[Action]/{userId}")]
        [HttpGet]
        public IActionResult GetMyContacts(string userId)
        {
            return Ok(rep.GetMyContacts(userId));
        }

      

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult MakeVisit(ContactMedicalVisitModel obj)
        {
            return Ok(rep.MakeVisit(obj));
        }



        [Route("[controller]/[Action]/{userId}")]
        [HttpGet]
        public IActionResult GetMyVisits(string userId)
        {
            return Ok(rep.GetMyVisits(userId));
        }



        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetVisitsByDate(AccountSalesVisitByDateModel obj)
        {
            return Ok(rep.GetVisitsByDate(obj));
        }


   
        [Route("[controller]/[Action]/{visitId}")]
        [HttpGet]
        public IActionResult GetVisitById(int visitId)
        {
            return Ok(rep.GetVisitById(visitId));
        }

        [Route("[controller]/[Action]/{visitid}")]
        [HttpGet]
        public IActionResult RequestDeleteContactMedical(int visitid)
        {
            return Ok(rep.RequestDeleteContactMedical(visitid));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetRequestDeleteContactMedical()
        {
            return Ok(rep.GetCMVDeleteRequests());
        }

        [Route("[controller]/[Action]/{visitid}")]
        [HttpGet]
        public IActionResult ConfirmRequestDeleteContactMedical(int visitid)
        {
            return Ok(rep.ConfirmCMVDeleting(visitid));
        }

        [Route("[controller]/[Action]/{visitid}")]
        [HttpGet]
        public IActionResult RejectRequestDeleteContactMedical(int visitid)
        {
            return Ok(rep.RejectCMVDeleting(visitid));
        }


        [Route("[controller]/[Action]/{visitId}")]
        [HttpGet]
        public IActionResult GetVisitByIdExcel(int visitId)
        {
            CustomContactMedicalVisit vis = rep.GetVisitById(visitId);
            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visit");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Contact";
            worksheet.Cell(currentRow, 2).Value = "Account Affiliation";
            worksheet.Cell(currentRow, 3).Value = "District";
            worksheet.Cell(currentRow, 4).Value = "Phone Number";
            worksheet.Cell(currentRow, 5).Value = "Contact Type";
            worksheet.Cell(currentRow, 6).Value = "Category";
            worksheet.Cell(currentRow, 7).Value = "Products";
            worksheet.Cell(currentRow, 8).Value = "Sales Aid";
            worksheet.Cell(currentRow, 9).Value = "Visit Notes";
            worksheet.Cell(currentRow, 10).Value = "Requests";
            worksheet.Cell(currentRow, 11).Value = "Visit Date";
            worksheet.Cell(currentRow, 12).Value = "Visit Time";


            currentRow = 2;
            worksheet.Cell(currentRow, 1).Value = vis.ContactName;
            worksheet.Cell(currentRow, 2).Value = vis.AccountName;
            worksheet.Cell(currentRow, 3).Value = vis.DistrictName;
            worksheet.Cell(currentRow, 4).Value = vis.MobileNumber;
            worksheet.Cell(currentRow, 5).Value = vis.ContactTypeName;
            worksheet.Cell(currentRow, 6).Value = vis.CategoryName;
            worksheet.Cell(currentRow, 9).Value = vis.VisitNotes;
            worksheet.Cell(currentRow, 10).Value = vis.Requests;
            string date = vis.VisitDate.ToString("dd MMMM yyyy");
            TimeSpan time = vis.VisitTime.TimeOfDay;
            worksheet.Cell(currentRow, 11).Value = date;
            worksheet.Cell(currentRow, 12).Value = time;

            foreach (var item in vis.customcontactmedicalvisitproduct)
            {
                worksheet.Cell(currentRow, 7).Value ="Product: "+item.ProductName +" | Product Share: "+item.ProductShare+"/10";
                currentRow++;
            }
            currentRow = 2;
            foreach (var item in vis.customcontactsalesaid)
            {
                worksheet.Cell(currentRow, 8).Value = item.SalesAidName;
                currentRow++;
            }
            MemoryStream stream = new MemoryStream();

            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "visit to " + vis.ContactName + ".xlsx");
        }


    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.AccountMedicalVisitModels;
using AMEKSA.Context;
using AMEKSA.CustomEntities;
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
    public class AccountMedicalVisitController : ControllerBase
    {
        private readonly IProductRep productRep;
        private readonly IAccountMedicalVisitRep medicalVisitRep;

        public AccountMedicalVisitController(IProductRep productRep,IAccountMedicalVisitRep medicalVisitRep)
        {
            this.productRep = productRep;
            this.medicalVisitRep = medicalVisitRep;
        }


        [Route("[controller]/[Action]/{userId}")]
        [HttpGet("{userId}")]
        public IActionResult GetMyProducts(string userId)
        {
            return Ok(productRep.GetMyProducts(userId));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult MakeVisit(AccountMedicalVisitModel obj)
        {
            return Ok(medicalVisitRep.MakeVisit(obj));
        }

        [Route("[controller]/[Action]/{visitId}")]
        [HttpGet]
        public IActionResult GetVisitById(int visitId)
        {
            return Ok(medicalVisitRep.GetVisitById(visitId));
        }

        [Route("[controller]/[Action]/{visitId}")]
        [HttpGet]
        public IActionResult GetVisitByIdExcel(int visitId)
        {
            CustomAccountMedicalVisit vis = medicalVisitRep.GetVisitById(visitId);
            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visit");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Account";
            worksheet.Cell(currentRow, 2).Value = "District";
            worksheet.Cell(currentRow, 3).Value = "Phone Number";
            worksheet.Cell(currentRow, 4).Value = "Account Type";
            worksheet.Cell(currentRow, 5).Value = "Category";
            worksheet.Cell(currentRow, 6).Value = "Products";
            worksheet.Cell(currentRow, 7).Value = "Persons Met";
            worksheet.Cell(currentRow, 8).Value = "Visit Notes";
            worksheet.Cell(currentRow, 9).Value = "Additinal Notes";
            worksheet.Cell(currentRow, 10).Value = "Visit Date";
            worksheet.Cell(currentRow, 11).Value = "Visit Time";


            currentRow = 2;
            worksheet.Cell(currentRow, 1).Value = vis.AccountName;
            worksheet.Cell(currentRow, 2).Value = vis.DistrictName;
            worksheet.Cell(currentRow, 3).Value = vis.PhoneNumber;
            worksheet.Cell(currentRow, 4).Value = vis.AccountTypeName;
            worksheet.Cell(currentRow, 5).Value = vis.CategoryName;
            worksheet.Cell(currentRow, 8).Value = vis.VisitNotes;
            worksheet.Cell(currentRow, 9).Value = vis.AdditionalNotes;
            string date = vis.VisitDate.ToString("dd MMMM yyyy");
            TimeSpan time = vis.VisitTime.TimeOfDay;
            worksheet.Cell(currentRow, 10).Value = date;
            worksheet.Cell(currentRow, 11).Value = time;

            foreach (var item in vis.product)
            {
                worksheet.Cell(currentRow, 6).Value = item.ProductName;
                currentRow++;
            }
            currentRow = 2;
            foreach (var item in vis.person)
            {
                worksheet.Cell(currentRow, 7).Value =item.PersonName+" - "+item.PersonPosition;
                currentRow++;
            }
            MemoryStream stream = new MemoryStream();

            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "visit to "+vis.AccountName+".xlsx");
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetVisitByDate(AccountSalesVisitByDateModel obj)
        {
            return Ok(medicalVisitRep.GetMyVisitsByDate(obj));
        }

        [Route("[controller]/[Action]/{visitid}")]
        [HttpGet]
        public IActionResult RequestDeleteAccountMedical(int visitid)
        {
            return Ok(medicalVisitRep.RequestDeleteAccountMedical(visitid));
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetRequestDeleteAccountMedical()
        {
            return Ok(medicalVisitRep.GetAMVDeleteRequests());
        }

        [Route("[controller]/[Action]/{visitid}")]
        [HttpGet]
        public IActionResult ConfirmDeleteVisit(int visitid)
        {
            return Ok(medicalVisitRep.ConfirmAMVDeleting(visitid));
        }

        [Route("[controller]/[Action]/{visitid}")]
        [HttpGet]
        public IActionResult RejectDeleteVisit(int visitid)
        {
            return Ok(medicalVisitRep.RejectAMVDeleting(visitid));
        }
    }
}

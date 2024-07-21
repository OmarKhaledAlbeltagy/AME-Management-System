using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.AccountSalesVisitModels;
using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using AMEKSA.Repo;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class AccountSalesVisitController : ControllerBase
    {
        private readonly IAccountSalesVisitRep rep;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public AccountSalesVisitController(IAccountSalesVisitRep rep,UserManager<ExtendIdentityUser> userManager)
        {
            this.rep = rep;
            this.userManager = userManager;
        }

        [Route("[controller]/[Action]/{userId}")]
        [HttpGet]
        public IActionResult AccountsToVisitNow(string userId)
        {

            return Ok(rep.GetAccountsToVisitNow(userId));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult Register(TopLineManagerRegisterModel obj)
        {
                ExtendIdentityUser u = new ExtendIdentityUser();
                u.Email = obj.Email;
                u.UserName = obj.Email;
                u.FullName = obj.FullName;
                u.PhoneNumber = obj.PhoneNumber;
                u.SecurityStamp = Guid.NewGuid().ToString();
                var result = userManager.CreateAsync(u, obj.Password).Result;
                var addtorole = userManager.AddToRoleAsync(u, obj.RoleName).Result;

            if (result.Succeeded && addtorole.Succeeded)
            {
                ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
                string token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                IdentityResult confirm = userManager.ConfirmEmailAsync(user, token).Result;
                return Ok(true);
            }
            else
            { 
                return Ok(false);
            }
        }






        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult MakeAVisit(AccountSalesVisitModel obj)
        {

            return Ok(rep.MakeVisit(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult MakeACollection(MakeCollection obj)
        {
            return Ok(rep.MakeACollection(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult RequestNewOpenning(OpenningRequest obj)
        {
            return Ok(rep.RequestNewOpenning(obj));
        }
    
        [Route("[controller]/[Action]/{userId?}")]
        [HttpGet("{userId}")]
        public IActionResult GetMyRequests(string userId)
        {
            return Ok(rep.GetMyRequests(userId));
        }

        [Route("[controller]/[Action]/{userId?}")]
        [HttpGet("{userId}")]
        public IActionResult GetMyAccounts(string userId)
        {
            return Ok(rep.GetMyAccounts(userId));
        }

        [Route("[controller]/[Action]/{userId?}")]
        [HttpGet("{userId}")]
        public IActionResult GetMyVisits(string userId)
        {
            return Ok(rep.GetMyVisits(userId));
        }

        [Route("[controller]/[Action]/{visitId?}")]
        [HttpGet("{visitId}")]
        public IActionResult GetVisitById(int visitId)
        {
            return Ok(rep.GetVisitById(visitId));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetVisitByDate(AccountSalesVisitByDateModel obj)
        {
            return Ok(rep.GetMyVisitsByDate(obj));
        }

        [Route("[controller]/[Action]/{visitid}")]
        [HttpGet]
        public IActionResult RequestDeleteAccountSales(int visitid)
        {
            return Ok(rep.RequestDeleteAccountSales(visitid));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetRequestDeleteAccountSales()
        {
            return Ok(rep.GetASVDeleteRequests());
        }

        [Route("[controller]/[Action]/{visitid}")]
        [HttpGet]
        public IActionResult ConfirmRequestDeleteAccountSales(int visitid)
        {
            return Ok(rep.ConfirmASVDeleting(visitid));
        }

        [Route("[controller]/[Action]/{visitid}")]
        [HttpGet]
        public IActionResult RejectRequestDeleteAccountSales(int visitid)
        {
            return Ok(rep.RejectASVDeleting(visitid));
        }

        [Route("[controller]/[Action]/{visitId}")]
        [HttpGet]
        public IActionResult GetVisitByIdExcel(int visitId)
        {
            CustomAccountSalesVisit vis = rep.GetVisitById(visitId);
            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Visit");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Account";
            worksheet.Cell(currentRow, 2).Value = "District";
            worksheet.Cell(currentRow, 3).Value = "Phone Number";
            worksheet.Cell(currentRow, 4).Value = "Account Type";
            worksheet.Cell(currentRow, 5).Value = "Category";
            worksheet.Cell(currentRow, 6).Value = "Brands";
            worksheet.Cell(currentRow, 7).Value = "Persons Met";
            worksheet.Cell(currentRow, 8).Value = "Visit Notes";
            worksheet.Cell(currentRow, 9).Value = "Payment Notes";
            worksheet.Cell(currentRow, 10).Value = "Visit Date";
            worksheet.Cell(currentRow, 11).Value = "Visit Time";


            currentRow = 2;
            worksheet.Cell(currentRow, 1).Value = vis.AccountName;
            worksheet.Cell(currentRow, 2).Value = vis.DistrictName;
            worksheet.Cell(currentRow, 3).Value = vis.PhoneNumber;
            worksheet.Cell(currentRow, 4).Value = vis.AccountTypeName;
            worksheet.Cell(currentRow, 5).Value = vis.CategoryName;
            worksheet.Cell(currentRow, 8).Value = vis.VisitNotes;
            worksheet.Cell(currentRow, 9).Value = vis.PaymentNotes;
            string date = vis.VisitDate.ToString("dd MMMM yyyy");
            TimeSpan time = vis.VisitTime.TimeOfDay;
            worksheet.Cell(currentRow, 10).Value = date;
            worksheet.Cell(currentRow, 11).Value = time;

            foreach (var item in vis.brands)
            {
                worksheet.Cell(currentRow, 6).Value = item.BrandName;
                currentRow++;
            }
            currentRow = 2;
            foreach (var item in vis.persons)
            {
                worksheet.Cell(currentRow, 7).Value = item.PersonName + " - " + item.PersonPosition;
                currentRow++;
            }
            MemoryStream stream = new MemoryStream();

            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "visit to " + vis.AccountName + ".xlsx");
        }
    }
}

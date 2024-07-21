using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.CustomEntities;
using AMEKSA.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Context;
using Microsoft.AspNetCore.Identity;
using AMEKSA.Privilage;
using ClosedXML.Excel;
using System.IO;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRep accountRep;
        private readonly IAccountTypeRep accountTypeRep;
        private readonly DbContainer db;

        public AccountController(IAccountRep accountRep, IAccountTypeRep accountTypeRep,DbContainer db)
        {
            this.accountRep = accountRep;
            this.accountTypeRep = accountTypeRep;
            this.db = db;
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetAccountResidualCreditLimit(int id)
        {
            return Ok(accountRep.GetAccountResidualCreditLimit(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetAccountCreditLimit(int id)
        {

            return Ok(accountRep.GetAccountCreditLimit(id));
        }

        [Route("[controller]/[Action]/{id}/{creditLimit}")]
        [HttpGet]
        public IActionResult EditAccountCreditLimit(int id, float creditlimit)
        {

            return Ok(accountRep.EditAccountCreditLimit(id, creditlimit));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult SearchAccount(SearchByWord accountName)
        {
            
            return Ok(accountRep.SearchAccount(accountName));
        }



        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllAccounts()
        {
            IEnumerable<CustomAccount> accounts = accountRep.GetAllAccounts();
            return Ok(accounts);
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllAccountNames()
        {
            return Ok(accountRep.GetAccountNames());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllAccountTypes()
        {
            return Ok(accountTypeRep.GetAllAccountTypes());
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddAccount(AccountModel obj)
        {
            return Ok(accountRep.AddAccount(obj).Result);
        }

        [Route("[controller]/[Action]/{accountId}")]
        [HttpGet("{accountId}")]
        public IActionResult GetAccountById(int accountId)
        {
            return Ok(accountRep.GetAccountById(accountId));
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditAccountGeneralInfo(AccountGeneralInfo obj)
        {
            return Ok(accountRep.EditAccountGeneralInfo(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditAccountLocationInfo(AccountLocationInfo obj)
        {
            return Ok(accountRep.EditAccountLocationInfo(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditAccountContactInfo(AccountContactInfo obj)
        {
            return Ok(accountRep.EditAccountContactinfo(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditAccountTimeInfo(AccountTimeInfo obj)
        {
            return Ok(accountRep.EditAccountTimeInfo(obj));
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditAccountNotesInfo(AccountNoteInfo obj)
        {
            return Ok(accountRep.EditAccountNotesInfo(obj));
        }

        [Route("[controller]/[Action]/{accountId}")]
        [HttpGet("{accountId}")]
        public IActionResult GetAvailableBrandsForOppening(int accountId)
        {
            return Ok(accountRep.GetAvailableBrandsForOppening(accountId));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult SetAccountOpenning(NewOpenning obj)
        {
            return Ok(accountRep.SetAccountOpenning(obj));
        }

        [Route("[controller]/[Action]/{accountId}")]
        [HttpGet("{accountId}")]
        public IActionResult GetAccountOpenningsByAccountId(int accountId)
        {
            return Ok(accountRep.GetAccountOpenningsByAccountId(accountId));
        }

        
        [Route("[controller]/[Action]/{openningId}/{openning}")]
        [HttpGet("{accountId}")]
        public IActionResult EditAccountOpenning(int openningId, int openning)
        {
            return Ok(accountRep.EditAccountOpenning(openningId,openning));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditAccountType(AccountType obj)
        {
            return Ok(accountTypeRep.EditAccountType(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddAccountType(AccountType obj)
        {
            return Ok(accountTypeRep.AddAccountType(obj));
        }


        [Route("[controller]/[Action]/{OpenningId?}")]
        [HttpGet("{OpenningId?}")]
        public IActionResult GetOpenningById(int OpenningId)
        {
            return Ok(accountRep.GetOpenningById(OpenningId));
        }



        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetAllAccountsfiltered(FilteringAccounts obj)
        {
            return Ok(accountRep.GetAllAccountsfiltered(obj));
        }

        [Route("[controller]/[Action]/{AccountTypeId}/{CityId}/{DistrictId}")]
        [HttpGet]
        public IActionResult GetAllAccountsfilteredExcel(int AccountTypeId, int CityId, int DistrictId)
        {
            FilteringAccounts obj = new FilteringAccounts();
            obj.AccountTypeId = AccountTypeId;
            obj.DistrictId = DistrictId;
            obj.CityId= CityId;
            List<CustomAccount> res = accountRep.GetAllAccountsfiltered(obj).ToList();

            List<AccountType> accountType = db.accountType.ToList();
            List<Entities.Category> category = db.category.ToList();

            XLWorkbook workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Accounts");
            worksheet.Cell("A1").Value = "Account";
            worksheet.Cell("B1").Value = "District";
            worksheet.Cell("C1").Value = "Account Type";
            worksheet.Cell("D1").Value = "Category";
            worksheet.Range("A1:D1").Style.Font.FontSize = 16;
            worksheet.Range("A1:D1").Style.Font.Bold = true;
            worksheet.Range("A1:D2000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:D2000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Range("A1:D1").Style.Fill.BackgroundColor = XLColor.FromArgb(146, 205, 220);
            worksheet.Columns("A:D").Width = 27;
            worksheet.Rows("1:2000").Height = 27;
            var row = 2;
            foreach (var item in res)
            {
                worksheet.Cell("A" + row).Value = item.AccountName;
                worksheet.Cell("B" + row).Value = item.DistrictName;
                if (item.AccountTypeId == null)
                {

                }
                else
                {
                    worksheet.Cell("C" + row).Value = accountType.Where(a => a.Id == item.AccountTypeId).FirstOrDefault().AccountTypeName;
                }


                if (item.CategoryId == null)
                {

                }
                else
                {

                    worksheet.Cell("D" + row).Value = category.Where(a => a.Id == item.CategoryId).FirstOrDefault().CategoryName;
                }
                row++;
            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               "Accounts.xlsx");
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetAllAccountsfilteredWithouttype(FilteringAccounts obj)
        {
            return Ok(accountRep.GetAllAccountsfilteredWithouttype(obj));
        }

        [Route("[controller]/[Action]/{userId}")]
        [HttpGet]
        public IActionResult GetFLMAccounts(string userId)
        {
            return Ok(accountRep.GetFLMAccounts(userId));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeleteAccount(int id)
        {
            return Ok(accountRep.DeleteAccount(id));
        }


        //[Route("[controller]/[Action]/{id}")]
        //[HttpGet]
        //public IActionResult DeleteUser(string id)
        //{
        //    IEnumerable<AccountMedicalVisit> MV = db.accountMedicalVisit.Where(a => a.extendidentityuserid == id);

        //    foreach (var item in MV)
        //    {
        //        IEnumerable<AccountMedicalVisitPerson> AMP = db.accountMedicalVisitPerson.Where(a => a.AccountMedicalVisitId == item.Id);
        //        IEnumerable<AccountMedicalVisitProducts> AMPro = db.accountMedicalVisitProducts.Where(a => a.AccountMedicalVisitId == item.Id);
        //        IEnumerable<RequestDeleteAccountMedical> RDAM = db.requestDeleteAccountMedical.Where(a => a.AccountMedicalVisitId == item.Id);
        //        foreach (var person in AMP)
        //        {
        //            db.accountMedicalVisitPerson.Remove(person);
        //        }
        //        foreach (var product in AMPro)
        //        {
        //            db.accountMedicalVisitProducts.Remove(product);
        //        }
        //        foreach (var RDAMO in RDAM)
        //        {
        //            db.requestDeleteAccountMedical.Remove(RDAMO);
        //        }
        //        db.accountMedicalVisit.Remove(item);
        //    }
        //    IEnumerable<ContactMedicalVisit> CV = db.contactMedicalVisit.Where(a => a.extendidentityuserid == id);
        //        foreach (var visit in CV)
        //        {
        //            IEnumerable<ContactMedicalVisitProduct> CMP = db.contactMedicalVisitProduct.Where(a => a.ContactMedicalVisitId == visit.Id);
        //            IEnumerable<ContactSalesAid> CSA = db.contactSalesAid.Where(a => a.ContactMedicalVisitId == visit.Id);
        //            IEnumerable<RequestDeleteContactMedical> RDCM = db.requestDeleteContactMedical.Where(a => a.ContactMedicalVisitId == visit.Id);
        //            foreach (var productt in CMP)
        //            {
        //                db.contactMedicalVisitProduct.Remove(productt);
        //            }
        //            foreach (var Aid in CSA)
        //            {
        //                db.contactSalesAid.Remove(Aid);
        //            }
        //            foreach (var RDCMO in RDCM)
        //            {
        //                db.requestDeleteContactMedical.Remove(RDCMO);
        //            }
        //            db.contactMedicalVisit.Remove(visit);
        //        }

        //        IEnumerable<AccountSalesVisit> AS = db.accountSalesVisit.Where(a => a.extendidentityuserid == id);
        //        foreach (var v in AS)
        //        {
        //            IEnumerable<AccountSalesVisitBrand> ASB = db.accountSalesVisitBrand.Where(a => a.AccountSalesVisitId == v.Id);
        //            IEnumerable<AccountSalesVisitPerson> ASP = db.accountSalesVisitPerson.Where(a => a.AccountSalesVisitId == v.Id);
        //            IEnumerable<RequestDeleteAccountSales> RDAS = db.requestDeleteAccountSales.Where(a => a.AccountSalesVisitId == v.Id);
        //            foreach (var brand in ASB)
        //            {
        //                db.accountSalesVisitBrand.Remove(brand);
        //            }
        //            foreach (var personn in ASP)
        //            {
        //                db.accountSalesVisitPerson.Remove(personn);
        //            }
        //            foreach (var RDASO in RDAS)
        //            {
        //                db.requestDeleteAccountSales.Remove(RDASO);
        //            }
        //            db.accountSalesVisit.Remove(v);
        //        }

        //        IEnumerable<UserAccount> UA = db.userAccount.Where(a => a.extendidentityuserid == id);
        //        IEnumerable<UserContact> UC = db.userContact.Where(a => a.extendidentityuserid == id);
        //        IEnumerable<UserBrand> UB = db.userBrand.Where(a => a.extendidentityuserid == id);
        //        IEnumerable<OpenningRequest> OR = db.openningRequest.Where(a => a.ExtendIdentityUserId == id);
        //        foreach (var useraccount in UA)
        //        {
        //            db.userAccount.Remove(useraccount);
        //        }

        //        foreach (var usercontact in UC)
        //        {
        //            db.userContact.Remove(usercontact);
        //        }

        //        foreach (var userbrand in UB)
        //        {
        //            db.userBrand.Remove(userbrand);
        //        }

        //        foreach (var openning in OR)
        //        {
        //            db.openningRequest.Remove(openning);
        //        }
              

        //       ExtendIdentityUser x = db.Users.Find(id);

        //        db.Users.Remove(x);

        //        db.SaveChanges();

        //        return Ok(true);

            
        //}
    }
}

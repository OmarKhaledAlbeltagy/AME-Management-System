using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Repo;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class ContactController : ControllerBase
    {
        private readonly IContactRep contactRep;
        private readonly IContactTypeRep contactTypeRep;
        private readonly DbContainer db;

        public ContactController(IContactRep contactRep, IContactTypeRep contactTypeRep, DbContainer db)
        {
            this.contactRep = contactRep;
            this.contactTypeRep = contactTypeRep;
            this.db = db;
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddHsanByContactId(AddHsanModel obj)
        {
            return Ok(contactRep.AddHsanByContactId(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddHsan(AddHsanModel obj)
        {
            return Ok(contactRep.AddHsan(obj));
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddHsanDirect(AddHsanModel obj)
        {
            return Ok(contactRep.AddHsanDirect(obj));
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult SearchContact(SearchByWord contactName)
        {
            return Ok(contactRep.SearchContact(contactName));
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllContactTypes()
        {
            return Ok(contactTypeRep.GetAllContactTypes());
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditContactType(ContactType obj)
        {
            return Ok(contactTypeRep.EditContactType(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddNewContactType(ContactType obj)
        {
            return Ok(contactTypeRep.AddContactType(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllContacts()
        {
            return Ok(contactRep.GetAllContacts());
        }


        [Route("[controller]/[Action]/{contactId}")]
        [HttpGet("{contactId}")]
        public IActionResult GetContactById(int contactId)
        {

            int? accountid = db.contact.Where(a => a.Id == contactId).Select(a => a.AccountId).SingleOrDefault();

            CustomContact result;

            if (accountid == null)
            {
                result = contactRep.GetContactByIdWithoutAccount(contactId);
            }
            else
            {
                result = contactRep.GetContactByIdWithAccount(contactId);
            }

            return Ok(result);
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddContact(AddContact obj)
        {
            if (obj.AccountName == null)
            {
                Contact contact = new Contact();
                contact.ContactName = obj.ContactName;
                contact.Gender = obj.Gender;
                contact.DistrictId = obj.DistrictId;
                contact.Address = obj.Address;
                contact.LandLineNumber = obj.LandLineNumber;
                contact.MobileNumber = obj.MobileNumber;
                contact.Email = obj.Email;
                contact.ContactTypeId = obj.ContactTypeId;
                contact.PaymentNotes = obj.PaymentNotes;
                contact.RelationshipNote = obj.RelationshipNote;
                contact.BestTimeFrom = obj.BestTimeFrom;
                contact.BestTimeTo = obj.BestTimeTo;
                contact.PurchaseTypeId = obj.PurchaseTypeId;
                contact.AccountId = null;
                return Ok(contactRep.AddContact(contact));
            }
            else
            {
                Account account = db.account.Where(a => a.AccountName == obj.AccountName).FirstOrDefault();

                if (account == null)
                {
                    return Ok(false);
                }
                else
                {
                    Contact contact = new Contact();
                    contact.ContactName = obj.ContactName;
                    contact.Gender = obj.Gender;
                    contact.DistrictId = obj.DistrictId;
                    contact.Address = obj.Address;
                    contact.LandLineNumber = obj.LandLineNumber;
                    contact.MobileNumber = obj.MobileNumber;
                    contact.Email = obj.Email;
                    contact.ContactTypeId = obj.ContactTypeId;
                    contact.PaymentNotes = obj.PaymentNotes;
                    contact.RelationshipNote = obj.RelationshipNote;
                    contact.BestTimeFrom = obj.BestTimeFrom;
                    contact.BestTimeTo = obj.BestTimeTo;
                    contact.PurchaseTypeId = obj.PurchaseTypeId;
                    contact.AccountId = account.Id;
                    return Ok(contactRep.AddContact(contact));
                }
            }

            
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditContactGeneralInfo(ContactGeneralInfo obj)
        {
            if (obj.AccountName == null)
            {
                return Ok(contactRep.EditContactGeneralInfo(obj));
            }

            else
            {
                Account account = db.account.Where(a => a.AccountName == obj.AccountName).FirstOrDefault();

                if (account == null)
                {
                    return Ok(false);
                }
                else
                {
                    obj.AccountId = account.Id;
                    return Ok(contactRep.EditContactGeneralInfo(obj));
                }
            }
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditContactLocationInfo(ContactLocationInfo obj)
        {
            return Ok(contactRep.EditContactLocationInfo(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditContactContactInfo(ContactContactInfo obj)
        {
            return Ok(contactRep.EditContactContactinfo(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditContactTimeInfo(ContactTimeInfo obj)
        {
            return Ok(contactRep.EditContactTimeInfo(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditContactNoteInfo(ContactNoteInfo obj)
        {
            return Ok(contactRep.EditContactNotesInfo(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetAllContactsFiltered(FilteringContactsModel obj)
        {
            return Ok(contactRep.GetAllContactsFiltered(obj));
        }

        [Route("[controller]/[Action]/{ContactTypeId}/{CityId}/{DistrictId}")]
        [HttpGet]
        public IActionResult GetAllContactsFilteredExcel(int ContactTypeId, int CityId, int DistrictId)
        {
            FilteringContactsModel filter = new FilteringContactsModel();
            filter.ContactTypeId = ContactTypeId;
            filter.CityId = CityId; 
            filter.DistrictId = DistrictId;

           List<CustomContact> res = contactRep.GetAllContactsFiltered(filter).OrderBy(a=>a.ContactName).ToList();

            List<ContactType> contactType = db.contactType.ToList();
            List<Entities.Category> category = db.category.ToList();

           XLWorkbook workbook = new XLWorkbook();
           var worksheet = workbook.Worksheets.Add("Contacts");
            worksheet.Cell("A1").Value = "Contact";
            worksheet.Cell("B1").Value = "District";
            worksheet.Cell("C1").Value = "Contact Type";
            worksheet.Cell("D1").Value = "Account Affiliation";
            worksheet.Cell("E1").Value = "Category";
            worksheet.Range("A1:E1").Style.Font.FontSize = 16;
            worksheet.Range("A1:E1").Style.Font.Bold = true;
            worksheet.Range("A1:E2000").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:E2000").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Range("A1:E1").Style.Fill.BackgroundColor = XLColor.FromArgb(146, 205, 220);
            worksheet.Columns("A:E").Width = 27;
            worksheet.Rows("1:2000").Height = 27;
            var row = 2;
            foreach (var item in res)
            {
                worksheet.Cell("A" + row).Value = item.ContactName;
                worksheet.Cell("B" + row).Value = item.DistrictName;
                if (item.ContactTypeId == null)
                {

                }
                else
                {
                    worksheet.Cell("C" + row).Value = contactType.Where(a=>a.Id == item.ContactTypeId).FirstOrDefault().ContactTypeName;
                }
                worksheet.Cell("D" + row).Value = item.AccountName;

                if (item.CategoryId == null)
                {

                }
                else
                {
    
                    worksheet.Cell("E" + row).Value = category.Where(a => a.Id == item.CategoryId).FirstOrDefault().CategoryName;
                }
                row++;
            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               "Contacts.xlsx");
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeleteContact(int id)
        {
            return Ok(contactRep.DeleteContact(id));
        }

   

    }
}

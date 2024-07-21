using AMEKSA.Context;
using AMEKSA.Entities;
using ClosedXML.Excel;
using System;
using System.IO;

namespace AMEKSA.Repo
{
    public class EventExpenseDocumentRep : IEventExpenseDocumentRep
    {

        private readonly DbContainer db;

        public EventExpenseDocumentRep(DbContainer db)
        {
            this.db = db;
        }

        public XLWorkbook AddDocument(/*ExpensRequestDocument obj*/)
        {
            //string stringamount = obj.Amount.ToString();
            //if (stringamount.Split('.')[1].Length > 0)
            //{
            //    stringamount = stringamount + "0";
            //}

            //DateTime now = new DateTime();
            //NumberStringfy str = new NumberStringfy(stringamount);
            //ExpensRequestDocument r = new ExpensRequestDocument();
            //r.EventtId = obj.EventtId;
            //r.ExtendIdentityUserId = obj.ExtendIdentityUserId;
            //r.ContactId = obj.ContactId;
            //r.Amount = obj.Amount;
            //r.AmountWord = str.GetNumberAr();
            //r.BankAccounOwner = obj.BankAccounOwner;
            //r.BankName = obj.BankName;
            //r.CreationDateTime = now;
            //r.Iban = obj.Iban;
            //r.Name = obj.Name;
            //r.Note = obj.Note;

            XLWorkbook workbook = new XLWorkbook{RightToLeft = true};
            var worksheet = workbook.Worksheets.Add("Request Document");
            worksheet.PageSetup.PaperSize = XLPaperSize.A4Paper;
            worksheet.PageSetup.Margins.Top = 0.25;
            worksheet.PageSetup.Margins.Bottom = 0.25;
            

            worksheet.Row(1).Height = 55.8;
            worksheet.Columns("A:L").Width = 6.33;

            worksheet.Range("A5:I5").Merge();
            worksheet.Range("A6:I6").Merge();
            worksheet.Cell("A5").Value = "الإدارة المالية Accounting Department";
            worksheet.Cell("A6").Value = "أمر صرف Receipt Payment";

            worksheet.Range("A7:C7").Merge();
            worksheet.Range("D7:I7").Merge();
            worksheet.Cell("A7").Value = "التاريخ Date";
            worksheet.Cell("D7").Value = "25 March 2023";
            worksheet.Range("A8:C8").Merge();
            worksheet.Range("D8:I8").Merge();
            worksheet.Cell("A8").Value = "الاسم Name";
            worksheet.Cell("D8").Value = "Mohamed Elsawy";

            worksheet.Range("A10:I10").Merge();
            worksheet.Cell("A10").Value = "تفاصيل صرف Description";

            worksheet.Range("A11:C11").Merge();
            worksheet.Range("D11:I11").Merge();
            worksheet.Cell("A11").Value = "المبلغ رقما Amount";
            worksheet.Cell("D11").Value = 5300.5;

            worksheet.Range("A12:C12").Merge();
            worksheet.Range("D12:I12").Merge();
            worksheet.Cell("A12").Value = "Amount in Words المبلغ كتابة";
            worksheet.Cell("D12").Value = "خمسة آلاف وثلاث مائة ريال وخمسون هللة";

            worksheet.Range("A13:C13").Merge();
            worksheet.Range("D13:I13").Merge();
            worksheet.Cell("A13").Value = "الفعالية Event";
            worksheet.Cell("D13").Value = "IMCAS 2023";

            worksheet.Range("A14:C14").Merge();
            worksheet.Range("D14:I14").Merge();
            worksheet.Cell("A14").Value = "Contact";
            worksheet.Cell("D14").Value = "Dhafer Hafez Alqahtani";

            worksheet.Range("A15:C15").Merge();
            worksheet.Range("D15:I15").Merge();
            worksheet.Cell("A15").Value = "ملاحظات Notes";
            worksheet.Cell("D15").Value = "Teoxane Riyadh";

            worksheet.Range("A17:I17").Merge();
            worksheet.Cell("A17").Value = "تفاصيل الحساب البنكي Bank Details";

            worksheet.Range("A18:C18").Merge();
            worksheet.Range("D18:I18").Merge();
            worksheet.Cell("A18").Value = "اسم صاحب الحساب Account Hold Name";
            worksheet.Cell("D18").Value = "Mohamed Elsawy";

            worksheet.Range("A19:C19").Merge();
            worksheet.Range("D19:I19").Merge();
            worksheet.Cell("A19").Value = "اسم البنك Bank Name";
            worksheet.Cell("D19").Value = "البنك الأهلي التجاري";

            worksheet.Range("A20:C20").Merge();
            worksheet.Range("D20:I20").Merge();
            worksheet.Cell("A20").Value = "رقم الآي بان IBAN";
            worksheet.Cell("D20").Value = "ABC123DEF";


            worksheet.Range("A22:C22").Merge();
            worksheet.Range("D22:F22").Merge();
            worksheet.Range("G22:I22").Merge();
            worksheet.Cell("A22").Value = "رئيس الحسابات";
            worksheet.Cell("D22").Value = "الرئيس التنفيذي";
            worksheet.Cell("G22").Value = "العضو المنتدب";


            worksheet.Range("A23:C23").Merge();
            worksheet.Range("D23:F23").Merge();
            worksheet.Range("G23:I23").Merge();
            worksheet.Cell("A23").Value = "Chief Accountant";
            worksheet.Cell("D23").Value = "CEO";
            worksheet.Cell("G23").Value = "MD";


            worksheet.Range("A24:C24").Merge();
            worksheet.Range("D24:F24").Merge();
            worksheet.Range("G24:I24").Merge();
            worksheet.Cell("A24").Value = "أ. أحمد حمدي";
            worksheet.Cell("D24").Value = "د. عبد الفتاح عبد الله";
            worksheet.Cell("G24").Value = "د. علي عيد";

            worksheet.Range("A1:I51").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:I51").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.AddPicture(Directory.GetCurrentDirectory() + "\\wwwroot\\HeaderAndFooter\\Header.png").MoveTo(1, 1);

            return workbook;
        }
    }
}

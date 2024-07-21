using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    public class VisitsCollectionController : ControllerBase
    {
        private readonly IVisitsCollectionRep rep;

        public VisitsCollectionController(IVisitsCollectionRep rep)
        {
            this.rep = rep;
        }



     

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}/{UserId}")]
        [HttpGet]
        public IActionResult AccountByBalanceRepExcel(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string UserId)
        {
            IEnumerable<AccountBalanceModel> res = rep.AccountByBalanceRepExcel(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, UserId);

            XLWorkbook workbook = new XLWorkbook();
            foreach (var item in res)
            {
                string n = item.AccountName;
                if (item.AccountName.Length > 30)
                {
                     n = item.AccountName.Substring(0, 30);
                }
                
                var worksheet = workbook.Worksheets.Add(n);
                worksheet.Range("A1:F300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:F300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Rows(1, 300).Height = 20;
                worksheet.Columns(1, 6).Width = 20;
                worksheet.Range("A3:F3").Style.Font.Bold = true;
                worksheet.Range("A1:F300").Style.Font.FontSize = 16;
                worksheet.Range("A6:B7").Style.Font.Bold = true;
                worksheet.Range("D6:E7").Style.Font.Bold = true;
                
                worksheet.Range("A3:F4").Style.Fill.BackgroundColor = XLColor.FromArgb(197, 217, 241);
                worksheet.Range("C1:D2").Style.Fill.BackgroundColor = XLColor.FromArgb(197, 217, 241);
                worksheet.Range("C1:D2").Style.Border.OutsideBorder = XLBorderStyleValues.Double;
                worksheet.Range("A3:F4").Style.Border.OutsideBorder = XLBorderStyleValues.Double;
                worksheet.Range("C3:D3").Style.Border.TopBorder = XLBorderStyleValues.None;

                var currentRow = 1;
                worksheet.Cell(currentRow, 3).Value = "From";
                worksheet.Cell(currentRow, 4).Value = "To";
                currentRow = 2;
                worksheet.Cell(currentRow, 3).Value = dayfrom.ToString() + "/" + monthfrom.ToString() + "/" + yearfrom.ToString();
                worksheet.Cell(currentRow, 4).Value = dayto.ToString() + "/" + monthto.ToString() + "/" + yearto.ToString();
                currentRow = 3;
                worksheet.Range("C3:D3").Merge();
                worksheet.Range("C4:D4").Merge();
                worksheet.Cell(currentRow, 1).Value = "Account";
                worksheet.Cell(currentRow, 2).Value = "Total Balance";
                worksheet.Cell(currentRow, 3).Value = "Total Collection";
                worksheet.Cell(currentRow, 5).Value = "Credit Limit";
                worksheet.Cell(currentRow, 6).Value = "Credit Residual";
                currentRow = 4;
                worksheet.Cell(currentRow, 1).Value = item.AccountName;
                worksheet.Cell(currentRow, 2).Value = item.TotalBalance;
                worksheet.Cell(currentRow, 3).Value = item.TotalCollection;
                worksheet.Cell(currentRow, 5).Value = item.CreditLimit;
                worksheet.Cell(currentRow, 6).Value = item.Residual;
                currentRow = 6;
                worksheet.Range("A6:B6").Merge();
                worksheet.Range("D6:E6").Merge();
                worksheet.Cell("A6").Value = "Balance";
                worksheet.Cell("D6").Value = "Collection";
                currentRow = 7;
                worksheet.Cell(currentRow, 1).Value = "Brand";
                worksheet.Cell(currentRow, 2).Value = "Balance";
                worksheet.Cell(currentRow, 4).Value = "Brand";
                worksheet.Cell(currentRow, 5).Value = "Collection";
                    currentRow = 8;
                foreach (var bal in item.BalanceByBrand)
                {
                        worksheet.Cell(currentRow, 1).Value = bal.BrandName;
                        worksheet.Cell(currentRow, 2).Value = bal.Balance;
                        currentRow++;
                }
                var r = currentRow - 1;
                worksheet.Range("A6:B"+r.ToString()).Style.Fill.BackgroundColor = XLColor.FromArgb(242, 220, 219);
                currentRow = 8;
                
                

                foreach (var col in item.CollectionByBrand)
                {
                        worksheet.Cell(currentRow, 4).Value = col.BrandName;
                        worksheet.Cell(currentRow, 5).Value = col.ActualCollection;
                        currentRow++;
                }
                var s = currentRow - 1;
                worksheet.Range("D6:E"+s.ToString()).Style.Fill.BackgroundColor = XLColor.FromArgb(242, 220, 219);

            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Collection Report From.xlsx");
        }
        
        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}/{AccountId}")]
        [HttpGet]
        public IActionResult AccountBalanceAccountExcel(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, int AccountId)
        {
            AccountBalanceModel item = rep.AccountBalanceAccount(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, AccountId);

            XLWorkbook workbook = new XLWorkbook();
         
                string n = item.AccountName;
                if (item.AccountName.Length > 30)
                {
                    n = item.AccountName.Substring(0, 30);
                }

                var worksheet = workbook.Worksheets.Add(n);
                worksheet.Range("A1:F300").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A1:F300").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Rows(1, 300).Height = 20;
                worksheet.Columns(1, 6).Width = 20;
                worksheet.Range("A3:F3").Style.Font.Bold = true;
                worksheet.Range("A1:F300").Style.Font.FontSize = 16;
                worksheet.Range("A6:B7").Style.Font.Bold = true;
                worksheet.Range("D6:E7").Style.Font.Bold = true;

                worksheet.Range("A3:F4").Style.Fill.BackgroundColor = XLColor.FromArgb(197, 217, 241);
                worksheet.Range("C1:D2").Style.Fill.BackgroundColor = XLColor.FromArgb(197, 217, 241);
                worksheet.Range("C1:D2").Style.Border.OutsideBorder = XLBorderStyleValues.Double;
                worksheet.Range("A3:F4").Style.Border.OutsideBorder = XLBorderStyleValues.Double;
                worksheet.Range("C3:D3").Style.Border.TopBorder = XLBorderStyleValues.None;

                var currentRow = 1;
                worksheet.Cell(currentRow, 3).Value = "From";
                worksheet.Cell(currentRow, 4).Value = "To";
                currentRow = 2;
                worksheet.Cell(currentRow, 3).Value = dayfrom.ToString() + "/" + monthfrom.ToString() + "/" + yearfrom.ToString();
                worksheet.Cell(currentRow, 4).Value = dayto.ToString() + "/" + monthto.ToString() + "/" + yearto.ToString();
                currentRow = 3;
                worksheet.Range("C3:D3").Merge();
                worksheet.Range("C4:D4").Merge();
                worksheet.Cell(currentRow, 1).Value = "Account";
                worksheet.Cell(currentRow, 2).Value = "Total Balance";
                worksheet.Cell(currentRow, 3).Value = "Total Collection";
                worksheet.Cell(currentRow, 5).Value = "Credit Limit";
                worksheet.Cell(currentRow, 6).Value = "Credit Residual";
                currentRow = 4;
                worksheet.Cell(currentRow, 1).Value = item.AccountName;
                worksheet.Cell(currentRow, 2).Value = item.TotalBalance;
                worksheet.Cell(currentRow, 3).Value = item.TotalCollection;
                worksheet.Cell(currentRow, 5).Value = item.CreditLimit;
                worksheet.Cell(currentRow, 6).Value = item.Residual;
                currentRow = 6;
                worksheet.Range("A6:B6").Merge();
                worksheet.Range("D6:E6").Merge();
                worksheet.Cell("A6").Value = "Balance";
                worksheet.Cell("D6").Value = "Collection";
                currentRow = 7;
                worksheet.Cell(currentRow, 1).Value = "Brand";
                worksheet.Cell(currentRow, 2).Value = "Balance";
                worksheet.Cell(currentRow, 4).Value = "Brand";
                worksheet.Cell(currentRow, 5).Value = "Collection";
                currentRow = 8;
                foreach (var bal in item.BalanceByBrand)
                {
                    worksheet.Cell(currentRow, 1).Value = bal.BrandName;
                    worksheet.Cell(currentRow, 2).Value = bal.Balance;
                    currentRow++;
                }
                var r = currentRow - 1;
                worksheet.Range("A6:B" + r.ToString()).Style.Fill.BackgroundColor = XLColor.FromArgb(242, 220, 219);
                currentRow = 8;



                foreach (var col in item.CollectionByBrand)
                {
                    worksheet.Cell(currentRow, 4).Value = col.BrandName;
                    worksheet.Cell(currentRow, 5).Value = col.ActualCollection;
                    currentRow++;
                }
                var s = currentRow - 1;
                worksheet.Range("D6:E" + s.ToString()).Style.Fill.BackgroundColor = XLColor.FromArgb(242, 220, 219);

          

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Collection Report From.xlsx");
        }


        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}")]
        [HttpPost]
        public IActionResult AccountByBalanceFirstLineManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, SearchByWord s)
        {
            return Ok(rep.AccountByBalanceFirstLineManager(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, s));
        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}")]
        [HttpPost]
        public IActionResult AccountByBalanceTopManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, SearchByWord s)
        {
            return Ok(rep.AccountByBalanceTopManager(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto,s));
        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}/{UserId}")]
        [HttpGet]
        public IActionResult AccountByBalanceRep(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string UserId)
        {
            return Ok(rep.AccountByBalanceRep(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, UserId));
        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}/{AccountId}")]
        [HttpGet]
        public IActionResult AccountBalanceAccount(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, int AccountId)
        {
            return Ok(rep.AccountBalanceAccount(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, AccountId));
        }


        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}/{ManagerId}")]
        [HttpGet]
        public IActionResult CollectionFirstLineManagerExcel(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string ManagerId)
        {

            string start = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd/MM/yyyy");
            string end = new DateTime(yearto, monthto, dayto).ToString("dd/MM/yyyy");

            IEnumerable<CollectionByBrandModel> brands = rep.CollectionByBrandFirstLineManager(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, ManagerId);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Total");

            worksheet.Column("A").Width = 32;
            worksheet.Column("B").Width = 32;
            worksheet.Column("C").Width = 32;
            worksheet.Column("D").Width = 32;
            worksheet.Column("E").Width = 32;
            worksheet.Rows("1:30").Height = 25;
            worksheet.Range("A1:E10").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:E10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A1:C1").Style.Font.Bold = true;
         
        


            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Brand";
            worksheet.Cell(currentRow, 2).Value = "Planned Collection";
            worksheet.Cell(currentRow, 3).Value = "Actual Collection";
          
          
            float? planned = 0;
            float? actual = 0;

            currentRow = 2;
            foreach (var brand in brands)
            {
                worksheet.Cell(currentRow, 1).Value = brand.BrandName;
                worksheet.Cell(currentRow, 2).Value = brand.PlannedCollection;
                worksheet.Cell(currentRow, 3).Value = brand.ActualCollection;
                planned = planned + brand.PlannedCollection;
                actual = actual + brand.ActualCollection;

                currentRow++;
            }
            worksheet.Cell("A" + currentRow).Value = "Total";
            worksheet.Cell("B" + currentRow).Value = planned;
            worksheet.Cell("C" + currentRow).Value = actual;
            worksheet.Range("A"+currentRow,"C"+currentRow).Style.Border.TopBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A" + currentRow, "C" + currentRow).Style.Font.Bold = true;
            worksheet.Range("A" + currentRow, "C" + currentRow).Style.Font.FontSize = 16;



        

            IEnumerable<CollectionByRepModel> sales = rep.CollectionByRepFirstLineManager(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, ManagerId);

            foreach (var item in sales)
            {

               IEnumerable<BrandAccountCollectionModel> BrandAccount = rep.BrandAccountCollectionByRep(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, item.UserId);
                var repsheet = workbook.Worksheets.Add(item.FullName);

                repsheet.Column("A").Width = 32;
                repsheet.Column("B").Width = 32;
                repsheet.Column("C").Width = 32;
                repsheet.Column("D").Width = 32;
                repsheet.Column("E").Width = 32;
                repsheet.Column("F").Width = 32;
                repsheet.Column("G").Width = 32;
                repsheet.Column("H").Width = 32;
                repsheet.Rows("1:500").Height = 25;
                repsheet.Rows("1:3").Height = 40;

                repsheet.Range("A1:J100").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                repsheet.Range("A1:J100").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                repsheet.Range("A1:H4").Style.Font.Bold = true;
                repsheet.Range("D4:D500").Style.Border.RightBorder = XLBorderStyleValues.Double;
                repsheet.Range("A3:Z3").Style.Border.BottomBorder = XLBorderStyleValues.Double;
              
                repsheet.Range("A1:G3").Style.Font.FontSize = 22;
                repsheet.Range("A4:D4").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
                repsheet.Range("E4:G4").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);

                repsheet.Range("A1", "G1").Merge();
                repsheet.Range("A2", "D2").Merge();
                repsheet.Range("A3", "D3").Merge();
                repsheet.Range("E2", "G2").Merge();
                repsheet.Range("E3", "G3").Merge();
                repsheet.Cell("A1").Value = item.FullName;
                repsheet.Cell("A2").Value = "Planned Collection";
                repsheet.Cell("E2").Value = item.PlannedTotal;
                repsheet.Cell("A3").Value = "Actual Collection";
                repsheet.Cell("E3").Value = item.ActualTotal;
                var Row = 4;
                repsheet.Cell(Row, 1).Value = "Account";
                repsheet.Cell(Row, 2).Value = "Brand";
                repsheet.Cell(Row, 3).Value = "Planned Collection";
                repsheet.Cell(Row, 4).Value = "Actual Collection";

                repsheet.Cell(Row, 5).Value = "Brand";
                repsheet.Cell(Row, 6).Value = "Planned Collection";
                repsheet.Cell(Row, 7).Value = "Actual Collection";


                Row = 5;

                foreach (var br in item.list)
                {
                    repsheet.Cell(Row, 5).Value = br.BrandName;
                    repsheet.Cell(Row, 6).Value = br.PlannedCollection;
                    repsheet.Cell(Row, 7).Value = br.ActualCollection;
                    Row++;
                }
                repsheet.Range("E" + Row, "G" + Row).Style.Border.TopBorder = XLBorderStyleValues.Medium;
                repsheet.Cell("E" + Row).Value = "Total";
                repsheet.Cell("F" + Row).Value = item.PlannedTotal;
                repsheet.Cell("G" + Row).Value = item.ActualTotal;
                repsheet.Range("E"+Row,"G"+Row).Style.Font.Bold = true;
                repsheet.Range("E" + Row, "G" + Row).Style.Font.FontSize = 16;
                Row = 5;
                foreach (var ac in BrandAccount)
                {
                    repsheet.Cell(Row, 1).Value = ac.AccountName;
                    repsheet.Cell(Row, 2).Value = ac.BrandName;
                    repsheet.Cell(Row, 3).Value = ac.PlannedCollection;
                    repsheet.Cell(Row, 4).Value = ac.ActualCollection;
                    Row++;
                }
                repsheet.Range("A" + Row, "D" + Row).Style.Border.TopBorder = XLBorderStyleValues.Medium;
                repsheet.Range("A" + Row, "B" + Row).Merge();
                repsheet.Cell("A" + Row).Value = "Total";
                repsheet.Cell("C" + Row).Value = item.PlannedTotal;
                repsheet.Cell("D" + Row).Value = item.ActualTotal;
                repsheet.Range("A" + Row, "D" + Row).Style.Font.Bold = true;
                repsheet.Range("A" + Row, "D" + Row).Style.Font.FontSize = 16;

            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Collection Report From "+start+" To "+end+".xlsx");


        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}/{ManagerId}")]
        [HttpGet]
        public IActionResult CollectionFLMExcel(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string ManagerId)
        {

            string start = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd/MM/yyyy");
            string end = new DateTime(yearto, monthto, dayto).ToString("dd/MM/yyyy");

            IEnumerable<CollectionByBrandModel> brands = rep.CollectionByBrandFLM(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, ManagerId);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Total");

            worksheet.Column("A").Width = 32;
            worksheet.Column("B").Width = 32;
            worksheet.Column("C").Width = 32;
            worksheet.Column("D").Width = 32;
            worksheet.Column("E").Width = 32;
            worksheet.Rows("1:30").Height = 25;
            worksheet.Range("A1:E10").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:E10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A1:C1").Style.Font.Bold = true;




            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Brand";
            worksheet.Cell(currentRow, 2).Value = "Planned Collection";
            worksheet.Cell(currentRow, 3).Value = "Actual Collection";


            float? planned = 0;
            float? actual = 0;

            currentRow = 2;
            foreach (var brand in brands)
            {
                worksheet.Cell(currentRow, 1).Value = brand.BrandName;
                worksheet.Cell(currentRow, 2).Value = brand.PlannedCollection;
                worksheet.Cell(currentRow, 3).Value = brand.ActualCollection;
                planned = planned + brand.PlannedCollection;
                actual = actual + brand.ActualCollection;

                currentRow++;
            }
            worksheet.Cell("A" + currentRow).Value = "Total";
            worksheet.Cell("B" + currentRow).Value = planned;
            worksheet.Cell("C" + currentRow).Value = actual;
            worksheet.Range("A" + currentRow, "C" + currentRow).Style.Border.TopBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A" + currentRow, "C" + currentRow).Style.Font.Bold = true;
            worksheet.Range("A" + currentRow, "C" + currentRow).Style.Font.FontSize = 16;





            IEnumerable<CollectionByRepModel> sales = rep.CollectionByRepFLM(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, ManagerId);

            foreach (var item in sales)
            {

                IEnumerable<BrandAccountCollectionModel> BrandAccount = rep.BrandAccountCollectionByRepFLM(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, item.UserId, ManagerId);
                var repsheet = workbook.Worksheets.Add(item.FullName);

                repsheet.Column("A").Width = 32;
                repsheet.Column("B").Width = 32;
                repsheet.Column("C").Width = 32;
                repsheet.Column("D").Width = 32;
                repsheet.Column("E").Width = 32;
                repsheet.Column("F").Width = 32;
                repsheet.Column("G").Width = 32;
                repsheet.Column("H").Width = 32;
                repsheet.Rows("1:500").Height = 25;
                repsheet.Rows("1:3").Height = 40;

                repsheet.Range("A1:J100").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                repsheet.Range("A1:J100").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                repsheet.Range("A1:H4").Style.Font.Bold = true;
                repsheet.Range("D4:D500").Style.Border.RightBorder = XLBorderStyleValues.Double;
                repsheet.Range("A3:Z3").Style.Border.BottomBorder = XLBorderStyleValues.Double;

                repsheet.Range("A1:G3").Style.Font.FontSize = 22;
                repsheet.Range("A4:D4").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
                repsheet.Range("E4:G4").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);

                repsheet.Range("A1", "G1").Merge();
                repsheet.Range("A2", "D2").Merge();
                repsheet.Range("A3", "D3").Merge();
                repsheet.Range("E2", "G2").Merge();
                repsheet.Range("E3", "G3").Merge();
                repsheet.Cell("A1").Value = item.FullName;
                repsheet.Cell("A2").Value = "Planned Collection";
                repsheet.Cell("E2").Value = item.PlannedTotal;
                repsheet.Cell("A3").Value = "Actual Collection";
                repsheet.Cell("E3").Value = item.ActualTotal;
                var Row = 4;
                repsheet.Cell(Row, 1).Value = "Account";
                repsheet.Cell(Row, 2).Value = "Brand";
                repsheet.Cell(Row, 3).Value = "Planned Collection";
                repsheet.Cell(Row, 4).Value = "Actual Collection";

                repsheet.Cell(Row, 5).Value = "Brand";
                repsheet.Cell(Row, 6).Value = "Planned Collection";
                repsheet.Cell(Row, 7).Value = "Actual Collection";


                Row = 5;

                foreach (var br in item.list)
                {
                    repsheet.Cell(Row, 5).Value = br.BrandName;
                    repsheet.Cell(Row, 6).Value = br.PlannedCollection;
                    repsheet.Cell(Row, 7).Value = br.ActualCollection;
                    Row++;
                }
                repsheet.Range("E" + Row, "G" + Row).Style.Border.TopBorder = XLBorderStyleValues.Medium;
                repsheet.Cell("E" + Row).Value = "Total";
                repsheet.Cell("F" + Row).Value = item.PlannedTotal;
                repsheet.Cell("G" + Row).Value = item.ActualTotal;
                repsheet.Range("E" + Row, "G" + Row).Style.Font.Bold = true;
                repsheet.Range("E" + Row, "G" + Row).Style.Font.FontSize = 16;
                Row = 5;
                foreach (var ac in BrandAccount)
                {
                    repsheet.Cell(Row, 1).Value = ac.AccountName;
                    repsheet.Cell(Row, 2).Value = ac.BrandName;
                    repsheet.Cell(Row, 3).Value = ac.PlannedCollection;
                    repsheet.Cell(Row, 4).Value = ac.ActualCollection;
                    Row++;
                }
                repsheet.Range("A" + Row, "D" + Row).Style.Border.TopBorder = XLBorderStyleValues.Medium;
                repsheet.Range("A" + Row, "B" + Row).Merge();
                repsheet.Cell("A" + Row).Value = "Total";
                repsheet.Cell("C" + Row).Value = item.PlannedTotal;
                repsheet.Cell("D" + Row).Value = item.ActualTotal;
                repsheet.Range("A" + Row, "D" + Row).Style.Font.Bold = true;
                repsheet.Range("A" + Row, "D" + Row).Style.Font.FontSize = 16;

            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Collection Report From " + start + " To " + end + ".xlsx");


        }


        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}")]
        [HttpGet]
        public IActionResult CollectionTopManagerExcel(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto)
        {

            string start = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd/MM/yyyy");
            string end = new DateTime(yearto, monthto, dayto).ToString("dd/MM/yyyy");

            List<CollectionByBrandModel> brands = rep.CollectionByBrandTopManager(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto);

            XLWorkbook workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Total");

            worksheet.Column("A").Width = 32;
            worksheet.Column("B").Width = 32;
            worksheet.Column("C").Width = 32;
            worksheet.Column("D").Width = 32;
            worksheet.Column("E").Width = 32;
            worksheet.Rows("1:30").Height = 25;
            worksheet.Range("A1:E10").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("A1:E10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("A1:C1").Style.Font.Bold = true;




            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Brand";
            worksheet.Cell(currentRow, 2).Value = "Planned Collection";
            worksheet.Cell(currentRow, 3).Value = "Actual Collection";


            float? planned = 0;
            float? actual = 0;

            currentRow = 2;
            foreach (var brand in brands)
            {
                worksheet.Cell(currentRow, 1).Value = brand.BrandName;
                worksheet.Cell(currentRow, 2).Value = brand.PlannedCollection;
                worksheet.Cell(currentRow, 3).Value = brand.ActualCollection;
                planned = planned + brand.PlannedCollection;
                actual = actual + brand.ActualCollection;

                currentRow++;
            }
            worksheet.Cell("A" + currentRow).Value = "Total";
            worksheet.Cell("B" + currentRow).Value = planned;
            worksheet.Cell("C" + currentRow).Value = actual;
            worksheet.Range("A" + currentRow, "C" + currentRow).Style.Border.TopBorder = XLBorderStyleValues.Medium;
            worksheet.Range("A" + currentRow, "C" + currentRow).Style.Font.Bold = true;
            worksheet.Range("A" + currentRow, "C" + currentRow).Style.Font.FontSize = 16;





            List<CollectionByRepModel> sales = rep.CollectionByRepTopManager(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto);

            foreach (var item in sales)
            {

                 List<BrandAccountCollectionModel> BrandAccount = rep.BrandAccountCollectionByRep(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, item.UserId);
                var repsheet = workbook.Worksheets.Add(item.FullName);

                repsheet.Column("A").Width = 32;
                repsheet.Column("B").Width = 32;
                repsheet.Column("C").Width = 32;
                repsheet.Column("D").Width = 32;
                repsheet.Column("E").Width = 32;
                repsheet.Column("F").Width = 32;
                repsheet.Column("G").Width = 32;
                repsheet.Column("H").Width = 32;
                repsheet.Rows("1:500").Height = 25;
                repsheet.Rows("1:3").Height = 40;

                repsheet.Range("A1:J100").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                repsheet.Range("A1:J100").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                repsheet.Range("A1:H4").Style.Font.Bold = true;
                repsheet.Range("D4:D500").Style.Border.RightBorder = XLBorderStyleValues.Double;
                repsheet.Range("A3:Z3").Style.Border.BottomBorder = XLBorderStyleValues.Double;

                repsheet.Range("A1:G3").Style.Font.FontSize = 22;
                repsheet.Range("A4:D4").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
                repsheet.Range("E4:G4").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);

                repsheet.Range("A1", "G1").Merge();
                repsheet.Range("A2", "D2").Merge();
                repsheet.Range("A3", "D3").Merge();
                repsheet.Range("E2", "G2").Merge();
                repsheet.Range("E3", "G3").Merge();
                repsheet.Cell("A1").Value = item.FullName;
                repsheet.Cell("A2").Value = "Planned Collection";
                repsheet.Cell("E2").Value = item.PlannedTotal;
                repsheet.Cell("A3").Value = "Actual Collection";
                repsheet.Cell("E3").Value = item.ActualTotal;
                var Row = 4;
                repsheet.Cell(Row, 1).Value = "Account";
                repsheet.Cell(Row, 2).Value = "Brand";
                repsheet.Cell(Row, 3).Value = "Planned Collection";
                repsheet.Cell(Row, 4).Value = "Actual Collection";

                repsheet.Cell(Row, 5).Value = "Brand";
                repsheet.Cell(Row, 6).Value = "Planned Collection";
                repsheet.Cell(Row, 7).Value = "Actual Collection";


                Row = 5;

                foreach (var br in item.list)
                {
                    repsheet.Cell(Row, 5).Value = br.BrandName;
                    repsheet.Cell(Row, 6).Value = br.PlannedCollection;
                    repsheet.Cell(Row, 7).Value = br.ActualCollection;
                    Row++;
                }
                repsheet.Range("E" + Row, "G" + Row).Style.Border.TopBorder = XLBorderStyleValues.Medium;
                repsheet.Cell("E" + Row).Value = "Total";
                repsheet.Cell("F" + Row).Value = item.PlannedTotal;
                repsheet.Cell("G" + Row).Value = item.ActualTotal;
                repsheet.Range("E" + Row, "G" + Row).Style.Font.Bold = true;
                repsheet.Range("E" + Row, "G" + Row).Style.Font.FontSize = 16;
                Row = 5;
                foreach (var ac in BrandAccount)
                {
                    repsheet.Cell(Row, 1).Value = ac.AccountName;
                    repsheet.Cell(Row, 2).Value = ac.BrandName;
                    repsheet.Cell(Row, 3).Value = ac.PlannedCollection;
                    repsheet.Cell(Row, 4).Value = ac.ActualCollection;
                    Row++;
                }
                repsheet.Range("A" + Row, "D" + Row).Style.Border.TopBorder = XLBorderStyleValues.Medium;
                repsheet.Range("A" + Row, "B" + Row).Merge();
                repsheet.Cell("A" + Row).Value = "Total";
                repsheet.Cell("C" + Row).Value = item.PlannedTotal;
                repsheet.Cell("D" + Row).Value = item.ActualTotal;
                repsheet.Range("A" + Row, "D" + Row).Style.Font.Bold = true;
                repsheet.Range("A" + Row, "D" + Row).Style.Font.FontSize = 16;

            }

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Collection Report From " + start + " To " + end + ".xlsx");


        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}/{userId}")]
        [HttpGet]
        public IActionResult CollectionExcel(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string userId)
        {

            string start = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd/MM/yyyy");
            string end = new DateTime(yearto, monthto, dayto).ToString("dd/MM/yyyy");

            

            XLWorkbook workbook = new XLWorkbook();


            CollectionByRepModel sales = rep.CollectionByRep(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, userId);

      

                IEnumerable<BrandAccountCollectionModel> BrandAccount = rep.BrandAccountCollectionByRep(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, userId);
                var repsheet = workbook.Worksheets.Add(sales.FullName);

                repsheet.Column("A").Width = 32;
                repsheet.Column("B").Width = 32;
                repsheet.Column("C").Width = 32;
                repsheet.Column("D").Width = 32;
                repsheet.Column("E").Width = 32;
                repsheet.Column("F").Width = 32;
                repsheet.Column("G").Width = 32;
                repsheet.Column("H").Width = 32;
                repsheet.Rows("1:500").Height = 25;
                repsheet.Rows("1:3").Height = 40;

                repsheet.Range("A1:J100").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                repsheet.Range("A1:J100").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                repsheet.Range("A1:H4").Style.Font.Bold = true;
                repsheet.Range("D4:D500").Style.Border.RightBorder = XLBorderStyleValues.Double;
                repsheet.Range("A3:Z3").Style.Border.BottomBorder = XLBorderStyleValues.Double;

                repsheet.Range("A1:G3").Style.Font.FontSize = 22;
                repsheet.Range("A4:D4").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
                repsheet.Range("E4:G4").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);

                repsheet.Range("A1", "G1").Merge();
                repsheet.Range("A2", "D2").Merge();
                repsheet.Range("A3", "D3").Merge();
                repsheet.Range("E2", "G2").Merge();
                repsheet.Range("E3", "G3").Merge();
                repsheet.Cell("A1").Value = sales.FullName;
                repsheet.Cell("A2").Value = "Planned Collection";
                repsheet.Cell("E2").Value = sales.PlannedTotal;
                repsheet.Cell("A3").Value = "Actual Collection";
                repsheet.Cell("E3").Value = sales.ActualTotal;
                var Row = 4;
                repsheet.Cell(Row, 1).Value = "Account";
                repsheet.Cell(Row, 2).Value = "Brand";
                repsheet.Cell(Row, 3).Value = "Planned Collection";
                repsheet.Cell(Row, 4).Value = "Actual Collection";

                repsheet.Cell(Row, 5).Value = "Brand";
                repsheet.Cell(Row, 6).Value = "Planned Collection";
                repsheet.Cell(Row, 7).Value = "Actual Collection";


                Row = 5;

                foreach (var br in sales.list)
                {
                    repsheet.Cell(Row, 5).Value = br.BrandName;
                    repsheet.Cell(Row, 6).Value = br.PlannedCollection;
                    repsheet.Cell(Row, 7).Value = br.ActualCollection;
                    Row++;
                }
                repsheet.Range("E" + Row, "G" + Row).Style.Border.TopBorder = XLBorderStyleValues.Medium;
                repsheet.Cell("E" + Row).Value = "Total";
                repsheet.Cell("F" + Row).Value = sales.PlannedTotal;
                repsheet.Cell("G" + Row).Value = sales.ActualTotal;
                repsheet.Range("E" + Row, "G" + Row).Style.Font.Bold = true;
                repsheet.Range("E" + Row, "G" + Row).Style.Font.FontSize = 16;
                Row = 5;
                foreach (var ac in BrandAccount)
                {
                    repsheet.Cell(Row, 1).Value = ac.AccountName;
                    repsheet.Cell(Row, 2).Value = ac.BrandName;
                    repsheet.Cell(Row, 3).Value = ac.PlannedCollection;
                    repsheet.Cell(Row, 4).Value = ac.ActualCollection;
                    Row++;
                }
                repsheet.Range("A" + Row, "D" + Row).Style.Border.TopBorder = XLBorderStyleValues.Medium;
                repsheet.Range("A" + Row, "B" + Row).Merge();
                repsheet.Cell("A" + Row).Value = "Total";
                repsheet.Cell("C" + Row).Value = sales.PlannedTotal;
                repsheet.Cell("D" + Row).Value = sales.ActualTotal;
                repsheet.Range("A" + Row, "D" + Row).Style.Font.Bold = true;
                repsheet.Range("A" + Row, "D" + Row).Style.Font.FontSize = 16;

       

            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Collection Report From " + start + " To " + end + ".xlsx");


        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}/{userId}/{ManagerId}")]
        [HttpGet]
        public IActionResult CollectionExcelFLM(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string userId, string ManagerId)
        {

            string start = new DateTime(yearfrom, monthfrom, dayfrom).ToString("dd/MM/yyyy");
            string end = new DateTime(yearto, monthto, dayto).ToString("dd/MM/yyyy");



            XLWorkbook workbook = new XLWorkbook();


            CollectionByRepModel sales = rep.CollectionByRepFLMFLM(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, userId, ManagerId);



            IEnumerable<BrandAccountCollectionModel> BrandAccount = rep.BrandAccountCollectionByRep(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, userId);
            var repsheet = workbook.Worksheets.Add(sales.FullName);

            repsheet.Column("A").Width = 32;
            repsheet.Column("B").Width = 32;
            repsheet.Column("C").Width = 32;
            repsheet.Column("D").Width = 32;
            repsheet.Column("E").Width = 32;
            repsheet.Column("F").Width = 32;
            repsheet.Column("G").Width = 32;
            repsheet.Column("H").Width = 32;
            repsheet.Rows("1:500").Height = 25;
            repsheet.Rows("1:3").Height = 40;

            repsheet.Range("A1:J100").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            repsheet.Range("A1:J100").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            repsheet.Range("A1:H4").Style.Font.Bold = true;
            repsheet.Range("D4:D500").Style.Border.RightBorder = XLBorderStyleValues.Double;
            repsheet.Range("A3:Z3").Style.Border.BottomBorder = XLBorderStyleValues.Double;

            repsheet.Range("A1:G3").Style.Font.FontSize = 22;
            repsheet.Range("A4:D4").Style.Fill.BackgroundColor = XLColor.FromArgb(141, 180, 226);
            repsheet.Range("E4:G4").Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);

            repsheet.Range("A1", "G1").Merge();
            repsheet.Range("A2", "D2").Merge();
            repsheet.Range("A3", "D3").Merge();
            repsheet.Range("E2", "G2").Merge();
            repsheet.Range("E3", "G3").Merge();
            repsheet.Cell("A1").Value = sales.FullName;
            repsheet.Cell("A2").Value = "Planned Collection";
            repsheet.Cell("E2").Value = sales.PlannedTotal;
            repsheet.Cell("A3").Value = "Actual Collection";
            repsheet.Cell("E3").Value = sales.ActualTotal;
            var Row = 4;
            repsheet.Cell(Row, 1).Value = "Account";
            repsheet.Cell(Row, 2).Value = "Brand";
            repsheet.Cell(Row, 3).Value = "Planned Collection";
            repsheet.Cell(Row, 4).Value = "Actual Collection";

            repsheet.Cell(Row, 5).Value = "Brand";
            repsheet.Cell(Row, 6).Value = "Planned Collection";
            repsheet.Cell(Row, 7).Value = "Actual Collection";


            Row = 5;

            foreach (var br in sales.list)
            {
                repsheet.Cell(Row, 5).Value = br.BrandName;
                repsheet.Cell(Row, 6).Value = br.PlannedCollection;
                repsheet.Cell(Row, 7).Value = br.ActualCollection;
                Row++;
            }
            repsheet.Range("E" + Row, "G" + Row).Style.Border.TopBorder = XLBorderStyleValues.Medium;
            repsheet.Cell("E" + Row).Value = "Total";
            repsheet.Cell("F" + Row).Value = sales.PlannedTotal;
            repsheet.Cell("G" + Row).Value = sales.ActualTotal;
            repsheet.Range("E" + Row, "G" + Row).Style.Font.Bold = true;
            repsheet.Range("E" + Row, "G" + Row).Style.Font.FontSize = 16;
            Row = 5;
            foreach (var ac in BrandAccount)
            {
                repsheet.Cell(Row, 1).Value = ac.AccountName;
                repsheet.Cell(Row, 2).Value = ac.BrandName;
                repsheet.Cell(Row, 3).Value = ac.PlannedCollection;
                repsheet.Cell(Row, 4).Value = ac.ActualCollection;
                Row++;
            }
            repsheet.Range("A" + Row, "D" + Row).Style.Border.TopBorder = XLBorderStyleValues.Medium;
            repsheet.Range("A" + Row, "B" + Row).Merge();
            repsheet.Cell("A" + Row).Value = "Total";
            repsheet.Cell("C" + Row).Value = sales.PlannedTotal;
            repsheet.Cell("D" + Row).Value = sales.ActualTotal;
            repsheet.Range("A" + Row, "D" + Row).Style.Font.Bold = true;
            repsheet.Range("A" + Row, "D" + Row).Style.Font.FontSize = 16;



            MemoryStream stream = new MemoryStream();


            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Collection Report From " + start + " To " + end + ".xlsx");


        }


        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}/{userId}")]
        [HttpGet]
        public IActionResult Collection(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string userId)
        {
            return Ok( rep.CollectionByRep(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, userId) );
        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}/{userId}/{managerId}")]
        [HttpGet]
        public IActionResult CollectionFLM(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string userId, string managerId)
        {
            return Ok(rep.CollectionByRepFLMFLM(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, userId, managerId));
        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}/{ManagerId}")]
        [HttpGet]
        public IActionResult CollectionFirstLineManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string ManagerId)
        {
            return Ok(rep.CollectionByRepFirstLineManager(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, ManagerId));
        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}/{ManagerId}")]
        [HttpGet]
        public IActionResult CollectionByRepFLM(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto, string ManagerId)
        {
            return Ok(rep.CollectionByRepFLM(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto, ManagerId));
        }

        [Route("[controller]/[Action]/{yearfrom}/{monthfrom}/{dayfrom}/{yearto}/{monthto}/{dayto}")]
        [HttpGet]
        public IActionResult CollectionTopManager(int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto)
        {
            return Ok(rep.CollectionByRepTopManager(yearfrom, monthfrom, dayfrom, yearto, monthto, dayto));
        }
    }
}

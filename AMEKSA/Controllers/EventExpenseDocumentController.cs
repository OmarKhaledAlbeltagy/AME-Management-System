using AMEKSA.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class EventExpenseDocumentController : ControllerBase
    {
        private readonly IEventExpenseDocumentRep rep;

        public EventExpenseDocumentController(IEventExpenseDocumentRep rep)
        {
            this.rep = rep;
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult tryy()
        {
           var res = rep.AddDocument();

            MemoryStream stream = new MemoryStream();
            res.SaveAs(stream);
            var content = stream.ToArray();

            return File(
            content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Events Totals.xlsx");

        }
    }
}

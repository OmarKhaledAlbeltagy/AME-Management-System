using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Models;
using AMEKSA.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class AccountSupportiveVisitController : ControllerBase
    {
        private readonly IAccountSupportiveVisitRep rep;

        public AccountSupportiveVisitController(IAccountSupportiveVisitRep rep)
        {
            this.rep = rep;
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult MakeVisit(AccountSupportiveVisitModel obj)
        {
            return Ok(rep.MakeVisit(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetMyVisitsByDate(AccountSalesVisitByDateModel obj)
        {
            return Ok(rep.GetMyVisitsByDate(obj));
        }

        
        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetVisitById(int id)
        {
            return Ok(rep.GetVisitById(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult RequestDeleteAccountSupportive(int id)
        {
            return Ok(rep.RequestDeleteAccountSupportive(id));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAccountSupportiveDeleteRequests()
        {
            return Ok(rep.GetAccountSupportiveDeleteRequests());
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult ConfirmAccountSupportiveVDeleting(int id)
        {
            return Ok(rep.ConfirmAccountSupportiveVDeleting(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult RejectAccountSupportiveVDeleting(int id)
        {
            return Ok(rep.RejectAccountSupportiveVDeleting(id));
        }
    }
}

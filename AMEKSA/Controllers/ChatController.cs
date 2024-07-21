using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Entities;
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
    public class ChatController : ControllerBase
    {
        private readonly IChatRep rep;

        public ChatController(IChatRep rep)
        {
            this.rep = rep;
        }

        
        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AccountSupportiveManagerComment(AccountSupportiveVisitChat obj)
        {
            return Ok(rep.AccountSupportiveManagerComment(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AccountMedicalManagerComment(AccountMedicalVisitChat obj)
        {
            return Ok(rep.AccountMedicalManagerComment(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AccountSalesManagerComment(AccountSalesVisitChat obj)
        {
            return Ok(rep.AccountSalesManagerComment(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult ContactMedicalManagerComment(ContactMedicalVisitChat obj)
        {
            return Ok(rep.ContactMedicalManagerComment(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetCommentsByDateRep(SearchByDate obj)
        {
            return Ok(rep.GetCommentsByDateRep(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AccountMedicalRepReply(RepReplyModel obj)
        {
            return Ok(rep.AccountMedicalRepReply(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AccountSupportiveRepReply(RepReplyModel obj)
        {
            return Ok(rep.AccountSupportiveRepReply(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AccountSalesRepReply(RepReplyModel obj)
        {
            return Ok(rep.AccountSalesRepReply(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult ContactMedicalRepReply(RepReplyModel obj)
        {
            return Ok(rep.ContactMedicalRepReply(obj));
        }
    }
}

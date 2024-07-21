using AMEKSA.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AMEKSA.Models;
using AMEKSA.Entities;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class ContactChangeTargetController : ControllerBase
    {
        private readonly IChangeContactTargetRep rep;

        public ContactChangeTargetController(IChangeContactTargetRep rep)
        {
            this.rep = rep;
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult AcceptRequest(int id)
        {
            return Ok(rep.AcceptRequest(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult RejectRequest(int id)
        {
            return Ok(rep.RejectRequest(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetMyTeamRequests(string id)
        {
            return Ok(rep.GetMyTeamRequests(id));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult ChangeTarget(ContactChangeTargetModel obj)
        {
            return Ok(rep.ChangeTarget(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult RequestChangeTarget(RequestChangeContactTarget obj)
        {
            return Ok(rep.RequestChangeTarget(obj));
        }
    }
}

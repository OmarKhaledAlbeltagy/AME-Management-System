using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class TimeOffRequestController : ControllerBase
    {
        private readonly ITimeOffRep rep;

        public TimeOffRequestController(ITimeOffRep rep)
        {
            this.rep = rep;
        }

   

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetMyTimeOffStatus(string id)
        {
            return Ok(rep.GetMyTimeOffStatus(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet("id")]
        public IActionResult GetTimeOffById(int id)
        {
            return Ok(rep.GetTimeOffById(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult AcceptTimeOff(int id)
        {
            return Ok(rep.AcceptTimeOff(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult RejectTimeOff(int id)
        {
            return Ok(rep.RejectTimeOff(id));
        }

        [Route("[controller]/[Action]/{userid}")]
        [HttpGet]
        public IActionResult GetTimeOffRequestsToTakeAction(string userid)
        {
            return Ok(rep.GetTimeOffRequestsToTakeAction(userid));
        }
    }
}

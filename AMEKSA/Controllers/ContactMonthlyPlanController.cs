using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Entities;
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
    public class ContactMonthlyPlanController : ControllerBase
    {
        private readonly IContactMonthlyPlanRep rep;

        public ContactMonthlyPlanController(IContactMonthlyPlanRep rep)
        {
            this.rep = rep;
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult PlanVisit(ContactMonthlyPlan obj)
        {
            ContactMonthlyPlan x = rep.PlanVisit(obj);
            return Ok(x);
        }

        [Route("[controller]/[Action]/{UserId}")]
        [HttpGet]
        public IActionResult GetMyPlanThisMonth(string UserId)
        {
            return Ok(rep.GetMyPlanThisMonth(UserId));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeletePlannedVisit(int id)
        {
            return Ok(rep.DeletePlannedVisit(id));
        }


     
        [Route("[controller]/[Action]/{userid}")]
        [HttpGet]
        public IActionResult GetMyContactsForPlan(string userid)
        {
            return Ok(rep.GetMyContactsForPlan(userid));
        }

    }
}

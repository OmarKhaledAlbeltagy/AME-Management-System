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
    public class AccountMonthlyPlanController : ControllerBase
    {
        private readonly IAccountMonthlyPlanRep rep;

        public AccountMonthlyPlanController(IAccountMonthlyPlanRep rep)
        {
            this.rep = rep;
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult PlanVisit(AccountMonthlyPlan obj)
        {
            AccountMonthlyPlan x = rep.PlanVisit(obj);
            return Ok(x);
        }

        [Route("[controller]/[Action]/{UserId}")]
        [HttpGet]
        public IActionResult GetMyPlanThisMonth(string UserId)
        {
            return Ok(rep.GetMyPlanThisMonth(UserId));
        }

        [Route("[controller]/[Action]/{UserId}")]
        [HttpGet]
        public IActionResult GetMyPlanThisMonthSales(string UserId)
        {
            return Ok(rep.GetMyPlanThisMonthSales(UserId));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeletePlannedVisit(int id)
        {
            return Ok(rep.DeletePlannedVisit(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeletePlannedVisitSales(int id)
        {
            return Ok(rep.DeletePlannedVisitSales(id));
        }


        //GetMyAccountsForPlan
        [Route("[controller]/[Action]/{userid}")]
        [HttpGet]
        public IActionResult GetMyAccountsForPlan(string userid)
        {
            return Ok(rep.GetMyAccountsForPlan(userid));
        }

        //PlanVisitSales
        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult PlanVisitSales(AccountMonthlyPlan obj)
        {
            return Ok(rep.PlanVisitSales(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult PlanVisitSupportive(AccountMonthlyPlan obj)
        {
            return Ok(rep.PlanVisitSupportive(obj));
        }

    }
}

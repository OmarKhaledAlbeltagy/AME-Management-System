using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using AMEKSA.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class TimeOffController : ControllerBase
    {
        private readonly ITimeOffRep rep;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public TimeOffController(ITimeOffRep rep, UserManager<ExtendIdentityUser> userManager)
        {
            this.rep = rep;
            this.userManager = userManager;
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

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetWorkingDaysSet()
        {
            return Ok(rep.GetWorkingDaysSet());
        }

      
        [Route("[controller]/[Action]/{month}/{year}/{workingdays}")]
        [HttpGet]
        public IActionResult SetWorkingDays(int month, int year, int workingdays)
        {
            return Ok(rep.SetWorkingDays(month,year,workingdays));
        }

        
        [Route("[controller]/[Action]/{Id}/{WorkingDays}")]
        [HttpGet]
        public IActionResult EditWorkingDays(int Id, int WorkingDays)
        {
            return Ok(rep.EditWorkingDays(Id, WorkingDays));
        }

        
        [Route("[controller]/[Action]/{userId}")]
        [HttpGet]
        public IActionResult Getmytimesoff(string userId)
        {
            return Ok(rep.Getmytimesoff(userId));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetTimeOffTerritoryReasons()
        {
            return Ok(rep.GetTimeOffTerritoryReasons());
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult MakeTimeOffTerritory(UserTimeOff obj)
        {
            TimeOffMail res = rep.MakeTimeOffTerritory(obj);
            rep.mail(res);



            return Ok(true);
        }

   
        [Route("[controller]/[Action]/{userId}")]
        [HttpGet]
        public IActionResult GetMyTimeOffData(string userId)
        {
            return Ok(rep.GetMyTimeOffData(userId));
        }


        [Route("[controller]/[Action]/{Id}")]
        [HttpGet]
        public IActionResult DeleteTimeOffTerritory(int Id)
        {
            return Ok(rep.DeleteTimeOffTerritory(Id));
        }


        [Route("[controller]/[Action]/{Id}")]
        [HttpGet]
        public IActionResult GetMyTeamTimeOff(string Id)
        {
            return Ok(rep.GetMyTeamTimeOff(Id));
        }

      
        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllTimeOff()
        {
            return Ok(rep.GetAllTimeOff());
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetVacancyReasons()
        {
            return Ok(rep.GetVacancyReasons());
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult MakeVacancyRequest(VacancyRequests obj)
        {
            return Ok(rep.MakeVacancyRequest(obj));
        }

        [Route("[controller]/[Action]/{userid}")]
        [HttpGet]
        public IActionResult GetMyVacancies(string userid)
        {
            return Ok(rep.GetMyVacancies(userid));
        }

        [Route("[controller]/[Action]/{managerid}")]
        [HttpGet]
        public IActionResult GetMyTeamVacanciesRequests(string managerid)
        {
            return Ok(rep.GetMyTeamVacanciesRequests(managerid));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult RejectVacancyRequest(int id)
        {
            return Ok(rep.RejectVacancyRequest(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult AcceptVacancyRequest(int id)
        {
            return Ok(rep.AcceptVacancyRequest(id));
        }


        [Route("[controller]/[Action]/{year}/{month}/{ManagerId}")]
        [HttpGet]
        public IActionResult CaculateActualWorkingDaysByMonth(int year, int month, string ManagerId)
        {
            return Ok(rep.CaculateActualWorkingDaysByMonth(year,month,ManagerId));
        }

        [Route("[controller]/[Action]/{UserId}")]
        [HttpGet]
        public IActionResult CaculateActualWorkingDaysByMonthMedical(string UserId)
        {
            return Ok(rep.CaculateActualWorkingDaysByMonthMedical(UserId));
        }

        [Route("[controller]/[Action]/{UserId}")]
        [HttpGet]
        public IActionResult CaculateActualWorkingDaysByMonthSales(string UserId)
        {
            return Ok(rep.CaculateActualWorkingDaysByMonthSales(UserId));
        }


        [Route("[controller]/[Action]/{year}/{month}")]
        [HttpGet]
        public IActionResult CaculateActualWorkingDaysByMonthTopManager(int year, int month)
        {
            return Ok(rep.CaculateActualWorkingDaysByMonthTopManager(year,month));
        }



    }
}

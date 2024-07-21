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
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRep rep;

        public NotificationController(INotificationRep rep)
        {
            this.rep = rep;
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetLastFifteen(string id)
        {
            return Ok(rep.GetLastFifteen(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult GetLastMonth(string id)
        {
            return Ok(rep.GetLastMonth(id));
        }

        [Route("[controller]/[Action]/{UserId}")]
        [HttpGet]
        public IActionResult SetAsSeen(string UserId)
        {
            return Ok(rep.SetAsSeen(UserId));
        }
    }
}

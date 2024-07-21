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
    public class DateTimeController : ControllerBase
    {
        private readonly ITimeRep ti;

        public DateTimeController(ITimeRep ti)
        {
            this.ti = ti;
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetCurrentTime()
        {
            return Ok(ti.GetCurrentTime());
        }
    }
}

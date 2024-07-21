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
    public class StringPropertiesController : ControllerBase
    {
        private readonly IStringPropertiesRep rep;

        public StringPropertiesController(IStringPropertiesRep rep)
        {
            this.rep = rep;
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetMotivation()
        {
            return Ok(rep.GetMotivation());
        }

        [Route("[controller]/[Action]/{m}")]
        [HttpGet]
        public IActionResult EditMotivation(string m)
        {
            return Ok(rep.EditMotivation(m));
        }
    }
}

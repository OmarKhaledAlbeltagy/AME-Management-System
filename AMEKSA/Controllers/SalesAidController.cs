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
    public class SalesAidController : ControllerBase
    {
        private readonly ISalesAidRep salesaidRep;

        public SalesAidController(ISalesAidRep salesaidRep)
        {
            this.salesaidRep = salesaidRep;
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllSalesAids()
        {
            return Ok(salesaidRep.GetAllSalesAids());
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddSalesAid(SalesAid obj)
        {
            return Ok(salesaidRep.AddSalesAid(obj));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditSalesAid(SalesAid obj)
        {
            return Ok(salesaidRep.EditSalesAid(obj));
        }



        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetShowSalesAids()
        {
            return Ok(salesaidRep.GetShownSalesAids());
        }
    }
}

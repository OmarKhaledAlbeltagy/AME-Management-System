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
    public class PurchaseTypeController : ControllerBase
    {
        private readonly IPurchaseTypeRep rep;

        public PurchaseTypeController(IPurchaseTypeRep rep)
        {
            this.rep = rep;
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllPurchaseTypes()
        {
            return Ok(rep.GetAllPurchaseTypes());
        }
    }
}

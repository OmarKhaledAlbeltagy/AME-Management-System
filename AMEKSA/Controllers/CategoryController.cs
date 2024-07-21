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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRep rep;
        private readonly ITimeRep ti;

        public CategoryController(ICategoryRep rep,ITimeRep ti)
        {
            this.rep = rep;
            this.ti = ti;
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult date()
        {
            return Ok(rep.date());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult nonocateg()
        {
            return Ok(rep.nonocateg());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllCategories()
        {
            return Ok(rep.GetAllCategories());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetTimeNow()
        {
            return Ok(rep.GetTimeNow());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetTimeThreeDaysAgo()
        {
            DateTime now = rep.GetTimeNow();
            DateTime res = now.AddDays(-4);
            return Ok(res);
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult today()
        {
            return Ok(ti.getnumberofweektoday());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult categ()
        {
            return Ok(rep.categ());
        }




    }
}

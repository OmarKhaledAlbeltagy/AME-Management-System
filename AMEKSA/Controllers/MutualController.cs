using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Context;
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
    public class MutualController : ControllerBase
    {
        private readonly IUserRep userRep;
        private readonly DbContainer db;

        public MutualController(IUserRep userRep,DbContainer db)
        {
            this.userRep = userRep;
            this.db = db;
        }

        [Route("[controller]/[action]/{userId?}")]
        [HttpGet("{userId?}")]
        public IActionResult GetUserById(string userId)
        {
            return Ok(userRep.GetUserById(userId));
        }

        [Route("[controller]/[action]")]
        [HttpGet]
        public IActionResult forr()
        {
            for (int y = 2021; y <= 2050; y++)
            {
                for (int m = 1; m <= 12; m++)
                {
                    WorkingDays obj = new WorkingDays();
                    obj.Year = y;
                    obj.Month = m;
                    db.workingDays.Add(obj);
                }
            }
            db.SaveChanges();
            return Ok(true);
        }
    }
}

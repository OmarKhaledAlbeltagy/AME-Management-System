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
    public class MagellanDayController : ControllerBase
    {
        private readonly IMagellanDayRep rep;

        public MagellanDayController(IMagellanDayRep rep)
        {
            this.rep = rep;
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetContact()
        {
            return Ok(rep.GetContact());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAttended()
        {
            return Ok(rep.GetAttended());
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult ConfirmAttendance(int id)
        {
            return Ok(rep.ConfirmAttendance(id));
        }
    }
}

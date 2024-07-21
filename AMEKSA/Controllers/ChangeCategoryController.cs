using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ChangeCategoryController : ControllerBase
    {
        private readonly IChangeCategoryRep rep;

        public ChangeCategoryController(IChangeCategoryRep rep)
        {
            this.rep = rep;
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult MakeRequest(ChangeCategoryRequestModel obj)
        {
            return Ok(rep.MakeRequest(obj));
        }

        [Route("[controller]/[Action]/{ManagerId}")]
        [HttpGet]
        public IActionResult GetMyTeamRequests(string ManagerId)
        {
            return Ok(rep.GetMyTeamRequests(ManagerId));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllRequests()
        {
            return Ok(rep.GetAllRequests());
        }

        [Route("[controller]/[Action]/{RequestId}")]
        [HttpGet]
        public IActionResult ConfirmRequest(int RequestId)
        {
            return Ok(rep.ConfirmRequest(RequestId));
        }

        [Route("[controller]/[Action]/{RequestId}")]
        [HttpGet]
        public IActionResult RejectRequest(int RequestId)
        {
            return Ok(rep.RejectRequest(RequestId));
        }
    }
}

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
    public class LoginController : ControllerBase
    {
        private readonly ILoginRep rep;

        public LoginController(ILoginRep rep)
        {
            this.rep = rep;
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult Login(LoginModel obj)
        {
    
            return Ok(rep.Login(obj).Result);
        }

    }
}

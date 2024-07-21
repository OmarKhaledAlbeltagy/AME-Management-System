using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using AMEKSA.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class PasswordChangeController : ControllerBase
    {
        private readonly IPasswordchangeRep rep;
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly ITimeRep ti;
        private readonly DbContainer db;

        public PasswordChangeController(IPasswordchangeRep rep, UserManager<ExtendIdentityUser> userManager, ITimeRep ti, DbContainer db)
        {
            this.rep = rep;
            this.userManager = userManager;
            this.ti = ti;
            this.db = db;
        }

        [Route("[controller]/[Action]/{UserId}")]
        [HttpGet]
        public IActionResult CheckChanging(string UserId)
        {
            return Ok(rep.CheckChanging(UserId));
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult ChangePassword(ResetPasswordModel obj)
        {
            
            ExtendIdentityUser user = userManager.FindByIdAsync(obj.UserId).Result;

            IdentityResult newpassword = userManager.ChangePasswordAsync(user, obj.OldPassword, obj.NewPassword).Result;



            if (newpassword.Succeeded)
            {
                DateTime now = ti.GetCurrentTime();
                PasswordChange pass = new PasswordChange();
                pass.ExtendIdentityUserId = obj.UserId;
                pass.ChangingDateTime = now;
                pass.OldPassword = obj.OldPassword;
                pass.NewPassword = obj.NewPassword;
                db.passwordChange.Add(pass);
                db.SaveChanges();
                return Ok(true);
            }

            else
            {
                return Ok(false);
            }
        }
    }
}

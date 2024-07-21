using System;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class LoginRep : ILoginRep
    {
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly SignInManager<ExtendIdentityUser> signInManager;

        public LoginRep(UserManager<ExtendIdentityUser> userManager, SignInManager<ExtendIdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<dynamic> Login(LoginModel obj)
        {
            
                SignInResult res = await signInManager.PasswordSignInAsync(obj.Email, obj.Password, true, false);

                if (res.Succeeded)
                {
                    ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
                    string role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
                    CustomUserRole userrole = new CustomUserRole();
                    userrole.FullName = user.FullName;
                    userrole.UserEmail = user.Email;
                    userrole.UserId = user.Id;
                    userrole.RoleName = role;
                    userrole.RepId = user.RepId;
                    userrole.Active = user.Active;
                    return userrole;
                }
                else
                {
                    return 0;
                }
               
          
        

           
           

        }
    }
}

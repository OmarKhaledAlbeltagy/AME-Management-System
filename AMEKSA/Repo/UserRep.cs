using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class UserRep : IUserRep
    {
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly RoleManager<ExtendIdentityRole> roleManager;
        private readonly DbContainer db;

        public UserRep(UserManager<ExtendIdentityUser> userManager, RoleManager<ExtendIdentityRole> roleManager, DbContainer db)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.db = db;
        }

        public bool ActivateUser(string userId)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(userId).Result;
            user.Active = true;
            db.SaveChanges();

            return true;
        }

        public void ChangeUserManager(string userId, string managerId)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(userId).Result;
            user.extendidentityuserid = managerId;
            db.SaveChanges();
        }

        public bool DeactivateUser(string userId)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(userId).Result;
            user.Active = false;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<CustomUsers> GetAllUsers()
        {
            List<CustomUsers> result = new List<CustomUsers>();

            IEnumerable<CustomUsers> supportive = userManager.GetUsersInRoleAsync("Supportive").Result.Join(db.UserRoles,
               a => a.Id,
               b => b.UserId,
               (a, b) => new
               {
                   Id = a.Id,
                   FullName = a.FullName,
                   Email = a.Email,
                   PhoneNumber = a.PhoneNumber,
                   CityId = a.CityId,
                   RoleId = b.RoleId
               }).Join(db.Roles,
               a => a.RoleId,
               b => b.Id,
               (a, b) => new
               {
                   Id = a.Id,
                   FullName = a.FullName,
                   Email = a.Email,
                   PhoneNumber = a.PhoneNumber,
                   CityId = a.CityId,
                   RoleName = b.Name
               }).Join(db.city, a => a.CityId, b => b.Id, (a, b) => new CustomUsers
               {
                   UserId = a.Id,
                   CityName = b.CityName,
                   Email = a.Email,
                   PhoneNumber = a.PhoneNumber,
                   FullName = a.FullName,
                   RoleName = a.RoleName
               });

            IEnumerable<CustomUsers> sales = userManager.GetUsersInRoleAsync("Sales Representative").Result.Join(db.UserRoles,
                a => a.Id,
                b => b.UserId,
                (a, b) => new
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber,
                    CityId = a.CityId,
                    RoleId = b.RoleId
                }).Join(db.Roles,
                a => a.RoleId,
                b => b.Id,
                (a, b) => new
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber,
                    CityId = a.CityId,
                    RoleName = b.Name
                }).Join(db.city, a => a.CityId, b => b.Id, (a, b) => new CustomUsers
                {
                    UserId = a.Id,
                    CityName = b.CityName,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber,
                    FullName = a.FullName,
                    RoleName = a.RoleName
                });

            IEnumerable<CustomUsers> medical = userManager.GetUsersInRoleAsync("Medical Representative").Result.Join(db.UserRoles,
            a => a.Id,
            b => b.UserId,
            (a, b) => new
            {
                Id = a.Id,
                FullName = a.FullName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                CityId = a.CityId,
                RoleId = b.RoleId
            }).Join(db.Roles,
            a => a.RoleId,
            b => b.Id,
            (a, b) => new
            {
                Id = a.Id,
                FullName = a.FullName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                CityId = a.CityId,
                RoleName = b.Name
            }).Join(db.city, a => a.CityId, b => b.Id, (a, b) => new CustomUsers
            {
                UserId = a.Id,
                CityName = b.CityName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                FullName = a.FullName,
                RoleName = a.RoleName
            });

            IEnumerable<CustomUsers> first = userManager.GetUsersInRoleAsync("First Line Manager").Result.Join(db.UserRoles,
            a => a.Id,
            b => b.UserId,
            (a, b) => new
            {
                Id = a.Id,
                FullName = a.FullName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                CityId = a.CityId,
                RoleId = b.RoleId
            }).Join(db.Roles,
            a => a.RoleId,
            b => b.Id,
            (a, b) => new
            {
                Id = a.Id,
                FullName = a.FullName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                CityId = a.CityId,
                RoleName = b.Name
            }).Join(db.city, a => a.CityId, b => b.Id, (a, b) => new CustomUsers
            {
                UserId = a.Id,
                CityName = b.CityName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                FullName = a.FullName,
                RoleName = a.RoleName
            });

            IEnumerable<CustomUsers> top = userManager.GetUsersInRoleAsync("Top Line Manager").Result.Join(db.UserRoles,
            a => a.Id,
            b => b.UserId,
            (a, b) => new
            {
                Id = a.Id,
                FullName = a.FullName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                CityId = a.CityId,
                RoleId = b.RoleId
            }).Join(db.Roles,
            a => a.RoleId,
            b => b.Id,
            (a, b) => new CustomUsers
            {
                UserId = a.Id,
                FullName = a.FullName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                RoleName = b.Name,
                CityName = "المملكة العربية السعودية"
            });

            IEnumerable<CustomUsers> systemm = userManager.GetUsersInRoleAsync("System Adminn").Result.Join(db.UserRoles,
            a => a.Id,
            b => b.UserId,
            (a, b) => new
            {
                Id = a.Id,
                FullName = a.FullName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                CityId = a.CityId,
                RoleId = b.RoleId
            }).Join(db.Roles,
            a => a.RoleId,
            b => b.Id,
            (a, b) => new CustomUsers
            {
                UserId = a.Id,
                FullName = a.FullName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                RoleName = b.Name,
                CityName = "المملكة العربية السعودية"
            });

            IEnumerable<CustomUsers> system = userManager.GetUsersInRoleAsync("System Admin").Result.Join(db.UserRoles,
         a => a.Id,
         b => b.UserId,
         (a, b) => new
         {
             Id = a.Id,
             FullName = a.FullName,
             Email = a.Email,
             PhoneNumber = a.PhoneNumber,
             CityId = a.CityId,
             RoleId = b.RoleId
         }).Join(db.Roles,
         a => a.RoleId,
         b => b.Id,
         (a, b) => new CustomUsers
         {
             UserId = a.Id,
             FullName = a.FullName,
             Email = a.Email,
             PhoneNumber = a.PhoneNumber,
             RoleName = b.Name,
             CityName = "المملكة العربية السعودية"
         });

            IEnumerable<CustomUsers> hr = userManager.GetUsersInRoleAsync("HR").Result.Join(db.UserRoles,
           a => a.Id,
           b => b.UserId,
           (a, b) => new
           {
               Id = a.Id,
               FullName = a.FullName,
               Email = a.Email,
               PhoneNumber = a.PhoneNumber,
               CityId = a.CityId,
               RoleId = b.RoleId
           }).Join(db.Roles,
           a => a.RoleId,
           b => b.Id,
           (a, b) => new CustomUsers
           {
               UserId = a.Id,
               FullName = a.FullName,
               Email = a.Email,
               PhoneNumber = a.PhoneNumber,
               RoleName = b.Name,
               CityName = "المملكة العربية السعودية"
           });



            IEnumerable<CustomUsers> maintenance = userManager.GetUsersInRoleAsync("Maintenance Representative").Result.Join(db.UserRoles,
          a => a.Id,
          b => b.UserId,
          (a, b) => new
          {
              Id = a.Id,
              FullName = a.FullName,
              Email = a.Email,
              PhoneNumber = a.PhoneNumber,
              CityId = a.CityId,
              RoleId = b.RoleId
          }).Join(db.Roles,
          a => a.RoleId,
          b => b.Id,
          (a, b) => new CustomUsers
          {
              UserId = a.Id,
              FullName = a.FullName,
              Email = a.Email,
              PhoneNumber = a.PhoneNumber,
              RoleName = b.Name,
              CityName = "المملكة العربية السعودية"
          });

                    IEnumerable<CustomUsers> helpdesk = userManager.GetUsersInRoleAsync("Help Desk").Result.Join(db.UserRoles,
               a => a.Id,
               b => b.UserId,
               (a, b) => new
               {
                   Id = a.Id,
                   FullName = a.FullName,
                   Email = a.Email,
                   PhoneNumber = a.PhoneNumber,
                   CityId = a.CityId,
                   RoleId = b.RoleId
               }).Join(db.Roles,
               a => a.RoleId,
               b => b.Id,
               (a, b) => new CustomUsers
               {
                   UserId = a.Id,
                   FullName = a.FullName,
                   Email = a.Email,
                   PhoneNumber = a.PhoneNumber,
                   RoleName = b.Name,
                   CityName = "المملكة العربية السعودية"
               });


            IEnumerable<CustomUsers> AccountingManager = userManager.GetUsersInRoleAsync("Accounting Manager").Result.Join(db.UserRoles,
             a => a.Id,
             b => b.UserId,
             (a, b) => new
             {
                 Id = a.Id,
                 FullName = a.FullName,
                 Email = a.Email,
                 PhoneNumber = a.PhoneNumber,
                 CityId = a.CityId,
                 RoleId = b.RoleId
             }).Join(db.Roles,
             a => a.RoleId,
             b => b.Id,
             (a, b) => new CustomUsers
             {
                 UserId = a.Id,
                 FullName = a.FullName,
                 Email = a.Email,
                 PhoneNumber = a.PhoneNumber,
                 RoleName = b.Name,
                 CityName = "المملكة العربية السعودية"
             });

            IEnumerable<CustomUsers> Accountant = userManager.GetUsersInRoleAsync("Accountant").Result.Join(db.UserRoles,
            a => a.Id,
            b => b.UserId,
            (a, b) => new
            {
                Id = a.Id,
                FullName = a.FullName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                CityId = a.CityId,
                RoleId = b.RoleId
            }).Join(db.Roles,
            a => a.RoleId,
            b => b.Id,
            (a, b) => new
            {
                Id = a.Id,
                FullName = a.FullName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                CityId = a.CityId,
                RoleName = b.Name
            }).Join(db.city, a => a.CityId, b => b.Id, (a, b) => new CustomUsers
            {
                UserId = a.Id,
                CityName = b.CityName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                FullName = a.FullName,
                RoleName = a.RoleName
            });





            foreach (var item in sales)
            {
                result.Add(item);
            }

            foreach (var item in medical)
            {
                result.Add(item);
            }

            foreach (var item in first)
            {
                result.Add(item);
            }
            foreach (var item in top)
            {
                result.Add(item);
            }
            foreach (var item in system)
            {
                result.Add(item);
            }
            foreach (var item in systemm)
            {
                result.Add(item);
            }
            foreach (var item in hr)
            {
                result.Add(item);
            }

            foreach (var item in supportive)
            {
                result.Add(item);
            }

            foreach (var item in maintenance)
            {
                result.Add(item);
            }

            foreach (var item in helpdesk)
            {
                result.Add(item);
            }

            result.AddRange(Accountant);
            result.AddRange(AccountingManager);

            return result.OrderBy(a=>a.FullName);


        }

        public IEnumerable<CustomUsers> GetUserByRole(string roleName)
        {
            if (roleName == "System Admin")
            {
                IEnumerable<CustomUsers> users = userManager.Users.Join(db.UserRoles,
                   a => a.Id,
                   b => b.UserId,
                   (a, b) => new
                   {
                       Id = a.Id,
                       FullName = a.FullName,
                       Email = a.Email,
                       PhoneNumber = a.PhoneNumber,
                       RoleId = b.RoleId
                   }).Join(db.Roles,
                   a => a.RoleId,
                   b => b.Id,
                   (a, b) => new CustomUsers
                   {
                       UserId = a.Id,
                       FullName = a.FullName,
                       Email = a.Email,
                       PhoneNumber = a.PhoneNumber,
                       RoleName = b.Name
                   }).Where(r => r.RoleName == roleName);



                return users;
            }
            else
            {
                IEnumerable<CustomUsers> users = userManager.Users.Join(db.UserRoles,
                   a => a.Id,
                   b => b.UserId,
                   (a, b) => new
                   {
                       Id = a.Id,
                       FullName = a.FullName,
                       Email = a.Email,
                       PhoneNumber = a.PhoneNumber,
                       CityId = a.CityId,
                       RoleId = b.RoleId
                   }).Join(db.Roles,
                   a => a.RoleId,
                   b => b.Id,
                   (a, b) => new
                   {
                       Id = a.Id,
                       FullName = a.FullName,
                       Email = a.Email,
                       PhoneNumber = a.PhoneNumber,
                       CityId = a.CityId,
                       RoleName = b.Name
                   }).Join(db.city, a => a.CityId, b => b.Id, (a, b) => new CustomUsers
                   {
                       UserId = a.Id,
                       CityName = b.CityName,
                       Email = a.Email,
                       PhoneNumber = a.PhoneNumber,
                       FullName = a.FullName,
                       RoleName = a.RoleName
                   }).Where(r => r.RoleName == roleName);



                return users.OrderBy(a=>a.FullName);
            }
           
        }

        public void LinkUserToBrands(UserBrandCustomEntity usersBrands)
        {
            foreach (var item in usersBrands.BrandsIds)
            {
                UserBrand UB = new UserBrand();
                UB.BrandId = item;
                UB.extendidentityuserid = usersBrands.UserId;
                db.userBrand.Add(UB);
                db.SaveChanges();
            }

        }

        public CustomUsers GetUserById(string userId)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(userId).Result;
            string role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
            
            if (role == "First Line Manager")
            {
                string city = db.city.Find(user.CityId).CityName;
                CustomUsers result = new CustomUsers();
                result.Active = user.Active;
                result.CityName = city;
                result.RoleName = role;
                result.PhoneNumber = user.PhoneNumber;
                result.FullName = user.FullName;
                result.Email = user.Email;

                return result;
            }

            else
            {
                if (role == "Medical Representative" || role == "Sales Representative" || role == "Supportive")
                {
                    City city = db.city.Find(user.CityId);
                    ExtendIdentityUser Manager = userManager.FindByIdAsync(user.extendidentityuserid).Result;
                    string managerName = Manager.FullName;
                    CustomUsers result = new CustomUsers();
                    result.Active = user.Active;
                    result.CityId = city.Id;
                    result.CityName = city.CityName;
                    result.RoleName = role;
                    result.PhoneNumber = user.PhoneNumber;
                    result.FullName = user.FullName;
                    result.Email = user.Email;
                    result.ManagerId = user.extendidentityuserid;
                    result.ManagerName = managerName;
                    return result;
                }

                else
                {
                    CustomUsers result = new CustomUsers();
                    result.Active = user.Active;
                    result.RoleName = role;
                    result.PhoneNumber = user.PhoneNumber;
                    result.FullName = user.FullName;
                    result.Email = user.Email;

                    return result;
                }


            }

        }

        public IEnumerable<CustomUserCityCount> usersCitiesCount()
        {
            List<CustomUserCityCount> count = new List<CustomUserCityCount>();


            List<City> cities = db.city.Select(a => a).ToList();
            //db.Dispose();

            foreach (var city in cities)
            {
                int userscount = db.Users.Where(a => a.CityId == city.Id).Count();
                CustomUserCityCount obj = new CustomUserCityCount();
                obj.CityName = city.CityName;
                obj.Count = userscount;
                count.Add(obj);
   

            }
            return count;
        }

        public IEnumerable<CustomUsersRolesCount> usersRolesCount()
        {
            List<CustomUsersRolesCount> count = new List<CustomUsersRolesCount>();

            int systemadminscount = userManager.GetUsersInRoleAsync("System Admin").Result.Count + userManager.GetUsersInRoleAsync("System Adminn").Result.Count;
            CustomUsersRolesCount systemadminobj = new CustomUsersRolesCount();
            systemadminobj.RoleName = "System Admin";
            systemadminobj.Count = systemadminscount;
            count.Add(systemadminobj);

            int topmanagerscount = userManager.GetUsersInRoleAsync("Top Line Manager").Result.Count;
            CustomUsersRolesCount topmanagerobj = new CustomUsersRolesCount();
            topmanagerobj.RoleName = "Top Manager";
            topmanagerobj.Count = topmanagerscount;
            count.Add(topmanagerobj);

            int firstmanagerscount = userManager.GetUsersInRoleAsync("First Line Manager").Result.Count;
            CustomUsersRolesCount firstmanagerobj = new CustomUsersRolesCount();
            firstmanagerobj.RoleName = "First Line Manager";
            firstmanagerobj.Count = firstmanagerscount;
            count.Add(firstmanagerobj);

            int medicalcount = userManager.GetUsersInRoleAsync("Medical Representative").Result.Count;
            CustomUsersRolesCount medicalobj = new CustomUsersRolesCount();
            medicalobj.RoleName = "Medical Representative";
            medicalobj.Count = medicalcount;
            count.Add(medicalobj);

            int salescount = userManager.GetUsersInRoleAsync("Sales Representative").Result.Count;
            CustomUsersRolesCount salesobj = new CustomUsersRolesCount();
            salesobj.RoleName = "Sales Representative";
            salesobj.Count = salescount;
            count.Add(salesobj);

            int supportivecount = userManager.GetUsersInRoleAsync("Supportive").Result.Count;
            CustomUsersRolesCount supportiveobj = new CustomUsersRolesCount();
            supportiveobj.RoleName = "Supportive Representative";
            supportiveobj.Count = supportivecount;
            count.Add(supportiveobj);

            int hrcount = userManager.GetUsersInRoleAsync("HR").Result.Count;
            CustomUsersRolesCount hrobj = new CustomUsersRolesCount();
            hrobj.RoleName = "HR";
            hrobj.Count = hrcount;
            count.Add(hrobj);

            return count;
        }

        public bool EditUser(ExtendIdentityUser obj)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(obj.Id).Result;
            string role = userManager.GetRolesAsync(user).Result.FirstOrDefault();
            switch (role)
            {
                case "System Admin":
                   string systememailtoken = userManager.GenerateChangeEmailTokenAsync(user, obj.Email).Result;
                   IdentityResult systemchangemail = userManager.ChangeEmailAsync(user, obj.Email, systememailtoken).Result;

                    string systemphonetoken = userManager.GenerateChangePhoneNumberTokenAsync(user, obj.PhoneNumber).Result;
                    IdentityResult systemchangephone = userManager.ChangePhoneNumberAsync(user, obj.PhoneNumber, systemphonetoken).Result;

                    IdentityResult systemusername = userManager.SetUserNameAsync(user, obj.Email).Result;
                    userManager.UpdateNormalizedUserNameAsync(user);

                    user.FullName = obj.FullName;

                    if (systemchangemail.Succeeded && systemchangephone.Succeeded && systemusername.Succeeded)
                    {
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case "System Adminn":
                    string systemmemailtoken = userManager.GenerateChangeEmailTokenAsync(user, obj.Email).Result;
                    IdentityResult systemmchangemail = userManager.ChangeEmailAsync(user, obj.Email, systemmemailtoken).Result;

                    string systemmphonetoken = userManager.GenerateChangePhoneNumberTokenAsync(user, obj.PhoneNumber).Result;
                    IdentityResult systemmchangephone = userManager.ChangePhoneNumberAsync(user, obj.PhoneNumber, systemmphonetoken).Result;

                    IdentityResult systemmusername = userManager.SetUserNameAsync(user, obj.Email).Result;
                    userManager.UpdateNormalizedUserNameAsync(user);

                    user.FullName = obj.FullName;

                    if (systemmchangemail.Succeeded && systemmchangephone.Succeeded && systemmusername.Succeeded)
                    {
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case "Top Line Manager":
                    string topemailtoken = userManager.GenerateChangeEmailTokenAsync(user, obj.Email).Result;
                    IdentityResult topchangemail = userManager.ChangeEmailAsync(user, obj.Email, topemailtoken).Result;

                    string topphonetoken = userManager.GenerateChangePhoneNumberTokenAsync(user, obj.PhoneNumber).Result;
                    IdentityResult topchangephone = userManager.ChangePhoneNumberAsync(user, obj.PhoneNumber, topphonetoken).Result;

                   IdentityResult topusername = userManager.SetUserNameAsync(user, obj.Email).Result;
                   userManager.UpdateNormalizedUserNameAsync(user);

                    user.FullName = obj.FullName;

                    if (topchangemail.Succeeded && topchangephone.Succeeded && topusername.Succeeded)
                    {
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case "First Line Manager":
                    string firstemailtoken = userManager.GenerateChangeEmailTokenAsync(user, obj.Email).Result;
                    IdentityResult firstchangemail = userManager.ChangeEmailAsync(user, obj.Email, firstemailtoken).Result;

                    string firstphonetoken = userManager.GenerateChangePhoneNumberTokenAsync(user, obj.PhoneNumber).Result;
                    IdentityResult firstchangephone = userManager.ChangePhoneNumberAsync(user, obj.PhoneNumber, firstphonetoken).Result;

                    IdentityResult firstusername = userManager.SetUserNameAsync(user, obj.Email).Result;
                    userManager.UpdateNormalizedUserNameAsync(user);

                    user.FullName = obj.FullName;
                    

                    if (firstchangemail.Succeeded && firstchangephone.Succeeded && firstusername.Succeeded)
                    {
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case "Medical Representative":
                    string medicalemailtoken = userManager.GenerateChangeEmailTokenAsync(user, obj.Email).Result;
                    IdentityResult medicalchangemail = userManager.ChangeEmailAsync(user, obj.Email, medicalemailtoken).Result;

                    string medicalphonetoken = userManager.GenerateChangePhoneNumberTokenAsync(user, obj.PhoneNumber).Result;
                    IdentityResult medicalchangephone = userManager.ChangePhoneNumberAsync(user, obj.PhoneNumber, medicalphonetoken).Result;

                    IdentityResult medicalusername = userManager.SetUserNameAsync(user, obj.Email).Result;
                    userManager.UpdateNormalizedUserNameAsync(user);

                    user.FullName = obj.FullName;

                    if (medicalchangemail.Succeeded && medicalchangephone.Succeeded && medicalusername.Succeeded)
                    {
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case "Sales Representative":
                    string salesemailtoken = userManager.GenerateChangeEmailTokenAsync(user, obj.Email).Result;
                    IdentityResult saleschangemail = userManager.ChangeEmailAsync(user, obj.Email, salesemailtoken).Result;

                    string salesphonetoken = userManager.GenerateChangePhoneNumberTokenAsync(user, obj.PhoneNumber).Result;
                    IdentityResult saleschangephone = userManager.ChangePhoneNumberAsync(user, obj.PhoneNumber, salesphonetoken).Result;

                    IdentityResult salesusername = userManager.SetUserNameAsync(user, obj.Email).Result;
                    userManager.UpdateNormalizedUserNameAsync(user);

                    user.FullName = obj.FullName;



                    if (saleschangemail.Succeeded && saleschangephone.Succeeded && salesusername.Succeeded)
                    {
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                default:
                    return false;

            }
        }

        public IEnumerable<Account> GetUserLinkedAccounts(string userId)
        {
            IEnumerable<Account> accounts = db.account
                .Join(db.userAccount,
                a => a.Id,
                b => b.AccountId,
                (a, b) => new
                {
                    AccountId = a.Id,
                    AccountName = a.AccountName,
                    UserId = b.extendidentityuserid
                }).Where(x => x.UserId == userId).Join(userManager.Users, a => a.UserId, b => b.Id, (a, b) => new Account
                {
                    Id = a.AccountId,
                    AccountName = a.AccountName
                });

            return accounts;
        }

        public IEnumerable<CustomAccount> GetUserUnlinkedAccounts(string userId)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(userId).Result;
            List<int> UA = db.userAccount.Where(a => a.extendidentityuserid == user.Id).Select(a=>a.AccountId).ToList();
            List<int> Districtsid = db.district.Where(a => a.CityId != user.CityId).Select(a=>a.Id).Distinct().ToList();

            List<Account> accs = db.account.Select(a=>a).ToList();

            if (UA != null)
            {
                foreach (var item in UA)
                {
                    Account a = db.account.Find(item);
                    accs.Remove(a);
                }
            }
            if (Districtsid != null)
            {
                foreach(var id in Districtsid)
                {
                    List<Account> a = db.account.Where(x => x.DistrictId == id).ToList();
                    if (a != null)
                    {
                        foreach(var acc in a)
                        {
                            accs.Remove(acc);
                        }
                    }
                  
                }
            }

            IEnumerable<CustomAccount> res = accs.Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new CustomAccount
            {
                Id = a.Id,
                AccountName = a.AccountName,
                DistrictName = b.DistrictName
            });



            return res;
        }

        public bool LinkUserWithAccount(UserAccountModel obj)
        {
            foreach (var accountId in obj.AccountsIds)
            {
                UserAccount ua = new UserAccount();
                ua.AccountId = accountId;
                ua.extendidentityuserid = obj.userId;
                db.userAccount.Add(ua);
                db.SaveChanges();
            }

           
            return true;
        }

        public bool UnlinkUserWithAccount(UserAccountModel obj)
        {
         

            foreach (var accountId in obj.AccountsIds)
            {
                UserAccount ua = db.userAccount.Where(a => a.AccountId == accountId && a.extendidentityuserid == obj.userId).FirstOrDefault();
                db.userAccount.Remove(ua);
            }
            db.SaveChanges();
            return true;
        }

        public List<Contact> GetUserLinkedContacts(string userId)
        {
            List<Contact> contacts = DistinctByExtension.DistinctBy(db.contact
             .Join(db.userContact,
             a => a.Id,
             b => b.ContactId,
             (a, b) => new
             {
                 ContactId = a.Id,
                 ContactName = a.ContactName,
                 UserId = b.extendidentityuserid
             }).Where(x => x.UserId == userId).Join(userManager.Users, a => a.UserId, b => b.Id, (a, b) => new Contact
             {
                 Id = a.ContactId,
                 ContactName = a.ContactName
             }), a => a.Id).ToList();

            return contacts;
        }

        public IEnumerable<CustomContact> GetUserUnlinkedContacts(string userId)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(userId).Result;

            List<int> districtsids = db.district.Where(a => a.CityId == user.CityId).Select(a=>a.Id).ToList();

            List<int> LinkedContactsIds = db.userContact.Where(a => a.extendidentityuserid == userId).Select(a=>a.ContactId).ToList();

            List<Contact> LinkedContacts = new List<Contact>();

            foreach (var id in LinkedContactsIds)
            {
                LinkedContacts.Add(db.contact.Find(id));
            }

            List<Contact> result = new List<Contact>();
            foreach  (var id in districtsids)
            {
                IEnumerable<Contact> c = db.contact.Where(a => a.DistrictId == id);

                foreach (var item in c)
                {
                    result.Add(item);
                }
            }
            foreach (var item in LinkedContacts)
            {
                result.Remove(item);
            }

            IEnumerable<CustomContact> res = result.Join(db.district, a => a.DistrictId, b => b.Id, (a, b) => new CustomContact
            {
                Id = a.Id,
                ContactName = a.ContactName,
                DistrictName = b.DistrictName
            });

            return res;
        }

        public bool LinkUserWithContact(UserContactModel obj)
        {
            foreach (var contactId in obj.ContactsIds)
            {
                UserContact uc = new UserContact();
                uc.ContactId = contactId.ContactId;
                uc.CategoryId = contactId.CategoryId;
                uc.extendidentityuserid = obj.UserId;
                uc.MonthlyTarget = 0;
                db.userContact.Add(uc);
                db.SaveChanges();
            }

           
            return true;
        }

        public bool UnlinkUserWithContact(UserContactModel obj)
        {
            foreach (var contactId in obj.ContactsIds)
            {
                IEnumerable<UserContact> uc = db.userContact.Where(a => a.ContactId == contactId.ContactId && a.extendidentityuserid == obj.UserId);

                foreach (var item in uc)
                {
                    db.Remove(item);
                }
                
                
            }
            db.SaveChanges();
            return true;
        }

        public IEnumerable<Brand> GetUserLinkedBrands(string userId)
        {
            string role = userManager.GetRolesAsync(userManager.FindByIdAsync(userId).Result).Result.FirstOrDefault();

            if (role == "First Line Manager")
            {
                IEnumerable<Brand> brands = userManager.Users
               .Join(db.userBrand,
               a => a.Id,
               b => b.extendidentityuserid,
               (a, b) => new
               {
                   UserId = a.Id,
                   BrandId = b.BrandId,
               }).Where(x => x.UserId == userId).Join(db.brand,
               a => a.BrandId,
               b => b.Id,
               (a, b) => new Brand
               {
                   Id = a.BrandId,
                   BrandName = b.BrandName
               });

                return brands;
            }

            else
            {
                ExtendIdentityUser user = userManager.FindByIdAsync(userId).Result;
                string ManagerId = userManager.FindByIdAsync(user.extendidentityuserid).Result.Id;

                IEnumerable<Brand> brands = userManager.Users
              .Join(db.userBrand,
              a => a.Id,
              b => b.extendidentityuserid,
              (a, b) => new
              {
                  UserId = a.Id,
                  BrandId = b.BrandId,
              }).Where(x => x.UserId == ManagerId).Join(db.brand,
              a => a.BrandId,
              b => b.Id,
              (a, b) => new Brand
              {
                  Id = a.BrandId,
                  BrandName = b.BrandName
              });

                return brands;

            }

           
        }

        public IEnumerable<Brand> GetUserUnlinkedBrands(string userId)
        {

            string role = userManager.GetRolesAsync(userManager.FindByIdAsync(userId).Result).Result.FirstOrDefault();

            if (role == "First Line Manager")
            {
                List<int> linkedbrandsids = db.userBrand.Where(a => a.extendidentityuserid == userId).Select(a => a.BrandId).ToList();
                List<Brand> unlinkedbrands = db.brand.Select(a => a).ToList();
                List<Brand> linkedbrands = new List<Brand>();
                foreach (var id in linkedbrandsids)
                {
                    linkedbrands.Add(db.brand.Find(id));
                }

                foreach (var item in linkedbrands)
                {
                    unlinkedbrands.Remove(item);
                }

                return unlinkedbrands;
            }


            else
            {
                ExtendIdentityUser user = userManager.FindByIdAsync(userId).Result;
                string ManagerId = userManager.FindByIdAsync(user.extendidentityuserid).Result.Id;

                List<int> linkedbrandsids = db.userBrand.Where(a => a.extendidentityuserid == ManagerId).Select(a => a.BrandId).ToList();
                List<Brand> unlinkedbrands = db.brand.Select(a => a).ToList();
                List<Brand> linkedbrands = new List<Brand>();
                foreach (var id in linkedbrandsids)
                {
                    linkedbrands.Add(db.brand.Find(id));
                }

                foreach (var item in linkedbrands)
                {
                    unlinkedbrands.Remove(item);
                }

                return unlinkedbrands;

            }

           
        }

        public bool LinkUserWithBrand(string userId, int brandId)
        {
            UserBrand UB = new UserBrand();
            UB.BrandId = brandId;
            UB.extendidentityuserid = userId;
            db.userBrand.Add(UB);
            db.SaveChanges();

            return true;
        }

        public bool UnlinkUserWithBrand(string userId, int brandId)
        {
            UserBrand UB = db.userBrand.Where(a => a.extendidentityuserid == userId && a.extendidentityuserid == userId).FirstOrDefault();
            db.userBrand.Remove(UB);
            db.SaveChanges();
            return true;

        }

        public bool ChangeManager(string UserId, string ManagerId)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(UserId).Result;
            user.extendidentityuserid = ManagerId;
            db.SaveChanges();
            return true;
        }

        public bool ChangeCity(string UserId, int CityId)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(UserId).Result;
            user.CityId = CityId;
            db.SaveChanges();
            return true;
        }
    }
}

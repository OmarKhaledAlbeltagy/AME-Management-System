using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IUserRep
    {

        bool ChangeManager(string UserId, string ManagerId);

        bool ChangeCity(string UserId, int CityId);

        void LinkUserToBrands(UserBrandCustomEntity usersBrands);

        void ChangeUserManager(string userId,string managerId);

        IEnumerable<CustomUsersRolesCount> usersRolesCount();

        IEnumerable<CustomUserCityCount> usersCitiesCount();

        bool DeactivateUser(string userId);

        bool ActivateUser(string userId);

        IEnumerable<CustomUsers> GetAllUsers();

        IEnumerable<CustomUsers> GetUserByRole(string roleName);

        CustomUsers GetUserById(string userId);

        bool EditUser(ExtendIdentityUser obj);

        IEnumerable<Account> GetUserLinkedAccounts(string userId);

        IEnumerable<CustomAccount> GetUserUnlinkedAccounts(string userId);

        bool LinkUserWithAccount(UserAccountModel obj);

        bool UnlinkUserWithAccount(UserAccountModel obj);

        List<Contact> GetUserLinkedContacts(string userId);

        IEnumerable<CustomContact> GetUserUnlinkedContacts(string userId);

        bool LinkUserWithContact(UserContactModel obj);

        bool UnlinkUserWithContact(UserContactModel obj);

        IEnumerable<Brand> GetUserLinkedBrands(string userId);

        IEnumerable<Brand> GetUserUnlinkedBrands(string userId);

        bool LinkUserWithBrand(string userId, int brandId);

        bool UnlinkUserWithBrand(string userId, int brandId);

    }
}

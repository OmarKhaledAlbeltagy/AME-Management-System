using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using Microsoft.AspNetCore.Identity;
namespace AMEKSA.Repo
{
   public interface ISystemAdminRep
    {
        //IEnumerable<IdentityUser> GetAllUsers();

        //IEnumerable<IdentityUser> GetUsersByRoleId(string roleid);

        //IdentityUser GetUserById(string userid);

        //void LinkUserWithAccount(string userid, int accountid);

        //void LinkUserWithBrand(string userid, int brandid);

        //void LinkUserWithContact(string userid, int contactid);

        IEnumerable<AccountsPerUserModel> GetAllMedicalAccounts();

        IEnumerable<AccountsPerUserModel> GetAllSalesAccounts();

        IEnumerable<AccountsPerUserModel> GetTeamAccounts(string ManagerId);

        AccountsPerUserModel GetRepAccounts(string UserId);

        IEnumerable<ContactsPerUserModel> GetAllMedicalContacts();

        IEnumerable<ContactsPerUserModel> GetTeamContacts(string ManagerId);

        ContactsPerUserModel GetRepContacts(string UserId);

        IEnumerable<CustomContact> GetUnAssignedContacts();

        IEnumerable<CustomAccount> GetUnAssignedAccounts();

        IEnumerable<RepByCategory> GetRepsByCategory(int x);

        IEnumerable<SystemAdminQuickVisitsReport> GetAccountMedicalVisitByAccountId(int id);

        IEnumerable<SystemAdminQuickVisitsReport> GetAccountSalesVisitByAccountId(int id);

        IEnumerable<SystemAdminQuickVisitsReport> GetContactMedicalVisitByContactId(int id);

        IEnumerable<AccountsPerUserModel> GetAllSupportiveAccounts();

        IEnumerable<AccountsPerUserModel> GetTeamSalesAccounts(string ManagerId);

        IEnumerable<AccountsPerUserModel> GetTeamSupportiveAccounts(string ManagerId);

        IEnumerable<UserSubmanagerModel> GetRepLinkedSubManagers(string RepId);

        IEnumerable<UserSubmanagerModel> GetRepNotLinkedSubManagers(string RepId);

        bool LinkSubManager(string RepId, string ManagerId);

        bool UnLinkSubManager(string RepId, string ManagerId);

        bool WorkingDaysRemidner();

        IEnumerable<SystemAdminQuickVisitsReport> GetAccountSupportiveVisitByAccountId(int id);
    }
}

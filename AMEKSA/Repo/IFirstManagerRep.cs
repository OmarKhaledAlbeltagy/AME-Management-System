using AMEKSA.FirstManagerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.CustomEntities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using AMEKSA.Entities;

namespace AMEKSA.Repo
{
   public interface IFirstManagerRep
    {
        IEnumerable<VisitsCountModel> GetVisitsCountByMonth(int year, int month, string myId);

        IEnumerable<AccountVisitsPercentageByCategoryModel> AccountThisMonthPercentage(string myId);

        IEnumerable<AccountVisitsPercentageByCategoryModel> AccountpastMonthPercentage(string myId);

        IEnumerable<AccountVisitsPercentageByCategoryModel> ContactThisMonthPercentage(string myId);

        IEnumerable<AccountVisitsPercentageByCategoryModel> ContactPastMonthPercentage(string myId);

        IEnumerable<CustomAccountMedicalVisit> GetMyTeamAMVisits(VisitsSearchModel obj);

        IEnumerable<CustomAccountSalesVisit> GetMyTeamASVisits(VisitsSearchModel obj);

        IEnumerable<CustomContactMedicalVisit> GetMyTeamCMVisits(VisitsSearchModel obj);

        IEnumerable<MyTeamModel> GetMyTeamMedical(string managerId);

        IEnumerable<MyTeamModel> GetMyTeamSales(string managerId);

        IEnumerable<MyTeamModel> GetMyTeamSupportive(string managerId);

        IEnumerable<CustomAccountSalesVisit> GetASDetailedVisits(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

        IEnumerable<CustomOpenningRequest> GetMyTeamRequests(string managerId);

        bool DeclineRequest(int id);

        bool AcceptRequest(int id);

        IEnumerable<CustomAccountSalesVisit> GetAccountSalesVisitsByUserId(string userId);

        IEnumerable<CustomAccountSupportiveVisit> GetSupportiveDetailedVisits(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

       

        List<CustomAccountMedicalVisit> GetAMDetailedVisits(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

        IEnumerable<CustomAccountSupportiveVisit> GetMyTeamASupportiveVisits(VisitsSearchModel obj);

        List<CustomContactMedicalVisit> GetCMDetailedVisits(string userid, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

        IEnumerable<MyTeamNamesModel> GetMyTeamNames(string ManagerId);

        IEnumerable<Account> GetFirstLineManagerAccounts(SearchByWord obj);

        IEnumerable<Contact> GetMyTeamContacts(string ManagerId);
    }
}

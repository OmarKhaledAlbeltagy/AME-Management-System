using AMEKSA.AccountSalesVisitModels;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IAccountSalesVisitRep
    {
        IEnumerable<TimeSpan> test();

        IEnumerable<CustomAccount> GetAccountsToVisitNow(string userId);

        bool MakeVisit(AccountSalesVisitModel obj);

        void EditVisit(AccountSalesVisit obj);

        void AddPersonsToExistVisit(IEnumerable<AccountSalesVisitPerson> list);

        void AddBrandsToExistVisit(IEnumerable<AccountSalesVisitBrand> list);

        void MakeACollection(AccountBrandPayment obj);

        //IEnumerable<CustomAccountSalesVisit> GetAllMyAccountSalesVisits(string userId);

        bool MakeACollection(MakeCollection obj);

        bool RequestNewOpenning(OpenningRequest obj);

        IEnumerable<CustomOpenningRequest> GetMyRequests(string userId);

        IEnumerable<CustomAccount> GetMyAccounts(string userId);

        IEnumerable<CustomAccountSalesVisit> GetMyVisits(string userId);

        IEnumerable<CustomAccountSalesVisit> GetMyVisitsByDate(AccountSalesVisitByDateModel obj);

        CustomAccountSalesVisit GetVisitById(int visitId);

        bool RequestDeleteAccountSales(int VisitId);

        IEnumerable<CustomAccountSalesVisit> GetASVDeleteRequests();

        bool ConfirmASVDeleting(int visitid);

        bool RejectASVDeleting(int visitid);
    }
}

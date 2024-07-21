using AMEKSA.AccountSalesVisitModels;
using AMEKSA.CustomEntities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IAccountSupportiveVisitRep
    {
        bool MakeVisit(AccountSupportiveVisitModel obj);

        public IEnumerable<CustomAccountSupportiveVisit> GetMyVisitsByDate(AccountSalesVisitByDateModel obj);

        public CustomAccountSupportiveVisit GetVisitById(int visitId);

        public bool RequestDeleteAccountSupportive(int VisitId);

        public IEnumerable<CustomAccountSupportiveVisit> GetAccountSupportiveDeleteRequests();

        public bool ConfirmAccountSupportiveVDeleting(int visitid);

        public bool RejectAccountSupportiveVDeleting(int visitid);
    }
}

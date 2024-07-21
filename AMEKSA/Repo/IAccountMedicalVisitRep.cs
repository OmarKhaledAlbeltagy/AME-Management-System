using AMEKSA.AccountMedicalVisitModels;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IAccountMedicalVisitRep
    {
        void EditVisit(AccountMedicalVisit obj);

        void AddPersonsToExistVisit(IEnumerable<AccountMedicalVisitPerson> list);

        void AddProductsToExistVisit(IEnumerable<AccountMedicalVisitProducts> list);

        IEnumerable<CustomAccountMedicalVisit> GetMyVisitsByDate(AccountSalesVisitByDateModel obj);

        bool RequestDeleteAccountMedical(int VisitId);

        CustomAccountMedicalVisit GetVisitById(int visitId);

        bool MakeVisit(AccountMedicalVisitModel obj);

        IEnumerable<CustomAccountMedicalVisit> GetAMVDeleteRequests();

        bool ConfirmAMVDeleting(int visitid);

        bool RejectAMVDeleting(int visitid);
    }
}

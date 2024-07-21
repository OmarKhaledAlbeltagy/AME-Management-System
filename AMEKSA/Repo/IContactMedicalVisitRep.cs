using AMEKSA.ContactMedicalVisitModels;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IContactMedicalVisitRep
    {
     

        void EditVisit(ContactMedicalVisit obj);

        void AddProductsToExistVisit(IEnumerable<ContactMedicalVisitProduct> list);

        void AddAidsToExistVisit(IEnumerable<ContactSalesAid> list);

        CustomContactMedicalVisit GetAccountMedicalVisitById(int id);

        IEnumerable<CustomContact> GetContactsToVisitNow(string userId);

        IEnumerable<CustomContact> GetMyContacts(string userId);

        bool MakeVisit(ContactMedicalVisitModel obj);

        IEnumerable<CustomContactMedicalVisit> GetMyVisits(string userId);

        IEnumerable<CustomContactMedicalVisit> GetVisitsByDate(AccountSalesVisitByDateModel obj);

        CustomContactMedicalVisit GetVisitById(int visitId);

        bool RequestDeleteContactMedical(int VisitId);

        IEnumerable<CustomContactMedicalVisit> GetCMVDeleteRequests();

        bool ConfirmCMVDeleting(int visitid);

        bool RejectCMVDeleting(int visitid);

        TotalTargetModel GetMyTotalTarget(string id);


    }
}

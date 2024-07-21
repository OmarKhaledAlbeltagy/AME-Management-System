using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IPurchaseTypeRep
    {
        void AddPurchaseType(PurchaseType obj);

        void DeletePurchaseType(int id);

        void EditPurchaseType(PurchaseType obj);

        PurchaseType GetPurchaseTypeById(int id);

        IEnumerable<PurchaseType> GetAllPurchaseTypes();
    }
}

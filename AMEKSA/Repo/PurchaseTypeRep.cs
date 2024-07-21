using AMEKSA.Context;
using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class PurchaseTypeRep:IPurchaseTypeRep
    {
        private readonly DbContainer db;

        public PurchaseTypeRep(DbContainer db)
        {
            this.db = db;
        }

        public void AddPurchaseType(PurchaseType obj)
        {
            db.purchaseType.Add(obj);
            db.SaveChanges();
        }

        public void DeletePurchaseType(int id)
        {
            db.purchaseType.Remove(db.purchaseType.Find(id));
            db.SaveChanges();
        }

        public void EditPurchaseType(PurchaseType obj)
        {
            PurchaseType old = db.purchaseType.Find(obj.Id);
            old.PurchaseTypeName = obj.PurchaseTypeName;
            db.SaveChanges();
        }

        public PurchaseType GetPurchaseTypeById(int id)
        {
            return db.purchaseType.Find(id);
        }

        public IEnumerable<PurchaseType> GetAllPurchaseTypes()
        {
            return db.purchaseType.Select(a => a);
        }
    }
}

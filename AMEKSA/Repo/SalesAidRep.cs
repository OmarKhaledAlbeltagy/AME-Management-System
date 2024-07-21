using AMEKSA.Context;
using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class SalesAidRep:ISalesAidRep
    {
        private readonly DbContainer db;

        public SalesAidRep(DbContainer db)
        {
            this.db = db;
        }

        public SalesAid AddSalesAid(SalesAid obj)
        {
            db.salesAid.Add(obj);
            db.SaveChanges();
            return obj;
        }

        public bool EditSalesAid(SalesAid obj)
        {
            SalesAid old = db.salesAid.Find(obj.Id);

            old.SalesAidName = obj.SalesAidName;
            old.show = obj.show;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<SalesAid> GetAllSalesAids()
        {
            return db.salesAid.Select(a => a);
        }

        public IEnumerable<SalesAid> GetShownSalesAids()
        {
            return db.salesAid.Where(a => a.show == true);
        }
    }
}

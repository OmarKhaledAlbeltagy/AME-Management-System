using AMEKSA.Context;
using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class StringPropertiesRep:IStringPropertiesRep
    {
        private readonly DbContainer db;

        public StringPropertiesRep(DbContainer db)
        {
            this.db = db;
        }

        public bool EditMotivation(string m)
        {
            StringProperties obj = db.stringProperties.FirstOrDefault();
            obj.Value = m;
            db.SaveChanges();
            return true;
        }

        public string GetMotivation()
        {
            return db.stringProperties.FirstOrDefault().Value;
        }
    }
}

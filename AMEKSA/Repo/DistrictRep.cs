using AMEKSA.Context;
using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class DistrictRep:IDisrictRep
    {
        private readonly DbContainer db;

        public DistrictRep(DbContainer db)
        {
            this.db = db;
        }

        public District AddDistrict(District obj)
        {
            db.district.Add(obj);
            db.SaveChanges();
            return obj;
        }

        public void DeleteDistrict(int id)
        {
            db.district.Remove(db.district.Find(id));
            db.SaveChanges();
        }

        public bool EditDistrict(District obj)
        {
            District old = db.district.Find(obj.Id);

            old.DistrictName = obj.DistrictName;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<District> GetAllCityDistricts(int id)
        {
            return db.district.Where(a => a.CityId == id);
        }

        public IEnumerable<District> GetAllDistricts()
        {
            return db.district.Select(a => a);
        }

        public District GetDisrictById(int id)
        {
            return db.district.Find(id);
        }
    }
}

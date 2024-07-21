using AMEKSA.Context;
using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class CityRep:ICityRep
    {
        private readonly DbContainer db;

        public CityRep(DbContainer db)
        {
            this.db = db;
        }

        public City AddCity(City obj)
        {
            db.city.Add(obj);
            db.SaveChanges();
            return obj;
        }

        public void DeleteCity(int id)
        {
            db.city.Remove(db.city.Find(id));
            db.SaveChanges();
        }

        public bool EditCity(City obj)
        {
            City old = db.city.Find(obj.Id);
            old.CityName = obj.CityName;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<City> GetAllCities()
        {
            return db.city.Select(a => a);
        }

        public City GetCityById(int id)
        {
            return db.city.Find(id);
        }
    }
}

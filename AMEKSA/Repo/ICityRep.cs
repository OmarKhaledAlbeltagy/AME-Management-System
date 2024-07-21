using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface ICityRep
    {
        City AddCity(City obj);

        void DeleteCity(int id);

        bool EditCity(City obj);

        City GetCityById(int id);

        IEnumerable<City> GetAllCities();
    }
}

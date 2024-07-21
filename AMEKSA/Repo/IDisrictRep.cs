using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IDisrictRep
    {
        District AddDistrict(District obj);

        void DeleteDistrict(int id);

        bool EditDistrict(District obj);

        District GetDisrictById(int id);

        IEnumerable<District> GetAllDistricts();

        IEnumerable<District> GetAllCityDistricts(int id);
    }
}

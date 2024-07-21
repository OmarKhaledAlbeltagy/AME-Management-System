using AMEKSA.DevModels;
using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IDevRep
    {
        bool mail();

        int Addcad(AddCad m);

        List<YearlyWorkingDaysModel> WorkingDaysYearlyReport();

        int? GetWorkingDays(int yearfrom, int monthfrom, int yearto, int monthto);

        IEnumerable<SalesKpiModel> GetAllSalesKpi();

        IEnumerable<MedicalKpiModel> GetAllMedicalKpi();

        bool changeuserpassword(string id);

        bool configurelimit();
    }
}

using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IKpisRep
    {
        IEnumerable<Entities.Properties> GetAllProperties();

        bool EditProperty(int id, int value);

        SalesKpiModel GetSalesKpi(int yearfrom, int monthfrom, int yearto, int monthto, string userId);

        IEnumerable<SalesKpiModel> GetAllSalesKpi(int yearfrom, int monthfrom, int yearto, int monthto);

        IEnumerable<SalesKpiModel> GetTeamSalesKpi(int yearfrom, int monthfrom, int yearto, int monthto, string managerId);

        SalesKpiModel GetSupportiveKpi(int yearfrom, int monthfrom, int yearto, int monthto, string userId);

        IEnumerable<SalesKpiModel> GetAllSupportiveKpi(int yearfrom, int monthfrom, int yearto, int monthto);

        IEnumerable<SalesKpiModel> GetTeamSupportiveKpi(int yearfrom, int monthfrom, int yearto, int monthto, string managerId);

        MedicalKpiModel GetMedicalKpi(int yearfrom, int monthfrom, int yearto, int monthto, string userId);

        KPIScoreModel GetMedicalKpiChart(int yearfrom, int monthfrom, int yearto, int monthto, string userId);

        IEnumerable<MedicalKpiModel> GetAllMedicalKpi(int yearfrom, int monthfrom, int yearto, int monthto);

        List<KPIScoreModel> GetAllMedicalKpiChart(int yearfrom, int monthfrom, int yearto, int monthto);

        IEnumerable<MedicalKpiModel> GetTeamMedicalKpi(int yearfrom, int monthfrom, int yearto, int monthto, string managerId);

        List<KPIScoreModel> GetTeamMedicalKpiChart(int yearfrom, int monthfrom, int yearto, int monthto, string managerId);

        int GetTimeOffDiff(string id, int yearfrom, int monthfrom, int dayfrom, int yearto, int monthto, int dayto);



    }
}

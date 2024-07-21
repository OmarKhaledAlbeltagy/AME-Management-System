using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class SalesKpiModel
    {
        public string FullName { get; set; }

        public string CityName { get; set; }

        public string RoleName { get; set; }

        public string Month { get; set; }

        public double ActualTotalNumberOfVisits { get; set; }

        public double ListedAccounts { get; set; }

        public double SellingDaysInTheField { get; set; }

        //public double? TargetedNumberOfVisits { get; set; }

        public double VisitedAccounts { get; set; }

        public double? WorkingDays { get; set; }

        public int AverageVisitsPerDayKpiTarget { get; set; }

        //public double AverageVisitsPerDayActual { get; set; }

        //public double AverageVisitsPerDayRate { get; set; }

        //public double VisitsTargetAchievmentRate { get; set; }

        //public double CoverageForListedAccountsRate { get; set; }

        //public double SellingDaysInTheFieldKpiRate { get; set; }

        public int AverageVisitsPerDayWeight { get; set; }

        public int VisitsTargetAchievmentWeight { get; set; }

        public int CoverageForListedAccountsWeight { get; set; }

        public int SellingDaysInTheFieldKpiWeight { get; set; }

        public int? TimeOffDays { get; set; }

        //public double AverageVisitsPerDayScore { get; set; }

        //public double VisitsTargetAchievmentScore { get; set; }

        //public double CoverageForListedAccountsScore { get; set; }

        //public double SellingDaysInTheFieldScore { get; set; }

        //public double TotalScore { get; set; }
    }
}

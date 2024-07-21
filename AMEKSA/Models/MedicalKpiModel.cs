using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class MedicalKpiModel
    {
        public string FullName { get; set; }

        public string CityName { get; set; }

        public string RoleName { get; set; }

        public string Month { get; set; }

        public int ActualTotalNumberOfVisits { get; set; }

        public int APlusAndAVisited { get; set; }

        public int AplusAndAListed { get; set; }

        public int BVisited { get; set; }

        public int BListed { get; set; }

        public int CVisited { get; set; }

        public int CListed { get; set; }

        public int SellingDaysInTheField { get; set; }

        public int? WorkingDays { get; set; }

        public int AverageVisitsPerDayKpiTarget { get; set; }

        public int AverageVisitsPerDayWeight { get; set; }

        public int VisitsTargetAchievmentWeight { get; set; }

        public int APlusAndAWeight { get; set; }

        public int BWeight { get; set; }

        public int CWeight { get; set; }

        public int SellingDaysInTheFieldWeight { get; set; }

        public int? TimeOffDays { get; set; }
    }
}

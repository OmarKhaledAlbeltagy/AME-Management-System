using System.Collections.Generic;

namespace AMEKSA.MonthlyPlanModels
{
    public class CustomContactPlanExcel
    {
        public string UserName { get; set; }

        public List<CustomMonthlyPlan> planlist { get; set; }
    }
}

using System.Collections.Generic;

namespace AMEKSA.MonthlyPlanModels
{
    public class CustomAccountPlanExcel
    {
        public string UserName { get; set; }

        public List<CustomMonthlyPlanSales> planlist { get; set; }
    }
}

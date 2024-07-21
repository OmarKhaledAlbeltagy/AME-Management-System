using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.MonthlyPlanModels
{
    public class CustomMonthlyPlanSales
    {
        public int Id { get; set; }

        public int OrgId { get; set; }

        public string OrgName { get; set; }

        public DateTime PlannedDate { get; set; }

        public DateTime now { get; set; }

        public bool Status { get; set; }

        public ICollection<CustomMonthlyPlanCollection> customMonthlyPlanCollection { get; set; }
    }
}

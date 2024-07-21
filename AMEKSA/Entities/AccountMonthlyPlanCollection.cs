using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class AccountMonthlyPlanCollection
    {
        public int Id { get; set; }

        public float? Collection { get; set; }

        public int BrandId { get; set; }

        public Brand brand { get; set; }

        public int AccountMonthlyPlanId { get; set; }

        public AccountMonthlyPlan accountMonthlyPlan { get; set; }
    }
}

using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.MonthlyPlanModels;
using AMEKSA.Models;

namespace AMEKSA.Repo
{
   public interface IAccountMonthlyPlanRep
    {
        AccountMonthlyPlan PlanVisit(AccountMonthlyPlan obj);

        IEnumerable<CustomMonthlyPlan> GetMyPlanThisMonth(string userId);

        IEnumerable<CustomMonthlyPlanSales> GetMyPlanThisMonthSales(string userId);

        bool DeletePlannedVisit(int id);

        bool DeletePlannedVisitSales(int id);

        IEnumerable<AccountsForPlan> GetMyAccountsForPlan(string userid);

        int PlanVisitSales(AccountMonthlyPlan obj);

        public int PlanVisitSupportive(AccountMonthlyPlan obj);
    }
}

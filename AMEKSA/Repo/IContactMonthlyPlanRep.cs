using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.MonthlyPlanModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IContactMonthlyPlanRep
    {
        ContactMonthlyPlan PlanVisit(ContactMonthlyPlan obj);

        IEnumerable<CustomMonthlyPlan> GetMyPlanThisMonth(string userId);

        bool DeletePlannedVisit(int id);

        IEnumerable<ContactsForPlan> GetMyContactsForPlan(string userid);
    }
}

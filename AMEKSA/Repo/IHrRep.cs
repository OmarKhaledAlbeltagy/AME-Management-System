using AMEKSA.Models;
using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
  public interface IHrRep
    {
        IEnumerable<DateTime> GetActualWorkingDaysMedical(string userId);

        IEnumerable<DateTime> GetActualWorkingDaysSales(string userId);

        IEnumerable<DateTime> GetTimeOffDays(string userId);

        IEnumerable<ExtendIdentityUser> GetAllSalesRep();

        IEnumerable<ExtendIdentityUser> GetAllMedicalRep();

        TenClockReportModel GetTenClockReport();
    }
}

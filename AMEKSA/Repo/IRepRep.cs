using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IRepRep
    {
        IEnumerable<RepContactVisitsReport> ContactMedicalVisitReport(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

        IEnumerable<RepContactNotVisitsReport> ContactMedicalNotVisitReport(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

        IEnumerable<RepAccountVisitsReport> AccountMedicalVisitReport(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

        IEnumerable<RepAccountVisitsReport> AccountSupportiveVisitReport(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

        IEnumerable<RepAccountNotVisitsReport> AccountMedicalNotVisitReport(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

        IEnumerable<RepAccountVisitsReport> AccountSalesVisitReport(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

        IEnumerable<RepAccountNotVisitsReport> AccountSalesNotVisitReport(string userId, int dayfrom, int monthfrom, int yearfrom, int dayto, int monthto, int yearto);

        RepVisitedContacts VisitedContactsByMonth(int year, int month, string userId);

        IEnumerable<RepVisitedContactsByCategory> VisitedContactsThisMonthByCategory(string userId);

        RepVisitedContacts VisitedAccountsByMonthSales(int year, int month, string userId);

        DateTime GetVisitsDateLimit(string userId);

        RepVisitedContacts VisitedAccountsByMonthSupportive(int year, int month, string userId);

        int GetuserVisitLimit(string userId);

        bool EditVisitLimit(string userId,int days);

        bool SetAllRepsVisitLimit(int days);
    }
}

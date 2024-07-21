using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface ITimeOffRep
    {
        bool SetWorkingDays(int month, int year,int workingdays);

        IEnumerable<WorkingDaysModel> GetWorkingDaysSet();

        IEnumerable<CustomActualWorkingDays> CaculateActualWorkingDaysByMonth(int year, int month,string managerId);

        IEnumerable<CustomActualWorkingDays> CaculateActualWorkingDaysByMonthTopManager(int year, int month);

        bool EditWorkingDays(int Id, int WorkingDays);

        TimeOffMail MakeTimeOffTerritory(UserTimeOff obj);

        IEnumerable<DateTime> Getmytimesoff(string userId);

        IEnumerable<TimeOffTerritoryReasons> GetTimeOffTerritoryReasons();

        IEnumerable<CustomTimeOffTerritory> GetMyTimeOffData(string userId);

        bool DeleteTimeOffTerritory(int id);

        IEnumerable<CustomTimeOffTerritory> GetMyTeamTimeOff(string ManagerId);

        IEnumerable<CustomTimeOffTerritory> GetAllTimeOff();

        bool MakeVacancyRequest(VacancyRequests obj);

        IEnumerable<TimeOffTerritoryReasons> GetVacancyReasons();

        IEnumerable<CustomVacancyRequests> GetMyVacancies(string userId);

        IEnumerable<CustomVacancyRequests> GetMyTeamVacanciesRequests(string ManagerId);

        bool RejectVacancyRequest(int id);

        bool AcceptVacancyRequest(int id);

        IEnumerable<CustomActualWorkingDays> CaculateActualWorkingDaysByMonthMedical(string userId);

        IEnumerable<CustomActualWorkingDays> CaculateActualWorkingDaysByMonthSales(string userId);

        IEnumerable<CustomTimeOffTerritory> GetTimeOffRequestsToTakeAction(string userId);

        IEnumerable<CustomTimeOffTerritory> GetMyTimeOffStatus(string userId);

        bool AcceptTimeOff(int id);

        bool RejectTimeOff(int id);

        Task mail(TimeOffMail res);

        TimeOffMail GetTimeOffById(int id);

        
    }
}

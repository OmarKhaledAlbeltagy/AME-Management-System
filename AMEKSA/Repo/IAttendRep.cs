using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
  public interface IAttendRep
    {
        int ConfirmAttend(byte attend, int cityid);

        bool ConfirmForm(int id, string first, string last, string phone);

        bool ReConfirmAttend(int id);

        bool RejectForm(int id, string why);

        AttendCountModel GetAttendCount();

        IEnumerable<Attend> GetAllComingData(int cityid);

        IEnumerable<Attend> GetAllNotComingData(int cityid);

        AttendCountModelCity GetAttendCountCity(string ManagerId);

        string Came(int id);

        int ConfirmFormInEvent(string first, string last, string phone,int CityId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Models;
namespace AMEKSA.Repo
{
   public interface INotificationRep
    {
        bool SetAsSeen(string UserId);

        MainNotificationModel GetLastFifteen(string userid);

        MainNotificationModel GetLastMonth(string userid);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class MainNotificationModel
    {
        public int NotSeenNotificationsCount { get; set; }

        public List<DetailedNotificationsModel> list { get; set; }
    }
}

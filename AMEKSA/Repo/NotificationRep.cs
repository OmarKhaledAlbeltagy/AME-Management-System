using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class NotificationRep:INotificationRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;

        public NotificationRep(DbContainer db, ITimeRep ti)
        {
            this.db = db;
            this.ti = ti;
        }

        public MainNotificationModel GetLastFifteen(string userid)
        {
            DateTime today = ti.GetCurrentTime();
            IEnumerable<Notifications> n = db.notifications.Where(a => a.ExtendIdetityUserId == userid).OrderByDescending(a=>a.NitificationDateTime).Take(15);
            int notseen = db.notifications.Where(a => a.ExtendIdetityUserId == userid && a.Seen == false).Count();
            MainNotificationModel res = new MainNotificationModel();
            List<DetailedNotificationsModel> l = new List<DetailedNotificationsModel>();
            res.NotSeenNotificationsCount = notseen;
            foreach (var item in n)
            {
                DetailedNotificationsModel x = new DetailedNotificationsModel();
                x.Id = item.Id;
                x.NotificationsDetails = item.NotificationDetails;
                x.UserId = item.ExtendIdetityUserId;
                x.dt = item.NitificationDateTime;
                x.Url = item.Url;
                if(item.NitificationDateTime.Date == today.Date)
                {
                    x.DateTime = "Today";
                }
                else
                {
                    if(item.NitificationDateTime.Date == today.Date.AddDays(-1))
                    {
                        x.DateTime = "Yesterday";
                    }
                    else
                    {
                        if (item.NitificationDateTime.Year == today.Year)
                        {
                            x.DateTime = item.NitificationDateTime.ToString("dd MMMM");
                        }
                        else
                        {
                            x.DateTime = item.NitificationDateTime.ToString("dd/MM/yyyy");
                        }
                    }
                }

                l.Add(x);
               
            }
            res.list = l;
            return res;

        }

        public MainNotificationModel GetLastMonth(string userid)
        {
            DateTime today = ti.GetCurrentTime();
            DateTime start = today.AddMonths(-1);
            IEnumerable<Notifications> n = db.notifications.Where(a => a.ExtendIdetityUserId == userid && a.NitificationDateTime.Date >= start.Date && a.NitificationDateTime.Date <= today.Date).OrderByDescending(a => a.NitificationDateTime);
            int notseen = db.notifications.Where(a => a.ExtendIdetityUserId == userid && a.Seen == false).Count();
            MainNotificationModel res = new MainNotificationModel();
            List<DetailedNotificationsModel> l = new List<DetailedNotificationsModel>();
            res.NotSeenNotificationsCount = notseen;
            foreach (var item in n)
            {
                DetailedNotificationsModel x = new DetailedNotificationsModel();
                x.Id = item.Id;
                x.NotificationsDetails = item.NotificationDetails;
                x.UserId = item.ExtendIdetityUserId;
                x.Url = item.Url;
                if (item.NitificationDateTime.Date == today.Date)
                {
                    x.DateTime = "Today";
                }
                else
                {
                    if (item.NitificationDateTime.Date == today.Date.AddDays(-1))
                    {
                        x.DateTime = "Yesterday";
                    }
                    else
                    {
                        if (item.NitificationDateTime.Year == today.Year)
                        {
                            x.DateTime = item.NitificationDateTime.ToString("dd MMMM");
                        }
                        else
                        {
                            x.DateTime = item.NitificationDateTime.ToString("dd/MM/yyyy");
                        }
                    }
                }

                l.Add(x);

            }
            res.list = l;
            return res;
        }

        public bool SetAsSeen(string UserId)
        {
          List<Notifications> n = db.notifications.Where(a=>a.ExtendIdetityUserId == UserId).ToList();
            foreach (var item in n)
            {
                item.Seen = true;
              
            }
            db.SaveChanges();
            return true;
        }
    }
}

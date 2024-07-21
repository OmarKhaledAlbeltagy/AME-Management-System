using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class ChangeContactTargetRep:IChangeContactTargetRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public ChangeContactTargetRep(DbContainer db, ITimeRep ti, UserManager<ExtendIdentityUser> userManager)
        {
            this.db = db;
            this.ti = ti;
            this.userManager = userManager;
        }

        public bool AcceptRequest(int id)
        {
            RequestChangeContactTarget res = db.requestChangeContactTargets.Find(id);
            UserContact UC = db.userContact.Where(a => a.ContactId == res.ContactId && a.extendidentityuserid == res.RepId).FirstOrDefault();
            UC.MonthlyTarget = res.NewTarget;
            res.Status = true;
            Contact c = db.contact.Find(res.ContactId);
            Notifications n = new Notifications();
            n.NitificationDateTime = ti.GetCurrentTime();
            n.ExtendIdetityUserId = res.RepId;
            n.NotificationDetails = "Your request to change contact " + c.ContactName + " monthly target has been accepted";
            n.Url = "MyContacts.html";
            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }

        public bool ChangeTarget(ContactChangeTargetModel obj)
        {
            UserContact res = db.userContact.Where(a => a.ContactId == obj.ContactId && a.extendidentityuserid == obj.UserId).FirstOrDefault();
            res.MonthlyTarget = obj.NewTarget;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<CustomRequestContactChangeTarget> GetMyTeamRequests(string id)
        {
            IEnumerable<CustomRequestContactChangeTarget> res = db.requestChangeContactTargets.Where(a => a.ManagerId == id && a.Status == null).Join(db.Users, a => a.RepId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                ContactId = a.ContactId,
                RepName = b.FullName,
                OldTarget = a.OldTarget,
                NewTarget = a.NewTarget,
                Status = a.Status,
                RequestDateTime = a.RequestDateTime
            }).Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new CustomRequestContactChangeTarget
            {
                Id = a.Id,
                ContactName = b.ContactName,
                RepName = a.RepName,
                TargetFrom = a.OldTarget,
                TargetTo = a.NewTarget,
                Status = a.Status,
                DT = a.RequestDateTime,
                RequestDateTime = a.RequestDateTime.ToString("dd MMMM yyyy - hh:mm tt")
            });

            return res.OrderByDescending(a=>a.DT);
        }

        public bool RejectRequest(int id)
        {
            RequestChangeContactTarget res = db.requestChangeContactTargets.Find(id);
            res.Status = false;
            Contact c = db.contact.Find(res.ContactId);
            Notifications n = new Notifications();
            n.NitificationDateTime = ti.GetCurrentTime();
            n.ExtendIdetityUserId = res.RepId;
            n.NotificationDetails = "Your request to change contact " + c.ContactName + " monthly target has been rejected";
            n.Url = "MyContacts.html";
            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }

        public bool RequestChangeTarget(RequestChangeContactTarget obj)
        {
            ExtendIdentityUser user = userManager.FindByIdAsync(obj.RepId).Result;
            Contact c = db.contact.Find(obj.ContactId);
            string ManagerId = db.Users.Find(obj.RepId).extendidentityuserid;
            Notifications n = new Notifications();
            n.NitificationDateTime = ti.GetCurrentTime();
            n.ExtendIdetityUserId = ManagerId;
            n.NotificationDetails = user.FullName + " requested to change contact " + c.ContactName + " Monthly Target";
            n.Url = "TargetRequests.html";
            obj.ManagerId = ManagerId;
            obj.RequestDateTime = ti.GetCurrentTime();
            db.requestChangeContactTargets.Add(obj);
            db.notifications.Add(n);
            db.SaveChanges();
            return true;
        }
    }
}

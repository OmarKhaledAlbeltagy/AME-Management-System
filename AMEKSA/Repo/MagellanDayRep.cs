using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMEKSA.Repo
{
    public class MagellanDayRep:IMagellanDayRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;

        public MagellanDayRep(DbContainer db, ITimeRep ti)
        {
            this.db = db;
            this.ti = ti;
        }

        public bool ConfirmAttendance(int id)
        {
         List<MagellanDayAttendance> m = db.magellanDayAttendance.Where(a => a.ContactId == id).ToList();

            if (m == null || m.Count == 0)
            {
                DateTime now = ti.GetCurrentTime();
                MagellanDayAttendance obj = new MagellanDayAttendance();
                obj.ContactId = id;
                obj.AttendanceTime = now;
                db.magellanDayAttendance.Add(obj);
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
          
        }

        public List<MagellanDayContactSearch> GetAttended()
        {
            List<MagellanDayContactSearch> res = db.magellanDayAttendance.Join(db.contact, a => a.ContactId, b => b.Id, (a, b) => new
            {
                Id = a.Id,
                AccountId = b.AccountId,
                ContactName = b.ContactName,
                Time = a.AttendanceTime.ToString("dd MMMM yyyy - hh:mm tt")
            }).Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new MagellanDayContactSearch
            {
                Id = a.Id,
                ContactName = a.ContactName,
                AccountName = b.AccountName,
                Time = a.Time
            }).OrderBy(a=>a.ContactName).ToList();
            return res;
        }

        public List<MagellanDayContactSearch> GetContact()
        {
  

            List<MagellanDayContactSearch> res = db.contact.Join(db.account, a => a.AccountId, b => b.Id, (a, b) => new MagellanDayContactSearch
            {
                Id = a.Id,
                ContactName = a.ContactName,
                AccountName = b.AccountName
            }).ToList();
            return res;
        }


    }
}

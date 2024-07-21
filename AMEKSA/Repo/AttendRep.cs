using AMEKSA.Context;
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
    public class AttendRep:IAttendRep
    {
        private readonly DbContainer db;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public AttendRep(DbContainer db, UserManager<ExtendIdentityUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public string Came(int id)
        {
            Attend obj = db.attend.Find(id);
            obj.Came = true;
            var n = obj.FirstName;
            db.SaveChanges();
            return n;
        }

        public int ConfirmAttend(byte attend, int cityid)
        {
            Attend obj = new Attend();
            if (attend == 1)
            {
                
                obj.Attendd = true;
                obj.CityId = cityid;
                db.attend.Add(obj);
                db.SaveChanges();
                return obj.Id;
            }
            else
            {
               
                
                    obj.Attendd = false;
                    obj.CityId = cityid;
                    db.attend.Add(obj);
                    db.SaveChanges();
                    return obj.Id;
              
            }
        }

        public bool ConfirmForm(int id, string first, string last, string phone)
        {
            Attend x = db.attend.Find(id);
            x.FirstName = first;
            x.LastName = last;
            x.PhoneNumber = phone;
            db.SaveChanges();
            return true;
        }

        public int ConfirmFormInEvent(string first, string last, string phone, int CityId)
        {
            Attend x = new Attend();
            x.Attendd = true;
            x.FirstName = first;
            x.LastName = last;
            x.PhoneNumber = phone;
            x.CityId = CityId;
            x.Came = true;
            db.attend.Add(x);
            db.SaveChanges();
            return x.Id;
        }

        public IEnumerable<Attend> GetAllComingData(int cityid)
        {
            IEnumerable<Attend> res = db.attend.Where(a => a.CityId == cityid && a.Attendd == true && a.FirstName != null);

            return res;
        }

        public IEnumerable<Attend> GetAllNotComingData(int cityid)
        {
            IEnumerable<Attend> res = db.attend.Where(a => a.CityId == cityid && a.Attendd == false && a.why != null);
            return res;
        }

        public AttendCountModel GetAttendCount()
        {
            int JeddahComing = db.attend.Where(a => a.CityId == 3 && a.Attendd == true).Count();
            int JeddahNotComing = db.attend.Where(a => a.CityId == 3 && a.Attendd == false).Count();

            int RiyadhComing = db.attend.Where(a => a.CityId == 1 && a.Attendd == true).Count();
            int RiyadhNotComing = db.attend.Where(a => a.CityId == 1 && a.Attendd == false).Count();

            int DammamComing = db.attend.Where(a => a.CityId == 2 && a.Attendd == true).Count();
            int DammamNotComing = db.attend.Where(a => a.CityId == 2 && a.Attendd == false).Count();

            int TotalComing = JeddahComing + RiyadhComing + DammamComing;
            int TotalNotComing = JeddahNotComing + RiyadhNotComing + DammamNotComing;

            AttendCountModel x = new AttendCountModel();
            x.JeddahComing = JeddahComing;
            x.JeddahNotComing = JeddahNotComing;
            x.DammamComing = DammamComing;
            x.DammamNotComing = DammamNotComing;
            x.RiyadhComing = RiyadhComing;
            x.RiyadhNotComing = RiyadhNotComing;
            x.TotalComing = TotalComing;
            x.TotalNotComing = TotalNotComing;

            return x;

        }

        public AttendCountModelCity GetAttendCountCity(string ManagerId)
        {
            int? CityId = userManager.FindByIdAsync(ManagerId).Result.CityId;
            string name = db.city.Find(CityId).CityName;

            int Coming = db.attend.Where(a => a.CityId == CityId && a.Attendd == true).Count();
            int NotComing = db.attend.Where(a => a.CityId == CityId && a.Attendd == false).Count();

            AttendCountModelCity res = new AttendCountModelCity();
            res.CityName = name;
            res.Coming = Coming;
            res.NotComing = NotComing;

            return res;
        }

        public bool ReConfirmAttend(int id)
        {
            Attend obj = db.attend.Find(id);
            obj.Attendd = true;
            db.SaveChanges();
            return true;
        }

        public bool RejectForm(int id, string why)
        {
            Attend obj = db.attend.Find(id);
            obj.why = why;
            db.SaveChanges();
            return true;
        }

      
    }
}

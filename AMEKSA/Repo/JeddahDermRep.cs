using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AMEKSA.Repo
{
    public class JeddahDermRep : IJeddahDermRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;

        public JeddahDermRep(DbContainer db, ITimeRep ti)
        {
            this.db = db;
            this.ti = ti;
        }

        public bool Accept(SaamRegisterModel r)
        {
            JeddahDerm s = db.jeddahDerm.Find(r.Id);
            s.IsComing = true;
            s.Email = r.Email;
            s.PhoneNumber = r.PhoneNumber;
            s.HSAN = r.HSAN;
            s.Date = ti.GetCurrentTime();
            s.workshop = (int)r.Workshop;
            db.SaveChanges();
            return true;
        }

        public bool AddJeddaDermBoth(JeddaDermBothModel obj)
        {
            DateTime now = ti.GetCurrentTime();
            JeddaDermBoth j = new JeddaDermBoth();
            j.FullName = obj.FullName;
            j.MobileNumber = obj.MobileNumber;
            j.City = obj.City;
            j.Clinic = obj.Clinic;
            j.ExtendIdentityUserId = obj.ExtendIdentityUserId;
            j.AboutWhatQuery = obj.AboutWhatQuery;
            j.datetime = now;
            db.jeddaDermBoth.Add(j);
            db.SaveChanges();
            return true;
        }

        public SaamDataModel AnonRegister(SaamAnonRegisterModel s)
        {
            JeddahDerm saam = db.jeddahDerm.Where(a => a.HSAN == s.HSAN).FirstOrDefault();

            if (saam == null)
            {
                Contact c = new Contact();
                c.ContactTypeId = 11;
                c.AccountId = 738;
                c.ContactName = s.FullName;
                c.DistrictId = 1;
                c.Guidd = Guid.NewGuid();
                c.MobileNumber = s.PhoneNumber;
                c.Email = s.Email;
                c.RelationshipNote = "JeddaDerm Registration";
                db.contact.Add(c);
                db.SaveChanges();

                List<JeddahDerm> saamlist = db.jeddahDerm.ToList();

                int code;
                if (saamlist == null || saamlist.Count == 0)
                {
                    code = 1001;
                }
                else
                {
                    int maxcode = db.jeddahDerm.Select(a => a.RegistrationCode).Max();
                    code = (int)maxcode + 1;
                }

                JeddahDerm obj = new JeddahDerm();
                obj.ContactId = c.Id;
                obj.Date = ti.GetCurrentTime();
                obj.RegistrationCode = code;
                obj.guidd = c.Guidd;
                obj.IsComing = true;
                obj.Email = c.Email;
                obj.PhoneNumber = c.MobileNumber;
                obj.HSAN = s.HSAN;
                obj.workshop = (int)s.Workshop;
                db.jeddahDerm.Add(obj);
                db.SaveChanges();
                SaamDataModel res = new SaamDataModel();
                res.Id = obj.Id;
                res.ContactId = obj.ContactId;
                res.ContactName = c.ContactName;
                res.Email = c.Email;
                res.PhoneNumber = c.MobileNumber;
                res.hsan = obj.HSAN;
                res.IsComing = obj.IsComing;
                res.Workshop = obj.workshop;
                return res;
            }
            else
            {
                string cname = db.contact.Find(saam.ContactId).ContactName;
                string email = db.contact.Find(saam.ContactId).Email;
                string e = email.Split('@')[0];
                string a = email.Split('@')[1];
                string phone = db.contact.Find(saam.ContactId).MobileNumber;
                SaamDataModel res = new SaamDataModel();
                res.Id = 0;
                res.ContactId = saam.ContactId;
                res.ContactName = "Dr. " + "**********" + cname.Substring(cname.Length - 4);
                res.Email = "**********" + e.Substring(e.Length - 4) + "@" + a;
                res.PhoneNumber = "**********" + phone.Substring(phone.Length - 4);
                res.hsan = "**********" + saam.HSAN.Substring(saam.HSAN.Length - 4);
                res.Workshop = saam.workshop;
                res.IsComing = saam.IsComing;
                return res;
            }



        }

        public SaamRegisterModel ChangeInfo(int id)
        {
            throw new NotImplementedException();
        }

        public bool EditRegister(SaamAnonRegisterModel s)
        {
            JeddahDerm saam = db.jeddahDerm.Find(s.Id);
            saam.PhoneNumber = s.PhoneNumber;
            saam.Email = s.Email;
            saam.HSAN = s.HSAN;
            saam.workshop = (int)s.Workshop;
            Contact c = db.contact.Find(saam.ContactId);
            c.ContactName = s.FullName;
            c.Email = s.Email;
            c.MobileNumber = s.PhoneNumber;
            db.SaveChanges();
            return true;
        }

        public List<SaamAnonRegisterModel> GetAll()
        {
            List<JeddahDerm> l = db.jeddahDerm.ToList();
            List<SaamAnonRegisterModel> res = new List<SaamAnonRegisterModel>();
            foreach (var item in l)
            {
                SaamAnonRegisterModel s = new SaamAnonRegisterModel();
                s.FullName = db.contact.Find(item.ContactId).ContactName;
                s.PhoneNumber = item.PhoneNumber;
                s.Email = item.Email;
                s.HSAN = item.HSAN;
                s.Workshop = item.workshop;
                res.Add(s);
            }

            return res;
        }

        public List<JeddaDermBothExcelModel> GetBothData()
        {
            List<JeddaDermBothExcelModel> res = db.jeddaDermBoth.Join(db.Users, a => a.ExtendIdentityUserId, b => b.Id, (a, b) => new JeddaDermBothExcelModel
            {
                Id = a.Id,
                FullName = a.FullName,
                MobileNumber = a.MobileNumber,
                Clinic = a.Clinic,
                City = a.City,
                AboutWhatQuery = a.AboutWhatQuery,
                datetime = a.datetime.ToString("dd MMMM hh:mm tt"),
                UserName = b.FullName
            }).ToList();

            return res;
        }

        public string GetDoctorName(string g)
        {
            throw new System.NotImplementedException();
        }

        public SaamInvitationModel GetInvitationData(int id)
        {
            Contact c = db.contact.Find(id);
            SaamInvitationModel s = new SaamInvitationModel();
            s.Id = c.Id;
            s.ContactName = c.ContactName;
            s.Email = c.Email;
            s.PhoneNumber = c.MobileNumber;
            s.guidd = c.Guidd.ToString();
            return s;
        }

        public List<SaamReport> GetSaamReport()
        {
            throw new System.NotImplementedException();
        }

        public SaamDataModel Start(string gu)
        {
            Guid g = new Guid(gu);
            Contact c = db.contact.Where(a => a.Guidd == g).FirstOrDefault();
            JeddahDerm s = db.jeddahDerm.Where(a => a.ContactId == c.Id).FirstOrDefault();
            if (s == null)
            {
                List<JeddahDerm> saamlist = db.jeddahDerm.ToList();

                int code;
                if (saamlist == null || saamlist.Count == 0)
                {
                    code = 1001;
                }
                else
                {
                    int maxcode = db.jeddahDerm.Select(a => a.RegistrationCode).Max();
                    code = (int)maxcode + 1;
                }

                JeddahDerm obj = new JeddahDerm();
                obj.ContactId = c.Id;
                obj.Date = ti.GetCurrentTime();
                obj.RegistrationCode = code;
                obj.guidd = g;
                db.jeddahDerm.Add(obj);
                db.SaveChanges();
                SaamDataModel res = new SaamDataModel();
                res.Id = obj.Id;
                res.ContactId = obj.ContactId;
                res.ContactName = c.ContactName;
                res.Email = c.Email;
                res.PhoneNumber = c.MobileNumber;
                res.hsan = obj.HSAN;
                res.IsComing = obj.IsComing;
                return res;
            }
            else
            {
                SaamDataModel res = new SaamDataModel();
                res.Id = s.Id;
                res.ContactId = s.ContactId;
                res.ContactName = db.contact.Find(s.ContactId).ContactName;
                res.Email = c.Email;
                res.PhoneNumber = c.MobileNumber;
                res.IsComing = s.IsComing;
                res.Workshop = s.workshop;
                return res;
            }
        }
    }
}

using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AMEKSA.Repo
{
    public class DermRep:IDermRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;

        public DermRep(DbContainer db, ITimeRep ti)
        {
            this.db = db;
            this.ti = ti;
        }


        public bool Accept(SaamRegisterModel r)
        {
            SaudiDerm s = db.saudiDerm.Find(r.Id);
            s.IsComing = true;
            s.Email = r.Email;
            s.PhoneNumber = r.PhoneNumber;
            s.HSAN = r.HSAN;
            s.Date = ti.GetCurrentTime();
            db.SaveChanges();
            return true;
        }

        public SaamDataModel AnonRegister(SaamAnonRegisterModel s)
        {
            SaudiDerm saam = db.saudiDerm.Where(a => a.HSAN == s.HSAN).FirstOrDefault();

            if (saam == null)
            {
                Contact c = new Contact();
                c.ContactTypeId = 12;
                c.AccountId = 738;
                c.ContactName = s.FullName;
                c.DistrictId = 1;
                c.Guidd = Guid.NewGuid();
                c.MobileNumber = s.PhoneNumber;
                c.Email = s.Email;
                c.RelationshipNote = "Saudi Derm Registration";
                db.contact.Add(c);
                db.SaveChanges();

                List<SaudiDerm> saamlist = db.saudiDerm.ToList();

                int code;
                if (saamlist == null || saamlist.Count == 0)
                {
                    code = 1001;
                }
                else
                {
                    int maxcode = db.saudiDerm.Select(a => a.RegistrationCode).Max();
                    code = (int)maxcode + 1;
                }

                SaudiDerm obj = new SaudiDerm();
                obj.ContactId = c.Id;
                obj.Date = ti.GetCurrentTime();
                obj.RegistrationCode = code;
                obj.guidd = c.Guidd;
                obj.IsComing = true;
                obj.Email = c.Email;
                obj.PhoneNumber = c.MobileNumber;
                obj.HSAN = s.HSAN;
                db.saudiDerm.Add(obj);
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
                res.IsComing = saam.IsComing;
                return res;
            }



        }

        public SaamRegisterModel ChangeInfo(int id)
        {
            throw new System.NotImplementedException();
        }

        public bool EditRegister(SaamAnonRegisterModel s)
        {
            SaudiDerm saam = db.saudiDerm.Find(s.Id);
            saam.PhoneNumber = s.PhoneNumber;
            saam.Email = s.Email;
            saam.HSAN = s.HSAN;
            Contact c = db.contact.Find(saam.ContactId);
            c.ContactName = s.FullName;
            c.Email = s.Email;
            c.MobileNumber = s.PhoneNumber;
            db.SaveChanges();
            return true;
        }

        public List<SaamAnonRegisterModel> GetAll()
        {
            List<SaudiDerm> l = db.saudiDerm.ToList();
            List<SaamAnonRegisterModel> res = new List<SaamAnonRegisterModel>();
            foreach (var item in l)
            {
                SaamAnonRegisterModel s = new SaamAnonRegisterModel();
                s.FullName = db.contact.Find(item.ContactId).ContactName;
                s.PhoneNumber = item.PhoneNumber;
                s.Email = item.Email;
                s.HSAN = item.HSAN;
                res.Add(s);
            }

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
            SaudiDerm s = db.saudiDerm.Where(a => a.ContactId == c.Id).FirstOrDefault();
            if (s == null)
            {
                List<SaudiDerm> saamlist = db.saudiDerm.ToList();

                int code;
                if (saamlist == null || saamlist.Count == 0)
                {
                    code = 1001;
                }
                else
                {
                    int maxcode = db.saam.Select(a => a.RegistrationCode).Max();
                    code = (int)maxcode + 1;
                }

                SaudiDerm obj = new SaudiDerm();
                obj.ContactId = c.Id;
                obj.Date = ti.GetCurrentTime();
                obj.RegistrationCode = code;
                obj.guidd = g;
                db.saudiDerm.Add(obj);
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
                return res;
            }
        }
    }
}

using AMEKSA.Context;
using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class ContactTypeRep:IContactTypeRep
    {
        private readonly DbContainer db;

        public ContactTypeRep(DbContainer db)
        {
            this.db = db;
        }

        public ContactType AddContactType(ContactType obj)
        {
            db.contactType.Add(obj);
            db.SaveChanges();
            return obj;
        }

        public void DeleteContactType(int id)
        {
            db.contactType.Remove(db.contactType.Find(id));
            db.SaveChanges();
        }

        public bool EditContactType(ContactType obj)
        {
            ContactType old = db.contactType.Find(obj.Id);
            old.ContactTypeName = obj.ContactTypeName;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<ContactType> GetAllContactTypes()
        {
            return db.contactType.Select(a => a);
        }

        public ContactType GetContactTypeById(int id)
        {
            return db.contactType.Find(id);
        }
    }
}

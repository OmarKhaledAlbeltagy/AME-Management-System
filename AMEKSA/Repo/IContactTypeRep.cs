using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IContactTypeRep
    {
        ContactType AddContactType(ContactType obj);

        void DeleteContactType(int id);

        bool EditContactType(ContactType obj);

        IEnumerable<ContactType> GetAllContactTypes();

        ContactType GetContactTypeById(int id);
    }
}

using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IContactRep
    {
        bool AddContact(Contact obj);

        bool DeleteContact(int id);

        bool EditContactGeneralInfo(ContactGeneralInfo obj);

        bool EditContactLocationInfo(ContactLocationInfo obj);

        bool EditContactContactinfo(ContactContactInfo obj);

        bool EditContactTimeInfo(ContactTimeInfo obj);

        bool EditContactNotesInfo(ContactNoteInfo obj);

        IEnumerable<CustomContact> GetAllContacts();

        CustomContact GetContactByIdWithAccount(int id);

        CustomContact GetContactByIdWithoutAccount(int id);

        IEnumerable<CustomContact> GetAllContactsFiltered(FilteringContactsModel obj);

        IEnumerable<CustomContact> SearchContact(SearchByWord contactName);


        bool AddHsan(AddHsanModel obj);

        bool AddHsanByContactId(AddHsanModel obj);

        bool AddHsanDirect(AddHsanModel obj);


    }
}

using AMEKSA.CustomEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class ContactsPerUserModel
    {
        public string FullName { get; set; }

        public List<CustomContact> Contacts { get; set; }
    }
}

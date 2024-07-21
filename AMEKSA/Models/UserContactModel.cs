using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class UserContactModel
    {
        public string UserId { get; set; }

        public int? ContactId { get; set; }

        public List<ContactCategoryModel> ContactsIds { get; set; }
    }
}

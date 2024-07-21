using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class RequestDeleteContactMedical
    {
        public int Id { get; set; }

        public int ContactMedicalVisitId { get; set; }

        public ContactMedicalVisit contactmedicalvisit { get; set; }
    }
}

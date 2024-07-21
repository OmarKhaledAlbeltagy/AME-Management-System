using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class RequestDeleteAccountMedical
    {
        public int Id { get; set; }

        public int AccountMedicalVisitId { get; set; }

        public AccountMedicalVisit accountmedicalvisit { get; set; }
    }
}

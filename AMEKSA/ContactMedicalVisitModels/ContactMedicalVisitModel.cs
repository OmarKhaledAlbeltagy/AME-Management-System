using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.ContactMedicalVisitModels
{
    public class ContactMedicalVisitModel
    {
        public int Id { get; set; }

        public string extendidentityuserid { get; set; }

        public int ContactId { get; set; }

        public DateTime VisitDate { get; set; }

        public DateTime VisitTime { get; set; }

        public string VisitNotes { get; set; }

        public string Requests { get; set; }

        public string ManagerId { get; set; }

        public IEnumerable<ContactMedicalVisitAids> aids { get; set; }

        public IEnumerable<ContactMedicalVisitNewAidModel> newaids { get; set; }

        public IEnumerable<ContactMedicalVisitProductModel> products { get; set; }
    }
}

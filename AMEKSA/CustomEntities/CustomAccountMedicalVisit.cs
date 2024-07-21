using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomAccountMedicalVisit
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; }

        public string DistrictName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public int? NumberOfDoctors { get; set; }

        public string AccountTypeName { get; set; }

        public string CategoryName { get; set; }

        public string PurchaseTypeName { get; set; }

        public string Address { get; set; }

        public DateTime VisitDate { get; set; }

        public DateTime VisitTime { get; set; }

        public DateTime SubmittingDate { get; set; }

        public DateTime SubmittingTime { get; set; }

        public string VisitNotes { get; set; }

        public string AdditionalNotes { get; set; }

        public List<CustomAccountMedicalVisitProducts> product { get; set; }

        public List<CustomVisitPerson> person { get; set; }

        public bool Requested { get; set; }
    }
}

using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomContactMedicalVisit
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public int ContactId { get; set; }

        public string ContactName { get; set; }

        public string AccountName { get; set; }

        public DateTime VisitDate { get; set; }

        public DateTime VisitTime { get; set; }

        public DateTime SubmittingDate { get; set; }

        public DateTime SubmittingTime { get; set; }

        public string ContactTypeName { get; set; }

        public string DistrictName { get; set; }

        public string VisitNotes { get; set; }

        public string Requests { get; set; }

        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string PurchaseTypeName { get; set; }

        public string PhoneNumber { get; set; }

        public string LandLineNumber { get; set; }

        public string MobileNumber { get; set; }

        public IEnumerable<CustomContactSalesAid> customcontactsalesaid { get; set; }

        public IEnumerable<CustomContactMedicalVisitProducts> customcontactmedicalvisitproduct { get; set; }

        public bool Requested { get; set; }


    }
}

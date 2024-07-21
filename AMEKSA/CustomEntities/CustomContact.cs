using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomContact
    {
        public int Id { get; set; }

        public string ContactName { get; set; }

        public bool Gender { get; set; }

        public int? DistrictId { get; set; }

        public string DistrictName { get; set; }

        public int? CityId { get; set; }

        public string CityName { get; set; }

        public string Address { get; set; }

        public string LandLineNumber { get; set; }

        public string MobileNumber { get; set; }

        public string Email { get; set; }

        public int? ContactTypeId { get; set; }

        public string ContactTypeName { get; set; }

        public string PaymentNotes { get; set; }

        public string RelationshipNote { get; set; }

        public DateTime? BestTimeFrom { get; set; }

        public DateTime? BestTimeTo { get; set; }

        public int? PurchaseTypeId { get; set; }

        public string PurchaseTypeName { get; set; }

        public int? AccountId { get; set; }

        public string AccountName { get; set; }

        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int? NumberOfMedicalVisits { get; set; }

        public int? MonthlyTarget { get; set; }

        public int? CurrentVisits { get; set; }

        public bool? CategoryRequest { get; set; }

        public bool? TargetRequest { get; set; }

        public string? Hsan { get; set; }

        public IEnumerable<string> FirstLineNames { get; set; }

        public IEnumerable<string> MedicalNames { get; set; }

        public List<Event> events { get; set; }
    }
}

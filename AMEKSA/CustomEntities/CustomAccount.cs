using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomAccount
    {
        public int Id { get; set; }

        public string AccountName { get; set; }

        public int? AccountTypeId { get; set; }

        public string AccountTypeName { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public int? DistrictId { get; set; }

        public string DistrictName { get; set; }

        public int? CityId { get; set; }

        public string CityName { get; set; }

        public string Email { get; set; }

        public int? NumberOfDoctors { get; set; }

        public int? PurchaseTypeId { get; set; }

        public string PurchaseTypeName { get; set; }

        public string PaymentNote { get; set; }

        public DateTime? BestTimeFrom { get; set; }

        public DateTime? BestTimeTo { get; set; }

        public string RelationshipNote { get; set; }

        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int? NumberOfMedicalVisits { get; set; }

        public int? NumberOfSalesVisits { get; set; }

        public int? NumberOfSupportiveVisits { get; set; }

        public IEnumerable<string> FirstLineNames { get; set; }

        public IEnumerable<string> MedicalsNames { get; set; }

        public IEnumerable<string> SupportiveNames { get; set; }

        public IEnumerable<string> SalesNames { get; set; }

        public IEnumerable<string> ContactsNames { get; set; }
    }
}

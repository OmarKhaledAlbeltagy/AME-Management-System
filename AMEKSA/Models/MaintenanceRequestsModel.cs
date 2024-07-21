using AMEKSA.Entities;
using System;

namespace AMEKSA.Models
{
    public class MaintenanceRequestsModel
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string AccountName { get; set; }

        public string ProductName { get; set; }

        public string SerialNumber { get; set; }

        public string? Model { get; set; }

        public string MobileNumber { get; set; }

        public string? LandLineNumber { get; set; }

        public string? Email { get; set; }

        public string brief { get; set; }

        public string RequestDateTime { get; set; }

        public DateTime d { get; set; }

        public bool UnderWarranty { get; set; } = false;

        public string Warranty { get; set; }

        public bool UnderContract { get; set; } = false;

        public string Contract { get; set; }

        public Guid guid { get; set; }

        public string? EngineerName { get; set; }
    }
}

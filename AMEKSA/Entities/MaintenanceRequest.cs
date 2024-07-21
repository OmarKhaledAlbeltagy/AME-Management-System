using System;

namespace AMEKSA.Entities
{
    public class MaintenanceRequest
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public int AccountDevicesId { get; set; }

        public AccountDevices accountDevices { get; set; }

        public string MobileNumber { get; set; }

        public string? LandLineNumber { get; set; }

        public string? Email { get; set; }

        public string brief { get; set; }

        public DateTime RequestDateTime { get; set; }

        public bool Done { get; set; } = false;

        public DateTime VisitDate { get; set; }

        public string VisitReport { get; set; }

        public Guid guid { get; set; } = Guid.NewGuid();

        public string? ClientResponse { get; set; }

        public string? ManagementResponse { get; set; }

        public string? EngineerName { get; set; }
    }
}

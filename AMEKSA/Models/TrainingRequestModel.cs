using DocumentFormat.OpenXml.EMMA;
using System;

namespace AMEKSA.Models
{
    public class TrainingRequestModel
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

        public string RequestedDate { get; set; }

        public string RequestDateTime { get; set; }

        public DateTime d { get; set; }

        public bool NotDoneOnTime { get; set; } = false;

        public Guid guid { get; set; }

        public string? EngineerName { get; set; }
    }
}

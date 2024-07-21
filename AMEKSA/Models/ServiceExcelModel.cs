using System;

namespace AMEKSA.Models
{
    public class ServiceExcelModel
    {
        public string FullName { get; set; }

        public string AccountName { get; set; }

        public string ProductName { get; set; }

        public string MobileNumber { get; set; }

        public string? brief { get; set; }

        public string RequestDateTime { get; set; }

        public string? RequestedDate { get; set; }

        public DateTime d { get; set; }

        public bool Done { get; set; }

        public string VisitDate { get; set; }

        public string VisitReport { get; set; }
    }
}

using System;

namespace AMEKSA.Models
{
    public class VisitDetailsModel
    {
        public int AccountDeviceId { get; set; }

        public string ProductName { get; set; }

        public string SerialNumber { get; set; }

        public string? Model { get; set; }

        public string AccountName { get; set; }

        public bool? warranty { get; set; }

        public string? WarrantyDate { get; set; }

        public bool? Contract { get; set; }

        public string? ServiceContract { get; set; }





        public int RequestId { get; set; }

        public string FullName { get; set; }

        public string MobileNumber { get; set; }

        public string? LandLineNumber { get; set; }

        public string? Email { get; set; }

        public string? brief { get; set; }

        public string? RequestedDate { get; set; }

        public string RequestDateTime { get; set; }






    }
}

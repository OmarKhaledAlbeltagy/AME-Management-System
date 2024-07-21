using System;

namespace AMEKSA.Models
{
    public class ScanDeviceQrCodeModel
    {
        public int Id { get; set; }

        public string ProductName { get; set; }

        public string SerialNumber { get; set; }

        public string? Model { get; set; }

        public string AccountName { get; set; }

        public bool? warranty { get; set; }

        public string? WarrantyDate { get; set; }

        public bool? Contract { get; set; }

        public string? ServiceContract { get; set; }

        public DateTime Now { get; set; }


    }
}

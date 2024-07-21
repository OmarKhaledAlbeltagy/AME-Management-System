using AMEKSA.Entities;
using System;

namespace AMEKSA.Models
{
    public class AccountDeviceModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string? Model { get; set; }

        public string SerialNumber { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; }

        public Guid guid { get; set; }

        public string? Warranty { get; set; }

        public string? ServiceContract { get; set; }

        public string CityName { get; set; }
    }
}

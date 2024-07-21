using System;

namespace AMEKSA.Entities
{
    public class AccountDevices
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product product { get; set; }

        public string? Model { get; set; }

        public string SerialNumber { get; set; }

        public int AccountId { get; set; }

        public Account account { get; set; }

        public Guid guid { get; set; } = Guid.NewGuid();

        public DateTime? Warranty { get; set; }

        public DateTime? ServiceContract { get; set; }

        public bool bulk { get; set; } = false;

        public bool IsEmpty { get; set; } = false;
    }
}

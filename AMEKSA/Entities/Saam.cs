using System;

namespace AMEKSA.Entities
{
    public class Saam
    {
        public int Id { get; set; }

        public int ContactId { get; set; }

        public Contact contact { get; set; }

        public bool IsComing { get; set; } = false;

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string HSAN { get; set; }

        public DateTime Date { get; set; }

        public int RegistrationCode { get; set; }

        public Guid guidd { get; set; }
    }
}

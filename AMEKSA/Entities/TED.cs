using System;

namespace AMEKSA.Entities
{
    public class TED
    {
        public int Id { get; set; }

        public int ContactId { get; set; }

        public Contact contact { get; set; }

        public bool IsComing { get; set; } = false;

        public bool IsNotComing { get; set; } = false;

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime Date { get; set; }

        public bool Attend { get; set; } = false;

        public DateTime? AttendTime { get; set; }

        public bool Sms { get; set; } = false;

        public int RegistrationCode { get; set; }

        public Guid guidd { get; set; }
    }
}

namespace AMEKSA.Models
{
    public class SaamReport
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string HSAN { get; set; }

        public string RegistrationDate { get; set; }

        public bool Registered { get; set; } = false;

        public int? Workshop { get; set; }
    }
}

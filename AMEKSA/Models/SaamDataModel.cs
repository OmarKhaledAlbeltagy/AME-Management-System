namespace AMEKSA.Models
{
    public class SaamDataModel
    {
        public int Id { get; set; }

        public int ContactId { get; set; }

        public string ContactName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string hsan { get; set; }

        public bool IsComing { get; set; } = false;

        public int? Workshop { get; set; }
    }
}

namespace AMEKSA.Models
{
    public class TedDataModel
    {
        public int Id { get; set; }

        public int ContactId { get; set; }

        public string ContactName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsComing { get; set; } = false;

        public bool IsNotComing { get; set; } = false;

        public string g { get; set; }
    }
}

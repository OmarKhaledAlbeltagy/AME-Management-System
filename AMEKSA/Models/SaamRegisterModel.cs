namespace AMEKSA.Models
{
    public class SaamRegisterModel
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string HSAN { get; set; }

        public int? Workshop { get; set; }
    }
}

namespace AMEKSA.Models
{
    public class AccountantRegisterModel
    {
        public string Email { get; set; }

        public string FullName { get; set; }

        public string RoleName { get; set; } = "Accountant";

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public int CityId { get; set; }
    }
}

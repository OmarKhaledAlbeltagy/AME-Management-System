namespace AMEKSA.Models
{
    public class AccountingManagerRegisterModel
    {
        public string Email { get; set; }

        public string FullName { get; set; }

        public string RoleName { get; set; } = "Accounting Manager";

        public string Password { get; set; }

        public string PhoneNumber { get; set; }
    }
}

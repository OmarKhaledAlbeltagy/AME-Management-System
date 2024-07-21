namespace AMEKSA.Models
{
    public class SemiSystemAdminRegisterModel
    {
        public string Email { get; set; }

        public string FullName { get; set; }

        public string RoleName { get; set; } = "System Adminn";

        public string Password { get; set; }

        public string PhoneNumber { get; set; }
    }
}

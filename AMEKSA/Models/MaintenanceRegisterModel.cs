namespace AMEKSA.Models
{
    public class MaintenanceRegisterModel
    {
        public string Email { get; set; }

        public string FullName { get; set; }

        public string RoleName { get; set; } = "Maintenance Representative";

        public string Password { get; set; }

        public string PhoneNumber { get; set; }
    }
}

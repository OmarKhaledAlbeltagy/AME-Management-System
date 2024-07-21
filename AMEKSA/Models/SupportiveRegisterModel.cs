using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class SupportiveRegisterModel
    {
        public string Email { get; set; }

        public string FullName { get; set; }

        public string RoleName { get; set; } = "Supportive";

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string ManagerId { get; set; }

        public int CityId { get; set; }
    }
}

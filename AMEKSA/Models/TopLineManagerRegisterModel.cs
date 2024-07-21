using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class TopLineManagerRegisterModel
    {
        public string Email { get; set; }

        public string FullName { get; set; }

        public string RoleName { get; set; } = "Top Line Manager";

        public string Password { get; set; }

        public string PhoneNumber { get; set; }
    }
}

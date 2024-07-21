using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class FirstLineManagerRegisterModel
    {
        public string Email { get; set; }

        public string FullName { get; set; }

        public string RoleName { get; set; } = "First Line Manager";

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public int CityId { get; set; }
    }
}

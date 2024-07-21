using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class MedicalRegisterModel
    {

        public string Email { get; set; }

        public string FullName { get; set; }

        public string RoleName { get; set; } = "Medical Representative";

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string ManagerId { get; set; }

        public int CityId { get; set; }
    }
}

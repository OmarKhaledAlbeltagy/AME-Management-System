using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class SystemAdminRegisterModel
    {

        public string Email { get; set; }

        public string FullName { get; set; }

        public string RoleName { get; set; } = "System Admin";

        public string Password { get; set; }

        public string PhoneNumber { get; set; }
    }
}

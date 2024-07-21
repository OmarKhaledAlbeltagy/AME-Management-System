using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomUserRole
    {
        public string UserId { get; set; }

        public string FullName { get; set; }

        public string UserEmail { get; set; }

        public string RoleName { get; set; }

        public string? RepId { get; set; }

        public bool Active { get; set; }
    }
}

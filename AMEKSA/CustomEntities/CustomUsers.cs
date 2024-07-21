using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomUsers
    {
        public string UserId { get; set; }

        public int? CityId { get; set; }

        public string CityName { get; set; }

        public string RoleName { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }

        public string ManagerId { get; set; }

        public string ManagerName { get; set; }
    }
}

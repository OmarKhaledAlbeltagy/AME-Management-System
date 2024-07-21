using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class UserSubmanager
    {
        public int Id { get; set; }

        public string RepId { get; set; }

        public ExtendIdentityUser Rep { get; set; }

        public string ManagerId { get; set; }

        public ExtendIdentityUser Manager { get; set; }
    }
}

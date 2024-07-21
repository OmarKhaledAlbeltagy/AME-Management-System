using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class Notifications
    {
        public int Id { get; set; }

        public string NotificationDetails { get; set; }

        public string ExtendIdetityUserId { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        public DateTime NitificationDateTime { get; set; }

        public bool Seen { get; set; } = false;

        public string Url { get; set; }
    }
}

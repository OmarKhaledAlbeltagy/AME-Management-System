using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class DetailedNotificationsModel
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public DateTime dt { get; set; }

        public string DateTime { get; set; }

        public string NotificationsDetails { get; set; }

        public bool Seen { get; set; }

        public string Url { get; set; }
    }
}

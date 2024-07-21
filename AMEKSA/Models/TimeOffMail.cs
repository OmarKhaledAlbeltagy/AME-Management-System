using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class TimeOffMail
    {
        public int Id { get; set; }

        public string Date { get; set; }

        public string TimeFrom { get; set; }

        public string TimeTo { get; set; }

        public string Reason { get; set; }

        public string Description { get; set; }

        public string UserId { get; set; }

        public string FullName { get; set; }
    }
}

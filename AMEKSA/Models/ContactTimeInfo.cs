using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class ContactTimeInfo
    {
        public int Id { get; set; }

        public DateTime? BestTimeFrom { get; set; }

        public DateTime? BestTimeTo { get; set; }
    }
}

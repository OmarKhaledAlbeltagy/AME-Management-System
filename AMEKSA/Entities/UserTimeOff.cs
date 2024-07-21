using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class UserTimeOff
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateTimeFrom { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateTimeTo { get; set; }

        public int TimeOffTerritoryReasonsId { get; set; }

        public string Description { get; set; }

        public TimeOffTerritoryReasons timeoffterritroyreasons { get; set; }

        public string ExtendIdentityUserId { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        public bool? Accepted { get; set; }
    }
}

using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class VacancyRequests
    {
        public int Id { get; set; }

        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime VacancyDateTimeFrom { get; set; }

        [DataType(DataType.Date)]
        public DateTime VacancyDateTimeTo { get; set; }

        public int TimeOffTerritoryReasonsId { get; set; }

        public TimeOffTerritoryReasons timeoffterritoryreasons { get; set; }

        public string extendidentityuserid { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        public bool Accepted { get; set; } = false;

        public bool Rejected { get; set; } = false;
    }
}

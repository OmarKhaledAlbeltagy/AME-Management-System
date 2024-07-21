using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomVacancyRequests
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime VacancyDateTimeFrom { get; set; }

        public DateTime VacancyDateTimeTo { get; set; }

        public string Reason { get; set; }

        public string RepName { get; set; }

        public bool Accepted { get; set; } = false;

        public bool Rejected { get; set; } = false;
    }
}

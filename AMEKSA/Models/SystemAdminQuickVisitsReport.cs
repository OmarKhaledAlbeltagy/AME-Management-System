using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class SystemAdminQuickVisitsReport
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public DateTime Date { get; set; }

        public string VisitDate { get; set; }

        public string VisitTime { get; set; }

        public string SubmittingDate { get; set; }

        public string SubmittingTime { get; set; }
    }
}

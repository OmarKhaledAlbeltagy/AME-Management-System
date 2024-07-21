using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class TenClockReportModel
    {
        public IEnumerable<ExtendIdentityUser> MedicalReported { get; set; }

        public IEnumerable<ExtendIdentityUser> MedicalNotReported { get; set; }

        public IEnumerable<ExtendIdentityUser> SalesReported { get; set; }

        public IEnumerable<ExtendIdentityUser> SalesNotReported { get; set; }
    }
}

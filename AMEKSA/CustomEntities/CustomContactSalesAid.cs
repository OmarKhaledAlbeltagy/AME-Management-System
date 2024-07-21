using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomContactSalesAid
    {
        public int SalesAidId { get; set; }

        public string SalesAidName { get; set; }

        public int ContactMedicalVisitId { get; set; }
    }
}

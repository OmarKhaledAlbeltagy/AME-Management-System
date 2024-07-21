using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomContactMedicalVisitProducts
    {
        public int ContactMedicalVisitId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int ProductShare { get; set; }
    }
}

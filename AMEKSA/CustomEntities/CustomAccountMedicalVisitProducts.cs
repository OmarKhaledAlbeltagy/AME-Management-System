using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomAccountMedicalVisitProducts
    {
        public int AccountMedicalVisitId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomAccountOpenning
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; }

        public int BrandId { get; set; }

        public string BrandName { get; set; }

        public decimal Openning { get; set; }

        public decimal Collection { get; set; }

        public decimal Balance { get; set; }
    }
}

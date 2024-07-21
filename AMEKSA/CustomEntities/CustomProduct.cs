using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomProduct
    {
        public int BrandId { get; set; }

        public string BrandName { get; set; }

        public IEnumerable<Product> Products { get; set; }
    }
}

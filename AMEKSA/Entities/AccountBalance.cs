using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class AccountBalance
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public Account account { get; set; }

        public int BrandId { get; set; }

        public Brand brand { get; set; }

        public float Balance { get; set; }
    }
}

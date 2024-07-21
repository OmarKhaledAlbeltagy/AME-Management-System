using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMEKSA.Entities
{
    public class AccountBrandPayment
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public Account account { get; set; }

        public int BrandId { get; set; }

        public Brand brand { get; set; }

        [Column(TypeName = "money")]
        public decimal Openning { get; set; }

        [Column(TypeName = "money")]
        public decimal Collection { get; set; }

        [Column(TypeName = "money")]
        public decimal Balance { get; set; }
    }
}

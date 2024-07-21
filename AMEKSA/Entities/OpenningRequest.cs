using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class OpenningRequest
    {
        public int Id { get; set; }

        public int AccountBrandPaymentId { get; set; }

        public OpenningRequest openningrequest { get; set; }

        [Column(TypeName = "money")]
        public decimal RequestedOpenning { get; set; }

        public string ExtendIdentityUserId { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        public bool Confirmed { get; set; } = false;

        public bool Rejected { get; set; } = false;
    }
}

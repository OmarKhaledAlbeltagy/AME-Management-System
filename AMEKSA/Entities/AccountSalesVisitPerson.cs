using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class AccountSalesVisitPerson
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string PersonName { get; set; }

        [MaxLength(30)]
        public string PersonPosition { get; set; }

        public bool Gender { get; set; } = false;

        public int AccountSalesVisitId { get; set; }

        public AccountSalesVisit accountsalesvisit { get; set; }
    }
}

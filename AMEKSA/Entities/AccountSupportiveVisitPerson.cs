using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class AccountSupportiveVisitPerson
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string PersonName { get; set; }

        [MaxLength(30)]
        public string PersonPosition { get; set; }

        public int AccountSupportiveVisitId { get; set; }

        public AccountSupportiveVisit accountsupportivevisit { get; set; }
    }
}

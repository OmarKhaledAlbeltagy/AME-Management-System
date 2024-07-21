using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class AccountMedicalVisitPerson
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string PersonName { get; set; }

        [MaxLength(30)]
        public string PersonPosition { get; set; }

        public bool Gender { get; set; }

        public int AccountMedicalVisitId { get; set; }

        public AccountMedicalVisit accountmedicalvisit { get; set; }
    }
}

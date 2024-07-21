using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMEKSA.Entities
{
    public class PurchaseType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string PurchaseTypeName { get; set; }

        public ICollection<Account> account { get; set; }

        public ICollection<Contact> contact { get; set; }
    }
}

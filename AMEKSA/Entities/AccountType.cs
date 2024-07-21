using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMEKSA.Entities
{
    public class AccountType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string AccountTypeName { get; set; }

        public ICollection<Account> account { get; set; }
    }
}

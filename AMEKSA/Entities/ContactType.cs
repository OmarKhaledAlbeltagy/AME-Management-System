using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMEKSA.Entities
{
    public class ContactType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string ContactTypeName { get; set; }

        public ICollection<Contact> contact { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class SalesAid
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string SalesAidName { get; set; }

        public bool show { get; set; } = false;

        public ICollection<ContactSalesAid> contactsalesaid { get; set; }
    }
}

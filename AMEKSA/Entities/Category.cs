using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMEKSA.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(2)]
        public string CategoryName { get; set; }

        public ICollection<Contact> contact { get; set; }

        public ICollection<Account> account { get; set; }

        public ICollection<UserContact> userContact { get; set; }
    }
}

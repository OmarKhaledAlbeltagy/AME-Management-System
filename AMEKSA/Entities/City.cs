using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AMEKSA.Privilage;

namespace AMEKSA.Entities
{
    public class City
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string CityName { get; set; }

        public ICollection<District> district { get; set; }

        public ICollection<ExtendIdentityUser> extendIdentityUsers { get; set; }
    }
}

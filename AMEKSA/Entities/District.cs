using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMEKSA.Entities
{
    public class District
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string DistrictName { get; set; }

        public int CityId { get; set; }

        public City city { get; set; }

        public ICollection<Account> account { get; set; }

        public ICollection<Contact> contact { get; set; }
    }
}

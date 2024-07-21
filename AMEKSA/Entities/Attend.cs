using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class Attend
    {
        public int Id { get; set; }

        public bool? Attendd { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public int? CityId { get; set; }

        public City city { get; set; }

        public string why { get; set; }

        public bool Came { get; set; } = false;
    }
}

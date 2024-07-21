using AMEKSA.Privilage;
using System;

namespace AMEKSA.Entities
{
    public class JeddaDermBoth
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string MobileNumber { get; set; }

        public string Clinic { get; set; }

        public string City { get; set; }

        public string AboutWhatQuery { get; set; }

        public string ExtendIdentityUserId { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        public DateTime datetime { get; set; }
    }
}

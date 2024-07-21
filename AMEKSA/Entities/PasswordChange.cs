using AMEKSA.Privilage;
using System;

namespace AMEKSA.Entities
{
    public class PasswordChange
    {
        public int Id { get; set; }

        public string ExtendIdentityUserId { get; set; }

        public ExtendIdentityUser extendIdentityUser { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public DateTime ChangingDateTime { get; set; }
    }
}

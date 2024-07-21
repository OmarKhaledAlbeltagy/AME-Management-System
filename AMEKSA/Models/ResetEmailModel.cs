using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class ResetEmailModel
    {
        public string Id { get; set; }

        public string OldEmail { get; set; }

        public string NewEmail { get; set; }
    }
}

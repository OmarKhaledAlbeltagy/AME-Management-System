using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class ChangeCategoryRequestsModel
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string  ContactName { get; set; }

        public string CategoryFromName { get; set; }

        public string CategoryToName { get; set; }

        public bool Confirmed { get; set; }

        public bool Rejected { get; set; }
    }
}

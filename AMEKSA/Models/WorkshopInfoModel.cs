using AMEKSA.Entities;
using System.Collections.Generic;

namespace AMEKSA.Models
{
    public class WorkshopInfoModel
    {
        public string location { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public List<string> speakers { get; set; }
    }
}

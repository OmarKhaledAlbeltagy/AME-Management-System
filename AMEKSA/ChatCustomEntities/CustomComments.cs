using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.ChatCustomEntities
{
    public class CustomComments
    {
        public int Id { get; set; }

        public string ManagerId { get; set; }

        public string ManagerName { get; set; }

        public string RepId { get; set; }

        public string RepName { get; set; }

        public string ManagerComment { get; set; }

        public DateTime ManagerCommentDateTime { get; set; }

        public string RepReply { get; set; }

        public DateTime RepReplyDateTime { get; set; }

        public int OrgId { get; set; }

        public string OrgName { get; set; }

        public DateTime VisitDate { get; set; }

        public DateTime VisitTime { get; set; }

        public int VisitId { get; set; }

        public int VisitType { get; set; }
    }
}

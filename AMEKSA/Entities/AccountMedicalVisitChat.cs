using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Entities
{
    public class AccountMedicalVisitChat
    {
        public int Id { get; set; }

        [Required]
        public string ManagerId { get; set; }

        public ExtendIdentityUser Manager { get; set; }

        public string RepId { get; set; }

        public ExtendIdentityUser Rep { get; set; }

        [Required]
        [MaxLength(500)]
        public string ManagerComment { get; set; }


        [DataType(DataType.Date)]
        public DateTime ManagerCommentDateTime { get; set; }

        [MaxLength(500)]
        public string RepReply { get; set; }

        [DataType(DataType.Date)]
        public DateTime RepReplyDateTime { get; set; }

        [Required]
        public int AccountMedicalVisitId { get; set; }

        public AccountMedicalVisit AccountMedicalVisit { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AMEKSA.Privilage
{
    public class ExtendIdentityUser:IdentityUser
    {
        public string FullName { get; set; }

        [ForeignKey("extendidentityuser")]
        public string extendidentityuserid { get; set; }

        public ExtendIdentityUser extendidentityuser { get; set; }

        public int? CityId { get; set; }

        public City city { get; set; }

        public bool Active { get; set; } = true;
        
        [ForeignKey("Rep")]
        public string? RepId { get; set; }

        public ExtendIdentityUser Rep { get; set; }

        public ICollection<AccountMedicalVisit> accountmedicalvisit { get; set; }

        

        public ICollection<AccountSalesVisit> accountsalesvisit { get; set; }


        public ICollection<ContactMedicalVisit> contactmedicalvisit { get; set; }

 
        public ICollection<ContactMedicalVisit> contactmedicalvisit2 { get; set; }

  
        public ICollection<AccountMedicalVisit> accountmedicalvisit2 { get; set; }


        public ICollection<AccountSalesVisit> accountsalesvisit2 { get; set; }


        public ICollection<AccountSupportiveVisit> accountsupportivevisit2 { get; set; }

        public ICollection<UserAccount> useraccount { get; set; }

        public ICollection<UserBrand> userbrand { get; set; }

        public ICollection<UserContact> usercontact { get; set; }

        

        public ICollection<UserSubmanager> Reps { get; set; }


        public ICollection<UserSubmanager> Managers { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AMEKSA.Entities;

namespace AMEKSA.Privilage
{
    public class ExtendIdentityRole:IdentityRole
    {
        public ICollection<RoleMenu> rolemenu { get; set; }
    }
}

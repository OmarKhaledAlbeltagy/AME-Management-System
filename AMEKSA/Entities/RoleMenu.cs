using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AMEKSA.Privilage;

namespace AMEKSA.Entities
{
    public class RoleMenu
    {
        public int Id { get; set; }

        [Required]
        public int MenuId { get; set; }

        public Menu menu { get; set; }

        public ExtendIdentityRole extendidentityrole { get; set; }
    }
}

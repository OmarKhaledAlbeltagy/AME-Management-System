using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class ChangeCategoryRequestModel
    {
        public string UserId { get; set; }

        public int ContactId { get; set; }

        public string CurrentCategoryName { get; set; }

        public int NewCategoryId { get; set; }
    }
}

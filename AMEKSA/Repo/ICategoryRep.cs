using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface ICategoryRep
    {
        IEnumerable<Category> GetAllCategories();

        DateTime GetTimeNow();

        bool categ();

        bool date();

        IEnumerable<Contact> nonocateg();
    }
}

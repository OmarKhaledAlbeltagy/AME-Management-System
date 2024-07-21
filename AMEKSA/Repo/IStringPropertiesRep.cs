using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
     public interface IStringPropertiesRep
    {
        bool EditMotivation(string m);

        string GetMotivation();
    }
}

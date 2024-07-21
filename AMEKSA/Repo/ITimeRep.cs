using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface ITimeRep
    {
        DateTime GetCurrentTime();

        DayOfWeek getnumberofweektoday();
    }
}

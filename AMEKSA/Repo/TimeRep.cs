using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class TimeRep : ITimeRep
    {
        public DateTime GetCurrentTime()
        {
            DateTime serverTime = DateTime.Now;
            DateTime _localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "Arab Standard Time");
            
            return _localTime;
        }

        public DayOfWeek getnumberofweektoday()
        {
           DateTime now = GetCurrentTime();

            DayOfWeek today = now.DayOfWeek;
            return today;
        }
    }
}

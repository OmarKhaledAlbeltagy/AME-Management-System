using System;

namespace AMEKSA.Entities
{
    public class MagellanDayAttendance
    {
        public int Id { get; set; }

        public int ContactId { get; set; }

        public DateTime AttendanceTime { get; set; }
    }
}

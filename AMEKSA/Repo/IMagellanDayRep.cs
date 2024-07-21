using AMEKSA.Models;
using System.Collections.Generic;

namespace AMEKSA.Repo
{
    public interface IMagellanDayRep
    {
        List<MagellanDayContactSearch> GetContact();

        bool ConfirmAttendance(int id);

        List<MagellanDayContactSearch> GetAttended();
    }
}

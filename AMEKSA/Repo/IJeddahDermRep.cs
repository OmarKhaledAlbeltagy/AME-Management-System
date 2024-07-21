using AMEKSA.Models;
using System.Collections.Generic;

namespace AMEKSA.Repo
{
    public interface IJeddahDermRep
    {
        SaamInvitationModel GetInvitationData(int id);

        List<SaamReport> GetSaamReport();

        string GetDoctorName(string g);

        SaamDataModel Start(string gu);

        bool Accept(SaamRegisterModel r);

        SaamRegisterModel ChangeInfo(int id);

        SaamDataModel AnonRegister(SaamAnonRegisterModel s);

        bool EditRegister(SaamAnonRegisterModel s);

        List<SaamAnonRegisterModel> GetAll();

        bool AddJeddaDermBoth(JeddaDermBothModel obj);

        List<JeddaDermBothExcelModel> GetBothData();
    }
}

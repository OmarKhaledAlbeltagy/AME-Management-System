using AMEKSA.Entities;
using AMEKSA.Models;
using System.Collections.Generic;

namespace AMEKSA.Repo
{
    public interface ITedRep
    {
        List<TedReport> GetTedReport();

        List<TedAttendanceTable> GetAttendanceReport();

        string GetDoctorName(string g);

        TedDataModel Start(string gu);

        bool Accept(TedRegisterModel r);

        bool Reject(int id);

        bool ChangeMind(int id);

        TedRegisterModel ChangeInfo(int id);

        bool SubmitChangeInfo(TedRegisterModel m);

        TedRegistrationData GetDataByGuid(string g);

        TedRegistrationData GetDataByCode(int c);

        bool Attendance(int id);

        TedDataModel AnonRegister(SaamAnonRegisterModel obj);
    }
}

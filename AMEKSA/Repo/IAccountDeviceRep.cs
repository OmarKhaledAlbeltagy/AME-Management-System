using AMEKSA.Entities;
using AMEKSA.Models;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public interface IAccountDeviceRep
    {
        bool AssignTrainingEngineer(RequestEngineer obj);

        bool AssignMaintenanceEngineer(RequestEngineer obj);

       int AddNewDevice(AccountDevices obj);

       List<AccountDeviceModel> GetAllDevices();

       AccountDevices GetAccountDeviceById(int id);

        bool EditDevice(AccountDevices obj);

        bool PrepareBulkQR(ListOfIdsModel obj);

        dynamic ScanQrCode(string guid);

        dynamic GetMaintenanceVisitDetailsRep(string g);

        dynamic GetTrainingVisitDetailsRep(string g);

        Task<dynamic> MantenanceRequest(MaintenanceRequest obj);

        Task<bool> TrainingRequest(TrainingRequest obj);

        List<MaintenanceRequestsModel> GetUpcomingMaintenanceRequests();

        List<TrainingRequestModel> GetUpcomingTrainingRequests();

        bool SubmitMaintenanceVisit(SubmitTrainingVisitModel obj);

        bool SubmitTrainingVisit(SubmitTrainingVisitModel obj);

        List<ServiceExcelModel> MaintenanceReportByMonth(int month, int year);

        List<ServiceExcelModel> TrainingReportByMonth(int month, int year);

        List<SimpleDeviceModel> GetDevicesByAccountId(int Id);
    }
}

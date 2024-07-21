using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;

namespace AMEKSA.Repo
{
    public interface IEventAccountingRep
    {
        bool ConfirmRequest(int id);

        bool RejectRequest(RejectExpenseRequestModel obj);

        bool RejectRequestAfterConfirm(RejectExpenseRequestModel obj);

        bool HoldRequest(int id);

        List<EventTotalFeesModel> GetEventsTotalFees();

        List<EventTotalFeesModel> GetMyEventsTotalFees(string UserId);

        dynamic GetMyEventAccounting(int EventId, string UserId);

        dynamic GetMyEventAccountingRequests(int EventId, string UserId);

        List<EventFeesSimpleModel> GetNotMyEventAccountingRequests(int EventId, string UserId);

        List<EventFeesSimpleModel> GetAccountingManagerAccountingRequests(int EventId, string UserId);

        EventFeesSimpleModel AddAccountingItem(AddAccountingItemModel obj);

        EventFeesSimpleModel AddAccountingItemRequest(AddAccountingItemModel obj);

        List<EventFeesSimpleModel> GetPendingRequests();

        List<EventFeesSimpleModel> GetPreviousRequests();

        List<EventFeesSimpleModel> GetEventExpenses(int id, string UserId);

        bool DeleteExpense(int id);

        bool DeleteExpenseRequest(int id);

        bool EditExpenseWithoutFile(EditExpenseWithoutFileModel obj);

        bool EditExpenseWithFile(EditExpenseWithFileModel obj);

        bool EditExpenseRequestWithoutFile(EditExpenseWithoutFileModel obj);

        bool EditExpenseRequestWithFile(EditExpenseWithFileModel obj);

        EventExcelModel GetMyExpensesForExcel(int EventId, string UserId);

        EventExcelModel GetAllExpensesForExcel(int EventId, string UserId);

        List<MorrisDonutModel> GetMorrisChart(DateTime from, DateTime to);

        List<EventExcelModel> GetEventsTotalExpenses(DateTime from, DateTime to, string UserId);
    }
}

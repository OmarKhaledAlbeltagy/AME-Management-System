using AMEKSA.Entities;
using ClosedXML.Excel;

namespace AMEKSA.Repo
{
    public interface IEventExpenseDocumentRep
    {
        XLWorkbook AddDocument(/*ExpensRequestDocument obj*/);
    }
}

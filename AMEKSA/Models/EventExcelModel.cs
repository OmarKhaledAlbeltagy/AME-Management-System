using AMEKSA.Entities;
using System.Collections.Generic;

namespace AMEKSA.Models
{
    public class EventExcelModel
    {
        public string EventName { get; set; }

        public string EventType { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string UserNameExported { get; set; }

        public string ExportingDateTime { get; set; }

        public int Attendees { get; set; }

        public List<EventSpeaker> speakers { get; set; }

        public List<EventFeesSimpleModel> Expenses { get; set; }

        public List<EventTotalExpensesModel> totals { get; set; }

        public double Total { get; set; }
    }
}

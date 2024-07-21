using Microsoft.AspNetCore.Http;

namespace AMEKSA.Models
{
    public class EditExpenseWithoutFileModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public double Value { get; set; }

        public string Note { get; set; }

        public int Sort { get; set; }
    }
}

using Microsoft.AspNetCore.Http;

namespace AMEKSA.Models
{
    public class EditExpenseWithFileModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public double Value { get; set; }

        public string Note { get; set; }

        public IFormFile file { get; set; }

        public int Sort { get; set; }
    }
}

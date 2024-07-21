using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMEKSA.Entities
{
    public class AccountMedicalVisitProducts
    {

        public int Id { get; set; }

        public int AccountMedicalVisitId { get; set; }

        public AccountMedicalVisit accountmedicalvisit { get; set; }

        public int ProductId { get; set; }

        public Product product { get; set; }

    }
}

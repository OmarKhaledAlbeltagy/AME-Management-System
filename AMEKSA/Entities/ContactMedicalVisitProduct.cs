using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMEKSA.Entities
{
    public class ContactMedicalVisitProduct
    {
        public int Id { get; set; }

        public int ContactMedicalVisitId { get; set; }

        public ContactMedicalVisit contactvisitmedical { get; set; }

        public int ProductId { get; set; }

        public Product product { get; set; }

        [Column(TypeName = "tinyint")]
        public int NumberOfSamples { get; set; }

        [Column(TypeName = "tinyint")]
        public int ProductShare { get; set; }
    }
}

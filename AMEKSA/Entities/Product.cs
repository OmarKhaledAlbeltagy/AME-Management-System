using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AMEKSA.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; }

        public int BrandId { get; set; }

        public Brand brand { get; set; }

        public ICollection<AccountMedicalVisitProducts> accountmedicalvisitproducts { get; set; }

        public ICollection<ContactMedicalVisitProduct> contactmedicalvisitproduct { get; set; }
    }
}

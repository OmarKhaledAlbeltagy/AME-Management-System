using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.CustomEntities
{
    public class CustomOpenningRequest
    {
        public int Id { get; set; }

        public int AccountBrandPaymentId { get; set; }

        public decimal CurrentOpenning { get; set; }

        public decimal CurrentCollection { get; set; }

        public decimal CurrentBalance { get; set; }

        public decimal RequestedOpenning { get; set; }

        public decimal NewOpenning { get; set; }

        public decimal NewCollection { get; set; }

        public decimal NewBalance { get; set; }

        public string ExtendIdentityUserId { get; set; }

        public string UserName { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; }

        public int BrandId { get; set; }

        public string BrandName { get; set; }

        public bool Confirmed { get; set; }

        public bool Rejected { get; set; }
    }
}

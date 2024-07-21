using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.ChatCustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
namespace AMEKSA.Repo
{
   public interface IChatRep
    {
        bool AccountMedicalManagerComment(AccountMedicalVisitChat obj);

        bool AccountMedicalRepReply(RepReplyModel obj);

        bool AccountSalesManagerComment(AccountSalesVisitChat obj);

        bool AccountSalesRepReply(RepReplyModel obj);

        bool AccountSupportiveRepReply(RepReplyModel obj);

        bool ContactMedicalManagerComment(ContactMedicalVisitChat obj);

        bool ContactMedicalRepReply(RepReplyModel obj);

        IEnumerable<CustomComments> GetCommentsByDateRep(SearchByDate obj);

        bool AccountSupportiveManagerComment(AccountSupportiveVisitChat obj);

        

    }
}

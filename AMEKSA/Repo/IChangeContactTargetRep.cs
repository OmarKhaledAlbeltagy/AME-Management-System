using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.CustomEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IChangeContactTargetRep
    {
        bool ChangeTarget(ContactChangeTargetModel obj);

        bool RequestChangeTarget(RequestChangeContactTarget obj);

        bool AcceptRequest(int id);

        bool RejectRequest(int id);

        IEnumerable<CustomRequestContactChangeTarget> GetMyTeamRequests(string id);


    }
}

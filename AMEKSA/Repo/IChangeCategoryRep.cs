using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IChangeCategoryRep
    {
        bool MakeRequest(ChangeCategoryRequestModel obj);

        bool ConfirmRequest(int RequestId);

        bool RejectRequest(int RequestId);

        IEnumerable<ChangeCategoryRequestsModel> GetMyTeamRequests(string ManagerId);

        IEnumerable<ChangeCategoryRequestsModel> GetAllRequests();
    }
}

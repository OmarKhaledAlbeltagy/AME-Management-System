using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
  public interface ISalesAidRep
    {
        IEnumerable<SalesAid> GetAllSalesAids();

        IEnumerable<SalesAid> GetShownSalesAids();

        bool EditSalesAid(SalesAid obj);

        SalesAid AddSalesAid(SalesAid obj);
    }
}

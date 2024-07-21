using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IAccountTypeRep
    {
        AccountType AddAccountType(AccountType obj);

        IEnumerable<AccountType> GetAllAccountTypes();

        AccountType GetAccountTypeById(int id);

        void DeleteAccountType(int id);

        bool EditAccountType(AccountType obj);

    
    }
}

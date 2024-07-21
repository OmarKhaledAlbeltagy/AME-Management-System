using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
      public interface IAccountRep
    {
        List<CustomAccount> SearchAccount(SearchByWord accountName);

        Task<bool> AddAccount(AccountModel obj);

        IEnumerable<Account> GetAccountNames();

        IEnumerable<CustomAccount> GetAllAccounts();

        CustomAccount GetAccountById(int id);

        bool DeleteAccount(int id);

        IEnumerable<Brand> GetAvailableBrandsForOppening(int accoundId);

        bool SetAccountOpenning(NewOpenning op);

        IEnumerable<CustomAccountOpenning> GetAccountOpenningsByAccountId(int accountId);

        CustomAccountOpenning GetOpenningById(int openningId);

        bool EditAccountOpenning(int openningId, int openning);

        bool EditAccountGeneralInfo(AccountGeneralInfo obj);

        bool EditAccountLocationInfo(AccountLocationInfo obj);

        bool EditAccountContactinfo(AccountContactInfo obj);

        bool EditAccountTimeInfo(AccountTimeInfo obj);

        bool EditAccountNotesInfo(AccountNoteInfo obj);

        IEnumerable<CustomAccount> GetAllAccountsfiltered(FilteringAccounts obj);

        IEnumerable<CustomAccount> GetAllAccountsfilteredWithouttype(FilteringAccounts obj);

        IEnumerable<Account> GetFLMAccounts(string userId);

        float GetAccountCreditLimit(int id);

        float GetAccountResidualCreditLimit(int id);

        bool EditAccountCreditLimit(int id, float creditlimit);

     
    }
}

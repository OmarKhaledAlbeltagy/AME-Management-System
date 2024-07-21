using AMEKSA.Context;
using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class AccountTypeRep : IAccountTypeRep
    {
        private readonly DbContainer db;

        public AccountTypeRep(DbContainer db)
        {
            this.db = db;
        }

        public AccountType AddAccountType(AccountType obj)
        {
            db.accountType.Add(obj);
            db.SaveChanges();
            return obj;
        }

        public void DeleteAccountType(int id)
        {
            db.accountType.Remove(db.accountType.Find(id));
            db.SaveChanges();
        }

        public bool EditAccountType(AccountType obj)
        {
            AccountType old = db.accountType.Find(obj.Id);
            old.AccountTypeName = obj.AccountTypeName;
            db.SaveChanges();
            return true;
        }

        public AccountType GetAccountTypeById(int id)
        {
            return db.accountType.Find(id);
        }

        public IEnumerable<AccountType> GetAllAccountTypes()
        {
            return db.accountType.Select(a => a);
        }
    }
}

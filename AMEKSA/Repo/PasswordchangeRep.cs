using AMEKSA.Context;
using AMEKSA.Entities;
using System.Linq;

namespace AMEKSA.Repo
{
    public class PasswordchangeRep:IPasswordchangeRep
    {
        private readonly DbContainer db;

        public PasswordchangeRep(DbContainer db)
        {
            this.db = db;
        }

        public bool CheckChanging(string UserId)
        {
            PasswordChange check = db.passwordChange.Where(a=>a.ExtendIdentityUserId == UserId).FirstOrDefault();

            if (check == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

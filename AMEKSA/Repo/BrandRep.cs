using AMEKSA.Context;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class BrandRep:IBrandRep
    {
        private readonly DbContainer db;

        public BrandRep(DbContainer db)
        {
            this.db = db;
        }

        public bool AccountBalanceSet()
        {
            List<int> Brands = db.brand.Select(a => a.Id).ToList();
            List<int> Accounts = db.account.Select(a => a.Id).ToList();

            foreach (var brand in Brands)
            {
                foreach (var account in Accounts)
                {
                    AccountBalance obj = new AccountBalance();
                    obj.AccountId = account;
                    obj.BrandId = brand;
                    db.accountBalance.Add(obj);
                    
                }
            }
            db.SaveChanges();

            return true;
        }

        public async Task<Brand> AddBrand(Brand obj)
        {
            db.brand.Add(obj);
            db.SaveChanges();
            List<int> accountids = db.account.Select(a => a.Id).ToList();

            List<AccountBalance> ab = new List<AccountBalance>();

            foreach (var item in accountids)
            {
                AccountBalance o = new AccountBalance();
                o.AccountId = item;
                o.BrandId = obj.Id;
                o.Balance = 0;
                ab.Add(o);
            }
            await db.accountBalance.AddRangeAsync(ab);
            db.SaveChangesAsync();
            return obj;
        }

        public bool DeleteBrand(int id)
        {
            db.product.RemoveRange(db.product.Where(a => a.BrandId == id));
            db.SaveChanges();
            db.brand.Remove(db.brand.Find(id));
            db.SaveChanges();
            return true;
        }

        public bool EditBrand(Brand obj)
        {
            Brand old = db.brand.Find(obj.Id);
            old.BrandName = obj.BrandName;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<Brand> GetAllBrands()
        {
            return db.brand.Select(a => a);
        }

        public Brand GetBrandById(int id)
        {
            return db.brand.Find(id);
        }

        public IEnumerable<Brand> GetMyBrands(string userId)
        {
            ExtendIdentityUser me = db.Users.Find(userId);

            string MyManagerId = me.extendidentityuserid;

            IEnumerable<int> mybrandsids = db.userBrand.Where(a => a.extendidentityuserid == MyManagerId).Select(a => a.BrandId);
            List<Brand> result = new List<Brand>();

            foreach (var item in mybrandsids)
            {
                Brand x = db.brand.Find(item);
                result.Add(x);
            }

            return result;
        }

        public IEnumerable<Brand> GetMyBrandsFLM(string userId)
        {
           

            List<int> mybrandsids = db.userBrand.Where(a => a.extendidentityuserid == userId).Select(a=>a.BrandId).ToList();
            List<Brand> result = new List<Brand>();

            foreach (var item in mybrandsids)
            {
                Brand x = db.brand.Find(item);
                result.Add(x);
            }

            return result;
        }
    }
}

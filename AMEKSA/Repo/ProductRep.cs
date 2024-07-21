using AMEKSA.Context;
using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using AMEKSA.Privilage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
    public class ProductRep : IProductRep
    {
        private readonly DbContainer db;

        public ProductRep(DbContainer db)
        {
            this.db = db;
        }

        public Product AddProduct(Product obj)
        {
            db.product.Add(obj);
            db.SaveChanges();
            return obj;
        }

        public bool DeleteProduct(int id)
        {
            db.product.Remove(db.product.Find(id));
            db.SaveChanges();
            return true;
        }

        public bool EditProduct(Product obj)
        {
            Product old = db.product.Find(obj.Id);

            old.ProductName = obj.ProductName;
            db.SaveChanges();
            return true;
        }

        public IEnumerable<Product> GetAllBrandProducts(int id)
        {
            return db.product.Where(a => a.BrandId == id).OrderBy(a => a.ProductName);
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return db.product.Select(a => a);
        }

        public IEnumerable<Product> GetMyProducts(string userId)
        {
            ExtendIdentityUser Me = db.Users.Find(userId);

            string ManagerId = Me.extendidentityuserid;

            List<int> mybrandsIds = db.userBrand.Where(a => a.extendidentityuserid == ManagerId).Select(a=>a.BrandId).ToList();

            List<Product> result = new List<Product>();

            foreach (var item in mybrandsIds)
            {
                IEnumerable <Product> P = db.product.Where(a => a.BrandId == item);

                foreach (var product in P)
                {
                    result.Add(product);
                }
            }

            return result;
        }

        public Product GetProductById(int id)
        {
            return db.product.Find(id);
        }
    }
}

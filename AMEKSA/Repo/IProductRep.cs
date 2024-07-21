using AMEKSA.CustomEntities;
using AMEKSA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Repo
{
   public interface IProductRep
    {
        Product AddProduct(Product obj);

        bool DeleteProduct(int id);

        bool EditProduct(Product obj);

        IEnumerable<Product> GetAllProducts();

        IEnumerable<Product> GetAllBrandProducts(int id);

        Product GetProductById(int id);

        IEnumerable<Product> GetMyProducts(string userId);
    }
}

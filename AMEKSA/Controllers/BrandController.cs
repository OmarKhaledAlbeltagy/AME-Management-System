using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Entities;
using AMEKSA.Models;
using AMEKSA.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class BrandController : ControllerBase
    {
        private readonly IBrandRep brandRep;
        private readonly IProductRep productRep;

        public BrandController(IBrandRep brandRep,IProductRep productRep)
        {
            this.brandRep = brandRep;
            this.productRep = productRep;
        }
      
        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult AccountBalanceSet()
        {
            return Ok(brandRep.AccountBalanceSet());
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(brandRep.GetAllBrands());
        }

        [Route("[controller]/[Action]/{brandId?}")]
        [HttpGet("{brandId?}")]
        public IActionResult GetBrandById(int brandId)
        {
            Brand brand = brandRep.GetBrandById(brandId);
            return Ok(brand);
        }

        [Route("[controller]/[Action]/{brandId?}")]
        [HttpGet("{brandId?}")]
        public IActionResult GetProductsByBrandId(int brandId)
        {
            IEnumerable<Product> products = productRep.GetAllBrandProducts(brandId);
            return Ok(products);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditBrand(Brand obj)
        {
            bool brand = brandRep.EditBrand(obj);
            return Ok(brand);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddProduct(Product obj)
        {
            Product product = productRep.AddProduct(obj);
            return Ok(product);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditProduct(Product obj)
        {
            bool product = productRep.EditProduct(obj);
            return Ok(product);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddBrand(Brand obj)
        {
            Brand brand = brandRep.AddBrand(obj).Result;
          
            return Ok(brand);
        }


  
        [Route("[controller]/[Action]/{userId}")]
        [HttpGet]
        public IActionResult GetMyBrands(string userId)
        {
            return Ok(brandRep.GetMyBrands(userId));
        }

        [Route("[controller]/[Action]/{userId}")]
        [HttpGet]
        public IActionResult GetMyBrandsFLM(string userId)
        {
            return Ok(brandRep.GetMyBrandsFLM(userId));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeleteBrand(int id)
        {
            return Ok(brandRep.DeleteBrand(id));
        }

        //DeleteProduct
        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            return Ok(productRep.DeleteProduct(id));
        }
    }
}

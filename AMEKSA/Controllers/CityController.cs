using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Entities;
using AMEKSA.Repo;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMEKSA.Controllers
{
    [ApiController]
    [EnableCors("allow")]
    public class CityController : ControllerBase
    {
        private readonly ICityRep cityRep;
        private readonly IDisrictRep districtRep;

        public CityController(ICityRep cityRep,IDisrictRep districtRep)
        {
            this.cityRep = cityRep;
            this.districtRep = districtRep;
        }


        [HttpGet]
        [Route("[controller]/[Action]")]
        public IActionResult GetAllCities()
        {
            IEnumerable<City> cities = cityRep.GetAllCities();
            return Ok(cities);
        }

        [HttpPost]
        [Route("[controller]/[Action]")]
        public IActionResult AddCity(City obj)
        {
            City city = cityRep.AddCity(obj);
            return Ok(city);
        }

        [Route("[controller]/[Action]/{cityId?}")]
        [HttpGet("{cityId?}")]
        public IActionResult GetCityById(int cityId)
        {
            City city = cityRep.GetCityById(cityId);
            return Ok(city);
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditCity(City obj)
        {
            bool city = cityRep.EditCity(obj);
            return Ok(city);
        }

        [Route("[controller]/[Action]/{cityId?}")]
        [HttpGet("{cityId}")]
        public IActionResult GetDistrictsByCityId(int cityId)
        {
            IEnumerable<District> districts = districtRep.GetAllCityDistricts(cityId);
            return Ok(districts);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddDistrict(District obj)
        {
            District district = districtRep.AddDistrict(obj);
            return Ok(district);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult EditDistrict(District obj)
        {
            bool district = districtRep.EditDistrict(obj);
            return Ok(district);
        }
    }
}

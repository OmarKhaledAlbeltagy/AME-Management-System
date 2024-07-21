using AMEKSA.Entities;
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
    public class TravelCitiesController : ControllerBase
    {
        private readonly ITravelCitiesRep rep;

        public TravelCitiesController(ITravelCitiesRep rep)
        {
            this.rep = rep;
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult AddCountryAndCity(TravelCities obj)
        {
            return Ok(rep.AddCountryAndCity(obj));
        }

        [Route("[controller]/[Action]/{id}/{NewName}")]
        [HttpGet]
        public IActionResult EditCityName(int id, string NewName)
        {
            return Ok(rep.EditCityName(id, NewName));
        }

        [Route("[controller]/[Action]/{OldName}/{NewName}")]
        [HttpGet]
        public IActionResult EditCountryName(string OldName, string NewName)
        {
            return Ok(rep.EditCountryName(OldName, NewName));
        }

        [Route("[controller]/[Action]/{country}")]
        [HttpGet]
        public IActionResult GetAllCitiesByCountryName(string country)
        {
            return Ok(rep.GetAllCitiesByCountryName(country));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllCountries()
        {
            return Ok(rep.GetAllCountries());
        }
    }
}

using AMEKSA.Entities;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;

namespace AMEKSA.Repo
{
    public interface ITravelCitiesRep
    {
        List<TravelCities> GetAllCountries();

        List<TravelCities> GetAllCitiesByCountryName(string country);

        bool AddCountryAndCity(TravelCities obj);

        bool EditCountryName(string OldName, string NewName);

        bool EditCityName(int id, string NewName);
    }
}

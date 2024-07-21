using AMEKSA.Context;
using AMEKSA.Entities;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore.Storage;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;

namespace AMEKSA.Repo
{
    public class TravelCitiesRep : ITravelCitiesRep
    {
        private readonly DbContainer db;

        public TravelCitiesRep(DbContainer db)
        {
            this.db = db;
        }

        public bool AddCountryAndCity(TravelCities obj)
        {
            TravelCities check = db.travelCities.Where(a => a.City == obj.City && a.Country == obj.Country).FirstOrDefault();

            if (check != null)
            {
                return false;
            }

            else
            {
                db.travelCities.Add(obj);
                db.SaveChanges();
                return true;
            }

        }

        public bool EditCityName(int id, string NewName)
        {
            TravelCities city = db.travelCities.Find(id);

            TravelCities check = db.travelCities.Where(a => a.Country == city.Country && a.City == NewName).FirstOrDefault();

            if (check != null)
            {
                return false;
            }

            else
            {
                city.City = NewName;
                db.SaveChanges();
                return true;
            }


        }

        public bool EditCountryName(string OldName, string NewName)
        {
            List<TravelCities> country = db.travelCities.Where(a=>a.Country == OldName).ToList();

            using (IDbContextTransaction transaction = db.Database.BeginTransaction())
            {

                try
                {
                    foreach (var item in country)
                    {
                        item.Country = NewName;
                    }

                    db.SaveChanges();
                    transaction.Commit();
                    return true;

                }
                catch (Exception ex)
                {
                    var message = ex.InnerException;
                    transaction.Rollback();
                    return false;
                }

            }
            
        }

        public List<TravelCities> GetAllCitiesByCountryName(string country)
        {
            List<TravelCities> result = db.travelCities.Where(a=>a.Country == country).ToList();
            return result;
        }

        public List<TravelCities> GetAllCountries()
        {
            List<TravelCities> result = DistinctByExtension.DistinctBy(db.travelCities.Select(x => x), a => a.Country).OrderBy(a=>a.Country).ToList();

            return result;
        }
    }
}

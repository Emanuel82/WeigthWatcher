using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeigthWatcher.Models;

namespace WeigthWatcher.Services
{
    public class AddressService
    {
        // private FatWatchEntities db = new FatWatchEntities();


        public static IQueryable<Country> GetCountries()
        {
            try
            {
                using (var db = new Models.FatWatchEntities())
                {
                    return db.Countries.AsQueryable();
                }
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public static IQueryable<Region> GetRegions(int countryId)
        {
            try
            {
                using (var db = new Models.FatWatchEntities())
                {
                    return db.Regions.Where(x => x.CountryId == countryId);
                }
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }


        public static IQueryable<City> GetCities(int regionId, int countryId)
        {
            IQueryable<City> cities = null;
            try
            {
                using (var db = new Models.FatWatchEntities())
                {

                    if (regionId == 0)
                        cities = db.Cities.Where(x => x.CountryId == countryId);
                    else
                        cities = db.Cities.Where(x => x.RegionId == regionId);

                }
            }
            catch (System.Exception ex)
            {

            }
            return cities;
        }

        public static int SaveAddress(int cityId, string addressDetails)
        {
            try
            {
                //TODO:
                using (var db = new Models.FatWatchEntities())
                {
                    Address address = new Address() { CityId=cityId };
                    return 0;
                }
            }
            catch (System.Exception ex)
            {
                return -1;
            }
        }
    }
}
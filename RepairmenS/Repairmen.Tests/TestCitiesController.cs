using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repairmen.Controllers;
using System.Collections.Generic;
using Repairmen.Models;
using System.Web.Http.Results;
using System.Web.Http;
using System.Linq;
using System.Net;
using System.Net.Http;
using RepairmenModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http.Hosting;
using DAL;
using AutoMapper;
using System.ComponentModel.DataAnnotations;


namespace Repairmen.Tests
{
    [TestClass]
    public class TestCitiesController
    {
        private Dictionary<Guid, City> cities;
        CitiesController controller;

        [TestInitialize]
        public void Initialize()
        {
            AutoMapperConfiguration.Configure();
            cities = new Dictionary<Guid, City>();
            cities.Clear();
            Guid id1 = new Guid("c42ae80e-346f-4a4d-b4d9-2834c762a27f");
            Guid id2 = new Guid("e4c61d79-dc41-4f9d-8619-05a4f8a6f9a9");
            Guid id3 = new Guid("be77764a-3283-4f9a-b3e6-eb2e65d0b8e8");
            City city1 = new City
            {
                Id = id1,
               CityName = "city1",
               CountryName = "country1"
            };
            City city2 = new City
            {
                Id = id2,
                CityName = "city2",
                CountryName = "country1"
            };
            City city3 = new City
            {
                Id = id3,
                CityName = "city3",
                CountryName = "country3"
            };
            
            cities.Add(id1, city1);
            cities.Add(id2, city2);
            cities.Add(id3, city3);
      
            FakeUnitOfWork unitOfWork = new FakeUnitOfWork();
            unitOfWork.Cities = cities;

            controller = new CitiesController(unitOfWork)
            {
                Request = new HttpRequestMessage()
                {
                    Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
                }
            };
        }

        [TestMethod]
        public void GetCities_ShouldReturnAllCities()
        {
            var response = controller.GetCities();
            Assert.IsNotNull(response);
            var city = response.Content.ReadAsAsync<IEnumerable<City>>().Result;
            Assert.AreEqual(cities.Count(), city.Count());
        }

        [TestMethod]
        public void GetCity_ShouldReturnCorrectCity()
        {
            Guid guid = new Guid("c42ae80e-346f-4a4d-b4d9-2834c762a27f");//city1
            var response = controller.GetCity(guid);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var city = response.Content.ReadAsAsync<City>().Result;
            Assert.AreEqual(guid, city.Id);
            Assert.AreEqual("city1", city.CityName);
        }

        [TestMethod]
        public void GetCity_ShouldReturnNotFound()
        {
            Guid guid = new Guid("CD9F83B0-4C01-4E33-94F5-1783297535E3"); // does not exists
            var response = controller.GetCity(guid);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(0, cities.Where(c => c.Value.Id == guid).Count());
        }

        [TestMethod]
        public void GetCityByName_ShouldReturnCorrectCity()
        {
            var response = controller.GetCityByName("city1");
           // Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var city = response.Content.ReadAsAsync<City>().Result;
            Assert.AreEqual("city1", city.CityName);     
        }

        [TestMethod]
        public void GetCityByName_ShouldReturnNotFound()
        {
            var response = controller.GetCityByName("city4");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(0, cities.Where(c => c.Value.CityName == "city4").Count());
        }

        [TestMethod]
        public void GetCitiesByCountry_ShouldReturnCorrectCity()
        {
            var response = controller.GetCitiesByCountry("country1");
            Assert.IsNotNull(response);
            var city = response.Content.ReadAsAsync<IEnumerable<City>>().Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(cities.Where(c => c.Value.CountryName == "country1").Count(), city.Count());
        }

        [TestMethod]
        public void GetCitiesByCountry_ShouldReturnNotFound()
        {
            var response = controller.GetCitiesByCountry("country4") ;
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(0, cities.Where(c => c.Value.CountryName == "country4").Count());
        }

         public void GetCityByName2_ShouldReturnCorrectCity()
        {
            var response = controller.GetCityByName("country1","city2");
            Assert.IsNotNull(response);
            var city = response.Content.ReadAsAsync<City>().Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("country1", city.CountryName);
            Assert.AreEqual("city2", city.CityName);
        }

        [TestMethod]
        public void GetCityByName2_ShouldReturnNotFound()
        {
            var response = controller.GetCityByName("country4","city2");
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

    
        [TestMethod]
        public void PutCity_ShouldBeInserted()
        {
            int count = cities.Count;
            var cityModel = new CityModel() { Id = Guid.NewGuid(), CityName = "TestCity", CountryName = "TestCountry" };
            var response = controller.PutCity(cityModel);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(count + 1, cities.Count);

        }

        [TestMethod]
        public void IsMarkedRequiredAttribute()
        {
            var propertyInfo = typeof(CityModel).GetProperty("CountryName");
            Assert.IsNotNull(propertyInfo.GetCustomAttributes(typeof(RequiredAttribute), true).FirstOrDefault());
        }

        [TestMethod]
        public void IsMarkedMaxLengthAttribute()
        {
            var propertyInfo = typeof(CityModel).GetProperty("CityName");
            Assert.IsNotNull(propertyInfo.GetCustomAttributes(typeof(MaxLengthAttribute), true).FirstOrDefault());
        }

        [TestMethod]
        public void IsMarkedStringLengthAttribute()
        {
            var propertyInfo = typeof(CityModel).GetProperty("CountryName");
            Assert.IsNotNull(propertyInfo.GetCustomAttributes(typeof(StringLengthAttribute), true).FirstOrDefault());
        }

    }
}

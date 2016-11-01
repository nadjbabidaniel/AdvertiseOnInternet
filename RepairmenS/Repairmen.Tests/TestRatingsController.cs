using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repairmen.Controllers;
using System.Collections.Generic;
using Repairmen.Models;
using System.Web.Http.Results;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using System.Web.Http.Hosting;
using RepairmenModel;
using Repairmen;
using AutoMapper;
using DAL;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace Repairmen.Tests
{
    [TestClass]
    public class TestRatingsController
    {
        Dictionary<Guid, Rating> ratings;
        RatingsController controller;
        Dictionary<Guid, Ad> ads;
        AdsController controllerA;
       
        [TestInitialize]
        public void Initialize()
        {
            AutoMapperConfiguration.Configure();
            FakeUnitOfWork unitOfWork = new FakeUnitOfWork();

            ratings = new Dictionary<Guid, Rating>();
            ratings.Clear();
            Guid id = new Guid("ee4cd8a2-eb0e-453d-a3b5-081b9165942e");

            Guid id1 = new Guid("911b3163-0401-49e8-8589-68b86153bb68");
            Guid id2 = new Guid("cd6aa915-6044-4354-8958-befc3bdfe02e");
           
            Rating rating1 = new Rating
            {
                Id = id1,
                Value = 2,
                UserId = new Guid("93297545-6aff-44cf-911c-fc4b5a88b404"),
                AdId =  id
            };
            Rating rating2 = new Rating
            {
                Id = id2,
                Value = 4,
                UserId = new Guid("13297545-6aff-44cf-911c-fc4b5a88b404"),
                AdId =id
            };
         
            ratings.Add(id1, rating1);
            ratings.Add(id2, rating2);

            unitOfWork.Ratings = ratings;
            controller = new RatingsController(unitOfWork)
            {
                Request = new HttpRequestMessage()
                {
                    Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
                }
            };


            ads = new Dictionary<Guid, Ad>();
            ads.Clear();

           
            Ad ad1 = new Ad
            {
                Id = id,
                Name = "ad1",
                Description = "test",
                Email = "test@mail.com",
                Location = "test",
                CityId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                AvgRate=3,
                VoteCounter=2
            };
            ads.Add(id,ad1);

            unitOfWork.Ads = ads;
            controllerA = new AdsController(unitOfWork)
            {
                Request = new HttpRequestMessage()
                {
                    Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
                }
            };
        }

        [TestMethod]
        public void GetByUserId_ShouldReturnCorrectRating()
        {
            Guid guid = new Guid("93297545-6aff-44cf-911c-fc4b5a88b404");
            var response = controller.GetByUserId(guid);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var rating = response.Content.ReadAsAsync<IEnumerable<Rating>>().Result;
            Assert.AreEqual(ratings.Where(c => c.Value.UserId == guid).Count(), rating.Count());       
        }

        [TestMethod]
        public void GetByUserId_ShouldReturnNotFound()
        {
            Guid guid = new Guid("00297545-6aff-44cf-911c-fc4b5a88b404"); //does not exist
            var response = controller.GetByUserId(guid);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(0, ratings.Where(c => c.Value.UserId == guid).Count());
        }

        [TestMethod]
        public void GetByAdId_ShouldReturnCorrectRating()
        {
            Guid guid = new Guid("ee4cd8a2-eb0e-453d-a3b5-081b9165942e");
            var response = controller.GetByAdId(guid);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var rating = response.Content.ReadAsAsync<IEnumerable<Rating>>().Result;
            Assert.AreEqual(ratings.Where(c => c.Value.AdId == guid).Count(), rating.Count());
        }

        [TestMethod]
        public void GetByAdId_ShouldReturnNotFound()
        {
            Guid guid = new Guid("00297545-6aff-44cf-911c-fc4b5a88b404"); //does not exist
            var response = controller.GetByAdId(guid);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(0, ratings.Where(c => c.Value.AdId == guid).Count());
        }

        [TestMethod]
        public void PutRating_ShouldBeInserted()
        {
            int count = ratings.Count;
            var ratingModel = new RatingModel 
            {
                Id = Guid.NewGuid(),
                Value=2,
                AdId = new Guid("ee4cd8a2-eb0e-453d-a3b5-081b9165942e"),
                UserId = Guid.NewGuid() 
            };
            var response = controller.PutRating(ratingModel);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(count + 1, ratings.Count);
        }

        [TestMethod]
        public void PutRating_ShouldBeUpdated()
        {
            int count = ratings.Count;
            var ratingModel = new RatingModel 
            {
                Id = Guid.NewGuid(),
                Value = 3,
                AdId = new Guid("ee4cd8a2-eb0e-453d-a3b5-081b9165942e"),
                UserId = new Guid("93297545-6aff-44cf-911c-fc4b5a88b404") 
            };
            var response = controller.PutRating(ratingModel);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(count, ratings.Count);
        }
    }
     
}

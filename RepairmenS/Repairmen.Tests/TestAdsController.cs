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
using System.ComponentModel.DataAnnotations;
using Repairmen.Helpers;
using System.Configuration;

namespace Repairmen.Tests
{
    [TestClass]
    public class TestAdsController
    {
        Dictionary<Guid, Ad> ads;
        AdsController controller;
        Dictionary<Guid, InappropriateAd> inappAds;
        Dictionary<Guid, User> users;
        public static CustomConfig Config { get; internal set; }

        [TestInitialize]
        public void Initialize()
        {
            AutoMapperConfiguration.Configure();
            FakeUnitOfWork unitOfWork = new FakeUnitOfWork();
           
            users = new Dictionary<Guid, User>();
            users.Clear();

            Guid idUser1 = new Guid("fa8849b6-c4b6-47d5-96a0-109b0500047a");
            Guid idUser2 = new Guid("39c3a066-78b2-43a3-a04b-28757864114e");

            User user1 = new User
            {
                Id = idUser1,
                Username = "user1",
                Password = "test",
                FirstName = "Test",
                LastName = "Test",
                Email = "test@mail.com",
                RoleId = new Guid("56dc86b4-1229-e411-9417-a41f7255f9b5")
            };

            User user2 = new User
            {
                Id = idUser2,
                Username = "user2",
                Password = "test",
                FirstName = "Test",
                LastName = "Test",
                Email = "test@mail.com",
                RoleId = new Guid("56dc86b4-1229-e411-9417-a41f7255f9b5")
            };
            users.Add(idUser1, user1);
            users.Add(idUser2, user2);

            unitOfWork.Users = users;


            ads = new Dictionary<Guid, Ad>();
            ads.Clear();
            Guid idAd1 = new Guid("ee4cd8a2-eb0e-453d-a3b5-081b9165942e");
            Guid idAd2 = new Guid("ee4cd8a3-eb0e-453d-a3b5-081b9165942e");
            Ad ad1 = new Ad
            {
                Id = idAd1,
                Name = "ad1",
                Description = "test",
                Email = "test@mail.com",
                Location = "test",
                CityId =  new Guid("c42ae80e-346f-4a4d-b4d9-2834c762a27f"), //city1
                CategoryId = new Guid("911b3163-0401-49e8-8589-68b86153bb68"), //category1
                UserId = user1.Id, // user1
                InappCounter=0
            };
            Ad ad2 = new Ad
            {
                Id = idAd2,
                Name = "ad2",
                Description = "test",
                Email = "test@mail.com",
                Location = "test",
                CityId = new Guid("c42ae80e-346f-4a4d-b4d9-2834c762a27f"), //city1
                CategoryId = new Guid("911b3163-0401-49e8-8589-68b86153bb68"), //category1
                UserId = user1.Id, // user1
                InappCounter = 4
            };
         
            ads.Add(idAd1, ad1);
            ads.Add(idAd2, ad2);

            unitOfWork.Ads = ads;

            controller = new AdsController(unitOfWork)
            {
                Request = new HttpRequestMessage()
                {
                    Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
                }
            };

            inappAds = new Dictionary<Guid, InappropriateAd>();
            inappAds.Clear();

            Guid idInapp1 = new Guid("999b3163-0401-49e8-8589-68b86153bb68");
            Guid idInapp2 = new Guid("cccaa915-6044-4354-8958-befc3bdfe02e");
            Guid idInapp3 = new Guid("111b3163-0401-49e8-8589-68b86153bb68");
            Guid idInapp4 = new Guid("bbbaa915-6044-4354-8958-befc3bdfe02e");

            InappropriateAd inappAd1 = new InappropriateAd
            {
                Id = idInapp1,
                Description = "desc1",
                AdId = idAd2,
                UserId = user1.Id
            };
            InappropriateAd inappAd2 = new InappropriateAd
            {
                Id = idInapp2,
                Description = "desc2",
                AdId = idAd2,
                UserId = Guid.NewGuid()
            };
            InappropriateAd inappAd3 = new InappropriateAd
            {
                Id = idInapp3,
                Description = "desc3",
                AdId = idAd2,
                UserId = Guid.NewGuid()
            };
            InappropriateAd inappAd4 = new InappropriateAd
            {
                Id = idInapp4,
                Description = "desc4",
                AdId = idAd2,
                UserId = Guid.NewGuid()
            };

            inappAds.Add(idInapp1, inappAd1);
            inappAds.Add(idInapp2, inappAd2);
            inappAds.Add(idInapp3, inappAd3);
            inappAds.Add(idInapp4, inappAd4);

            unitOfWork.InappropriateAds = inappAds;
        }

        [TestMethod]
        public void GetAds_ShouldReturnAllAds()
        {
            var response = controller.PostAds(null);
            Assert.IsNotNull(response);
            var ad= response.Content.ReadAsAsync<IEnumerable<Ad>>().Result;
            Assert.AreEqual(ads.Count(),ad.Count());
        }

        [TestMethod]
        public void GetAd_ShouldReturnCorrectAd()
        {
            System.Guid guid = new Guid("47054813-e54e-e411-941d-a41f7255f9b5"); //Bajsanski
            var response = controller.GetAd(guid);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var ad = response.Content.ReadAsAsync<Ad>().Result;
            Assert.AreEqual(guid, ad.Id);
            Assert.AreEqual("Bajsanski", ad.Name); 
        }

        [TestMethod]
        public void GetAd_ShouldReturnNotFound()
        {
            Guid guid = new Guid("3bfd6745-1329-e411-9417-a41f7255f9b5"); //random guid
            var response = controller.GetAd(guid);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(0, ads.Where(c => c.Value.Id == guid).Count());
        }

        [TestMethod]
        public void GetByCityId_ShouldReturnCorrectAd()
        {
            System.Guid guid = new Guid("c42ae80e-346f-4a4d-b4d9-2834c762a27f"); //city1
            var response = controller.GetByCityId(guid);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var ad = response.Content.ReadAsAsync<IEnumerable<Ad>>().Result;
            Assert.AreEqual(ads.Where(c => c.Value.CityId == guid).Count(), ad.Count());
        }

        [TestMethod]
        public void GetByCityId_ShouldReturnNotFound()
        {
            Guid guid = new Guid("3bfd6745-1329-e411-9417-a41f7255f9b5"); //random guid
            var response = controller.GetByCityId(guid);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(0, ads.Where(c => c.Value.CityId == guid).Count());
        }

        [TestMethod]
        public void GetByCategoryId_ShouldReturnCorrectAd()
        {
            System.Guid guid = new Guid("911b3163-0401-49e8-8589-68b86153bb68"); //category1
            var response = controller.GetByCategoryId(guid);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var ad = response.Content.ReadAsAsync<IEnumerable<Ad>>().Result;
            Assert.AreEqual(ads.Where(c => c.Value.CategoryId == guid).Count(), ad.Count());
        }

        [TestMethod]
        public void GetByCategoryId_ShouldReturnNotFound()
        {
            Guid guid = new Guid("3bfd6745-1329-e411-9417-a41f7255f9b5"); //random guid
            var response = controller.GetByCategoryId(guid);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(0, ads.Where(c => c.Value.CategoryId == guid).Count());
        }

        //[TestMethod]
        //public void PutAd_ShouldBeInserted()
        //{
        //    int count = ads.Count;
        //    var adModel = new InsertAdModel
        //    {
        //        Name = "Test1",
        //        Email = "test@mail.com",
        //        Description = "Test",
        //        PhoneNumber = "123",
        //        Location = "ns",
        //    };

        //    var response = controller.PutAd(adModel);
        //    Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        //    Assert.AreEqual(count + 1, ads.Count);
        //}



        //[TestMethod]
        //public void GetInappropriateAds_ShouldReturnAds()
        //{
        //    Config = ConfigurationManager.GetSection("customSection") as CustomConfig;
        //    var response = controller.GetInappropriateAds();
        //    Assert.IsNotNull(response);
        //    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        //    var comment = response.Content.ReadAsAsync<IEnumerable<Comment>>().Result;
        //    Assert.AreEqual(ads.Where(c => c.Value.InappCounter > Config.AdsCount).Count(), comment.Count());
        //}




        [TestMethod]
        public void GetDescriptions_ShouldReturnDescriptions()
        {
            Guid guid = new Guid("ee4cd8a3-eb0e-453d-a3b5-081b9165942e"); //ad2 (InappCounter=4)
            var response = controller.GetDescriptions(guid);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var inappComment = response.Content.ReadAsAsync<IEnumerable<InappropriateComment>>().Result;
            Assert.AreEqual(inappAds.Where(c => c.Value.AdId == guid).Count(), inappComment.Count());
        }



        [TestMethod]
        public void GetDescriptions_ShouldReturnNotFound()
        {
            Guid guid = new Guid("ee4cd8a2-eb0e-453d-a3b5-081b9165942e");  //ad1 (InappCounter=0)
            var response = controller.GetDescriptions(guid);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(0, inappAds.Where(c => c.Value.AdId == guid).Count());
        }

        [TestMethod]
        public void PutAdInappropriate_ShouldBeInserted()
        {
            int count = inappAds.Count;
            var inappModel = new InappropriateAdModel
            {
                Id = Guid.NewGuid(),
                Description = "AdDescription",
                AdId = new Guid("ee4cd8a3-eb0e-453d-a3b5-081b9165942e"),//ad2
                UserId = new Guid("39c3a066-78b2-43a3-a04b-28757864114e") //user2
            };
            var response = controller.PutAdInappropriate(inappModel);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(count + 1, inappAds.Count);
        }

        [TestMethod]
        public void PutAdInappropriate_ShouldNotBeInserted()
        {
            int count = inappAds.Count;
            var inappModel = new InappropriateAdModel
            {
                Id = Guid.NewGuid(),
                Description = "AdDescription",
                AdId = new Guid("ee4cd8a3-eb0e-453d-a3b5-081b9165942e"), //ad2
                UserId = new Guid("fa8849b6-c4b6-47d5-96a0-109b0500047a")//user1
            };
            var response = controller.PutAdInappropriate(inappModel);
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.AreEqual(count, inappAds.Count);
        }


        //[TestMethod]
        //public void PostAd_ShouldUpdateAd()
        //{
        //    int count = ads.Where(c => c.Value.InappCounter < 3).Count();

        //    var adModel = new AdModel
        //    {
        //        Id = new Guid("cd6aa915-6044-4354-8958-befc3bdfe02e"),  //comment2 , Counter=4
        //        InappCounter = 0,
        //        UserId = new Guid("39c3a066-78b2-43a3-a04b-28757864114e"),
        //        //AdId = new Guid("ee4cd8a2-eb0e-453d-a3b5-081b9165942e"),
        //        //Text = "Tex2"
        //    };
        //    int countInapp = inappAds.Where(c => c.Value.AdId == adModel.Id).Count();
        //    Assert.AreEqual(4, countInapp);
        //    var response = controller.PostAd(new List<AdModel> { adModel });
        //    countInapp = inappAds.Where(c => c.Value.AdId == adModel.Id).Count();
        //    Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        //    Assert.AreEqual(count + 1, ads.Count());
        //    Assert.AreEqual(0, countInapp);
        //}

        //[TestMethod]
        //public void PostAd_ShouldDeleteAd()
        //{
        //    int count = ads.Where(c => c.Value.InappCounter < 3).Count();

        //    var adModel = new AdModel
        //    {
        //        Id = new Guid("cd6aa915-6044-4354-8958-befc3bdfe02e"),  //comment2 , Counter=4
        //        InappCounter = 4,
        //        UserId = new Guid("39c3a066-78b2-43a3-a04b-28757864114e"),
        //        //AdId = new Guid("ee4cd8a2-eb0e-453d-a3b5-081b9165942e"),
        //        //Text = "Tex2"
        //    };
        //    int countInapp = inappAds.Where(c => c.Value.AdId == adModel.Id).Count();
        //    Assert.AreEqual(4, countInapp);
        //    var response = controller.PostAd(new List<AdModel> { adModel });
        //    countInapp = inappAds.Where(c => c.Value.AdId == adModel.Id).Count();
        //    Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        //    Assert.AreEqual(count, ads.Count());
        //    Assert.AreEqual(0, countInapp);
        //    //var adModel = controllerA.GetAd(commentModel.AdId);
        //    //var ad = adModel.Content.ReadAsAsync<AdModel>().Result;
        //    //Assert.AreEqual(ads.First().Value.CommentCounter, ad.CommentCounter);
        //}



        [TestMethod]
        public void IsMarkedEmail()
        {

            var propertyInfo = typeof(AdModel).GetProperty("Email");
            Assert.IsNotNull(propertyInfo.GetCustomAttributes(typeof(EmailAddressAttribute), true).FirstOrDefault());

        }

        [TestMethod]
        public void IsMarkedStringLengthAttribute()
        {
            var propertyInfo = typeof(AdModel).GetProperty("Location");
            Assert.IsNotNull(propertyInfo.GetCustomAttributes(typeof(StringLengthAttribute), true).FirstOrDefault());

        }

        [TestMethod]
        public void IsMarkedRequiredAttribute()
        {
            var propertyInfo = typeof(AdModel).GetProperty("Location");
            Assert.IsNotNull(propertyInfo.GetCustomAttributes(typeof(RequiredAttribute), true).FirstOrDefault());

        }

    }
}

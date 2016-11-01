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
using Repairmen.Helpers;
using System.Configuration;

namespace Repairmen.Tests
{
    [TestClass]
    public class TestCommentsController
    {
        Dictionary<Guid, Comment> comments;
        CommentsController controllerC;
        AdsController controllerA;
        Dictionary<Guid, InappropriateComment> inappComments;
        Dictionary<Guid, User> users;
        Dictionary<Guid, Ad> ads;
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

            Ad ad1 = new Ad
            {
                Id = idAd1,
                Name = "ad1",
                Description = "test",
                Email = "test@mail.com",
                Location = "test",
                CityId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                CommentCounter=2
            };
            ads.Add(idAd1, ad1);

            unitOfWork.Ads = ads;
           
            controllerA = new  AdsController(unitOfWork)
            {
                Request = new HttpRequestMessage()
                {
                    Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
                }
            };
           

            comments = new Dictionary<Guid, Comment>();
            comments.Clear();

            Guid idComm1 = new Guid("911b3163-0401-49e8-8589-68b86153bb68");
            Guid idComm2 = new Guid("cd6aa915-6044-4354-8958-befc3bdfe02e");
           
            Comment comment1 = new Comment
            {
                Id = idComm1,
                Text = "Text1",
                Counter = 0,
                UserId = user1.Id, 
                AdId = ad1.Id 
            };
            Comment comment2= new Comment
            {
                Id = idComm2,
                Text = "Text2",
                Counter = 4,
                UserId = user2.Id, 
                AdId = ad1.Id 
            };
         
            comments.Add(idComm1, comment1);
            comments.Add(idComm2, comment2);

            unitOfWork.Comments = comments;
            controllerC = new CommentsController(unitOfWork)
            {
                Request = new HttpRequestMessage()
                {
                    Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
                }
            };


            inappComments = new Dictionary<Guid, InappropriateComment>();
            inappComments.Clear();

            Guid idInapp1 = new Guid("999b3163-0401-49e8-8589-68b86153bb68");
            Guid idInapp2 = new Guid("cccaa915-6044-4354-8958-befc3bdfe02e");
            Guid idInapp3 = new Guid("111b3163-0401-49e8-8589-68b86153bb68");
            Guid idInapp4 = new Guid("bbbaa915-6044-4354-8958-befc3bdfe02e");

            InappropriateComment inappComment1 = new InappropriateComment
            {
                Id = idInapp1, 
                Description = "desc1",
                CommentId = idComm2,
                UserId = user1.Id
            };
            InappropriateComment inappCommet2 = new InappropriateComment
            {
                Id = idInapp2,
                Description = "desc2",
                CommentId = idComm2,
                UserId = Guid.NewGuid()
            };
            InappropriateComment inappComment3 = new InappropriateComment
            {
                Id = idInapp3,
                Description = "desc3",
                CommentId = idComm2,
                UserId = Guid.NewGuid()
            };
            InappropriateComment inappCommet4 = new InappropriateComment
            {
                Id = idInapp4,
                Description = "desc4",
                CommentId = idComm2,
                UserId = Guid.NewGuid()
            };

            inappComments.Add(idInapp1, inappComment1);
            inappComments.Add(idInapp2, inappCommet2);
            inappComments.Add(idInapp3, inappComment3);
            inappComments.Add(idInapp4, inappCommet4);

            unitOfWork.InappropriateComments = inappComments;
        }

        [TestMethod]
        public void GetCommentsByUserId_ShouldReturnCorrectComment()
        {
            Guid guid = new Guid("39c3a066-78b2-43a3-a04b-28757864114e");//user2
            var response = controllerC.GetCommentsByUserId(guid);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var comment = response.Content.ReadAsAsync<IEnumerable<Comment>>().Result;
            Assert.AreEqual(comments.Where(c => c.Value.UserId == guid).Count(), comment.Count());
        }

        [TestMethod]
        public void GetCommentsByUserId_ShouldReturnNotFound()
        {
            Guid guid = new Guid("00297545-6aff-44cf-911c-fc4b5a88b404"); //does not exist
            var response = controllerC.GetCommentsByUserId(guid);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(0,comments.Where(c => c.Value.UserId == guid).Count());
        }


        [TestMethod]
        public void GetCommentsByAdId_ShouldReturnCorrectComment()
        {
            Guid guid = new Guid("ee4cd8a2-eb0e-453d-a3b5-081b9165942e");
            var response = controllerC.GetCommentsByAdId(guid);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var comment = response.Content.ReadAsAsync<IEnumerable<Comment>>().Result;
            Assert.AreEqual(comments.Where(c => c.Value.AdId == guid).Count(), comment.Count());
        }

        [TestMethod]
        public void GetCommentsByAdId_ShouldReturnNotFound()
        {
            Guid guid = new Guid("00297545-6aff-44cf-911c-fc4b5a88b404"); //does not exist
            var response = controllerC.GetCommentsByAdId(guid);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(0, comments.Where(c => c.Value.AdId == guid).Count());
        }

        //[TestMethod]
        //public void GetInappropriateComments_ShouldReturnComments()
        //{
        //    Config = ConfigurationManager.GetSection("customSection") as CustomConfig;
        //    var response = controllerC.GetInappropriateComments();
        //    Assert.IsNotNull(response);
        //    //Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        //    var comment = response.Content.ReadAsAsync<IEnumerable<Comment>>().Result;
        //    Assert.AreEqual(comments.Where(c => c.Value.Counter > Config.CommentsCount).Count(), comment.Count());
        //}

        [TestMethod]
        public void GetDescriptions_ShouldReturnDescriptions()
        {
            Guid guid = new Guid("cd6aa915-6044-4354-8958-befc3bdfe02e"); //comment2 (Counter=4)
            var response = controllerC.GetDescriptions(guid);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var inappComment = response.Content.ReadAsAsync<IEnumerable<InappropriateComment>>().Result;
            Assert.AreEqual(inappComments.Where(c => c.Value.CommentId == guid).Count(), inappComment.Count());
        }

        [TestMethod]
        public void GetDescriptions_ShouldReturnNotFound()
        {
            Guid guid = new Guid("911b3163-0401-49e8-8589-68b86153bb68");  //comment1 (Counter=0)
            var response = controllerC.GetDescriptions(guid);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(0, inappComments.Where(c => c.Value.CommentId == guid).Count());
        }

        [TestMethod]
        public void PutComment_ShouldBeInserted()
        {
            int count = comments.Count;
            var commentModel = new CommentModel 
            { 
                Id = Guid.NewGuid(),
                Text = "CommentInsert",
                AdId = new Guid("ee4cd8a2-eb0e-453d-a3b5-081b9165942e"),
                UserId = new Guid("fa8849b6-c4b6-47d5-96a0-109b0500047a")//user1
            };
            var response = controllerC.PutComment(commentModel);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(count + 1, comments.Count);
            Assert.AreEqual(0, commentModel.Counter);
            var comment = response.Content.ReadAsAsync<Comment>().Result;
            Assert.AreEqual("user1", comment.Username);
            var adModel = controllerA.GetAd(comment.AdId);
            var ad=adModel.Content.ReadAsAsync<AdModel>().Result;
            Assert.AreEqual(ads.First().Value.CommentCounter,ad.CommentCounter);
        }

        [TestMethod]
        public void PutCommentInappropriate_ShouldBeInserted()
        {
            int count = inappComments.Count;
             var inappModel = new InappropriateCommentModel
            {
                Id = Guid.NewGuid(),
                Description = "CommentDescription",
                CommentId = new Guid("cd6aa915-6044-4354-8958-befc3bdfe02e"),//comment2
                UserId = new Guid("39c3a066-78b2-43a3-a04b-28757864114e") //user2
            };
            var response = controllerC.PutCommentInappropriate(inappModel);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(count + 1, inappComments.Count);
        }

        [TestMethod]
        public void PutCommentInappropriate_ShouldNotBeInserted()
        {
            int count = inappComments.Count;
            var inappModel = new InappropriateCommentModel
            {
                Id = Guid.NewGuid(),
                Description = "CommentDescription",
                CommentId = new Guid("cd6aa915-6044-4354-8958-befc3bdfe02e"), //comment2
                UserId = new Guid("fa8849b6-c4b6-47d5-96a0-109b0500047a")//user1
            };
            var response = controllerC.PutCommentInappropriate(inappModel);
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.AreEqual(count, inappComments.Count);
        }


        [TestMethod]
        public void PostComment_ShouldUpdateComment()
        {
            //Config = ConfigurationManager.GetSection("customSection") as CustomConfig;
            //int count = comments.Where(c => c.Value.Counter < Config.CommentsCount).Count();
            int count = comments.Where(c => c.Value.Counter < 3).Count();
           
           var commentModel = new CommentModel
           {
               Id = new Guid("cd6aa915-6044-4354-8958-befc3bdfe02e"),  //comment2 , Counter=4
               Counter = 0,
               UserId = new Guid("39c3a066-78b2-43a3-a04b-28757864114e"),
               AdId = new Guid("ee4cd8a2-eb0e-453d-a3b5-081b9165942e"),
               Text = "Tex2"
           };
           int countInapp = inappComments.Where(c => c.Value.CommentId == commentModel.Id).Count();
           Assert.AreEqual(4, countInapp);
           var response = controllerC.PostComment(new List<CommentModel> { commentModel });
           countInapp = inappComments.Where(c => c.Value.CommentId == commentModel.Id).Count();
           Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
           Assert.AreEqual(count+1, comments.Count());
           Assert.AreEqual(0,countInapp);
        }

        [TestMethod]
        public void PostComment_ShouldDeleteComment()
        {
            int count = comments.Where(c => c.Value.Counter < 3).Count();

            var commentModel = new CommentModel
            {
                Id = new Guid("cd6aa915-6044-4354-8958-befc3bdfe02e"),  //comment2 , Counter=4
                Counter = 4,
                UserId = new Guid("39c3a066-78b2-43a3-a04b-28757864114e"),
                AdId = new Guid("ee4cd8a2-eb0e-453d-a3b5-081b9165942e"),
                Text = "Tex2"
            };
            int countInapp = inappComments.Where(c => c.Value.CommentId == commentModel.Id).Count();
            Assert.AreEqual(4, countInapp);
            var response = controllerC.PostComment(new List<CommentModel> { commentModel });
            countInapp = inappComments.Where(c => c.Value.CommentId == commentModel.Id).Count();
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            Assert.AreEqual(count, comments.Count());
            Assert.AreEqual(0, countInapp);
            var adModel = controllerA.GetAd(commentModel.AdId);
            var ad = adModel.Content.ReadAsAsync<AdModel>().Result;
            Assert.AreEqual(ads.First().Value.CommentCounter, ad.CommentCounter);
        }

    }
}

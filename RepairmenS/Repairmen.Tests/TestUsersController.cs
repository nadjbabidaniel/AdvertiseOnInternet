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

namespace Repairmen.Tests
{
    [TestClass]
    public class TestUsersController
    {
        Dictionary<Guid,User> users;
        UsersController controller;

        [TestInitialize]
        public void Initialize()
        {
           
            AutoMapperConfiguration.Configure();
            users = new Dictionary<Guid, User>();
            Guid id1 = new Guid ("93297545-6aff-44cf-911c-fc4b5a88b404");
            Guid id2 = new Guid ("e4c61d79-dc41-4f9d-8619-05a4f8a6f9a9");
            Guid id3 = new Guid ("be77764a-3283-4f9a-b3e6-eb2e65d0b8e8");
            User user1 = new User  {
                Id = id1,
                Username = "user1",
                Password = "test",
                FirstName = "Test",
                LastName = "Test",
                Email = "test@mail.com",
                RoleId = new Guid("56dc86b4-1229-e411-9417-a41f7255f9b5") };
            User user2 = new User  {
                Id = id2,
                Username = "user2",
                Password = "test",
                FirstName = "Test",
                LastName = "Test",
                Email = "test@mail.com",
                RoleId = new Guid("56dc86b4-1229-e411-9417-a41f7255f9b5") };
            User user3 = new User  {
                Id= id3,
                Username = "user3",
                Password = "test",
                FirstName = "Test",
                LastName = "Test",
                Email = "test@mail.com",
                RoleId = new Guid("56dc86b4-1229-e411-9417-a41f7255f9b5") };
            users.Add(id1, user1);
            users.Add(id2, user2);
            users.Add(id3, user3);

            FakeUnitOfWork unitOfWork = new FakeUnitOfWork();
            unitOfWork.Users = users;
            controller = new UsersController(unitOfWork)
            {
                Request = new HttpRequestMessage()
                {
                    Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
                }
            };
            
        }

        [TestMethod]
        public void GetUsers_ShouldReturnAllUsers()
        {
            var response = controller.GetUsers();
            Assert.IsNotNull(response);
            var user = response.Content.ReadAsAsync<IEnumerable<User>>().Result;
            Assert.AreEqual(users.Count(), user.Count());
        }

        [TestMethod]
        public void GetUser_ShouldReturnCorrectUser()
        {
            System.Guid guid = Guid.Parse("e4c61d79-dc41-4f9d-8619-05a4f8a6f9a9"); //user2
            var response = controller.GetUser(guid);       
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var user = response.Content.ReadAsAsync<User>().Result;
            Assert.AreEqual(guid, user.Id);
            Assert.AreEqual("user2", user.Username);

        }

        [TestMethod]
        public void GetUser_ShouldReturnNotFound()
        {
            Guid guid = new Guid("64451e86-3016-40f3-8661-7dff8d01613f"); //random guid
            var response = controller.GetUser(guid);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}

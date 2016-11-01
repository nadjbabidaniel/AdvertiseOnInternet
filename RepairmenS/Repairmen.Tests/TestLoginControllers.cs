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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace Repairmen.Tests
{
    [TestClass]
    public class TestLoginControllers
    {
        Dictionary<Guid, User> userModel;
        Dictionary<Guid, Role> roleModel;
        LoginController controller;

        [TestInitialize]
        public void Initialize() {
            AutoMapperConfiguration.Configure();
            userModel = new Dictionary<Guid, User>();
            roleModel = new Dictionary<Guid, Role>();
            Guid id1 = new Guid("93297545-6aff-44cf-911c-fc4b5a88b404");
            Guid id2 = new Guid("e4c61d79-dc41-4f9d-8619-05a4f8a6f9a9");
            Guid id3 = new Guid("be77764a-3283-4f9a-b3e6-eb2e65d0b8e8");
            User user1 = new User
            {
                Id = id1,
                Username = "user1",
                Password = "test",
                FirstName = "Test",
                LastName = "Test",
                Email = "test@mail.com",
                RoleId = new Guid("56dc86b4-1229-e411-9417-a41f7255f9b5")
            };
            User user2 = new User
            {
                Id = id2,
                Username = "user2",
                Password = "test",
                FirstName = "Test",
                LastName = "Test",
                Email = "test@mail.com",
                RoleId = new Guid("56dc86b4-1229-e411-9417-a41f7255f9b5")
            };
            User user3 = new User
            {
                Id = id3,
                Username = "user3",
                Password = "test",
                FirstName = "Test",
                LastName = "Test",
                Email = "test@mail.com",
                RoleId = new Guid("56dc86b4-1229-e411-9417-a41f7255f9b5")
            };
            Role role = new Role { Name = "repairmen", Id = new Guid("56dc86b4-1229-e411-9417-a41f7255f9b5") };
            userModel.Add(id1, user1);
            userModel.Add(id2, user2);
            userModel.Add(id3, user3);
            roleModel.Add(role.Id,role);

            FakeUnitOfWork unitOfWork = new FakeUnitOfWork();
            unitOfWork.Users = userModel;
            unitOfWork.Roles = roleModel;

            controller = new LoginController(unitOfWork)
            {
                Request = new HttpRequestMessage()
                {
                    Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
                }
            };
            
        }

        [TestMethod]
        public void UserExists_ShouldReturnTrue()
        {
            var response = controller.UserExists("user1");
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var result= response.Content.ReadAsAsync<object>().Result;
            Assert.AreEqual("{ userExists = true }", result.ToString());  
        }

        [TestMethod]
        public void UserExists_ShouldReturnFalse()
        {
            var response = controller.UserExists("user33");
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var result = response.Content.ReadAsAsync<object>().Result;
            Assert.AreEqual("{ userExists = false }", result.ToString());
        }

        [TestMethod]
        public void RandomString_ShouldReturnJObject()
        {
            var username = "testUsername";
            var response = controller.RandomString(username);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void PutUser_ShouldBeInserted()
        {
            int count = userModel.Count;
            UserModel user = new UserModel
            {
                Id = new Guid("56dc86b4-1229-e411-9417-a41f7255f8b5"),
                Username = "user22",
                Password = "test",
                FirstName = "Test",
                LastName = "Test",
                Email = "test@mail.com",
                RoleId = new Guid("56dc86b4-1229-e411-9417-a41f7255f9b5")
            };
            var response = controller.PutUser(user);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(count + 1, userModel.Count);
        }

        [TestMethod]
        public void PutUser_ShouldNotBeInserted_AlreadyExist()
        {
            int count = userModel.Count;
            UserModel user = new UserModel
            {
                Username = "user1",
                Password = "test",
                FirstName = "Test",
                LastName = "Test",
                Email = "test@mail.com",
                RoleId = new Guid("56dc86b4-1229-e411-9417-a41f7255f9b5"),
                Id = new Guid("93297545-6aff-44cf-911c-fc4b5a88b404")
            };
            var response = controller.PutUser(user);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual(count, userModel.Count);
        }

        [TestMethod]
        public void IsMarkedStringLengthAttribute()
        {
            var propertyInfo = typeof(UserModel).GetProperty("Username");
            Assert.IsNotNull(propertyInfo.GetCustomAttributes(typeof(StringLengthAttribute), true).FirstOrDefault());
        }

        [TestMethod]
        public void IsMarkedRequiredAttribute()
        {
            var propertyInfo = typeof(UserModel).GetProperty("Username");
            Assert.IsNotNull(propertyInfo.GetCustomAttributes(typeof(RequiredAttribute), true).FirstOrDefault());
        }

        [TestMethod]
        public void IsMarkedEmailAdressAttribute()
        {
            var propertyInfo = typeof(UserModel).GetProperty("Email");
            Assert.IsNotNull(propertyInfo.GetCustomAttributes(typeof(EmailAddressAttribute), true).FirstOrDefault());
        }

        [TestMethod]
        public void IsMarkedMaxLengthAttribute()
        {
            var propertyInfo = typeof(UserModel).GetProperty("Password");
            Assert.IsNotNull(propertyInfo.GetCustomAttributes(typeof(MaxLengthAttribute), true).FirstOrDefault());
        }
    }
}

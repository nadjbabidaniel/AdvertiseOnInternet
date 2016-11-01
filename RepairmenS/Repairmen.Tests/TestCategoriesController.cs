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
    public class TestCategoriesController
    {
        Dictionary<Guid, Category> categories;
        CategoriesController controller;

        [TestInitialize]
        public void Initialize()
        {
            AutoMapperConfiguration.Configure();
            categories = new Dictionary<Guid, Category>();
            categories.Clear();
            Guid id1 = new Guid("911b3163-0401-49e8-8589-68b86153bb68");
            Guid id2 = new Guid("cd6aa915-6044-4354-8958-befc3bdfe02e");
            Guid id3 = new Guid("cd6aa914-6044-4354-8958-befc3bdfe02e");
            Category category1 = new Category
            { 
                Id = id1,
                CatName = "Category1", 
                Approved = false
            };
            Category category2 = new Category
            {
                Id = id2,
                CatName = "Category2",
                Approved = true
            };
            Category category3 = new Category
            {
                Id = id3,
                CatName = "Category3",
                Approved = true
            };
            categories.Add(id1, category1);
            categories.Add(id2, category2);
            categories.Add(id3, category3);
         
            FakeUnitOfWork unitOfWork = new FakeUnitOfWork();
            unitOfWork.Categories = categories;
            controller = new CategoriesController(unitOfWork)
            {
                Request = new HttpRequestMessage()
                {
                    Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
                }
            };
        }

        [TestMethod]
        public void GetCategories_ShouldReturnApprovedCategories()
        {
            var response = controller.GetCategories(true);
            Assert.IsNotNull(response);
            var category = response.Content.ReadAsAsync<IEnumerable<Category>>().Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(categories.Where(c => c.Value.Approved == true).Count(), category.Count());
        }

        [TestMethod]
        public void GetCategories_ShouldReturnUnapprovedCategories()
        {
            var response = controller.GetCategories(false);
            Assert.IsNotNull(response);
            var category = response.Content.ReadAsAsync<IEnumerable<Category>>().Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(categories.Where(c => c.Value.Approved == false).Count(), category.Count());
        }

        [TestMethod]
        public void GetCategory_ShouldNotReturnCorrectCategory()
        {
            Guid guid = new Guid("911b3163-0401-49e8-8589-68b86153bb68");//Category1
            var response = controller.GetCategory(guid);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var category = response.Content.ReadAsAsync<Category>().Result;
            Assert.AreEqual(guid, category.Id);
            Assert.AreEqual("Category1", category.CatName);
        }

        [TestMethod]
        public void GetCategory_ShouldReturnNotFound()
        {
            Guid guid = new Guid("5bf78d57-97ba-4ffe-b728-b24c55d015c6");//random guid
            var response = controller.GetCategory(guid);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(0, categories.Where(c => c.Value.Id == guid).Count());
        }

        [TestMethod]
        public void GetCategoryByName_ShouldReturnCorrectCategory()
        {
            var response = controller.GetCategoryByName("Category2");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var category = response.Content.ReadAsAsync<Category>().Result;
            Assert.AreEqual("Category2", category.CatName);
        }

        [TestMethod]
        public void GetCategoryByName_ShouldReturnNotFound()
        {
            var response = controller.GetCategoryByName("CategoryX");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(0, categories.Where(c => c.Value.CatName == "CategoryX").Count());
        }

        [TestMethod]
        public void PostCategory_ShouldUpdateCategory()
        {
            var categoryModel = new CategoryModel { Id = new Guid("911b3163-0401-49e8-8589-68b86153bb68"), CatName = "CategoryRename", Approved = true };
            var response = controller.PostCategory(new List<CategoryModel> { categoryModel });
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            var category = categories.Where(c => c.Value.Approved == true).Count();
            Assert.AreEqual("CategoryRename", categoryModel.CatName);
        }

        [TestMethod]
        public void PostCategory_ShouldDeleteCategory()
        {
            var categoryModel = new CategoryModel { Id = new Guid("911b3163-0401-49e8-8589-68b86153bb68"), CatName = "CategoryRename", Delete = true };
            var response = controller.PostCategory(new List<CategoryModel> { categoryModel });
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            var category = categories.Where(c => c.Value.Id.Equals("911b3163-0401-49e8-8589-68b86153bb68")).FirstOrDefault();
            Assert.AreEqual(null, category.Value);


        }


        [TestMethod]
        public void PutCategory_ShouldBeInserted()
        {
            int count = categories.Count;
            var categoryModel = new CategoryModel { Id = new Guid("618fc82d-f63d-4fb2-9386-aa00bc2abbf7"), CatName = "CategoryInsert", Approved = true };
            var response = controller.PutCategory(categoryModel);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(count+1,categories.Count);
        }
     

        [TestMethod]
        public void PutCategory_IsMarkedRequired()
        {
            var propertyInfo = typeof(CategoryModel).GetProperty("CatName");
            Assert.IsNotNull(propertyInfo.GetCustomAttributes(typeof(RequiredAttribute),true).FirstOrDefault());
        }

        [TestMethod]
        public void PutCategory_IsMarkedStringLength()
        {
            var propertyInfo = typeof(CategoryModel).GetProperty("CatName");
            Assert.IsNotNull(propertyInfo.GetCustomAttributes(typeof(StringLengthAttribute), true).FirstOrDefault());
        }
    }
}

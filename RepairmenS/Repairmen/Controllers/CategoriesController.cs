using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Cors;
using RepairmenModel;
using Repairmen.ActionFilters;
using Repairmen.Models;
using DAL;
using Newtonsoft.Json.Linq;
using AutoMapper;
using System.Text;


namespace Repairmen.Controllers
{
    [EnableCors("http://192.168.5.205:8089,http://localhost:60923,http://htrepairmen.cloudapp.net", "*", "*")] // With Enable cross-origin resource sharing, we allow this controller be called from different domain.
    [RoutePrefix("api/Categories")]
    public class CategoriesController : RepairmenApiControllerBase
    {
        private IUnitOfWork unitOfWork;

        public CategoriesController()
        {
            unitOfWork = new UnitOfWork();
        }

        public CategoriesController(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        // GET api/Categories
        [Route("")]
        public HttpResponseMessage GetCategories(bool approved)
        {
            IEnumerable<CategoryModel> catModels;
            try
            {
                catModels = unitOfWork.CategoryRepository.Get(c => c.Approved == approved).OrderBy(c=> c.CatName).Select(x => Mapper.Map<CategoryModel>(x));
                return OK(catModels);
            }
            catch (Exception ex)
            {
               return InternalServerError("Error. Cannot run query over database.",ex);
            }
        }

        // GET api/Categories/5
        [Route("{id:Guid}")]
        //[ResponseType(typeof(CategoryModel))]
        public HttpResponseMessage GetCategory(System.Guid id)
        {
            CategoryModel category = new CategoryModel();
            try
            {
                category = Mapper.Map<CategoryModel>(unitOfWork.CategoryRepository.GetByID(id));
                return OK(category);
            }
            catch
            {
                return NotFound("Not Found. There is no category with provided Guid.");
            }
            
        }

        // GET: api/Categories/CatName
        [Route("name/{CatName}")]
        [HttpGet]
        public HttpResponseMessage GetCategoryByName(string CatName)
        {
            CategoryModel category;
 
            try
            {
                category = Mapper.Map<CategoryModel>(unitOfWork.CategoryRepository.Get(u => u.CatName == CatName).FirstOrDefault());
                return OK(category);
            }
            catch
            {
                return NotFound("Not Found. There is no category with provided Name.");
            }
        }

        // PUT api/Categories
        //[ValidationResponseFilter]
        [Route("")]
        [HttpPut]
        public HttpResponseMessage PutCategory(CategoryModel categoryInput) 
        {
            var category = Mapper.Map<Category>(categoryInput);
            category.Id = Guid.NewGuid();
            category.CatName = categoryInput.CatName;
            category.Approved = false;

            try
            {
                unitOfWork.CategoryRepository.Insert(category);
                unitOfWork.Save();
                return Create();
            }
            catch (Exception ex)
            {
                return InternalServerError("Server Error: Can not insert category in database.", ex);
            }
        }

        // PUT api/Categories
        //[ValidationResponseFilter]
        [Route("")]
        [HttpPost]
        public HttpResponseMessage PostCategory(List<CategoryModel> categoriesInput)
        {         
            foreach (CategoryModel categoryInput in categoriesInput)
            {
                var category = Mapper.Map<Category>(categoryInput);
                try
                {
                    if (category.Approved == true)
                        unitOfWork.CategoryRepository.Update(category);
                    else if(categoryInput.Delete == true)
                        unitOfWork.CategoryRepository.Delete(category);
                    unitOfWork.Save();
                }
                catch (Exception ex)
                {
                    return InternalServerError("Server Error: Can not insert category in database.", ex);
                }
            }
            return NoContent();
        }

    }

}
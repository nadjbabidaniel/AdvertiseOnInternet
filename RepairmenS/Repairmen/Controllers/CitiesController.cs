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
using RepairmenModel;
using DAL;
using Repairmen.Models;
using Newtonsoft.Json.Linq;
using System.Web.Http.Cors;
using AutoMapper;

namespace Repairmen.Controllers
{
    [EnableCors("http://192.168.5.205:8089,http://localhost:60923,http://htrepairmen.cloudapp.net", "*", "*")] // With Enable cross-origin resource sharing, we allow this controller be called from different domain.
    [RoutePrefix("api/Cities")]
    public class CitiesController : RepairmenApiControllerBase
    {
        private IUnitOfWork unitOfWork;

       public CitiesController()
        {
            unitOfWork = new UnitOfWork();
        }

       public CitiesController(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public HttpResponseMessage Post(CityModel city)
        {
            return Create(city);
        }

        // GET: api/Cities
        [Route("")]
        public HttpResponseMessage GetCities()
        {
            IEnumerable<CityModel> cityModel;
            try
            {
                cityModel = unitOfWork.CityRepository.Get().Select(x => Mapper.Map<CityModel>(x));
                return OK(cityModel);
            }
            catch (Exception ex)
            {
                return InternalServerError("Error. Cannot run query over database. ",ex);
            }
        }

        // GET: api/Cities/5
        [Route("{id:Guid}")]
        [ResponseType(typeof(CityModel))]
        public HttpResponseMessage GetCity(Guid id)
        {

            CityModel cityModel = new CityModel();
            try
            {
                cityModel = Mapper.Map<CityModel>(unitOfWork.CityRepository.GetByID(id));
                return OK(cityModel);
            }
            catch
            {
                return NotFound("Not Acceptable. There is no City with provided Guid.");
            }

        }

        //GET: api/Cities/Country/{country}
        [Route("Country/{country}")]
        [ResponseType(typeof(CityModel))]
        public HttpResponseMessage GetCitiesByCountry(string country)
        {
            IEnumerable<CityModel> cityModel;
            try
            {
                cityModel = unitOfWork.CityRepository.Get(c => c.CountryName == country).OrderBy(c => c.CityName).Select(x => Mapper.Map<CityModel>(x));
                return OK(cityModel);
            }
            catch
            {
                return NotFound("Not Acceptable. There is no Cities for provided Country.");
            }
        }

        //GET: api/Cities/City/{CityName}
        [Route("City/{city}")]
        [ResponseType(typeof(CityModel))]
        public HttpResponseMessage GetCityByName(string city)
        {
            CityModel cityModel = new CityModel();
           

            try
            {
                cityModel = unitOfWork.CityRepository.Get(c => c.CityName.Equals(city)).Select(x => Mapper.Map<CityModel>(x)).FirstOrDefault();
                return OK(cityModel);
            }
            catch
            {
                return NotFound("Not Acceptable. There is no City with provided Name.");
            }
        }

        //GET: api/Cities/City/{Country}/{CityName}
        [Route("City/{country}/{city}")]
        [ResponseType(typeof(CityModel))]
        public HttpResponseMessage GetCityByName(string country, string city)
        {
            CityModel cityModel = new CityModel();

            try
            {
                cityModel = Mapper.Map<CityModel>(unitOfWork.CityRepository.Get(x => x.CountryName == country && x.CityName == city).FirstOrDefault());
                return OK(cityModel);
            }
            catch
            {
                return NotFound("Not Acceptable. There is no City with provided Country and City names.");
            }
        }


        // PUT api/Cities
        [Route("")]
        [HttpPut]
        public HttpResponseMessage PutCity(CityModel cityModel)
        {
            var city = Mapper.Map<City>(cityModel);
            city.Id = Guid.NewGuid();
            unitOfWork.CityRepository.Insert(city);
            try
            {
                unitOfWork.Save();
                return Create();
            }
            catch (Exception ex)
            {
                return InternalServerError("Server Error: Can not insert city in database.",ex);
            }
        }

    }
}
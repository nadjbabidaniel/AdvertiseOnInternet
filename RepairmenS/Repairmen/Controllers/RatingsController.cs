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
using AutoMapper;
using Repairmen.Helpers;
using System.Data.Entity.Core;
using Newtonsoft.Json.Linq;
using System.Security.Claims;


namespace Repairmen.Controllers
{
    [EnableCors("http://192.168.5.205:8089,http://localhost:60923,http://htrepairmen.cloudapp.net", "*", "*")] // With Enable cross-origin resource sharing, we allow this controller be called from different domain.
    [RoutePrefix("api/Ratings")]
    public class RatingsController : RepairmenApiControllerBase
    {
        private IUnitOfWork unitOfWork;

        public RatingsController()
        {
            unitOfWork = new UnitOfWork();
        }

        public RatingsController(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public HttpResponseMessage Post(RatingModel rating)
        {
            return OK(rating);
        }


        // GET api/Ratings/UserId/{Guid}
        [Route("UserId/{userId:Guid}")]
        [ResponseType(typeof(RatingModel))]
        public HttpResponseMessage GetByUserId(System.Guid userId)
        {
            IEnumerable<RatingModel> ratingModel;
            try
            {
                ratingModel = unitOfWork.RatingRepository.Get(a => a.UserId == userId).Select(x => Mapper.Map<RatingModel>(x));
                return OK(ratingModel);
            }
            catch
            {
                return NotFound("Not Acceptable. There is no Rating with provided Guid.");
            }

        }

        // GET api/Ratings/AdId/{Guid}
        [Route("AdId/{adId:Guid}")]
        [ResponseType(typeof(RatingModel))]
        public HttpResponseMessage GetByAdId(System.Guid adId)
        {
            IEnumerable<RatingModel> ratingModel;
            try
            {
                ratingModel = unitOfWork.RatingRepository.Get(r =>r.AdId == adId).Select(x => Mapper.Map<RatingModel>(x));
                return OK(ratingModel);
            }
            catch (ObjectNotFoundException ex)
            {
                return NotFound("Not Acceptable. There is no Rating with provided Guid.");
            }
            
        }

        // PUT api/Ratings
        //[ValidationResponseFilter]
        [Route("")]
        //[Authorize(Roles = "admin,repairmen")]
        [HttpPut]
        public HttpResponseMessage PutRating(RatingModel ratingModel)
        {

             AdModel adModel;
            
            try
            {
                adModel = unitOfWork.AdRepository.Get(a => a.Id == ratingModel.AdId).Select(x => Mapper.Map<AdModel>(x)).FirstOrDefault();
               
                Rating rating;
                decimal? oldRate = 0;
                if (RateExists(ratingModel) != null){
                    rating = Mapper.Map<Rating>(RateExists(ratingModel));
                    oldRate = rating.Value;
                    rating.Value = ratingModel.Value;
                    unitOfWork.RatingRepository.Update(rating);
                }
                   
                else
                {
                    rating = Mapper.Map<Rating>(ratingModel);
                    unitOfWork.RatingRepository.Insert(rating);
                adModel.VoteCounter++;
                }
                decimal? sumValue = 0;
                if (AdExists(adModel))
                    sumValue = unitOfWork.RatingRepository.Get(r => r.AdId == adModel.Id).Select(x => Mapper.Map<RatingModel>(x)).Sum(r => r.Value);
                decimal sum = Convert.ToDecimal(sumValue) - Convert.ToDecimal(oldRate - ratingModel.Value);
                adModel.AvgRate = sum / adModel.VoteCounter;
                 var ad = Mapper.Map<Ad>(adModel);
                unitOfWork.AdRepository.Update(ad);
               
                unitOfWork.Save();
                return Create();
                
            }
            catch (ObjectNotFoundException)
            {
                return NotFound("Not Found. There is no Ad");
              
            }
            catch (Exception ex)
            {
                return InternalServerError("Server error. Can not insert Rating in database.", ex);
            }
        }



        private bool AdExists(AdModel adModel)
        {
            try
            {
                unitOfWork.RatingRepository.Get(r => r.AdId == adModel.Id);
                return true;
            }
            catch
            {
                return false;
            }
        }


        private RatingModel RateExists(RatingModel ratingModel)
        {
            try
            {
                return unitOfWork.RatingRepository.Get(r => r.UserId == ratingModel.UserId && r.AdId == ratingModel.AdId).Select(x => Mapper.Map<RatingModel>(x)).First();

            }
            catch
            {
                return null;
            }
        }
        }
    }

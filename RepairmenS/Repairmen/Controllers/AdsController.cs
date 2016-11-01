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
using Common;
using System.Configuration;
using Repairmen.Controllers.Helpers;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;


namespace Repairmen.Controllers
{
    [EnableCors("http://192.168.5.205:8089,http://localhost:60923,http://htrepairmen.cloudapp.net", "*", "*")] // With Enable cross-origin resource sharing, we allow this controller be called from different domain.
    [RoutePrefix("api/Ads")]
    public class AdsController : RepairmenApiControllerBase
    {
        public static CustomConfig Config = ConfigurationManager.GetSection("customSection") as CustomConfig;
         
        private IUnitOfWork unitOfWork;

        public AdsController()
        {
            unitOfWork = new UnitOfWork();
        }

        public AdsController(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public HttpResponseMessage Post(AdModel ad)
        {
            return OK(ad);
        }
      
        // POST api/Ads/?data=filter
        // POST api/Ads/?sort=name
        // POST api/Ads/?dir=desc
        // POST api/Ads/?items=5&pageno=2
        // POST api/Ads/?sort=name&dir=desc&items=5
        [Route("")]
        public HttpResponseMessage PostAds(AdQueryModel data, string sort = "date", string dir = "desc", string items = Util.ITEMNUMBER, string pageno = Util.PAGENUMBER)
        {
            AdsResult result = new AdsResult();
            IEnumerable<AdModel> adModel;
            SearchHelper helper = new SearchHelper();

            int noOfItems = Util.parseQStringNumber(items);
            int pageNumber = Util.parseQStringNumber(pageno, true);
            SortBy sortBy = Util.parseQstringSortBy(sort);
            SortDirection sortDir = Util.parseQstringSortDirection(dir);
            try
            {
                if (data == null)
                {
                    adModel = unitOfWork.AdRepository.Get().Select(x => Mapper.Map<AdModel>(x));
                }
                else
                {
                    adModel = unitOfWork.AdRepository.Get(a => helper.AdSearchCriteria(a, data)).Select(x => Mapper.Map<AdModel>(x));
                }

                if (adModel != null)
                {

                    //picking up total number of records before we do sorting and get results only for one page
                    result.numberOfResults = adModel.Count();
                    adModel = (helper.SortResults(sortBy, sortDir, noOfItems, pageNumber, adModel));
                    result.adModel = AddImagePaths(adModel);
                    result.paidAds = AddImagePaths(GetPaidAds(data, helper));
                    return OK(result);
                }
                else return NotFound("Error occured. Could not query database.");
            }
            catch (ObjectNotFoundException)
            {
                return NotFound("Ad not found.");
            }
            catch (Exception ex)
            {
                return InternalServerError("Error occured. Could not query database.", ex);
            }
        }

        private IEnumerable<AdModel> GetPaidAds(AdQueryModel data, SearchHelper helper)
        {
            IEnumerable<AdModel> adModels = new List<AdModel>();
            try
            {
                if (data != null)
                {
                    //take random two paid ads
                    var ads = unitOfWork.AdRepository.Get(a => helper.PaidAdSearchCriteria(a, data)).OrderBy(arg => Guid.NewGuid()).Take(2);
                    foreach (Ad ad in ads)
                    {
                        ad.ViewCount++;
                        if (ad.ViewCount == Config.PaidAdViewCount)
                        {
                            HandleExpiredPaidAds(ad);
                        }
                        unitOfWork.AdRepository.Update(ad);
                    }
                    unitOfWork.Save();
                    adModels = ads.Select(x => Mapper.Map<AdModel>(x));
                }
                return adModels;
            }
            catch
            {
                return adModels;
            }
        }

        private IEnumerable<AdModel> AddImagePaths(IEnumerable<AdModel> adModel)
        {
            List<AdModel> list = adModel.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                var am = list[i];
                if (am.ImagePath == "")
                {
                    am.ImagePath = "../img/320x150.png";
                }
                else
                {
                    try
                    {
                        
                        string filePath = Config.ImgRoot + am.ImagePath;
                        am.ImagePath = File.ReadAllText(filePath);
                    }
                    catch
                    {
                        am.ImagePath = "../img/320x150.png";
                    }
                }
            }
            return list;
        }

        private void HandleExpiredPaidAds(Ad ad)
        {
            ad.IsPaid = false;
            unitOfWork.AdRepository.Update(ad);

            //TODO
            //Inform user
        }


        private Ad GetAdObject(System.Guid id)
        {
            Ad ad = new Ad();
            try
            {
                ad = unitOfWork.AdRepository.GetByID(id);
                return ad;
            }
            catch
            {
                return null;
            }
        }

        // GET api/Ads/5
        [Route("{id:Guid}")]
        [ResponseType(typeof(AdModel))]
        public HttpResponseMessage GetAd(System.Guid id)
        {
            AdModel ad = Mapper.Map<Ad, AdModel>(GetAdObject(id));
            if(ad != null)
            {
                ad = Mapper.Map<Ad, AdModel>(unitOfWork.AdRepository.GetByID(id));
                if(ad.ImagePath == "")
                {
                    ad.ImagePath = "../img/320x150.png";
                }
                else
                {
                    try
                    {
                        string filePath = Config.ImgRoot + ad.ImagePath;
                        ad.ImagePath = File.ReadAllText(filePath);
                    }
                    catch
                    {
                        ad.ImagePath = "../img/320x150.png";
                    }
                }

                ad.CityName = unitOfWork.CityRepository.GetByID(ad.CityId).CityName;
                ad.CategoryName = unitOfWork.CategoryRepository.GetByID(ad.CategoryId).CatName;
                
                return OK(ad);
            }
            else
            {
                return NotFound("Ad not found.");
            }


        }

        // GET api/Ads/CategoryId/{Guid}
        [Route("CategoryId/{CatId:Guid}")]
        [ResponseType(typeof(AdModel))]
        public HttpResponseMessage GetByCategoryId(System.Guid CatId)
        {
            IEnumerable<AdModel> adModel;
            try
            {
                adModel = unitOfWork.AdRepository.Get(a => a.CategoryId == CatId).Select(x => Mapper.Map<AdModel>(x));
                return OK(adModel);
            }
            catch
            {
                return NotFound("Not Acceptable. There is no Ad with provided Guid.");
            }

        }

        // GET api/Ads/CityID/{Guid}
        [Route("CityId/{cityID:Guid}")]
        [ResponseType(typeof(AdModel))]
        public HttpResponseMessage GetByCityId(System.Guid cityID)
        {
            IEnumerable<AdModel> adModel;
            try
            {
                adModel = unitOfWork.AdRepository.Get(a => a.CityId == cityID).Select(x => Mapper.Map<AdModel>(x));
                return OK(adModel);
            }
            catch
            {
                return NotFound("Not Acceptable. There is no Ad with provided Guid.");
            }

        }

        // GET: Ads by UserId
        [Route("UserId/{userID:Guid}")]
        [ResponseType(typeof(AdModel))]
        public HttpResponseMessage GetByUserId(System.Guid userID)
        {
            IEnumerable<AdModel> adModel;
            try
            {
                adModel = unitOfWork.AdRepository.Get(a => a.UserId == userID).Select(x => Mapper.Map<AdModel>(x));
                

                List<AdModel> tempModels = adModel.ToList();
                foreach (AdModel am in tempModels)
                {
                    if (am.ImagePath == "")
                    {
                        am.ImagePath = "../img/320x150.png";
                    }
                    else
                    {
                        try
                        {
                            string filePath = Config.ImgRoot + am.ImagePath;
                            am.ImagePath = File.ReadAllText(filePath);
                        }
                        catch
                        {
                            am.ImagePath = "../img/320x150.png";
                        }
                    }
                    am.CityName = unitOfWork.CityRepository.GetByID(am.CityId).CityName;
                    am.CategoryName = unitOfWork.CategoryRepository.GetByID(am.CategoryId).CatName;
                    if (am.IsPaid.HasValue && am.IsPaid.Value && am.PaidDate != null && am.ViewCount != null)
                    {
                        am.PaidDaysLeft = Config.PaidAdTimeLinit - (DateTime.Today - am.PaidDate.Value).Days;
                        am.PaidViewsLeft = Config.PaidAdViewCount - am.ViewCount.Value;
                    }

                }
                return OK(tempModels);
            }
            catch
            {
                return NotFound("Not Acceptable. There are no Ads for provided urer Guid.");
            }

        }


        // PUT api/Ads
        //[ValidationResponseFilter]
        [Route("")]
        [Authorize(Roles = "admin,repairmen")]
        [HttpPut]
        public HttpResponseMessage PutAd(InsertAdModel ad)
        {
                DateTime insertDate = DateTime.Now;
                // Getting current authorized user (userId)
                var cp = User as ClaimsPrincipal;
                string user = "";
                user = (from c in cp.Claims
                        where c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata"
                        select c.Value).FirstOrDefault();
                Guid userId = new Guid(user);
                // creating temporary AdModel and fill them with json data from input
                AdModel tempModel = new AdModel()
                {
                    CategoryId = ad.Category.Id,
                    CityId = ad.City.Id,
                    Description = ad.Description,
                    Email = ad.Email,
                    AvgRate = 0,
                    VoteCounter = 0,
                    Location = ad.Location,
                    Id = Guid.NewGuid(),
                    Name = ad.Name,
                    PhoneNumber = ad.PhoneNumber,
                    UserId = userId,
                    Date = insertDate,
                    CommentCounter = 0,
                    InappCounter = 0,
                    longitude = ad.longitude,
                    latitude = ad.latitude,
                    Website = CorrectWebiste(ad.Website)
                };



                var adToInsert = Mapper.Map<Ad>(tempModel);
                adToInsert.ImagePath = "";

                try
                {
                    unitOfWork.AdRepository.Insert(adToInsert);
                    unitOfWork.Save();
                    // Insert Image and update just inserted Ad
                    adToInsert.ImagePath = insertImg(insertDate, ad.Image);
                    unitOfWork.AdRepository.Update(adToInsert);
                    unitOfWork.Save();
                    return Create();
                }
                catch (Exception ex)
                {
                    return InternalServerError("Server Error: Can not insert Ads in database.", ex);
                }
        }

        // Update current Ad (from myPanel)
        [Route("UpdateSingleAd")]
        [Authorize(Roles = "admin,repairmen")]
        [HttpPut]
        public HttpResponseMessage UpdateSingle(AdModel ad)
        {
            Ad existingAd = null;
            existingAd = GetAdObject(ad.Id);

            existingAd.Description = ad.Description;
            existingAd.Email = ad.Email;
            existingAd.Location = ad.Location;
            existingAd.Name = ad.Name;
            existingAd.PhoneNumber = ad.PhoneNumber;
            existingAd.Website = CorrectWebiste(ad.Website);
            existingAd.CategoryId = ad.CategoryId;
            existingAd.CityId = ad.CityId;
            existingAd.longitude = ad.longitude;
            existingAd.latitude = ad.latitude;
            //


            try
            {
                unitOfWork.AdRepository.Update(existingAd);
                existingAd.ImagePath = replaceImgFile(ad.Id, ad.Date, ad.ImagePath);
                unitOfWork.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalServerError("Server Error: Can not update Ad in database.", ex);
            }
        }


        // GET api/Ads/Inappropriate
        [Route("Inappropriate")]
        public HttpResponseMessage GetInappropriateAds()
        {
            IEnumerable<AdModel> adModels;
            try
            {
                adModels = unitOfWork.AdRepository.Get(c => c.InappCounter > Config.AdsCount).Select(x => Mapper.Map<AdModel>(x));
                return OK(adModels);
            }
            catch
            {
                return NotFound("There are no inappropriate ads.");
            }
        }

        // GET api/Ads/Inappropriate
        [Route("Inappropriate/{adId:Guid}")]
        public HttpResponseMessage GetDescriptions(System.Guid adId)
        {
            IEnumerable<InappropriateAdModel> inappAdModels;
            try
            {
                inappAdModels = unitOfWork.InappropriateAdRepository.Get(c => c.AdId == adId).Select(x => Mapper.Map<InappropriateAdModel>(x));
                return OK(inappAdModels);
            }
            catch
            {
                return NotFound("Not Acceptable. There is no Description for provided AdId.");
            }
        }

        // PUT api/Ads/Inappropriate
        [Route("Inappropriate")]
        // [Authorize(Roles = "admin,repairmen")]
        [HttpPut]
        public HttpResponseMessage PutAdInappropriate(InappropriateAdModel inappropriateModel)
        {
            AdModel adModel;
            try
            {
                if (InappropriateAdExists(inappropriateModel) == null)
                {
                    adModel = unitOfWork.AdRepository.Get(a => a.Id == inappropriateModel.AdId).Select(x => Mapper.Map<AdModel>(x)).FirstOrDefault();
                    adModel.InappCounter++;
                    var inappropriate = Mapper.Map<InappropriateAd>(inappropriateModel);
                    unitOfWork.InappropriateAdRepository.Insert(inappropriate);
                    var ad = Mapper.Map<Ad>(adModel);
                    unitOfWork.AdRepository.Update(ad);
                    unitOfWork.Save();
                    return Create(inappropriate);
                }
                else
                {
                    return Forbidden();
                }
            }
            catch (ObjectNotFoundException)
            {
                return NotFound("Not Found. There is no Ad");

            }
            catch (Exception ex)
            {
                return InternalServerError("Server Error: Can not set Inappropriate flag for this Ad.", ex);
            }

        }

        // POST api/Ads/Inappropriate
        // Update Ads (approve or delete) - only admin
        [Route("Inappropriate")]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public HttpResponseMessage PostAd(List<AdModel> adsInput)
        {
            // foreach ad, we must first delete InappropriateAds, then if Ad has comments, than we must delete
            // (if exists) InappropriateComments, and delete comments. And, in the end, we can delete single Ad.
            foreach (AdModel adInput in adsInput)
            {
                var ad = Mapper.Map<Ad>(adInput);
                IEnumerable<InappropriateAdModel> inappModel;
                IEnumerable<CommentModel> commentModel;
                IEnumerable<InappropriateCommentModel> inappCommModel;
                IEnumerable<Rating> ratingModel;
                IEnumerable<CommentVote> votingModel;
                try
                {
                    inappModel = unitOfWork.InappropriateAdRepository.Get(a => a.AdId == ad.Id).Select(x => Mapper.Map<InappropriateAdModel>(x));
                    foreach (InappropriateAdModel inapp in inappModel)
                    {
                        var inappAd = Mapper.Map<InappropriateAd>(inapp);
                        unitOfWork.InappropriateAdRepository.Delete(inappAd);

                    }
                    //do all the changes only if admin flagged the ad to be deleted
                    if (adInput.Delete == true)
                    {
                        try
                        {
                            ratingModel = unitOfWork.RatingRepository.Get(r => r.AdId == ad.Id);
                            foreach (Rating rm in ratingModel)
                            {
                                unitOfWork.RatingRepository.Delete(rm);
                            }
                        }
                        catch
                        {

                        }

                        if (adInput.InappCounter == 0)
                            unitOfWork.AdRepository.Update(ad);
                        else
                        {
                            if (adInput.CommentCounter > 0)
                            {
                                commentModel = unitOfWork.CommentRepository.Get(c => c.AdId == adInput.Id).Select(x => Mapper.Map<CommentModel>(x));
                                foreach (CommentModel comModel in commentModel)
                                {
                                    var comment = Mapper.Map<Comment>(comModel);

                                    try
                                    {
                                        inappCommModel = unitOfWork.InappropriateCommentRepository.Get(i => i.CommentId == comModel.Id).Select(x => Mapper.Map<InappropriateCommentModel>(x));

                                        foreach (InappropriateCommentModel inapp in inappCommModel)
                                        {                                            var inappComm = Mapper.Map<InappropriateComment>(inapp);

                                            unitOfWork.InappropriateCommentRepository.Delete(inappComm);

                                        }
                                    }
                                    catch
                                        {

                                        }
                                    // deleting comment's vote
                                    try
                                    {
                                        votingModel = unitOfWork.CommentVoteRepository.Get(i => i.CommentId == comment.Id);
                                        foreach (CommentVote cmv in votingModel)
                                        {
                                            unitOfWork.CommentVoteRepository.Delete(cmv);
                                        }
                                    }
                                    catch
                                    {

                                    }
                                    unitOfWork.CommentRepository.Delete(comment);
                                }
                            }
                            unitOfWork.AdRepository.Delete(ad);
                            //delete image of add
                            ImageHelper img = new ImageHelper(adInput.Id, adInput.Date);
                            img.DeleteImage();
                        }
                    }
                    //if admin approved the ad, despite inappropriate flags, just set counter for inappropate to 0
                    else if (adInput.Approved == true)
                    {
                        ad.InappCounter = 0;
                        unitOfWork.AdRepository.Update(ad);
                    }

                    unitOfWork.Save();
                }
                catch (ObjectNotFoundException)
                {
                    return NotFound("Not Found. There is no Inappropriate ad.");

                }
                catch (Exception ex)
                {
                    return InternalServerError("Server Error: Can not update Ads in database.", ex);
                }
            }
            return NoContent();
        }

        //POST api/Ads/Delete
        [Route("Delete")]
        [Authorize(Roles="admin, repairmen")]
        [HttpPost]
        public HttpResponseMessage DeleteAd(AdModel adInput)
        {
            var ad = Mapper.Map<Ad>(adInput);
                IEnumerable<CommentModel> commentModel;
                IEnumerable<InappropriateCommentModel> inappCommModel;
                IEnumerable<Rating> ratingModel;
                IEnumerable<CommentVote> votingModel;
                try
                {
                        try
                        {
                            ratingModel = unitOfWork.RatingRepository.Get(r => r.AdId == ad.Id);
                            foreach (Rating rm in ratingModel)
                            {
                                unitOfWork.RatingRepository.Delete(rm);
                            }
                        }
                        catch
                        {

                        }
                            if (adInput.CommentCounter > 0)
                            {
                                commentModel = unitOfWork.CommentRepository.Get(c => c.AdId == adInput.Id).Select(x => Mapper.Map<CommentModel>(x));
                                foreach (CommentModel comModel in commentModel)
                                {
                                    var comment = Mapper.Map<Comment>(comModel);

                                    try
                                    {
                                        inappCommModel = unitOfWork.InappropriateCommentRepository.Get(i => i.CommentId == comModel.Id).Select(x => Mapper.Map<InappropriateCommentModel>(x));

                                        foreach (InappropriateCommentModel inapp in inappCommModel)
                                        {
                                            var inappComm = Mapper.Map<InappropriateComment>(inapp);

                                            unitOfWork.InappropriateCommentRepository.Delete(inappComm);

                                        }
                                    }
                                    catch
                                    {

                                    }
                                    // deleting comment's vote
                                    try
                                    {
                                        votingModel = unitOfWork.CommentVoteRepository.Get(i => i.CommentId == comment.Id);
                                        foreach (CommentVote cmv in votingModel)
                                        {
                                            unitOfWork.CommentVoteRepository.Delete(cmv);
                                        }
                                    }
                                    catch
                                    {

                                    }
                                    unitOfWork.CommentRepository.Delete(comment);
                                }
                            }
                            unitOfWork.AdRepository.Delete(ad);
                            //delete image of add
                            ImageHelper img = new ImageHelper(adInput.Id, adInput.Date);
                            img.DeleteImage();
                        unitOfWork.Save();
                        return NoContent(); 
                }
                catch (ObjectNotFoundException)
                {
                    return NotFound("Not Found. There is no Inappropriate ad.");

                }
                catch (Exception ex)
                {
                    return InternalServerError("Server Error: Can not update Ads in database.", ex);
                }
        }

        // helper functions
        private InappropriateAdModel InappropriateAdExists(InappropriateAdModel inappModel)
        {
            try
            {
                return unitOfWork.InappropriateAdRepository.Get(r => r.UserId == inappModel.UserId && r.AdId == inappModel.AdId).Select(x => Mapper.Map<InappropriateAdModel>(x)).First();

            }
            catch
            {
                return null;
            }
        }

        // Help function to insert image and update Ad
        public string insertImg(DateTime dt, string imgBytes)
        {
            if (imgBytes != "")
            {
                AdModel am = unitOfWork.AdRepository.Get(a => a.Date.ToString() == dt.ToString()).Select(x => Mapper.Map<AdModel>(x)).FirstOrDefault();
                ImageHelper img = new ImageHelper(am.Id, dt);
                string imgFile = img.SaveImage(imgBytes);
                return imgFile;
            }
            else
            {
                return "";
            }
        }

        // Helper function to update image file (if exist, change file, if doesn't, than create file).
        public string replaceImgFile(Guid adID, DateTime dt, string imgBytes)
        {
            if (imgBytes != "")
            {
                ImageHelper img = new ImageHelper(adID, dt);
                string imgFile = img.SaveImage(imgBytes);
                return imgFile;
            }
            else
            {
                return "";
            }
        }

        //Add http in front of website if user didn't type it

        private string CorrectWebiste(string website) 
        {
            if (website != "")
            {
                string pattern = @"http(?:s)?://";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(website);
                if (!match.Success)
                {
                    website = "http://" + website;
                }
                return website;
            }
            else
            {
                return "";
            }

        }
    }


}
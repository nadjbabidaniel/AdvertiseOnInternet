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
using Repairmen.Helpers;
using Newtonsoft.Json;


namespace Repairmen.Controllers
{
    [EnableCors("http://htrepairmen.cloudapp.net,https://www.sandbox.paypal.com,https://reports.sandbox.paypal.com,https://mobileclient.sandbox.paypal.com,https://ipn.sandbox.paypal.com,https://developer.paypal.com,https://business.sandbox.paypal.com,https://batch.sandbox.paypal.com", "*", "*")] // With Enable cross-origin resource sharing, we allow this controller be called from different domain.
    [RoutePrefix("api/PayPal")]
    public class PayPalController : RepairmenApiControllerBase
    {
        private IUnitOfWork unitOfWork;

        public PayPalController()
        {
            unitOfWork = new UnitOfWork();
        }

        public PayPalController(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        [Route("ProcessIPN")]
        [HttpPost]
        public HttpResponseMessage ProcessIPN(PayPalModel data)
        {
            //logger.LogError(JsonConvert.SerializeObject(data));
            if (data.payment_status == "Completed")
            {
                try
                {
                    Guid id = Guid.Parse(data.custom);
                    Ad ad = unitOfWork.AdRepository.GetByID(id);
                    ad.IsPaid = true;
                    ad.PaidDate = DateTime.Today;
                    ad.ViewCount = 0;
                    unitOfWork.AdRepository.Update(ad);
                    unitOfWork.Save();
                }
                catch (Exception ex)
                {
                    InternalServerError(ex);
                }
            }
            else
            { 
                //TODO
            }
            return NoContent();
        }

        [Route("IsAvailable")]
        [HttpGet]
        public HttpResponseMessage IsAvailable(Guid adId, bool yesNo)
        {
            CustomConfig config = new CustomConfig();
            Ad ad = new Ad();
            try
            {
                ad = unitOfWork.AdRepository.GetByID(adId);

                if ((!ad.IsPaid.HasValue || !ad.IsPaid.Value) && GetPaidAdsByCategoryAndCity(ad.Category.Id, ad.City.Id).Count() < config.MaximumNumberOfPaidAds)
                {
                    return NoContent();
                }
                else
                {
                    return Forbidden();
                }
            }
            catch (Exception ex)
            {
                //logger.LogError(ex);
                return NotFound("Ad is not found.");
            }
        }


        private List<Ad> GetPaidAdsByCategoryAndCity(Guid categoryId, Guid cityId)
        {
            List<Ad> result = new List<Ad>();
            try
            {
                result = unitOfWork.AdRepository.Get(x => x.IsPaid.HasValue && x.IsPaid.Value && x.City.Id == cityId && x.CategoryId == categoryId).ToList();
            }
            catch {}
            return result;
        }

    }

}
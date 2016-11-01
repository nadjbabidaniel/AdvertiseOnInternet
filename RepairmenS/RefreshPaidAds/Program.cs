using DAL;
using Repairmen.Controllers.Helpers;
using Repairmen.Helpers;
using RepairmenModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshPaidAds
{
    class Program
    {
        public static void Main(string[] args)
        {
            var unitOfWork = new UnitOfWork();
            var config = new CustomConfig();
            try
            {
                var paidAds = unitOfWork.AdRepository.Get(x => x.IsPaid.HasValue && x.IsPaid.Value && x.PaidDate.Value.AddDays(config.PaidAdTimeLinit) == DateTime.Today);

                string from = "noreply@repairmen.com";
                string subject = "Repairmen ad promotion will expire";
                string body =
    @"Hello {0}
Promotion for ad {1} on www.repairmen.com will expire today.";
                foreach (Ad ad in paidAds)
                {
                    var mailHelper = new MailHelper(MailHelper.MailType.Notification, from, ad.User.Email, subject, String.Format(body, " " + ad.User.FirstName, ad.Name));
                    mailHelper.SendEmail();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                var expiredPaidAds = unitOfWork.AdRepository.Get(x => x.IsPaid.HasValue && x.IsPaid.Value && x.PaidDate.Value.AddDays(config.PaidAdTimeLinit + 1) <= DateTime.Today);
                foreach (Ad ad in expiredPaidAds)
                {
                    ad.PaidDate = null;
                    ad.IsPaid = false;
                    ad.ViewCount = 0;
                    unitOfWork.AdRepository.Update(ad);
                }
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

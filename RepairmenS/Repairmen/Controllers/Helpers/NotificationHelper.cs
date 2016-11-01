using Repairmen.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using DAL;
using RepairmenModel;

namespace Repairmen.Controllers.Helpers
{
    public class NotificationHelper
    {
        private string _adId { get; set; }
        private string _userName { get; set; }
        private string _userEmail { get; set; }
        private string _adName { get; set; }
        private bool? _toMail { get; set; }
        private bool? _toSMS { get; set; }
        private string _phoneNumber { get; set; }
        private string _link { get; set; }
        
        public NotificationHelper(Guid adID, string adName, Guid userID)
        {
            IUnitOfWork uow = new UnitOfWork();
            User user = uow.UserRepository.GetByID(userID);
            _adId = adID.ToString();
            _adName = adName;
            _userName = user.Username;
            _userEmail = user.Email;
            _toMail = user.NotifyEmail;
            _toSMS = user.NotifySMS;
            _phoneNumber = user.PhoneNumber;

            //Link to single ad
            CustomConfig Config = ConfigurationManager.GetSection("customSection") as CustomConfig;
            string root = Config.ClientRoot;
            _link = root + "ads/" + _adId;
        }

        public void Send ()
        {
            if(_toMail == true)
            {
                SendMail();
            }
            if(_toSMS == true)
            {
                SendSMS();
            }
        }

        private void SendMail()
        {
            

            string sender = "NoReply@repairmen.com";
            string receiver = _userEmail;
            string subject = "Repairmen! You have a new comment.";
            string body = "Hello " + _userName + ".\n" +
                "You have a new comment about your ad: " + _adName + ".\n" +
                "To see your Ad and comment, go to link:\n" + _link + "\n\n" +
                "Kind Regards.\nRepairmenRepairmen website - at your service!\n\n www.repairmen.com";

            MailHelper mh = new MailHelper(MailHelper.MailType.Notification, sender, receiver, subject, body);
            try
            {
                mh.SendEmail();
            }
            catch
            { }
        }

        private void SendSMS()
        {
            SmsHelper sms = new SmsHelper(_phoneNumber);
            try
            {
                string smsMsg = sms.sendSms("Repairmen: new comment for your Ad: "+_adName+".");
            }
            catch { }
        }
    }
}
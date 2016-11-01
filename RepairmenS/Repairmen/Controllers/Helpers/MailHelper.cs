using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Repairmen.Controllers.Helpers
{
    public class MailHelper
    {
        public enum MailType { Registration, Notification };

        private MailMessage message;

        public MailHelper(MailType mailType, string from, string to, string subject, string body)
        {
            MailAddress sender = new MailAddress(from);
            MailAddress receiver = new MailAddress(to);
            message = new MailMessage(sender, receiver);
            message.Subject = subject;
            message.Body = body;
        }

        public bool SendEmail()
        {
            bool success = false;
            SmtpClient client = new SmtpClient();
            try
            {
                client.Send(message);
                success = true;
            }
            catch
            {
                success = false;
            }

            return success;
        }


    }
}
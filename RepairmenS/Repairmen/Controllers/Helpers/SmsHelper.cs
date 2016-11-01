using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Twilio;

namespace Repairmen.Controllers.Helpers
{
    public class SmsHelper
    {
        private string sid { get; set; }

        private string aToken { get; set; }

        private string _to { get; set; }

        public SmsHelper(string to)
        {
            _to = to;
            sid = "AC06ce497bc8f19f1c9dc1d6c9e95b4d17";
            aToken = "ae634e8a5fcc71c76694ca2778dd97e1";
        }

        public string sendSms(string smsText)
        {
            var twilio = new TwilioRestClient(sid, aToken);
            var msg = twilio.SendSmsMessage("+14247048980", _to, smsText);
            if (msg.RestException != null)
            {
                return msg.RestException.Message;
                // handle the error ...
            }
            else
            {
                return msg.Status;
            }
        }

    }
}
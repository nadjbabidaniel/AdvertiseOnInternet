using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using RepairmenModel;

namespace Repairmen.Models
{
    public class PayPalModel
    {
        public string mc_gross { get; set; }
        public string invoice { get; set; }
        public string settle_amount { get; set; }
        public string protection_eligibility { get; set; }
        public string address_status { get; set; }
        public string payer_id { get; set; }
        public string tax { get; set; }
        public string address_street { get; set; }
        public string payment_date { get; set; }
        public string payment_status { get; set; }
        public string charset { get; set; }
        public string address_zip { get; set; }
        public string mc_shipping { get; set; }
        public string mc_handling { get; set; }
        public string first_name { get; set; }
        public string mc_fee { get; set; }
        public string address_country_code { get; set; }
        public string exchange_rate { get; set; }
        public string address_name { get; set; }
        public string notify_version { get; set; }
        public string settle_currency { get; set; }
        public string custom { get; set; }
        public string payer_status { get; set; }
        public string business { get; set; }
        public string address_country { get; set; }
        public string address_city { get; set; }
        public string verify_sign { get; set; }
        public string payer_email { get; set; }
        public string txn_id { get; set; }
        public string payment_type { get; set; }
    }
}
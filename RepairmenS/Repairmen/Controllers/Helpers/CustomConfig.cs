using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Configuration;
using System.Xml;

namespace Repairmen.Helpers
{
    public class CustomConfig : ConfigurationSection
    {

        [ConfigurationProperty("ImgRoot", DefaultValue = "", IsRequired = false)]
        public string ImgRoot
        {
            get
            {
                return (string)this["ImgRoot"];
            }
            set
            {
                this["ImgRoot"] = value;
            }
        }


        [ConfigurationProperty("CommentsCount", IsRequired = false)]
        public int CommentsCount
        {
            get
            {
                return (int)this["CommentsCount"];
            }
            set
            {
                this["CommentsCount"] = value;
            }
        }


        [ConfigurationProperty("AdsCount", IsRequired = false)]
        public int AdsCount
        {
            get
            {
                return (int)this["AdsCount"];
            }
            set
            {
                this["AdsCount"] = value;
            }
        }

        [ConfigurationProperty("ClientRoot", DefaultValue = "", IsRequired = false)]
        public string ClientRoot
        {
            get
            {
                return (string)this["ClientRoot"];
            }
            set
            {
                this["ClientRoot"] = value;
            }
        }

        [ConfigurationProperty("PaidAdViewCount", DefaultValue = "10", IsRequired = false)]
        public int PaidAdViewCount
        {
            get
            {
                return (int)this["PaidAdViewCount"];
            }
        }

        [ConfigurationProperty("PaidAdTimeLinit", DefaultValue = "10", IsRequired = false)]
        public int PaidAdTimeLinit
        {
            get
            {
                return (int)this["PaidAdTimeLinit"];
            }
        }

        [ConfigurationProperty("MaximumNumberOfPaidAds", DefaultValue = "10", IsRequired = false)]
        public int MaximumNumberOfPaidAds
        {
            get
            {
                return (int)this["MaximumNumberOfPaidAds"];
            }
        }

    }
}
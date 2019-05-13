using Panacea.Models;
using Panacea.Multilinguality;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Panacea.Modules.Advertisements.Models
{

    [DataContract]
    public class AdResponse
    {
        [DataMember(Name = "Advertisements")]
        public AdvertisementCategories Advertisements { get; set; }
    }


    [DataContract]
    public class AdvertisementCategories
    {
        [DataMember(Name = "adCategories")]
        public List<AdvertisementCategory> AdCategories { get; set; }
    }
    public class AdvertisementCategory : ServerItem
    {
        [IsTranslatable]
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "banners")]
        public List<BannerItem> Banners { get; set; }
    }
}
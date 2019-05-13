using Panacea.Models;
using Panacea.Multilinguality;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Panacea.Modules.Advertisements.Models
{
    [DataContract]
    public class AdvertisementEntry : ServerItem
    {

        [IsTranslatable]
        [DataMember(Name = "description")]
        public string Description { get; set; }



        [DataMember(Name = "adType")]
        public AdvertisementType AdType { get; set; }

        [DataMember(Name = "bannerType")]
        public AdType Type { get; set; }

        [DataMember(Name = "placement")]
        public String Placement { get; set; }

        [DataMember(Name = "dataUrl")]
        public String DataUrl { get; set; }

        [DataMember(Name = "img")]
        public String Img { get; set; }

        [DataMember(Name = "start_date")]
        public DateTime StartDate { get; set; }

        [DataMember(Name = "expiration_date")]
        public DateTime ExpirationDate { get; set; }

        [DataMember(Name = "plugins")]
        public List<string> Plugins { get; set; }

        [DataMember(Name = "displayPlan")]
        public List<DisplayPlan> DisplayPlans { get; set; }

        [DataMember(Name = "duration_on_screen")]
        public int DurationOnScreen { get; set; }
    }

    [DataContract]
    public class DisplayPlan
    {
        [DataMember(Name = "day")]
        public string Day { get; set; }

        [DataMember(Name = "minutes")]
        public string Minute { get; set; }

        [DataMember(Name = "hour")]
        public string Hour { get; set; }

    }

    public enum AdType
    {
        Rotating,
        Notification,
        Splash
    };

    public enum AdvertisementType
    {
        Webpage,
        PhotoGallery,
        Video
    };


    [DataContract]
    public class BannerItem
    {
        [DataMember(Name = "banner")]
        public AdvertisementEntry Banner { get; set; }

        [DataMember(Name = "priority")]
        public int Priority { get; set; }
    }


}

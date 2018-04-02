using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.Models
{
    public partial class SocialMedia
    {
        public string Id { get; set; }
        public string AzureId { get; set; }
        public string Type { get; private set; }
        public string Link { get; private set; }
        public string Username { get; set; }

        public SocialMedia(string username)
        {
            Username = username;
        }

        public void asTwitter()
        {
            Type = "Twitter";
            Link = $"http://www.twitter.com/{Username}";
        }

        public void asFacebook()
        {
            Type = "Facebook";
            Link = $"http://www.facebook.com/{Username}";
        }

        public void asInstagram()
        {
            Type = "Instagram";
            Link = $"http://www.instagram.com/{Username}";
        }

        public void asDribbble()
        {
            Type = "Dribbble";
            Link = $"http://www.dribbble.com/{Username}";
        }

        public void setType(string type, string username)
        {
            type = type.ToLower();
            Username = username;

            switch (type)
            {
                case "twitter":
                    asTwitter();
                    break;
                case "facebook":
                    asFacebook();
                    break;
                case "instagram":
                    asInstagram();
                    break;
                case "dribbl":
                    asDribbble();
                    break;
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.Models
{
    public partial class SocialMedia
    {
        public string Type { get; private set; }
        public string BaseLink { get; private set; }
        public string Username { get; set; }

        public SocialMedia(string username)
        {
            Username = username
        }

        public void asTwitter()
        {
            Type = "Twitter";
            BaseLink = "http://www.twitter.com";
        }

        public void asFacebook()
        {
            Type = "Facebook";
            BaseLink = "http://www.facebook.com";
        }

        public void asInstagram()
        {
            Type = "Instagram";
            BaseLink = "http://www.instagram.com";
        }

        public void asDribbl()
        {
            Type = "Dribbl";
            BaseLink = "http://www.dribbl.com";
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
                    asDribbl();
                    break;
            }
        }
    }
}

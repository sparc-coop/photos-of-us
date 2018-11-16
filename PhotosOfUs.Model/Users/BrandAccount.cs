using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PhotosOfUs.Model.Models
{
    public partial class BrandAccount
    {
        public int BrandAccountId { get; set; }
        public int UserId { get; set; }

        public int SeparatorThickness { get; set; }
        public int SeparatorWidth { get; set; }
        public int CodeInputStyle { get; set; }
        public int BrandingStyle { get; set; }

        public string HomepageTemplate { get; set; }
        public string PersonalLogoUrl { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string OverlayColorCode { get; set; }
        public string OverlayOpacity { get; set; }
        public string AccentColorCode { get; set; }
        public string BackgroundColorCode { get; set; }
        public string HeaderColorCode { get; set; }
        public string BodyColorCode { get; set; }
        public string PageTitle { get; set; }
        public string Description { get; set; }
        public string SeparatorStyle { get; set; }

        public User User { get; set; }

        public enum HomepageTemplates
        {
            TwoOneSplit,
            OneTwoSplit,
            FullBackground
        }

        public enum SeparatorStyles
        {
            StraightLine,
            DottedLine
        }

        public enum FeatureTypes
        {
            Dark,
            Light,
            None
        }

        public BrandAccount CreateDefaultBrandAccount(int id)
        {
            UserId = id;
            BrandAccountId = id;
            HomepageTemplate = BrandAccount.HomepageTemplates.TwoOneSplit.ToString();
            PersonalLogoUrl = "";
            BackgroundColorCode = "F6FFFF";
            AccentColorCode = "FF6060";
            HeaderColorCode = "000000";
            BodyColorCode = "000000";
            FeaturedImageUrl = "";
            OverlayColorCode = "194952";
            OverlayOpacity = "50";
            PageTitle = "My Page Title";
            Description = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores.";
            SeparatorStyle = BrandAccount.SeparatorStyles.StraightLine.ToString();
            SeparatorThickness = 1;
            SeparatorWidth = 40;
            BrandingStyle = (int)BrandAccount.FeatureTypes.Dark;

            return this;
        }
    }
}

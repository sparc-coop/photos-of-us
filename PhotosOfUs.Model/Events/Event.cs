using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PhotosOfUs.Model.Models
{
    public partial class Event
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string PageTitle { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string HomepageTemplate { get; set; }
        public string PersonalLogoUrl { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string OverlayColorCode { get; set; }
        public decimal OverlayOpacity { get; set; }
        public string AccentColorCode { get; set; }
        public string BackgroundColorCode { get; set; }
        public string HeaderColorCode { get; set; }
        public string BodyColorCode { get; set; }
        public string SeparatorStyle { get; set; }
        public int SeparatorThickness { get; set; }
        public int SeparatorWidth { get; set; }
        public int BrandingStyle { get; set; }

        public ICollection<Card> Cards { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public User User { get; set; }

        public void AddNewCards(int quantity)
        {
            for (var i = 0; i < quantity; i++)
                Cards.Add(new Card(this));
        }


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

        public Event CreateDefaultBrandAccount(int id)
        {
            UserId = id;
            EventId = id;
            HomepageTemplate = Event.HomepageTemplates.TwoOneSplit.ToString();
            PersonalLogoUrl = "";
            BackgroundColorCode = "F6FFFF";
            AccentColorCode = "FF6060";
            HeaderColorCode = "000000";
            BodyColorCode = "000000";
            FeaturedImageUrl = "";
            OverlayColorCode = "194952";
            OverlayOpacity = 0.5M;
            PageTitle = "My Page Title";
            Description = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores.";
            SeparatorStyle = Event.SeparatorStyles.StraightLine.ToString();
            SeparatorThickness = 1;
            SeparatorWidth = 40;
            BrandingStyle = (int)Event.FeatureTypes.Dark;

            return this;
        }

        public void DeletePhotos(List<int> photoIds)
        {
            var photos = Photos.Where(x => photoIds.Contains(x.Id)).ToList();
            foreach (var photo in photos)
                Photos.Remove(photo);
        }
    }
}

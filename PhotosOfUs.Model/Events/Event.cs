using PhotosOfUs.Core.Photos;
using PhotosOfUs.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PhotosOfUs.Core.Events
{
    public partial class Event
    {
        private Event()
        {

        }
        public Event(int eventId, int userId, string name, string url, string description, string homepageTemplate, string personalLogoUrl, string featuredImageUrl, string overlayColorCode,
            decimal? overlayOpacity, string accentColorCode, string backgroundColorCode, string headerColorCode, string bodyColorCode, string separatorStyle, int separatorThickness,
            int separatorWidth, int brandingStyle)
        {
            EventId = eventId;
            UserId = userId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            CreatedDateUtc = DateTime.UtcNow;
            HomepageTemplate = homepageTemplate ?? throw new ArgumentNullException(nameof(homepageTemplate));
            PersonalLogoUrl = personalLogoUrl ?? throw new ArgumentNullException(nameof(personalLogoUrl));
            FeaturedImageUrl = featuredImageUrl ?? throw new ArgumentNullException(nameof(featuredImageUrl));
            OverlayColorCode = overlayColorCode ?? throw new ArgumentNullException(nameof(overlayColorCode));
            OverlayOpacity = overlayOpacity;
            AccentColorCode = accentColorCode ?? throw new ArgumentNullException(nameof(accentColorCode));
            BackgroundColorCode = backgroundColorCode ?? throw new ArgumentNullException(nameof(backgroundColorCode));
            HeaderColorCode = headerColorCode ?? throw new ArgumentNullException(nameof(headerColorCode));
            BodyColorCode = bodyColorCode ?? throw new ArgumentNullException(nameof(bodyColorCode));
            SeparatorStyle = separatorStyle ?? throw new ArgumentNullException(nameof(separatorStyle));
            SeparatorThickness = separatorThickness;
            SeparatorWidth = separatorWidth;
            BrandingStyle = brandingStyle;

            //Cards = cards ?? throw new ArgumentNullException(nameof(cards));
            //Photos = photos ?? throw new ArgumentNullException(nameof(photos));
            //User = user ?? throw new ArgumentNullException(nameof(user));
        }

        public int EventId { get; protected set; }
        public int UserId { get; protected set; }
        public string Name { get; protected set; }
        public string Url { get; protected set; }
        public string Description { get; protected set; }
        public DateTime CreatedDateUtc { get; protected set; }
        public string HomepageTemplate { get; protected set; }
        public string PersonalLogoUrl { get; protected set; }
        public string FeaturedImageUrl { get; protected set; }
        public string OverlayColorCode { get; protected set; }
        public decimal? OverlayOpacity { get; protected set; }
        public string AccentColorCode { get; protected set; }
        public string BackgroundColorCode { get; protected set; }
        public string HeaderColorCode { get; protected set; }
        public string BodyColorCode { get; protected set; }
        public string SeparatorStyle { get; protected set; }
        public int SeparatorThickness { get; protected set; }
        public int SeparatorWidth { get; protected set; }
        public int BrandingStyle { get; protected set; }

        private readonly HashSet<Card> _cards;
        public IReadOnlyCollection<Card> Cards => _cards;
        private readonly HashSet<Photo> _photos;
        public IReadOnlyCollection<Photo> Photos => _photos;
        public User User { get; protected set; }

        public void AddNewCards(int quantity)
        {
            for (var i = 0; i < quantity; i++)
                _cards.Add(new Card(this));
        }

        public void AddPhoto(Photo photo)
        {
            _photos.Add(photo);
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

        public void DeletePhotos(List<int> photoIds)
        {
            var photos = Photos.Where(x => photoIds.Contains(x.Id)).ToList();
            foreach (var photo in photos)
                _photos.Remove(photo);
        }

        public void BulkEdit(List<int> photoIds, List<string> tags, decimal? newPrice)
        {
            var photos = Photos.Where(x => photoIds.Contains(x.Id)).ToList();

            if (newPrice != null)
            {
                photos.ForEach(x => x.UpdatePrice(newPrice.Value));
            }

            if (tags?.Any() ?? false)
            {
                photos.ForEach(x => x.ReplaceTags(tags));
            }

            
        }
    }
}

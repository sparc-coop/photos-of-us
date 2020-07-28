using Kuvio.Kernel.Core;
using PhotosOfUs.Core.Photos;
using PhotosOfUs.Core.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PhotosOfUs.Core.Events.Commands
{
    public class CreateEventModel
    {
        public CreateEventModel()
        {
            Cards = new List<Card>();
            Photos = new List<Photo>();
        }

        public int EventId { get;  set; }
        public int UserId { get;  set; }
        public string Name { get;  set; }
        public string Url { get;  set; }
        public string Description { get;  set; }
        public DateTime CreatedDateUtc { get;  set; }
        public string HomepageTemplate { get;  set; }
        public string PersonalLogoUrl { get;  set; }
        public string FeaturedImageUrl { get;  set; }
        public string OverlayColorCode { get;  set; }
        public decimal? OverlayOpacity { get;  set; }
        public string AccentColorCode { get;  set; }
        public string BackgroundColorCode { get;  set; }
        public string HeaderColorCode { get;  set; }
        public string BodyColorCode { get;  set; }
        public string SeparatorStyle { get;  set; }
        public int SeparatorThickness { get;  set; }
        public int SeparatorWidth { get;  set; }
        public int BrandingStyle { get;  set; }

        public List<Card> Cards { get;  set; }
        public List<Photo> Photos { get;  set; }
        public User User { get;  set; }
    }

    public class CreateEventCommand
    {
        private readonly IDbRepository<User> _users;
        private readonly IMediaRepository<ProfilePhoto> _profilePhotos;
        private readonly IMediaRepository<ProfileThumbnail> _profileThumbnails;

        public async Task<string> ExecuteAsync(CreateEventModel model)
        {
            return null;
        }
    }
}

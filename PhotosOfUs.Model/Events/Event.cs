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

        public Event(int photographerId, string name, string url, string description, EventStyle eventStyle)
        {
            PhotographerId = photographerId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Description = description;
            Style = eventStyle ?? throw new ArgumentNullException(nameof(eventStyle));
            CreatedDateUtc = DateTime.UtcNow;
        }

        public int Id { get; protected set; }
        public int PhotographerId { get; protected set; }

        public string Name { get; protected set; }
        public string Url { get; protected set; }
        public string Description { get; protected set; }
        public DateTime CreatedDateUtc { get; protected set; }
        public EventStyle Style { get; protected set; }


        private readonly HashSet<Card> _cards;
        public IReadOnlyCollection<Card> Cards => _cards;
        private readonly HashSet<Photo> _photos;


        public IReadOnlyCollection<Photo> Photos => _photos;
        public User Photographer { get; protected set; }

        public void AddNewCards(int quantity)
        {
            for (var i = 0; i < quantity; i++)
                _cards.Add(new Card(this));
        }

        public void AddPhoto(Photo photo)
        {
            _photos.Add(photo);
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

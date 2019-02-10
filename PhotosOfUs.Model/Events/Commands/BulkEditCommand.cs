using System.Collections.Generic;
using System.Linq;
using Kuvio.Kernel.Core;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs.Model.Events
{
    public class BulkEditCommand : Command<Event>
    {
        public BulkEditCommand(IRepository<Event> repository) : base(repository)
        {
        }

        public void Execute(int eventId, List<int> photoIds, List<string> tags, decimal? newPrice)
        {
            var photos = Set.Include(x => x.Photos.SelectMany(y => y.PhotoTag))
                .Find(x => x.EventId == eventId)
                .Photos
                .Where(x => photoIds.Contains(x.Id))
                .ToList();

            // Price
            if (newPrice != null)
            {
                photos.ForEach(x => x.UpdatePrice(newPrice.Value));
                Commit();
            }

            // Tags
            if (tags == null || !tags.Any()) return;

            photos.ForEach(x => x.ReplaceTags(tags));
            Commit();
        }
    }
}
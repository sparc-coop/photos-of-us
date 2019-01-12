using Kuvio.Kernel.Architecture;
using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotosOfUs.Model.Photos.Commands
{
    public class UploadPhotoCommand : Command<Event>
    {
        private readonly IMediaRepository<Photo> _photoFiles;
        private readonly IRepository<Event> _events;
        private readonly IRepository<Photo> _photos;
        private readonly ICognitiveContext _cognitive;

        public UploadPhotoCommand(IRepository<Event> events, IRepository<Photo> photos, IMediaRepository<Photo> photoFiles, ICognitiveContext cognitiveContext) : base(events)
        {
            _photoFiles = photoFiles;
            _events = events;
            _photos = photos;
            _cognitive = cognitiveContext;
        }

        public async Task<UploadPhotoCommandResult> ExecuteAsync(int eventId, int userId, string filename, Stream stream, string photoCode)
        {
            var ev = _events.Include(x => x.Cards).Find(x => x.EventId == eventId && x.UserId == userId);

            var bytes = TransformImageIntoBytes(stream);
            var suggestedTags = await _cognitive.GetSuggestedTags(bytes);

            var possibleCodes = ev.Cards.Select(x => x.Code).ToList();
            var code = await _cognitive.GetPhotoCode(bytes, possibleCodes) ?? photoCode;
            
            var photo = new Photo(userId, filename, eventId, ev.Cards.First(x => x.Code == code)?.Id);
            photo.Url = (await _photoFiles.UploadAsync(photo)).AbsoluteUri;
            ev.AddPhoto(photo);
            Commit();

            return new UploadPhotoCommandResult
            {
                SuggestedTags = suggestedTags,
                Code = code,
                IsValid = true
            };
        }

        // For public photos
        public async Task<UploadPhotoCommandResult> ExecuteAsync(int userId, string filename, Stream stream)
        {
            var photo = new Photo(userId, filename, stream);
            photo.Url = (await _photoFiles.UploadAsync(photo)).AbsoluteUri;

            var bytes = TransformImageIntoBytes(stream);
            var suggestedTags = await _cognitive.GetSuggestedTags(bytes);
            
            _photos.Add(photo);
            Commit();

            return new UploadPhotoCommandResult
            {
                SuggestedTags = suggestedTags,
                IsValid = true,
                Url = photo.Url
            };
        }

        private static byte[] TransformImageIntoBytes(Stream stream)
        {
            byte[] fileBytes;
            stream.Position = 0;

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                fileBytes = ms.ToArray();
                //string s = Convert.ToBase64String(fileBytes);
                // act on the Base64 data
            }
            return fileBytes;
        }

        public class UploadPhotoCommandResult
        {
            public List<string> SuggestedTags { get; set; }
            public string Code { get; set; }
            public string Url { get; set; }
            public bool IsValid { get; set; }
        }
    }
}

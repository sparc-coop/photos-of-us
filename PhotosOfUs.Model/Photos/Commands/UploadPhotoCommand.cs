using Kuvio.Kernel.Core;
using PhotosOfUs.Core.Events;
using PhotosOfUs.Core.Photos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotosOfUs.Model.Photos.Commands
{
    public class UploadPhotoCommand
    {
        private readonly IMediaRepository<Photo> _photoFiles;
        private readonly IDbRepository<Event> _events;
        private readonly IDbRepository<Photo> _photos;
        private readonly ICognitiveContext _cognitive;

        public UploadPhotoCommand(IDbRepository<Event> events, IDbRepository<Photo> photos, IMediaRepository<Photo> photoFiles, ICognitiveContext cognitiveContext)
        {
            _photoFiles = photoFiles;
            _events = events;
            _photos = photos;
            _cognitive = cognitiveContext;
        }

        public async Task<UploadPhotoCommandResult> ExecuteAsync(int eventId, int userId, string filename, Stream stream, string photoCode)
        {
            List<string> suggestedTags = new List<string>();
            string code = null;

            await _events.ExecuteAsync(eventId, async ev =>
            {
                var bytes = TransformImageIntoBytes(stream);
                suggestedTags = await _cognitive.GetSuggestedTags(bytes);

                var possibleCodes = ev.Cards.Select(x => x.Code).ToList();
                code = await _cognitive.GetPhotoCode(bytes, possibleCodes) ?? photoCode;

                var photo = new Photo(userId, filename, eventId, ev.Cards.First(x => x.Code == code)?.Id, stream);
                photo.Url = (await _photoFiles.UploadAsync(photo)).AbsoluteUri;
                ev.AddPhoto(photo);
            });

            return new UploadPhotoCommandResult
            {
                SuggestedTags = suggestedTags,
                Code = code,
                IsValid = true
            };
        }

        // For public photos
        public async Task<UploadPhotoCommandResult> ExecuteAsync(int userId, int eventId, string filename, Stream stream)
        {
            var photo = new Photo(userId, filename, eventId, null, stream);
            photo.Url = (await _photoFiles.UploadAsync(photo)).AbsoluteUri;

            var bytes = TransformImageIntoBytes(stream);
            var suggestedTags = await _cognitive.GetSuggestedTags(bytes);
            
            await _photos.AddAsync(photo);
            await _photos.CommitAsync();

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

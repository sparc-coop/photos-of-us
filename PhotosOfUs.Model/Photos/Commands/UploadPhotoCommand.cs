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
    public class UploadPhotoCommand : Command<Photo>
    {
        private readonly IMediaRepository<Photo> _photos;
        private readonly IRepository<Photo> _photoRepository;
        private readonly IRepository<User> _users;
        private readonly IRepository<Tag> _tags;
        private readonly IRepository<PhotoTag> _photoTags;

        public UploadPhotoCommand(IRepository<User> users, IRepository<Photo> photoRepository, IMediaRepository<Photo> photos, IRepository<Tag> tagRepository, IRepository<PhotoTag> photoTagRepository) : base(photoRepository)
        {
            _photos = photos;
            _users = users;
            _photoRepository = photoRepository;
            _tags = tagRepository;
            _photoTags = photoTagRepository;
    }

        public async Task<Photo> ExecuteAsync(int userId, Stream stream, string photoName, string photoCode, string extension, int folderId, double? price, RootObject suggestedTags, List<Tag> listoftags, bool publicProfile = false)
        {
            var photo = new Photo(userId, photoName, extension, stream, photoCode, folderId, price, "empty", publicProfile);
            var uri = await _photos.UploadAsync(photo);
            photo.Url = uri.AbsoluteUri;
            

            var photothumbnail = new Photo(userId, photoName, extension, stream, photoCode, folderId, price, "empty", publicProfile);
            photothumbnail.FolderName = "thumbnails";
            photothumbnail.Filename = photo.Filename;
            photothumbnail.Stream = photothumbnail.ConvertToThumbnail(photo.Stream, extension, 300);
            await _photos.UploadAsync(photothumbnail);

            var photowatermark = new Photo(userId, photoName, extension, stream, photoCode, folderId, price, "empty", publicProfile);
            photowatermark.FolderName = "watermark";
            photowatermark.Filename = photo.Filename;
            photowatermark.Stream = photowatermark.AddWatermark(photo.Stream, extension);
            await _photos.UploadAsync(photowatermark);

            _photoRepository.Add(photo);

            foreach (var tag in listoftags)
            {
                var newTag = _tags.Find(x => x.Name == tag.Name);

                if (newTag == null)
                {
                    newTag = tag;
                    _tags.Add(newTag);
                }
                _photoTags.Add(new PhotoTag { PhotoId = photo.Id, TagId = newTag.Id });
            }

            return photo;
        }
    }
}

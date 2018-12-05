using Kuvio.Kernel.Architecture;
using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PhotosOfUs.Model.Photos.Commands
{
    public class UploadPhotoCommand : Command<Photo>
    {
        private readonly IMediaRepository<Photo> _photos;
        private readonly IRepository<Photo> _photoRepository;
        private readonly IRepository<User> _users;

        public UploadPhotoCommand(IRepository<User> users, IRepository<Photo> photoRepository, IMediaRepository<Photo> photos) : base(photoRepository)
        {
            _photos = photos;
            _users = users;
            _photoRepository = photoRepository;
        }

        public async Task<Photo> ExecuteAsync(int userId, Stream stream, string photoName, string photoCode, string extension, int folderId, double? price, RootObject suggestedTags, List<Tag> listoftags, bool publicProfile = false)
        {
            var photo = new Photo(userId, photoName, extension, stream, photoCode, folderId, price, "empty");
            var uri = await _photos.UploadAsync(photo);

            photo.Url = uri.AbsoluteUri;

            // 
            //photo.SuggestedTags = "";0

            foreach (var item in listoftags)
            {
                

                PhotoTag tag = new PhotoTag();
                tag.PhotoId = photo.Id;
                tag.RegisterDate = DateTime.Now;
                tag.TagId = item.Id;
                photo.PhotoTag.Add(tag);
            }
            

            _photoRepository.Add(photo);
            
            _photoRepository.Commit();

            return photo;
        }
    }
}

using PhotosOfUs.Model.Models;
using System.IO;
using System.Threading.Tasks;
using Kuvio.Kernel.Core;

namespace PhotosOfUs.Model.Repositories
{
    public class UploadProfileImageCommand
    {
        private readonly IRepository<User> _users;
        private readonly IMediaRepository<ProfilePhoto> _profilePhotos;
        private readonly IMediaRepository<ProfileThumbnail> _profileThumbnails;

        public UploadProfileImageCommand(IRepository<User> users, IMediaRepository<ProfilePhoto> profilePhotos, IMediaRepository<ProfileThumbnail> profileThumbnails)
        {
            _users = users;
            _profilePhotos = profilePhotos;
            _profileThumbnails = profileThumbnails;
        }

        public async Task<string> ExecuteAsync(int userId, Stream stream, string photoName, string extension)
        {
            var photo = new ProfilePhoto(userId, photoName, extension, stream);
            var uri = await _profilePhotos.UploadAsync(photo);

            var thumbnail = new ProfileThumbnail(userId, photoName, extension, stream);
            await _profileThumbnails.UploadAsync(thumbnail);

            _users.Execute(userId, u => u.SetProfilePhoto(uri.AbsoluteUri));

            return uri.AbsoluteUri;
        }
    }
}

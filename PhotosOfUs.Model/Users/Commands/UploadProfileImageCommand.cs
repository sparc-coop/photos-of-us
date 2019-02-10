using PhotosOfUs.Model.Models;
using System.IO;
using System.Threading.Tasks;
using Kuvio.Kernel.Core;

namespace PhotosOfUs.Model.Repositories
{
    public class UploadProfileImageCommand : Command<User>
    {
        private readonly IRepository<User> _users;
        private readonly IMediaRepository<ProfilePhoto> _profilePhotos;
        private readonly IMediaRepository<ProfileThumbnail> _profileThumbnails;

        public UploadProfileImageCommand(IRepository<User> users, IMediaRepository<ProfilePhoto> profilePhotos, IMediaRepository<ProfileThumbnail> profileThumbnails) : base(users)
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

            var user = _users.Find(x => x.Id == userId);
            user.SetProfilePhoto(uri.AbsoluteUri);
            Commit();

            return user.ProfilePhotoUrl;
        }
    }
}

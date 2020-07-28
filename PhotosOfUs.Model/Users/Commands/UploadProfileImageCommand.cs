using System.IO;
using System.Threading.Tasks;
using Kuvio.Kernel.Core;

namespace PhotosOfUs.Core.Users.Commands
{
    public class UploadProfileImageCommand
    {
        private readonly IDbRepository<User> _users;
        private readonly IMediaRepository<ProfilePhoto> _profilePhotos;
        private readonly IMediaRepository<ProfileThumbnail> _profileThumbnails;

        public UploadProfileImageCommand(IDbRepository<User> users, IMediaRepository<ProfilePhoto> profilePhotos, IMediaRepository<ProfileThumbnail> profileThumbnails)
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

            await _users.ExecuteAsync(userId, u => u.SetProfilePhoto(uri.AbsoluteUri));
            await _users.CommitAsync();

            return uri.AbsoluteUri;
        }
    }
}

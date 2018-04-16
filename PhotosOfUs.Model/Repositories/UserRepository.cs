using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhotosOfUs.Model.ViewModels;
using System.IO;
using PhotosOfUs.Model.Services;

namespace PhotosOfUs.Model.Repositories
{
    public class UserRepository
    {
        private PhotosOfUsContext _context;

        public UserRepository(PhotosOfUsContext context)
        {
            _context = context;
        }

        public User Find(int userId)
        {
            return _context.User.Find(userId);
        }

        public bool UpdateAccountProfileSettings(ProfileSettingsViewModel model)
        {
            var user = Find(model.UserId);

            if(null != model.Email)
                user.Email = model.Email;

            user.LastName = model.LastName;
            user.ProfilePhotoUrl = model.ProfilePhotoUrl;
            user.FirstName = model.FirstName;
            user.JobPosition = model.JobPosition;
            user.Bio = model.Bio;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool UpdateAccountSettings(PhotographerAccountViewModel model)
        {
            var user = Find(model.Id);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.DisplayName = model.DisplayName;
            user.JobPosition = model.JobPosition;
            user.Bio = model.Bio;

            _context.SaveChanges();

            return true;
        }

        public async System.Threading.Tasks.Task<string> UpdateProfileImageAsync(int photographerId, Stream stream, string photoName, string photoCode, string extension)
        {
            var urlTimeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var url = $"{photographerId}/profile/{photoName.Split('.')[0] + urlTimeStamp + extension}";

            stream.Position = 0;
            var container = new MemoryStream();
            var containerBlob = StorageHelpers.Container("photos").GetBlockBlobReference(url);
            containerBlob.Properties.CacheControl = "public, max-age=31556926";
            container.Position = 0;
            await containerBlob.UploadFromStreamAsync(stream);

            // Generate thumbnail
            stream.Position = 0;
            var thumbnail = new MemoryStream();
            ImageHelper.ConvertImageToThumbnailJpg(stream, thumbnail, extension);
            var thumbnailBlob = StorageHelpers.Container("thumbnails").GetBlockBlobReference(url);
            thumbnailBlob.Properties.CacheControl = "public, max-age=31556926";
            thumbnail.Position = 0;
            await thumbnailBlob.UploadFromStreamAsync(thumbnail);

            var user = _context.User.Find(photographerId);

            user.ProfilePhotoUrl = containerBlob.Uri.AbsoluteUri;

            _context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();

            return containerBlob.Uri.AbsoluteUri;
        }

        public Address GetAddress(int userId)
        {
            return _context.Address.Where(x => x.UserId == userId).First();
        }
    }
}

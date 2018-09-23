using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhotosOfUs.Model.ViewModels;
using System.IO;
using PhotosOfUs.Model.Services;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Kuvio.Kernel.Auth;
using Kuvio.Kernel.Azure;

namespace PhotosOfUs.Model.Repositories
{
    public class UserRepository
    {
        private PhotosOfUsContext _context;
        private StorageContext _storageContext;

        public UserRepository(PhotosOfUsContext context, StorageContext storageContext)
        {
            _context = context;
            _storageContext = storageContext;
        }

        public User Find(int userId)
        {
            return _context.User.Find(userId);
        }

        public async System.Threading.Tasks.Task<string> UpdateProfileImageAsync(int photographerId, Stream stream, string photoName, string photoCode, string extension)
        {
            var urlTimeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var url = $"{photographerId}/profile/{photoName.Split('.')[0] + urlTimeStamp + extension}";

            stream.Position = 0;
            var container = new MemoryStream();
            var containerBlob = _storageContext.Container("photos").GetBlockBlobReference(url);
            containerBlob.Properties.CacheControl = "public, max-age=31556926";
            container.Position = 0;
            await containerBlob.UploadFromStreamAsync(stream);

            // Generate thumbnail
            stream.Position = 0;
            var thumbnail = new MemoryStream();
            ImageHelper.ConvertImageToThumbnailJpg(stream, thumbnail, extension);
            var thumbnailBlob = _storageContext.Container("thumbnails").GetBlockBlobReference(url);
            thumbnailBlob.Properties.CacheControl = "public, max-age=31556926";
            thumbnail.Position = 0;
            await thumbnailBlob.UploadFromStreamAsync(thumbnail);

            var user = _context.User.Find(photographerId);

            user.ProfilePhotoUrl = containerBlob.Uri.AbsoluteUri;

            _context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();

            return containerBlob.Uri.AbsoluteUri;
        }
    }
}

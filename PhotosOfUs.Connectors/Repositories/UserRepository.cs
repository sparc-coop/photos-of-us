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

        public UserIdentity GetUser(string azureId)
        {
            return _context.UserIdentity.Include(x => x.Address).FirstOrDefault(x => x.AzureID == azureId);
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
            catch (Exception)
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
            user.Facebook = model.Facebook;
            user.Twitter = model.Twitter;
            user.Instagram = model.Instagram;
            user.Dribbble = model.Dribbble;

            _context.Update(user);
            _context.SaveChanges();

            return true;
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

        public Address GetAddress(int userId)
        {
            return _context.Address.Where(x => x.UserId == userId).First();
        }

        private UserIdentity GetIdentity(ClaimsPrincipal principal)
        {
            var identity = _context.UserIdentity.Include(x => x.User).FirstOrDefault(x => x.AzureID == principal.AzureID());
            if (identity == null)
            {
                var baseUser = _context.User.FirstOrDefault(x => x.Email == principal.Email());
                if (baseUser == null)
                {
                    baseUser = new User
                    {
                        CreateDate = DateTime.UtcNow,
                        DisplayName = principal.DisplayName(),
                        Email = principal.Email(),
                        IsPhotographer = principal.HasClaim("tfp", "B2C_1_SiUpOrIn_Photographer")

                    };
                    _context.User.Add(baseUser);
                    _context.SaveChanges();
                }

                identity = new UserIdentity
                {
                    AzureID = principal.AzureID(),
                    IdentityProvider = "Azure",
                    CreateDate = DateTime.UtcNow,
                    UserID = baseUser.Id
                };
                _context.UserIdentity.Add(identity);
                _context.SaveChanges();
            }

            return identity;
        }

        public void Login(ClaimsPrincipal principal)
        {
            var identity = GetIdentity(principal);

            // Logging in is simply adding claims to the existing principal
            foreach (var claim in identity.User.GenerateClaims().Where(x => !principal.HasClaim(y => y.Type == x.Type)))
                (principal.Identity as ClaimsIdentity)?.AddClaim(claim);

            // And setting a new login date
            identity.LastLoginDate = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
}

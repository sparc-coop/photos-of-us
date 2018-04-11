using Microsoft.EntityFrameworkCore;
using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.WindowsAzure.Storage.Blob;
using PhotosOfUs.Model.Services;
using System.Threading.Tasks;

namespace PhotosOfUs.Model.Repositories
{
    public class PhotoRepository
    {
        private PhotosOfUsContext _context;

        public PhotoRepository(PhotosOfUsContext context)
        {
            _context = context;
        }

        public List<Folder> GetFolders(int photographerId)
        {
            return _context.Folder.Include(x => x.Photo).Where(x => x.PhotographerId == photographerId).ToList();
        }

        public Folder GetPhotos(int photographerId, int folderId)
        {
            return _context.Folder.Include(x => x.Photo).Single(x => x.PhotographerId == photographerId && x.Id == folderId);
        }

        public List<Photo> GetPhotosByCode(string code)
        {
            return _context.Photo.Where(x => x.Code.Equals(code)).ToList();
        }

        public Photo GetPhoto(int photoId)
        {
            return _context.Photo.Include(x => x.Photographer).Single(x => x.Id == photoId);
        }

        public List<PrintType> GetPrintTypes()
        {
            return _context.PrintType.ToList();
        }

        public void SavePhoto(Photo photo)
        {
            _context.Photo.Attach(photo);
            _context.SaveChanges();
        }

        public bool IsPhotoCodeAlreadyUsed(int photographerId, string code)
        {
            return _context.Photo.Any(x => x.PhotographerId == photographerId && x.Code == code);
        }

        public async Task<Photo> Upload(int photographerId, Photo photo, Stream stream, string fileName, string photoName, string photoCode)
        {
            // TODO: Generate the code 
            photo.Code = "abcdef";
            photo.PhotographerId = photographerId;
            photo.UploadDate = DateTime.Now;
            photo.Name = fileName;

            var extension = Path.GetExtension(fileName);
            fileName = Guid.NewGuid() + extension;
            photo.Url = await UploadFile(photographerId, stream, photoName, photoCode, extension);

            await _context.Photo.AddAsync(photo);

            return photo;
        }

        public async Task<string> UploadFile(int photographerId, Stream stream, string photoName, string photoCode, string extension, bool publicProfile = false)
        {
            var urlTimeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var url = $"{photographerId}/{photoName.Split('.')[0] + urlTimeStamp + extension}";

            stream.Position = 0;
            var container = new MemoryStream();
            var containerBlob = StorageHelpers.Container("photos").GetBlockBlobReference(url);
            containerBlob.Properties.CacheControl = "public, max-age=31556926";
            container.Position = 0;
            await containerBlob.UploadFromStreamAsync(stream);

            // Generate thumbnail
            stream.Position = 0;
            var thumbnail = new MemoryStream();
            ConvertImageToThumbnailJpg(stream, thumbnail, extension);
            var thumbnailBlob = StorageHelpers.Container("thumbnails").GetBlockBlobReference(url);
            thumbnailBlob.Properties.CacheControl = "public, max-age=31556926";
            thumbnail.Position = 0;
            await thumbnailBlob.UploadFromStreamAsync(thumbnail);

            var photo = new Photo()
            {
                Name = photoName,
                PhotographerId = photographerId,
                UploadDate = DateTime.Now,
                Url = containerBlob.Uri.AbsoluteUri,
                Code = photoCode,
                FolderId = 1,
                PublicProfile = publicProfile
            };

            _context.Photo.Attach(photo);
            _context.SaveChanges();

            return containerBlob.Uri.AbsoluteUri;
        }

        private static void ConvertImageToThumbnailJpg(Stream input, Stream output, string extension)
        {
            var thumbnailsize = 300;
            int width;
            int height;
            var originalImage = new Bitmap(input);

            if (originalImage.Width > originalImage.Height)
            {
                width = thumbnailsize;
                height = thumbnailsize * originalImage.Height / originalImage.Width;
            }
            else
            {
                height = thumbnailsize;
                width = thumbnailsize * originalImage.Width / originalImage.Height;
            }

            Image thumbnailImage = null;
            try
            {
                if (height > originalImage.Height || width > originalImage.Width)
                {
                    originalImage.Save(output, GetImageFormatFromExtension(extension));
                }
                else
                {
                    thumbnailImage = originalImage.GetThumbnailImage(width, height, () => true, IntPtr.Zero);
                    thumbnailImage.Save(output, GetImageFormatFromExtension(extension));
                }
            }
            finally
            {
                thumbnailImage?.Dispose();
            }
        }

        private static ImageFormat GetImageFormatFromExtension(string extension)
        {
            if (extension.ToLower() == "png")
            {
                return ImageFormat.Png;
            }
            else if (extension.ToLower() == "tiff" || extension.ToLower() == "tif")
            {
                return ImageFormat.Tiff;
            }
            else if (extension.ToLower() == "bmp")
            {
                return ImageFormat.Bmp;
            }
            else if (extension.ToLower() == "jpg" || extension.ToLower() == "jpeg" || extension.ToLower() == "jpe")
            {
                return ImageFormat.Png;
            }
            else
            {
                return ImageFormat.Png;
            }
        }

        public List<Photo> GetProfilePhotos(int photographerId)
        {
            return _context.Photo.Where(x => x.PublicProfile).ToList();
        }

    }

  
}

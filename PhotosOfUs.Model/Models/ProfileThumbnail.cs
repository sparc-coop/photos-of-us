using System;
using System.IO;
using PhotosOfUs.Model.Services;

namespace PhotosOfUs.Model.Models
{
    public class ProfileThumbnail : ProfilePhoto
    {
        public ProfileThumbnail(int userId, string photoName, string extension, Stream stream) : base(userId, photoName, extension, stream)
        {
            FolderName = "thumbnails";
            Stream = new ImageService().ConvertToThumbnail(stream, extension, 300);
        }
    }
}
using Kuvio.Kernel.Core;
using Kuvio.Kernel.Core.Common;
using System;
using System.IO;

namespace PhotosOfUs.Core.Users
{
    public class ProfilePhoto : IFile
    {
        public ProfilePhoto(int userId, string photoName, string extension, Stream stream)
        {
            var urlTimeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            Filename = $"{userId}/profile/{photoName.Split('.')[0] + urlTimeStamp + extension}";
            FolderName = "photos";
            Stream = stream;
        }

        public string Filename { get; }
        public string FolderName { get; protected set; }
        public Stream Stream { get; protected set; }
    }
}
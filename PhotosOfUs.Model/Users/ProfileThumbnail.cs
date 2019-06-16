using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PhotosOfUs.Model.Models
{
    public class ProfileThumbnail : ProfilePhoto
    {
        public ProfileThumbnail(int userId, string photoName, string extension, Stream stream) : base(userId, photoName, extension, stream)
        {
            FolderName = "thumbnails";
            Stream = ConvertToThumbnail(stream, extension, 300);
        }

        public Stream ConvertToThumbnail(Stream image, string extension, int thumbnailSize)
        {
            image.Position = 0;
            var output = new MemoryStream();
            int width;
            int height;
            using (var originalImage = new Bitmap(image))
            {
                if (originalImage.Width > originalImage.Height)
                {
                    width = thumbnailSize;
                    height = thumbnailSize * originalImage.Height / originalImage.Width;
                }
                else
                {
                    height = thumbnailSize;
                    width = thumbnailSize * originalImage.Width / originalImage.Height;
                }

                Image thumbnailImage = null;
                try
                {
                    if (height > originalImage.Height || width > originalImage.Width)
                    {
                        originalImage.Save(output, Photo.GetImageFormatFromExtension(extension));
                    }
                    else
                    {
                        thumbnailImage = originalImage.GetThumbnailImage(width, height, () => true, IntPtr.Zero);
                        thumbnailImage.Save(output, Photo.GetImageFormatFromExtension(extension));
                    }
                }
                finally
                {
                    thumbnailImage?.Dispose();
                }
            }

            return output;
        }
    }
}